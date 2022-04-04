using ControlFinWeb.Dominio.ObjetoValor;
using System;

namespace ControlFinWeb.Dominio.Entidades
{
    public class AlertaContas
    {
        public AlertaContas()
        {
            TipoAlertaContas = TipoAlertaContas.Aviso;
        }

        public TipoAlertaContas TipoAlertaContas { get; set; }
        public String Mensagem { get; set; }
        public Parcela ContaPagamento { get; set; }
        public String DescricaoCompleta
        {
            get
            {
                return String.Format("{0}-{1}- Parcela {2}", ContaPagamento.Conta.Codigo, ContaPagamento.Conta.Nome, ContaPagamento.Numero);
            }
        }
    }
}
