using ControlFinWeb.Dominio.ObjetoValor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ControlFinWeb.Dominio.Entidades
{
    public class Pessoa : Base
    {
        public Pessoa()
        {
            PessoaRendas = new List<PessoaRendas>();
        }

        public virtual Situacao Situacao { get; set; }
        public virtual IList<PessoaRendas> PessoaRendas { get; set; }
    }
}
