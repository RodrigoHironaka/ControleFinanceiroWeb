using ControlFinWeb.Dominio.ObjetoValor;

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
