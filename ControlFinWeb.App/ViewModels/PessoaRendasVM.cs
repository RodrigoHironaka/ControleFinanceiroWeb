using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ControlFinWeb.App.ViewModels
{
    public class PessoaRendasVM
    {
        public Int64 Id { get; set; }
        public Decimal RendaBruta { get; set; }
        public Decimal RendaLiquida { get; set; }
        public Int64 TipoRendaId { get; set; }
        public RendaVM TipoRendaVM { get; set; }
        public bool Visivel { get; set; }
        public override string ToString()
        {
            return string.Format("{0} - Renda Bruta: {1} - Renda Liquida: {2}", TipoRendaVM.Nome, RendaBruta.ToString("N2"), RendaLiquida.ToString("N2"));
        }
    }
}
