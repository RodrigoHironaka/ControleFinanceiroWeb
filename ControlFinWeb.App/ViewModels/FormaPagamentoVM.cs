using ControlFinWeb.Dominio.ObjetoValor;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ControlFinWeb.App.ViewModels
{
    public class FormaPagamentoVM
    {
        public Int64 Id { get; set; }

        [DisplayName("Descrição")]
        [StringLength(70, ErrorMessage = "Limite máximo de 70 caracteres.")]
        [Required(ErrorMessage = "Campo Obrigatório")]
        public String Nome { get; set; }

        [DisplayName("Possui Fatura Mensal?")]
        public SimNao PossuiFaturaMensal { get; set; }

        [DisplayName("Possui função débito automático?")]
        public SimNao DebitoAutomatico { get; set; }

        [DisplayName("Dia do vencimento")]
        public Int32 DiaVencimento { get; set; }
    }
}
