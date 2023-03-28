using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ControlFinWeb.App.ViewModels
{
    public class FiltroContaBancariaVM
    {
        public FiltroContaBancariaVM()
        {
            if(Ano == 0)
                Ano = DateTime.Now.Year;
            if(Mes == 0)
                Mes = DateTime.Now.Month;
        }

        [Required(ErrorMessage = "Campo Obrigatório")]
        public Int32 Ano { get; set; }
        [Required(ErrorMessage = "Campo Obrigatório")]
        public Int32 Mes { get; set; }
        public Int64 BancoId { get; set; }
        public BancoVM BancoVM { get; set; }

        public IList<ContaBancariaVM> ContasBancarias { get; set; }
    }
}
