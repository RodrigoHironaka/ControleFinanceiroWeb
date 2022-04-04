using ControlFinWeb.Dominio.Interfaces;
using System;

namespace ControlFinWeb.Dominio.Entidades
{
    public class Base : IEntidade
    {
        public Base()
        {

        }
        public virtual Int64 Id { get; set; }
        public virtual String Nome { get; set; }
        public virtual DateTime DataGeracao { get; set; }
        public virtual DateTime? DataAlteracao { get; set; }
        public virtual Usuario UsuarioCriacao { get; set; }
        public virtual Usuario UsuarioAlteracao { get; set; }
    }
}
