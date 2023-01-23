using ControlFinWeb.Dominio.Dominios;
using ControlFinWeb.Dominio.ObjetoValor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ControlFinWeb.Dominio.Entidades
{
    public class Conta : Base
    {
        public Conta()
        {
            Parcelas = new List<Parcela>();
            Arquivos = new List<Arquivo>();
            Situacao = SituacaoConta.Aberto;
        }
        public virtual TipoConta TipoConta { get; set; }
        public virtual PeriodoConta TipoPeriodo { get; set; }
        public virtual SituacaoConta Situacao { get; set; }
        public virtual DateTime? DataEmissao { get; set; }
        public virtual String NumeroDocumento { get; set; }
        public virtual SubGasto SubGasto { get; set; }
        public virtual FormaPagamento FormaCompra { get; set; }
        public virtual Pessoa Pessoa { get; set; }
        public virtual String Observacao { get; set; }
        public virtual IList<Parcela> Parcelas { get; set; }
        public virtual IList<Arquivo> Arquivos { get; set; }

        public virtual String DescricaoCompleta
        {
            get
            {
                return $"{Nome} [{Id}]";
            }
        }
    }
}
