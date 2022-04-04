using System;

namespace ControlFinWeb.Dominio.Entidades
{
    public class CartaoCreditoItens : Base
    {
        public virtual Decimal Valor { get; set; }
        public virtual String NumeroParcelas { get; set; }
        public virtual DateTime? DataCompra { get; set; }
        public virtual SubGasto SubGasto { get; set; }
        public virtual Pessoa Pessoa { get; set; }
        public virtual CartaoCredito CartaoCredito { get; set; }
    }
}
