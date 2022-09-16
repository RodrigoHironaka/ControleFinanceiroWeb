using System;

namespace ControlFinWeb.Dominio.Entidades
{
    public class FaturaCartaoCreditoItens : Base
    {
        public virtual Decimal Valor { get; set; }
        public virtual DateTime? DataCompra { get; set; }
        public virtual SubGasto SubGasto { get; set; }
        public virtual Pessoa Pessoa { get; set; }
        public virtual FaturaCartaoCredito CartaoCredito { get; set; }
    }
}
