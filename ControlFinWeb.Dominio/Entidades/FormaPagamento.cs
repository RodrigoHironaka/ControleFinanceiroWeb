using ControlFinWeb.Dominio.Entidades;
using ControlFinWeb.Dominio.ObjetoValor;
using System;

namespace ControlFinWeb.Dominio.Dominios
{
    public class FormaPagamento : Base, ICloneable
    {
        public FormaPagamento()
        {
            Situacao = Situacao.Ativo;
        }
        public virtual Situacao Situacao { get; set; }
        public virtual bool DebitoAutomatico { get; set; }
        public virtual bool GerarRegistroFatura { get; set; }

        public virtual object Clone()
        {
           return (FormaPagamento)this.MemberwiseClone();
        }

        public override string ToString()
        {
            return Nome;
        }
    }
}
