using ControlFinWeb.Dominio.Dominios;
using ControlFinWeb.Dominio.ObjetoValor;
using System;

namespace ControlFinWeb.Dominio.Entidades
{
    public class ContaBancaria : Base, ICloneable
    {
        public virtual Decimal Valor { get; set; }
        public virtual DateTime? DataRegistro { get; set; }
        public virtual TransacaoBancaria TransacaoBancaria{ get; set; }
        public virtual EntradaSaida Situacao { get; set; }
        public virtual Caixa Caixa { get; set; }
        public virtual Banco Banco { get; set; }

        public virtual object Clone()
        {
            var clone = (ContaBancaria)this.MemberwiseClone();
            clone.Caixa = (Caixa)this.Caixa.Clone();
            clone.Banco = (Banco)this.Banco.Clone();
            return clone;
        }
    }
}
