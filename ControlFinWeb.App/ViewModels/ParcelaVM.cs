﻿using ControlFinWeb.Dominio.ObjetoValor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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

        [DisplayName("Nº")]
        public Int64 Numero { get; set; }

        [DisplayName("Parcela")]
        public String ParcelaDe { get; set; }

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

        [DisplayName("Fatura")]
        public Int64 FaturaId { get; set; }
        public FaturaVM FaturaVM { get; set; }

        #region Campos Gerar Parcelas

        [DisplayName("Valor")]
        [Required(ErrorMessage = "Campo Obrigatório")]
        public Decimal ValorDigitado { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        public Int32 Qtd { get; set; }

        [DisplayName("1º Vencimento")]
        [Range(typeof(DateTime), "01/01/1980", "31/12/5000", ErrorMessage = "Data Inválida!")]
        [Required(ErrorMessage = "Campo Obrigatório")]
        public DateTime? PrimeiroVencimento { get; set; }

        [DisplayName("Replicar?")]
        public Boolean Replicar { get; set; }

        public String JsonParcelas { get; set; }
        #endregion
    }
}
