using ControlFinWeb.Dominio.ObjetoValor;
using System.Collections.Generic;

namespace ControlFinWeb.Dominio.Entidades
{
    public class Gasto : Base
    {
        public Gasto()
        {
            Situacao = Situacao.Ativo;
        }
        public override string ToString()
        {
            return Nome;
        }
        public virtual Situacao Situacao { get; set; }

    }
}
