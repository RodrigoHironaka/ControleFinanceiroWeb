using ControlFinWeb.Dominio.Dominios;
using ControlFinWeb.Dominio.ObjetoValor;
using System;
using System.Collections.Generic;

namespace ControlFinWeb.Dominio.Entidades
{
    public class CartaoCredito : Base
    {
        public CartaoCredito()
        {
            CartoesCreditoItens = new List<CartaoCreditoItens>();
            SituacaoFatura = SituacaoFatura.Aberta;
        }
        public virtual DateTime MesAnoReferencia { get; set; }
        public virtual Decimal ValorFatura { get; set; }
        public virtual SituacaoFatura SituacaoFatura { get; set; }
        public virtual FormaPagamento Cartao { get; set; }
        public virtual IList<CartaoCreditoItens> CartoesCreditoItens { get; set; }

        public virtual string DescricaoCompleta
        {
            get
            {
                return string.Format("{0} - {1:MMyyyy}", Cartao, MesAnoReferencia);
            }
        }
       
    }
}
