using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ControlFinWeb.App.ViewModels
{
    public class PessoaRendasVM
    {
        public virtual Int64 Id { get; set; }
        public virtual Decimal RendaBruta { get; set; }
        public virtual Decimal RendaLiquida { get; set; }
        public virtual RendaVM TipoRendaVM { get; set; }
        public virtual PessoaVM PessoaVM { get; set; }

        public override string ToString()
        {
            return string.Format("{0} - Renda Bruta: {1} - Renda Liquida: {2}", TipoRendaVM.Nome, RendaBruta.ToString("N2"), RendaLiquida.ToString("N2"));
        }
    }
}
