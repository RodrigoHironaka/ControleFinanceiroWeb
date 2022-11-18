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

        [Range(0.01, double.MaxValue, ErrorMessage = "Valor deve ser maior que zero")]
        public Decimal Valor { get; set; }

        [DisplayName("Tipo")]
        public DebitoCredito DebitoCredito { get; set; }

        [DisplayName("Data")]
        [Range(typeof(DateTime), "01/01/1980", "31/12/5000", ErrorMessage = "Data Inválida!")]
        [Required(ErrorMessage = "Campo Obrigatório")]
        public virtual DateTime? Data { get; set; }

        #region Associações

        [DisplayName("Forma de Pagamento")]
        [Range(1, Int64.MaxValue, ErrorMessage = "Informe uma forma de pagamento")]
        public Int64 FormaPagamentoId { get; set; }
        public FormaPagamentoVM FormaPagamentoVM{ get; set; }
        public Int64 ParcelaId { get; set; }
        public ParcelaVM ParcelaVM { get; set; }

        public Int64 CaixaId { get; set; }
        public CaixaVM CaixaVM { get; set; }

        #endregion
    }
}
