using ControlFinWeb.App.Utilitarios;
using ControlFinWeb.Dominio.ObjetoValor;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ControlFinWeb.App.ViewModels
{
    public class ContaVM
    {
        public ContaVM()
        {
            ParcelasVM = new List<ParcelaVM>();
            ArquivosVM = new List<ArquivoVM>();
        }
        [DisplayName("Código")]
        public Int64 Id { get; set; }
       
        [DisplayName("Nº Documento")]
        public String NumeroDocumento { get; set; }

        [DisplayName("Descrição")]
        [StringLength(100, ErrorMessage = "Limite máximo de 100 caracteres.")]
        [Required(ErrorMessage = "Campo Obrigatório")]
        public String Nome { get; set; }

        [DisplayName("Tipo Conta")]
        public TipoConta TipoConta { get; set; }

        [DisplayName("Situação")]
        public SituacaoConta Situacao { get; set; }

        [DisplayName("Emissão")]
        [Required(ErrorMessage = "Campo Obrigatório")]
        public DateTime? DataEmissao { get; set; }

        [DisplayName("Observação")]
        public String Observacao { get; set; }

        #region Campos vinculados a outras classes

        [DisplayName("Gasto")]
        [Range(1, Int64.MaxValue, ErrorMessage = "Informe um Gasto")]
        public Int64 SubGastoId { get; set; }
        public SubGastoVM SubGastoVM { get; set; }
        public SelectList ComboSubGastos
        {
            get
            {
                return PreencheCombo.SubGastos();
            }
        }

        [DisplayName("Pessoa")]
        public Int64 PessoaId { get; set; }
        public PessoaVM PessoaVM { get; set; }
        public SelectList ComboPessoas
        {
            get
            {
                return PreencheCombo.Pessoas();
            }
        }
        #endregion


        public IList<ParcelaVM> ParcelasVM { get; set; }
        public IList<ArquivoVM> ArquivosVM { get; set; }

        #region Calculados

        [DisplayName("Valor Total Conta")]
        public Decimal? ValorTotalConta
        {
            get
            {
                return ParcelasVM.Select(x => x.ValorParcela).Sum();
            }
            
        }

        [DisplayName("Qtd Total Parcelas")]
        public Int64? QtdTotalParcelas
        {
            get
            {
                return ParcelasVM.Count();
            }
            
        }

        [DisplayName("Valor Total em Aberto")]
        public Decimal? ValorTotalAberto
        {
            get
            {
                return ParcelasVM.Select(x => x.ValorAberto).Sum();
            }
        }

        [DisplayName("Valor Total Reajustado")]
        public Decimal? ValorTotalReajustado
        {
            get
            {
                return ParcelasVM.Select(x => x.ValorReajustado).Sum();
            }
        }

        [DisplayName("Qtd Total em Aberto")]
        public Int64? QtdTotalParcelasEmAberto
        {
            get
            {
                return ParcelasVM.Where(x => x.ValorAberto > 0).Count();
            }
        }
        #endregion

        public String JsonParcelas { get; set; }

        public virtual String DescricaoCompleta
        {
            get
            {
                return $"CONTA: {Nome} [{Id}]";
            }
        }
    }
}
