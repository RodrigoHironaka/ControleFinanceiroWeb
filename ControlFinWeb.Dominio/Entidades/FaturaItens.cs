using System;
using System.Reflection.Metadata.Ecma335;

namespace ControlFinWeb.Dominio.Entidades
{
    public class FaturaItens : Base, ICloneable
    {
        public virtual String CodigoItemRelacionado { get; set; }
        public virtual String QuantidadeRelacionado { get; set; }

        public virtual Decimal Valor { get; set; }
        public virtual DateTime? DataCompra { get; set; }
        public virtual SubGasto SubGasto { get; set; }
        public virtual Pessoa Pessoa { get; set; }
        public virtual Fatura Fatura { get; set; }

        public override string ToString()
        {
            return $"[{Id}] - {Nome}";
        }

        public virtual object Clone()
        {
            var clone = (FaturaItens)this.MemberwiseClone();
            clone.SubGasto = (SubGasto)clone.SubGasto?.Clone();
            clone.Pessoa = (Pessoa)clone.Pessoa?.Clone();
            clone.Fatura = (Fatura)clone.Fatura?.Clone();
            return clone;
        }
    }
}
