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
        public virtual Int64 Codigo { get; set; }
        public virtual TipoConta TipoConta { get; set; }
        public virtual PeriodoConta TipoPeriodo { get; set; }
        public virtual SituacaoConta Situacao { get; set; }
        public virtual DateTime? DataEmissao { get; set; }
        
        public virtual Int64? NumeroDocumento { get; set; }
        public virtual SubGasto SubGasto { get; set; }
        public virtual FormaPagamento FormaCompra { get; set; }
        public virtual FaturaCartaoCredito FaturaCartaoCredito { get; set; }
        public virtual Pessoa Pessoa { get; set; }
        public virtual string Observacao { get; set; }
        public virtual IList<Parcela> Parcelas { get; set; }
        public virtual IList<Arquivo> Arquivos { get; set; }

        public override string ToString()
        {
            return String.Format("{0} - {1}", Codigo, Nome);
        }

        public virtual Decimal? ValorTotalGeral 
        {
            get
            {
                return Parcelas.Select(x => x.ValorParcela).Sum();
            }
        }

        public virtual Decimal? ValorTotalAberto
        {
            get
            {
                return Parcelas.Select(x => x.ValorAberto).Sum();
            }
        }
        public virtual Int64? QtdTotalParcelas 
        {
            get
            {
                return Parcelas.Count();
            }
        }
        public virtual Int64? QtdTotalParcelasEmAberto
        {
            get
            {
                return Parcelas.Where(x => x.ValorAberto > 0).Count();
            }
        }
    }
}
