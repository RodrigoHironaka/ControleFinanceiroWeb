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
        public Parcela Parcela { get; set; }
        public String DescricaoCompleta
        {
            get
            {
                return String.Format("{0}-{1}- Parcela {2}", Parcela.Conta.Codigo, Parcela.Conta.Nome, Parcela.ParcelaDe);
            }
        }
    }
}
