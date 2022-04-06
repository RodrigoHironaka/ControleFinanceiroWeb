using ControlFinWeb.Dominio.Entidades;
using System;

namespace ControlFinWeb.Dominio.Interfaces
{
    public interface IEntidade
    {
        public Int64 Id { get; set; }
        public  DateTime DataGeracao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public Usuario UsuarioCriacao { get; set; }
        public Usuario UsuarioAlteracao { get; set; }
    }
}
