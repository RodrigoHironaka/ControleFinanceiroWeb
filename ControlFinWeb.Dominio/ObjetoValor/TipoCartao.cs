using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlFinWeb.Dominio.ObjetoValor
{
    public enum TipoCartao
    {
        [Display(Name = "Conta Corrente")]
        ContaCorrente, 
        [Display(Name = "Conta Poupança")]
        ContaPoupanca, 
        [Display(Name = "Conta Salário")]
        ContaSalario, 
        Ticket,
        [Display(Name = "Vale Transporte")]
        ValeTransporte,
        Outros
    }
}
