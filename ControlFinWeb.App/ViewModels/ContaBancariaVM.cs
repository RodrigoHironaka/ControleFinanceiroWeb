﻿using ControlFinWeb.App.Utilitarios;
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
    public class ContaBancariaVM
    {
        public ContaBancariaVM()
        {
            TransfereParaCaixa = null;
        }

        [DisplayName("Código")]
        public Int64 Id { get; set; }

        public DateTime DataGeracao { get; set; }

        public Decimal Valor { get; set; }

        [DisplayName("Data")]
        [Required(ErrorMessage = "Campo Obrigatório")]
        public DateTime? DataRegistro { get; set; }

        [DisplayName("Descrição")]
        [StringLength(200, ErrorMessage = "Limite máximo de 200 caracteres.")]
        [Required(ErrorMessage = "Campo Obrigatório")]
        public String Nome { get; set; }

        [DisplayName("Transação Bancária")]
        public TransacaoBancaria TransacaoBancaria { get; set; }

        [DisplayName("Situação")]
        public EntradaSaida Situacao { get; set; }

        public Boolean GerarFluxoCaixa { get; set; }
        public Boolean? TransfereParaCaixa { get; set; }

        [Range(1, Int64.MaxValue, ErrorMessage = "Informe um Banco")]
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

        public Int64 CaixaId { get; set; }
        public CaixaVM CaixaVM { get; set; }

        public String DescricaoComCodigo
        {
            get { return String.Format("{0} - {1}", Id, Nome); }
        }
        public String DescricaoComTipoConta
        {
            get { return String.Format("{0} - {1}", Nome,  BancoVM != null ? BancoVM.TipoContaBanco: String.Empty); }
        }
    }
}
