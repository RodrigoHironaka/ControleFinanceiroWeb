using System;

namespace ControlFinWeb.Dominio.Entidades
{
    public class FaturaItens : Base
    {
        public virtual Decimal Valor { get; set; }
        public virtual DateTime? DataCompra { get; set; }
        public virtual SubGasto SubGasto { get; set; }
        public virtual Pessoa Pessoa { get; set; }
        public virtual Fatura CartaoCredito { get; set; }
    }
}
