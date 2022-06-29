using ControlFinWeb.Dominio.ObjetoValor;
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
        public Int64 Id { get; set; }

        [DisplayName("Código")]
        public Int64 Codigo { get; set; }
       
        [DisplayName("Nº Documento")]
        public String NumeroDocumento { get; set; }

        [DisplayName("Descrição")]
        [StringLength(100, ErrorMessage = "Limite máximo de 100 caracteres.")]
        [Required(ErrorMessage = "Campo Obrigatório")]
        public String Nome { get; set; }

        [DisplayName("Tipo Conta")]
        public TipoConta TipoConta { get; set; }

        [DisplayName("Período")]
        public PeriodoConta TipoPeriodo { get; set; }

        [DisplayName("Situação")]
        public SituacaoConta Situacao { get; set; }

        [DisplayName("Emissão")]
        [Range(typeof(DateTime), "01/01/1980", "31/12/5000", ErrorMessage = "Data Inválida!")]
        [Required(ErrorMessage = "Campo Obrigatório")]
        public DateTime? DataEmissao { get; set; }

        [DisplayName("Observação")]
        public String Observacao { get; set; }

        [DisplayName("Gasto")]
        [Range(1, Int64.MaxValue, ErrorMessage = "Informe um Gasto")]
        public Int64 SubGastoId { get; set; }
        public SubGastoVM SubGastoVM { get; set; }

        [DisplayName("Fatura")]
        public Int64 FaturaCartaoCreditoId { get; set; }
        public FaturaCartaoCreditoVM FaturaCartaoCreditoVM { get; set; }

        [DisplayName("Pessoa")]
        public Int64 PessoaId { get; set; }
        public PessoaVM PessoaVM { get; set; }

        public IList<ParcelaVM> ParcelasVM { get; set; }
        public IList<ArquivoVM> ArquivosVM { get; set; }

        public Decimal? ValorTotalGeral
        {
            get
            {
                return ParcelasVM.Select(x => x.ValorParcela).Sum();
            }
        }

        public Decimal? ValorTotalAberto
        {
            get
            {
                return ParcelasVM.Select(x => x.ValorAberto).Sum();
            }
        }
        public Int64? QtdTotalParcelas
        {
            get
            {
                return ParcelasVM.Count();
            }
        }
        public Int64? QtdTotalParcelasEmAberto
        {
            get
            {
                return ParcelasVM.Where(x => x.ValorAberto > 0).Count();
            }
        }
    }
}
