﻿using ControlFinWeb.App.Utilitarios;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        public SelectList ComboFormasPagamento
        {
            get
            {
                return PreencheCombo.FormasPagamentoGeraFatura();
            }
        }

        [Required(ErrorMessage = "Campo Obrigátorio")]
        [DisplayName("SubGasto")]
        public Int64 SubGastoId { get; set; }
        public SelectList ComboSubGastos
        {
            get
            {
                return PreencheCombo.SubGastos();
            }
        }

        [Required(ErrorMessage = "Campo Obrigátorio")]
        [DisplayName("Dividir em")]
        public Int32 Quantidade { get; set; }

        [Required(ErrorMessage = "Campo Obrigátorio")]
        [DisplayName("Juros")]
        public Decimal Juros { get; set; }


    }
}
