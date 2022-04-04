using ControlFinWeb.Dominio.ObjetoValor;

namespace ControlFinWeb.Dominio.Entidades
{
    public class Renda : Base 
    {
        public Renda()
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
