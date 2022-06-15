using ControlFinWeb.Dominio.ObjetoValor;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Utils.Extensions.Enums;

namespace ControlFinWeb.App.ViewModels
{
    public class BancoVM
    {
        public Int64 Id { get; set; }

        [DisplayName("Descrição")]
        [StringLength(70, ErrorMessage = "Limite máximo de 70 caracteres.")]
        [Required(ErrorMessage = "Campo Obrigatório")]
        public String Nome { get; set; }

        [DisplayName("Tipo de Conta")]
        public TipoCartao TipoContaBanco { get; set; }

        [DisplayName("Situação")]
        public Situacao Situacao { get; set; }

        [Range(1, Int64.MaxValue, ErrorMessage = "Informe uma Pessoa")]
        [DisplayName("Pessoa")]
        public Int64 PessoaRefBancoId { get; set; }
        public SelectList Pessoas { get; set; }
        public PessoaVM PessoaRefBancoVM { get; set; }

        public String NomeETipo
        {
            get { return String.Format("{0} ({1})", Nome, TipoContaBanco.GetDisplayName()); }
        }

        public override string ToString()
        {
            return String.Format("{0} ({1}) - {2}", Nome, TipoContaBanco, PessoaRefBancoVM);
        }
    }
}
