using ControlFinWeb.Dominio.ObjetoValor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ControlFinWeb.App.ViewModels
{
    public class FluxoCaixaVM
    {
        public Int64 Id { get; set; }

        [DisplayName("Descrição")]
        [StringLength(70, ErrorMessage = "Limite máximo de 70 caracteres.")]
        [Required(ErrorMessage = "Campo Obrigatório")]
        public String Nome { get; set; }

        public Decimal Valor { get; set; }

        [DisplayName("Tipo")]
        public DebitoCredito DebitoCredito { get; set; }

        [DisplayName("Data Criação")]
        public virtual DateTime DataGeracao { get; set; }

        #region Associações

        public Int64 FormaPagamentoId { get; set; }
        public FormaPagamentoVM FormaPagamentoVM{ get; set; }
        public Int64 ParcelaId { get; set; }
        public ParcelaVM ParcelaVM { get; set; }

        public Int64 CaixaId { get; set; }
        public CaixaVM CaixaVM { get; set; }

        #endregion
    }
}
