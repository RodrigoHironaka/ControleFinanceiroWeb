using ControlFinWeb.Dominio.ObjetoValor;
using System;
using Utils.Extensions.Enums;

namespace ControlFinWeb.Dominio.Entidades
{
    public class Banco : Base, ICloneable
    {
        public Banco()
        {
            Situacao = Situacao.Ativo;
        }
       
        public virtual TipoCartao TipoContaBanco { get; set; }
        public virtual Situacao Situacao { get; set; }
        public virtual Pessoa PessoaRefBanco { get; set; }

        public virtual String _DadosCompletos
        {
            get { return String.Format("{0} ({1}) - {2}", Nome, TipoContaBanco.GetDisplayName(), PessoaRefBanco?.Nome); }
        }

        public virtual object Clone()
        {
            var clone = (Banco)this.MemberwiseClone();
            clone.PessoaRefBanco = (Pessoa)clone.PessoaRefBanco?.Clone();
            return clone;
        }
    }
}
