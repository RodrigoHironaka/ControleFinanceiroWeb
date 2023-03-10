using ControlFinWeb.Dominio.Interfaces;
using ControlFinWeb.Dominio.ObjetoValor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ControlFinWeb.Dominio.Entidades
{
    public class Caixa : IEntidade
    {
        public Caixa()
        {
            FluxosCaixa = new List<FluxoCaixa>();
            Situacao = SituacaoCaixa.Aberto;
        }
        public override string ToString()
        {
            return Id.ToString();
        }

        public virtual Int64 Id { get; set; }
        public virtual Int64 Numero { get; set; }
        public virtual Decimal ValorInicial { get; set; } 
        public virtual SituacaoCaixa Situacao { get; set; }
        public virtual DateTime DataGeracao { get; set; }
        public virtual DateTime? DataAlteracao { get; set; }
        public virtual Usuario UsuarioCriacao { get; set; }
        public virtual Usuario UsuarioAlteracao { get; set; }
        public virtual IList<FluxoCaixa> FluxosCaixa { get; set; }

        public virtual Decimal TotalDebito //saída
        {
            get
            {
                return FluxosCaixa.Where(x => x.DebitoCredito == DebitoCredito.Débito).Select(x => x.Valor).Sum();
            } 
        }
        public virtual Decimal TotalCredito //Entrada
        {
            get
            {
                return FluxosCaixa.Where(x => x.DebitoCredito == DebitoCredito.Crédito).Select(x => x.Valor).Sum();
            }
        }
        public virtual Decimal BalancoFinal
        {
            get
            {
                return TotalCredito - TotalDebito;
            }
        }

    }
}
