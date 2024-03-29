﻿using ControlFinWeb.App.Utilitarios;
using ControlFinWeb.Dominio.ObjetoValor;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Utils.Extensions.Enums;

namespace ControlFinWeb.App.ViewModels
{
    public class FiltroParcelasVM
    {
        public FiltroParcelasVM()
        {
            DataInicio = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DataFinal = DateTime.Now.UltimoDiaMes().FinalDia();
            TipoConta = TipoConta.Pagar;
            if (String.IsNullOrEmpty(SituacaoParcela))
            {
                SituacaoParcela = "0,1,2";
            }
            Gastos = new List<GastosPessoaVM>();
        }

        [DisplayName("Início")]
        public DateTime? DataInicio { get; set; }

        [DisplayName("Final")]
        public DateTime? DataFinal { get; set; }

        [DisplayName("Tipo")]
        public TipoConta TipoConta { get; set; }

        [DisplayName("Conta/Fatura")]
        public ContaFatura? ContaFatura { get; set; }

        [DisplayName("Situação")]
        public String SituacaoParcela { get; set; }

        [DisplayName("Pessoa Referenciada")]
        public Int64 PessoaId { get; set; }
        public PessoaVM PessoaVM { get; set; }
        public SelectList ComboPessoas
        {
            get
            {
                return PreencheCombo.Pessoas2();
            }
        }

        public IList<ParcelaVM> Parcelas { get; set; }

        public IList<GastosPessoaVM> Gastos { get; set; }
        public String TotalParcelas
        {
            get
            {
                if(Parcelas != null && Parcelas.Count > 0)
                    return Parcelas.Sum(x => x.ValorAberto).ToString("N2");
                else
                    return "0";
            }
        }
    }
}
