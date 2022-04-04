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

        public virtual Decimal ValorTotalBruto { get; set; }
        public virtual Decimal ValorTotalLiquido { get; set; }
        public virtual SimNao UsarRendaParaCalculos { get; set; }
        public virtual Situacao Situacao { get; set; }
        public virtual IList<PessoaRendas> PessoaRendas { get; set; }

        public virtual Decimal TotalRendaBruta
        {
            get 
            {
                return PessoaRendas.Select(x => x.RendaBruta).Sum();
            }
        }

        public virtual Decimal TotalRendaLiquida
        {
            get
            {
                return PessoaRendas.Select(x => x.RendaLiquida).Sum();
            }
        }

        public override string ToString()
        {
            return Nome;
        }
    }
}
