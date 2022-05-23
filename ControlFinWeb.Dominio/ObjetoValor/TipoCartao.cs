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
        CC, //ContaCorrente
        [Display(Name = "Conta Poupança")]
        CP, //ContaPoupança
        [Display(Name = "Conta Salário")]
        CS, //ContaSalario
        Ticket,
        [Display(Name = "Vale Transporte")]
        ValeTransporte,
        Outros
    }
}
