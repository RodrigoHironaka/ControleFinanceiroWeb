using ControlFinWeb.Dominio.Dominios;
using ControlFinWeb.Dominio.ObjetoValor;
using System;

namespace ControlFinWeb.Dominio.Entidades
{
    public class ContaBancaria : Base
    {
        public virtual Int64 Codigo { get; set; }
        public virtual Decimal Valor { get; set; }
        public virtual DateTime? DataRegistro { get; set; }
        public virtual TransacaoBancaria TransacaoBancaria{ get; set; }
        public virtual EntradaSaida Situacao { get; set; }
        public virtual Caixa Caixa { get; set; }
        public virtual Banco Banco { get; set; }
    }
}
