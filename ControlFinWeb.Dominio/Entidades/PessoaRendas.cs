using System;

namespace ControlFinWeb.Dominio.Entidades
{
    public class PessoaRendas
    {
        public virtual Int64 ID { get; set; }
        public virtual Decimal RendaBruta { get; set; }
        public virtual Decimal RendaLiquida { get; set; }
        public virtual Renda TipoRenda { get; set; }
        public virtual Pessoa Pessoa { get; set; }

        public override string ToString()
        {
            return string.Format("{0} - Renda Bruta: {1} - Renda Liquida: {2}", TipoRenda.Nome, RendaBruta.ToString("N2"), RendaLiquida.ToString("N2"));
        }
    }
}
