using ControlFinWeb.Dominio.Dominios;
using ControlFinWeb.Dominio.ObjetoValor;
using System;

namespace ControlFinWeb.Dominio.Entidades
{
    public class Parcela
    {
        public Parcela()
        {
            SituacaoParcela = SituacaoParcela.Pendente;
        }
        public virtual Int64 ID { get; set; }
        public virtual Int32 Numero { get; set; }
        public virtual Decimal ValorParcela { get; set; }
        public virtual DateTime? DataVencimento { get; set; }
        public virtual DateTime? DataPagamento { get; set; }
        public virtual Decimal JurosPorcentual { get; set; }
        public virtual Decimal JurosValor { get; set; } 
        public virtual Decimal DescontoPorcentual { get; set; }
        public virtual Decimal DescontoValor { get; set; }
        public virtual Decimal ValorReajustado { get; set; }
        public virtual Decimal ValorPago { get; set; }
        public virtual Decimal ValorAberto { get; set; }
        public virtual SituacaoParcela SituacaoParcela { get; set; }
        public virtual FormaPagamento FormaPagamento { get; set; }
        public virtual Conta Conta { get; set; }

    }
}
