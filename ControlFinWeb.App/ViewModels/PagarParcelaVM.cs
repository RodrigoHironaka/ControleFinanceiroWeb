﻿using ControlFinWeb.App.Utilitarios;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ControlFinWeb.App.ViewModels
{
    public class PagarParcelaVM
    {
        public PagarParcelaVM()
        {
            Mensagens = new List<String>();
        }
        [DisplayName("Valor à Pagar")]
        public Decimal ValorAPagar { get; set; }
        
        [DisplayName("Pagamento")]
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
        public SelectList ComboFormasPagamento
        {
            get
            {
                return PreencheCombo.FormasPagamento();
            }
        }

        [DisplayName("Banco")]
        public Int64 BancoId { get; set; }
        public BancoVM BancoVM { get; set; }
        public SelectList ComboBancos
        {
            get
            {
                return PreencheCombo.BancosDadosCompletos();
            }
        }

        public String JsonParcelasPagar { get; set; }
        public List<String> Mensagens { get; set; }
    }
}
