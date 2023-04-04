using ControlFinWeb.Dominio.Interfaces;
using ControlFinWeb.Dominio.ObjetoValor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ControlFinWeb.Dominio.Entidades
{
    public class Caixa : IEntidade, ICloneable
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

        public virtual object Clone()
        {
            var clone = (Caixa)this.MemberwiseClone();
            clone.UsuarioCriacao = (Usuario)clone.UsuarioCriacao?.Clone();
            clone.UsuarioAlteracao = (Usuario)clone.UsuarioAlteracao?.Clone();
            return clone;
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

        public virtual Decimal _TotalDebito //saída
        {
            get
            {
                return FluxosCaixa.Where(x => x.DebitoCredito == DebitoCredito.Débito).Select(x => x.Valor).Sum();
            } 
        }
        public virtual Decimal _TotalCredito //Entrada
        {
            get
            {
                return FluxosCaixa.Where(x => x.DebitoCredito == DebitoCredito.Crédito).Select(x => x.Valor).Sum();
            }
        }
        public virtual Decimal _BalancoFinal
        {
            get
            {
                return _TotalCredito - _TotalDebito;
            }
        }

    }
}
