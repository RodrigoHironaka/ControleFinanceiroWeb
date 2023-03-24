using ControlFinWeb.Dominio.ObjetoValor;
using System;

namespace ControlFinWeb.Dominio.Entidades
{
    public class Renda : Base, ICloneable
    {
        public Renda()
        {
            Situacao = Situacao.Ativo;
        }

        public override string ToString()
        {
            return Nome;
        }

        public virtual object Clone()
        {
            return (Renda)this.MemberwiseClone();
        }

        public virtual Situacao Situacao { get; set; }

    }
}
