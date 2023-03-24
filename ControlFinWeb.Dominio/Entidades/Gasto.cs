using ControlFinWeb.Dominio.ObjetoValor;
using System;
using System.Collections.Generic;

namespace ControlFinWeb.Dominio.Entidades
{
    public class Gasto : Base, ICloneable
    {
        public Gasto()
        {
            Situacao = Situacao.Ativo;
        }
        public override string ToString()
        {
            return Nome;
        }

        public virtual object Clone()
        {
            return (Gasto)this.MemberwiseClone();
        }

        public virtual Situacao Situacao { get; set; }

    }
}
