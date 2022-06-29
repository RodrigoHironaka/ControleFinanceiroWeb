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

        public virtual Gasto Gasto { get; set; }
        public virtual Situacao Situacao { get; set; }

        public virtual String DescricaoCompleta
        {
            get
            {
                return String.Format("{0}/{1}", Gasto.Nome, Nome);
            }
        }
    }
}
