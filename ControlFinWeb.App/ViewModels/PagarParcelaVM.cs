using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ControlFinWeb.App.ViewModels
{
    public class PagarParcelaVM
    {
        [DisplayName("Valor à Pagar")]
        public Decimal ValorAPagar { get; set; }
        
        [DisplayName("Pagamento")]
        [Range(typeof(DateTime), "01/01/1980", "31/12/5000", ErrorMessage = "Data Inválida!")]
        [Required(ErrorMessage = "Campo Obrigatório")]
        public DateTime? DataPagamento { get; set; }

        [DisplayName("Juros(%)")]
        public Decimal JurosPorcentual { get; set; }

        [DisplayName("Juros(R$)")]
        public Decimal JurosValor { get; set; }

        [DisplayName("Desconto(%)")]
        public Decimal DescontoPorcentual { get; set; }

        [DisplayName("Desconto(R$)")]
        public Decimal DescontoValor { get; set; }

        [DisplayName("V. Reajustado.(R$)")]
        public Decimal ValorReajustado { get; set; }

        [DisplayName("Pago(R$)")]
        [Range(0.01, Double.PositiveInfinity, ErrorMessage = "Valor Inválido")]
        [Required(ErrorMessage = "Campo Obrigatório")]
        public Decimal ValorPago { get; set; }

        [DisplayName("V. Restante.(R$)")]
        public Decimal ValorRestante { get; set; }

        [Range(1, Int64.MaxValue, ErrorMessage = "Informe uma forma de pagamento")]
        [DisplayName("Forma Pagamento")]
        [Required(ErrorMessage = "Campo Obrigatório")]
        public Int64 FormaPagamentoId { get; set; }
        public FormaPagamentoVM FormaPagamentoVM { get; set; }

        [DisplayName("Banco")]
        public Int64 BancoId { get; set; }
        public BancoVM BancoVM { get; set; }

        public List<ParcelaVM> ParcelasVM { get; set; }
    }
}
