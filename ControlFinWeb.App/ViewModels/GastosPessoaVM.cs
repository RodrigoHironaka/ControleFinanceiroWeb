using System.ComponentModel;
using System;
using ControlFinWeb.App.Utilitarios;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace ControlFinWeb.App.ViewModels
{
    public class GastosPessoaVM
    {
        [DisplayName("Conta/Fatura")]
        public String ContaFatura { get; set; }
        public DateTime Data { get; set; }

        [DisplayName("Descrição")]
        public String Descricao { get; set; }

        [DisplayName("Valor")]
        public String Valor { get; set; }

        [DisplayName("Pessoa")]
        public String Pessoa { get; set; }
        
        [DisplayName("Situação")]
        public String Situacao { get; set; }

    }
}
