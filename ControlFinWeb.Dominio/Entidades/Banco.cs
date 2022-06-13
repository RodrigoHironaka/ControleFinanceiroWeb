using ControlFinWeb.Dominio.ObjetoValor;
using System;
using Utils.Extensions.Enums;

namespace ControlFinWeb.Dominio.Entidades
{
    public class Banco : Base
    {
        public Banco()
        {
            Situacao = Situacao.Ativo;
        }
       
        public virtual TipoCartao TipoContaBanco { get; set; }
        public virtual Situacao Situacao { get; set; }
        public virtual Pessoa PessoaRefBanco { get; set; }

        public virtual String DadosCompletos
        {
            get { return String.Format("{0} ({1}) - {2}", Nome, TipoContaBanco.GetDisplayName(), PessoaRefBanco.Nome); }
        }
    }
}
