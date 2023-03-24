using ControlFinWeb.Dominio.ObjetoValor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlFinWeb.Dominio.Entidades
{
    public class Cartao : Base, ICloneable
    {
        public virtual Decimal LimiteFatura { get; set; }
        public virtual Int32 DiaVencimento { get; set; }
        public virtual DateTime? VencimentoCartao { get; set; }
        public virtual Situacao Situacao { get; set; }
        public virtual Banco Banco { get; set; }

        public virtual object Clone()
        {
            var clone = (Cartao)this.MemberwiseClone();
            clone.Banco = (Banco)clone.Banco.Clone();
            return clone;
        }
    }
}
