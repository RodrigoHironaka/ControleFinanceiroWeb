using ControlFinWeb.Dominio.Dominios;
using ControlFinWeb.Dominio.ObjetoValor;
using System;
using System.Collections.Generic;

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
        public virtual Int64 Codigo { get; set; }
        public virtual TipoConta TipoConta { get; set; }
        public virtual TipoPeriodo TipoPeriodo { get; set; }
        public virtual SituacaoConta Situacao { get; set; }
        public virtual DateTime? DataEmissao { get; set; }
        public virtual Decimal? ValorTotal { get; set; }
        public virtual Int64? QtdParcelas { get; set; }
        public virtual Int64? NumeroDocumento { get; set; }
        public virtual SubGasto SubGasto { get; set; }
        public virtual FormaPagamento FormaCompra { get; set; }
        public virtual CartaoCredito FaturaCartaoCredito { get; set; }
        public virtual Pessoa Pessoa { get; set; }
        public virtual string Observacao { get; set; }
        public virtual IList<Parcela> Parcelas { get; set; }
        public virtual IList<Arquivo> Arquivos { get; set; }

        public override string ToString()
        {
            return String.Format("{0} - {1}", Codigo, Nome);
        }
    }
}
