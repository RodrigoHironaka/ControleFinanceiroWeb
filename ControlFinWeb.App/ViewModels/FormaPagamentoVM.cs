﻿using ControlFinWeb.Dominio.ObjetoValor;
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

        [DisplayName("Possui função débito automático?")]
        public bool DebitoAutomatico { get; set; }

        [DisplayName("Gerar registro na fatura?")]
        public bool GerarRegistroFatura { get; set; }

        [DisplayName("Situação")]
        public Situacao Situacao { get; set; }
    }
}
