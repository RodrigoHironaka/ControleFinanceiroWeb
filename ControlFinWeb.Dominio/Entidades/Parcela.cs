using ControlFinWeb.Dominio.Dominios;
using ControlFinWeb.Dominio.Interfaces;
using ControlFinWeb.Dominio.ObjetoValor;
using System;

namespace ControlFinWeb.Dominio.Entidades
{
    public class Parcela : IEntidade
    {
        public Parcela()
        {
            SituacaoParcela = SituacaoParcela.Pendente;
        }
        public virtual Int64 Id { get; set; }
        public virtual Int32 NumeroParcela { get; set; }
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
        public virtual String Observacao { get; set; }
        public virtual SituacaoParcela SituacaoParcela { get; set; }
        public virtual FormaPagamento FormaPagamento { get; set; }
        public virtual Conta Conta { get; set; }
        public virtual DateTime DataGeracao { get; set; }
        public virtual DateTime? DataAlteracao { get; set; }
        public virtual Usuario UsuarioCriacao { get; set; }
        public virtual Usuario UsuarioAlteracao { get; set; }
    }
}
