using ControlFinWeb.Dominio.ObjetoValor;
using System;

namespace ControlFinWeb.Dominio.Entidades
{
    public class Usuario : Base
    {
        public Usuario()
        {
            Situacao = Situacao.Ativo;
            Autorizado = SimNao.Não;
        }
        public virtual String Email { get; set; }
        public virtual String Senha { get; set; }
        public virtual String Imagem { get; set; }
        public virtual SimNao Autorizado { get; set; }
        public virtual TipoUsuario TipoUsuario { get; set; }
        public virtual Situacao Situacao { get; set; }
    }
}
