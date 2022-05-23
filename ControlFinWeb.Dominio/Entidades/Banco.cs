using ControlFinWeb.Dominio.ObjetoValor;
using System;

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
    }
}
