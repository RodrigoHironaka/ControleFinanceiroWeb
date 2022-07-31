using ControlFinWeb.Dominio.Interfaces;
using System;

namespace ControlFinWeb.Dominio.Entidades
{
    public class PessoaRendas
    {
        public virtual Int64 Id { get; set; }
        public virtual Decimal RendaBruta { get; set; }
        public virtual Decimal RendaLiquida { get; set; }
        public virtual Renda TipoRenda { get; set; }
        public virtual Pessoa Pessoa { get; set; }
    
    }
}
