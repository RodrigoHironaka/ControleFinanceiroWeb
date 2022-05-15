using ControlFinWeb.Dominio.ObjetoValor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ControlFinWeb.App.ViewModels
{
    public class PessoaVM
    {
        public PessoaVM()
        {
            PessoaRendasVM = new List<PessoaRendasVM>();
        }

        public Int64 Id { get; set; }

        [DisplayName("Descrição")]
        [StringLength(70, ErrorMessage = "Limite máximo de 70 caracteres.")]
        [Required(ErrorMessage = "Campo Obrigatório")]
        public String Nome { get; set; }

        [DisplayName("Situação")]
        public Situacao Situacao { get; set; }

        public IList<PessoaRendasVM> PessoaRendasVM { get; set; }
        
        [DisplayName("R.Bruta")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Column(TypeName = "decimal(18,2)")]
        public Decimal RendaBruta { get; set; }

        [DisplayName("R.Líquida")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public Decimal RendaLiquida { get; set; }

        [DisplayName("Renda")]
        public Int64 TipoRendaId { get; set; }
        public RendaVM TipoRendaVM { get; set; }

        [DisplayName("Total Renda Bruta")]
        public String TotalRendaBruta
        {
            get
            {
                return PessoaRendasVM.Select(x => x.RendaBruta).Sum().ToString("C2");
            }
        }

        [DisplayName("Total Renda Líquida")]
        public String TotalRendaLiquida
        {
            get
            {
                return PessoaRendasVM.Select(x => x.RendaLiquida).Sum().ToString("C2");
            }
        }

        public override string ToString()
        {
            return Nome;
        }
    }
}
