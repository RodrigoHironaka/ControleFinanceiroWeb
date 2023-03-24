using ControlFinWeb.Dominio.Dominios;
using ControlFinWeb.Dominio.Interfaces;
using ControlFinWeb.Dominio.ObjetoValor;
using System;

namespace ControlFinWeb.Dominio.Entidades
{
    public class Parcela : IEntidade, ICloneable
    {
        public Parcela()
        {
            SituacaoParcela = SituacaoParcela.Pendente;
        }
        public virtual Int64 Id { get; set; }
        public virtual Int64 Numero { get; set; }   
        public virtual String ParcelaDe { get; set; }
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
        public virtual Fatura Fatura { get; set; }
        public virtual DateTime DataGeracao { get; set; }
        public virtual DateTime? DataAlteracao { get; set; }
        public virtual Usuario UsuarioCriacao { get; set; }
        public virtual Usuario UsuarioAlteracao { get; set; }

        public virtual object Clone()
        {
            var clone = (Parcela)this.MemberwiseClone();
            clone.FormaPagamento = (FormaPagamento)clone.FormaPagamento?.Clone();
            clone.Conta = (Conta)clone.Conta?.Clone();
            clone.Fatura = (Fatura)clone.Fatura?.Clone();
            clone.UsuarioCriacao = (Usuario)clone.UsuarioCriacao.Clone();
            clone.UsuarioAlteracao = (Usuario)clone.UsuarioAlteracao?.Clone();
            return clone;
        }
    }
}
