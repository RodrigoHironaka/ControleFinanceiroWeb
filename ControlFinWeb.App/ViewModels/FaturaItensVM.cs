using ControlFinWeb.Dominio.ObjetoValor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ControlFinWeb.App.ViewModels
{
    public class FaturaItensVM
    {
        public Int64 Id { get; set; }

        [DisplayName("Descrição")]
        [StringLength(100, ErrorMessage = "Limite máximo de 100 caracteres.")]
        [Required(ErrorMessage = "Campo Obrigatório")]
        public String Nome { get; set; }

        [DisplayName("Valor")]
        [Required(ErrorMessage = "Campo Obrigatório")]
        public Decimal Valor { get; set; }

        [DisplayName("Qtd - ")]
        [Required(ErrorMessage = "Campo Obrigatório")]
        public String NumeroParcelas { get; set; }

        [DisplayName("Data da Compra")]
        [Range(typeof(DateTime), "01/01/1980", "31/12/5000", ErrorMessage = "Data Inválida!")]
        [Required(ErrorMessage = "Campo Obrigatório")]
        public DateTime? DataCompra { get; set; }

        [DisplayName("Replicar?")]
        public Boolean Replicar { get; set; }

        public IList<FaturaItensVM> FaturasItensVM { get; set; }

        #region Campos para verificar outros itens relacionados

        public String CodigoItemRelacionado { get; set; }
        public String QuantidadeRelacionado { get; set; }

        public String DescricaoCompleta
        {
            get { return String.Format("{0} ({1})", Nome, QuantidadeRelacionado); }
        }
        #endregion

        #region Campos vinculados a outras classes
        [DisplayName("Gasto")]
        [Range(1, Int64.MaxValue, ErrorMessage = "Informe um Gasto")]
        public Int64 SubGastoId { get; set; }
        public SubGastoVM SubGastoVM { get; set; }

        [DisplayName("Pessoa")]
        public Int64 PessoaId { get; set; }
        public PessoaVM PessoaVM { get; set; }

        [DisplayName("Fatura")]
        public Int64 FaturaId { get; set; }
        public FaturaVM FaturaVM { get; set; }
        #endregion



    }
}
