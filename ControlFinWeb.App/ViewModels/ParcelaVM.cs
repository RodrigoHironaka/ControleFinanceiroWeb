using ControlFinWeb.Dominio.ObjetoValor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ControlFinWeb.App.ViewModels
{
    public class ParcelaVM
    {
        public ParcelaVM()
        {
            SituacaoParcela = SituacaoParcela.Pendente;
        }
        public Int64 Id { get; set; }

        [DisplayName("Nº Parcela")]
        public String NumeroParcela { get; set; }

        [DisplayName("Valor da Parcela")]
        public Decimal ValorParcela { get; set; }

        [DisplayName("Vencimento")]
        public DateTime? DataVencimento { get; set; }

        [DisplayName("Pagamento")]
        public DateTime? DataPagamento { get; set; }

        [DisplayName("Juros(%)")]
        public Decimal JurosPorcentual { get; set; }

        [DisplayName("Juros(R$)")]
        public Decimal? JurosValor { get; set; }

        [DisplayName("Desconto(%)")]
        public Decimal DescontoPorcentual { get; set; }

        [DisplayName("Desconto(R$)")]
        public Decimal DescontoValor { get; set; }

        [DisplayName("V. Reajustado.(R$)")]
        public Decimal ValorReajustado { get; set; }

        [DisplayName("Pago(R$)")]
        public Decimal ValorPago { get; set; }

        [DisplayName("Aberto(R$)")]
        public Decimal ValorAberto { get; set; }

        [DisplayName("Observação")]
        public String Observacao { get; set; }

        [DisplayName("Situação")]
        public SituacaoParcela SituacaoParcela { get; set; }

        [DisplayName("Forma Pagamento")]
        public Int64 FormaPagamentoId { get; set; }
        public FormaPagamentoVM FormaPagamentoVM { get; set; }

        [DisplayName("Conta")]
        public Int64 ContaId { get; set; }
        public ContaVM ContaVM { get; set; }
    }
}
