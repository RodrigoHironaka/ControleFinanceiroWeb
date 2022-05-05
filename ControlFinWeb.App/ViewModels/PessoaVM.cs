using ControlFinWeb.Dominio.ObjetoValor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ControlFinWeb.App.ViewModels
{
    public class PessoaVM
    {
        public Int64 Id { get; set; }

        [DisplayName("Descrição")]
        [StringLength(70, ErrorMessage = "Limite máximo de 70 caracteres.")]
        [Required(ErrorMessage = "Campo Obrigatório")]
        public String Nome { get; set; }

        [DisplayName("Situação")]
        public Situacao Situacao { get; set; }

        public IList<PessoaRendasVM> PessoaRendasVM { get; set; }

        [DisplayName("Total Renda Bruta")]
        [DataType(DataType.Currency)]
        //[DisplayFormat(DataFormatString = "{0:C}")]
        public virtual Decimal TotalRendaBruta
        {
            get
            {
                return PessoaRendasVM.Select(x => x.RendaBruta).Sum();
            }
        }

        [DisplayName("Total Renda Líquida")]
        [DataType(DataType.Currency)]
        public virtual Decimal TotalRendaLiquida
        {
            get
            {
                return PessoaRendasVM.Select(x => x.RendaLiquida).Sum();
            }
        }

        public override string ToString()
        {
            return Nome;
        }
    }
}
