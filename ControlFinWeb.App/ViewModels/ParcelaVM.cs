﻿using ControlFinWeb.App.Utilitarios;
using ControlFinWeb.App.ViewModels.Acesso;
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

        [DisplayName("Parcela(R$)")]
        public Decimal ValorParcela { get; set; }

        [DisplayName("Vencimento")]
        public DateTime? DataVencimento { get; set; }

        [DisplayName("Pagamento")]
        public DateTime? DataPagamento { get; set; }

        [DisplayName("Juros(%)")]
        public Decimal JurosPorcentual { get; set; }

        [DisplayName("Juros(R$)")]
        public Decimal JurosValor { get; set; }

        [DisplayName("Desconto(%)")]
        public Decimal DescontoPorcentual { get; set; }

        [DisplayName("Desconto(R$)")]
        public Decimal DescontoValor { get; set; }

        [DisplayName("Reajustado(R$)")]
        public Decimal ValorReajustado { get; set; }

        [DisplayName("Pago(R$)")]
        public Decimal ValorPago { get; set; }

        [DisplayName("Aberto(R$)")]
        public Decimal ValorAberto { get; set; }

        [DisplayName("Observação")]
        public String Observacao { get; set; }

        [DisplayName("Situação")]
        public SituacaoParcela SituacaoParcela { get; set; }

        [DisplayName("F. Pagamento")]
        public Int64 FormaPagamentoId { get; set; }
        public FormaPagamentoVM FormaPagamentoVM { get; set; }
        public SelectList ComboFormasPagamento
        {
            get
            {
                return PreencheCombo.FormasPagamento();
            }
        }

        [DisplayName("Conta")]
        public Int64 ContaId { get; set; }
        public ContaVM ContaVM { get; set; }

        [DisplayName("Fatura")]
        public Int64 FaturaId { get; set; }
        public FaturaVM FaturaVM { get; set; }

        [DisplayName("Banco")]
        public Int64 BancoId { get; set; }
        public BancoVM BancoVM { get; set; }
        public SelectList ComboBancos
        {
            get
            {
                return PreencheCombo.Bancos();
            }
        }

        public DateTime DataGeracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public Int64 UsuarioCriacaoId { get; set; }
        public UsuarioVM UsuarioCriacaoVM { get; set; }
        public Int64 UsuarioAlteracaoId { get; set; }
        public UsuarioVM UsuarioAlteracaoVM { get; set; }

        public Boolean Selecionado { get; set; }

        public String _DadosCompletos
        {
            get
            {
                return $"Parcela Nº {Numero} - Valor: {ValorAberto:C2}";
            }
        }
    }
}
