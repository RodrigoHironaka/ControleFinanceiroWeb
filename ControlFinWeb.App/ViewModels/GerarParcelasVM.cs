using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ControlFinWeb.App.ViewModels
{
    public class GerarParcelasVM
    {
        [DisplayName("Valor")]
        public Decimal ValorDigitado { get; set; }

        public Int32 Qtd { get; set; }

        [DisplayName("1º Vencimento")]
        public DateTime? PrimeiroVencimento { get; set; }

        [DisplayName("Replicar?")]
        public Boolean Replicar { get; set; }

        public String JsonParcelas { get; set; }
    }
}
