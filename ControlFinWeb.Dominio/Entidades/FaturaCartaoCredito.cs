using ControlFinWeb.Dominio.Dominios;
using ControlFinWeb.Dominio.ObjetoValor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ControlFinWeb.Dominio.Entidades
{
    public class FaturaCartaoCredito : Base
    {
        public FaturaCartaoCredito()
        {
            FaturaItens = new List<FaturaCartaoCreditoItens>();
            SituacaoFatura = SituacaoFatura.Aberta;
        }
        public virtual DateTime MesAnoReferencia { get; set; }
        public virtual SituacaoFatura SituacaoFatura { get; set; }
        public virtual FormaPagamento Cartao { get; set; }
        public virtual IList<FaturaCartaoCreditoItens> FaturaItens { get; set; }

        public virtual string DescricaoCompleta
        {
            get
            {
                return string.Format("{0} - {1:MMyyyy}", Cartao, MesAnoReferencia);
            }
        }

        public virtual Decimal ValorFatura
        {
            get
            {
                return FaturaItens.Select(x => x.Valor).Sum();
            }
        }

    }
}
