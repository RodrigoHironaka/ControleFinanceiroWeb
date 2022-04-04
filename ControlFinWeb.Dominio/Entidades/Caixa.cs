using ControlFinWeb.Dominio.ObjetoValor;
using System;
using System.Collections.Generic;

namespace ControlFinWeb.Dominio.Entidades
{
    public class Caixa
    {
        public Caixa()
        {
            FluxoCaixas = new List<FluxoCaixa>();
            Situacao = SituacaoCaixa.Aberto;
        }
        public override string ToString()
        {
            return Codigo.ToString();
        }
        public virtual Int64 Id { get; set; }
        public virtual Int64 Codigo { get; set; }
        public virtual DateTime DataAbertura { get; set; }
        public virtual DateTime DataFechamento { get; set; }
        public virtual Decimal ValorInicial { get; set; }
        public virtual SituacaoCaixa Situacao { get; set; }
        public virtual Pessoa Pessoa { get; set; }
        public virtual Usuario UsuarioAbertura { get; set; }
        public virtual Usuario UsuarioFechamento { get; set; }
        public virtual Decimal TotalEntrada { get; set; }
        public virtual Decimal TotalSaida { get; set; }
        public virtual Decimal BalancoFinal { get; set; }
        public virtual IList<FluxoCaixa> FluxoCaixas { get; set; }

    }
}
