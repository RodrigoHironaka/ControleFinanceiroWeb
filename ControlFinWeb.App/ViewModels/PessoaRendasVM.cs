using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ControlFinWeb.App.ViewModels
{
    public class PessoaRendasVM
    {
        public Int64 Id { get; set; }

        [DisplayName("Renda Bruta")]
        [DataType(DataType.Currency)]
        public Decimal RendaBruta { get; set; }

        [DisplayName("Renda Líquida")]
        [DataType(DataType.Currency)]
        public Decimal RendaLiquida { get; set; }

        [Range(1, Int64.MaxValue, ErrorMessage = "Informe um Gasto")]
        public Int64 TipoRendaId { get; set; }
        public RendaVM TipoRendaVM { get; set; }

        public PessoaVM PessoaVM { get; set; }

        public override string ToString()
        {
            return string.Format("{0} - Renda Bruta: {1} - Renda Liquida: {2}", TipoRendaVM.Nome, RendaBruta.ToString("N2"), RendaLiquida.ToString("N2"));
        }
    }
}
