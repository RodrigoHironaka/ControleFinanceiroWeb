using ControlFinWeb.Dominio.ObjetoValor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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
        public Int64 PessoaId { get; set; }
        public PessoaVM PessoaRefBancoVM { get; set; }

        public override string ToString()
        {
            return String.Format("{0} {1} - {2}", Nome, TipoContaBanco, PessoaRefBancoVM);
        }
    }
}
