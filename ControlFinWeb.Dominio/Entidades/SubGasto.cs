using ControlFinWeb.Dominio.ObjetoValor;
using System;

namespace ControlFinWeb.Dominio.Entidades
{
    public class SubGasto : Base
    {
        public SubGasto()
        {
            Situacao = Situacao.Ativo;
        }
        public override string ToString()
        {
            return String.Format("{0}/{1}", Gasto.Nome, Nome);
        }

        public virtual Gasto Gasto { get; set; }
        public virtual Situacao Situacao { get; set; }
    }
}
