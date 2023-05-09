using ControlFinWeb.App.Utilitarios;
using ControlFinWeb.Dominio.Entidades;
using ControlFinWeb.Dominio.ObjetoValor;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ControlFinWeb.App.ViewModels
{
    public class TransferenciaEntreContasVM
    {
        [Required(ErrorMessage ="Campo Obrigatório")]
        [DisplayName("Banco Saída")]
        [Range(1, Int64.MaxValue, ErrorMessage = "Informe um Banco Saída")]
        public Int64 BancoSaidaId { get; set; }
        public SelectList ComboBancoSaida
        {
            get
            {
                return PreencheCombo.BancosDadosCompletos();
            }
        }

        [Required(ErrorMessage = "Campo Obrigatório")]
        [DisplayName("Banco Entrada")]
        [Range(1, Int64.MaxValue, ErrorMessage = "Informe um Banco Entrada")]
        public Int64 BancoEntradaId { get; set; }
        public SelectList ComboBancoEntrada
        {
            get
            {
                return PreencheCombo.BancosDadosCompletos();
            }
        }

        [Required(ErrorMessage = "Campo Obrigatório")]
        [DisplayName("Transação Bancária")]
        public TransacaoBancaria TransacaoBancaria { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        [DisplayName("Valor")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Valor precisa ser maior que zero!")]
        public Decimal Valor { get; set; }
    }
}
