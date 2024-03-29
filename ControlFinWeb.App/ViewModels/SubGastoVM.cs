﻿using ControlFinWeb.App.Utilitarios;
using ControlFinWeb.Dominio.Entidades;
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
    public class SubGastoVM
    {
        public Int64 Id { get; set; }

        [DisplayName("Descrição")]
        [StringLength(70, ErrorMessage = "Limite máximo de 70 caracteres.")]
        [Required(ErrorMessage = "Campo Obrigatório")]
        public String Nome { get; set; }

        [DisplayName("Situação")]
        public Situacao Situacao { get; set; }

        [Range(1, Int64.MaxValue, ErrorMessage = "Informe um Gasto")]
        [DisplayName("Gasto")]
        public Int64 GastoId { get; set; }
        public GastoVM GastoVM { get; set; }
        public SelectList ComboGastos
        {
            get
            {
                return PreencheCombo.Gastos();
            }
        }

        public String DescricaoCompleta
        {
            get 
            {
                if (GastoVM != null)
                    return String.Format("{0}/{1}", GastoVM.Nome, Nome);
                else
                    return Nome;
            }
        }
    }
}
