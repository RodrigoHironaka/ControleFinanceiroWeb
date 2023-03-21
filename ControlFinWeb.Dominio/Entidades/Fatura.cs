using ControlFinWeb.Dominio.Dominios;
using ControlFinWeb.Dominio.ObjetoValor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ControlFinWeb.Dominio.Entidades
{
    public class Fatura : Base
    {
        public Fatura()
        {
            FaturaItens = new List<FaturaItens>();
            SituacaoFatura = SituacaoFatura.Aberta;
        }
        public virtual DateTime MesAnoReferencia { get; set; }
        public virtual DateTime DataFechamento { get; set; }
        public virtual SituacaoFatura SituacaoFatura { get; set; }
        public virtual Cartao Cartao { get; set; }
        public virtual Pessoa Pessoa { get; set; }
        public virtual IList<FaturaItens> FaturaItens { get; set; }

        public virtual string DescricaoCompleta
        {
            get
            {
                if(Cartao != null)
                    return string.Format("{0} - {1:MMyyyy}", Cartao.Nome, MesAnoReferencia);
                return string.Empty;
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
