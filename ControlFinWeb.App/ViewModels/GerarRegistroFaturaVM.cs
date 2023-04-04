using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ControlFinWeb.App.ViewModels
{
    public class GerarRegistroFaturaVM
    {
        [Required(ErrorMessage ="Campo Obrigátorio")]
        [DisplayName("Dados Parcela")]
        public Int64 ParcelaId { get; set; }
        public ParcelaVM ParcelaVM { get; set; }

        [Required(ErrorMessage = "Campo Obrigátorio")]
        [DisplayName("Fatura")]
        public Int64 FaturaId { get; set; }

        [Required(ErrorMessage = "Campo Obrigátorio")]
        [DisplayName("Forma de Pagamento")]
        public Int64 FormaPagamentoId { get; set; }

        [Required(ErrorMessage = "Campo Obrigátorio")]
        [DisplayName("SubGasto")]
        public Int64 SubGastoId { get; set; }

        [Required(ErrorMessage = "Campo Obrigátorio")]
        [DisplayName("Dividir em")]
        public Int32 Quantidade { get; set; }
    }
}
