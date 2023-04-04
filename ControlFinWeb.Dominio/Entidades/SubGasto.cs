using ControlFinWeb.Dominio.ObjetoValor;
using System;

namespace ControlFinWeb.Dominio.Entidades
{
    public class SubGasto : Base, ICloneable
    {
        public SubGasto()
        {
            Situacao = Situacao.Ativo;
        }

        public virtual Gasto Gasto { get; set; }
        public virtual Situacao Situacao { get; set; }

        public virtual String _DescricaoCompleta
        {
            get
            {
                return String.Format("{0}/{1}", Gasto?.Nome, Nome);
            }
        }

        public virtual object Clone()
        {
            var clone = (SubGasto)this.MemberwiseClone();
            clone.Gasto = (Gasto)this.Gasto?.Clone();
            return clone;
        }
    }
}
