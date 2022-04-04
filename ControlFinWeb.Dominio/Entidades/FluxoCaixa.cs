using ControlFinWeb.Dominio.Dominios;
using ControlFinWeb.Dominio.ObjetoValor;
using System;

namespace ControlFinWeb.Dominio.Entidades
{
    public class FluxoCaixa : Base
    {
        public override string ToString()
        {
            return Nome;
        }
        public virtual Decimal Valor { get; set; }
        public virtual EntradaSaida TipoFluxo { get; set; }
        public virtual Conta Conta { get; set; }
        public virtual Caixa Caixa { get; set; }
        public virtual FormaPagamento FormaPagamento { get; set; }

    }
}
