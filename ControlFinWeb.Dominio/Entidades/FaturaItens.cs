using System;

namespace ControlFinWeb.Dominio.Entidades
{
    public class FaturaItens : Base
    {
        public virtual String CodigoItemRelacionado { get; set; }
        public virtual String QuantidadeRelacionado { get; set; }

        public virtual Decimal Valor { get; set; }
        public virtual DateTime? DataCompra { get; set; }
        public virtual SubGasto SubGasto { get; set; }
        public virtual Pessoa Pessoa { get; set; }
        public virtual Fatura Fatura { get; set; }
    }
}
