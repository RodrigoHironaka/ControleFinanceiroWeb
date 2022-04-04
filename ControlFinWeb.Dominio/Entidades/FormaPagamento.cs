using ControlFinWeb.Dominio.Entidades;
using ControlFinWeb.Dominio.ObjetoValor;
using System;

namespace ControlFinWeb.Dominio.Dominios
{
    public class FormaPagamento : Base
    {
        public FormaPagamento()
        {
            Situacao = Situacao.Ativo;
        }

        public virtual Int32 DiaVencimento { get; set; }
        public virtual Situacao Situacao { get; set; }
        public virtual SimNao PossuiFaturaMensal { get; set; }
        public virtual SimNao DebitoAutomatico { get; set; }
        
        public override string ToString()
        {
            return Nome;
        }
    }
}
