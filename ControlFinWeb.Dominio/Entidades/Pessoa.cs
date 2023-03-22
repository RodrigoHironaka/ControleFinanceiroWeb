using ControlFinWeb.Dominio.ObjetoValor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ControlFinWeb.Dominio.Entidades
{
    public class Pessoa : Base
    {
        public virtual Situacao Situacao { get; set; }
        public virtual Decimal Renda { get; set; }
    }
}
