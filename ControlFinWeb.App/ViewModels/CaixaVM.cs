using ControlFinWeb.Dominio.ObjetoValor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ControlFinWeb.App.ViewModels
{
    public class CaixaVM
    {
        public virtual Int64 Id { get; set; }

        [DisplayName("Código")]
        public virtual Int64 Codigo { get; set; }

        [DisplayName("Valor Inicial")]
        public virtual Decimal ValorInicial { get; set; }

        [DisplayName("Situação")]
        public virtual SituacaoCaixa Situacao { get; set; }
        
        public virtual IList<FluxoCaixaVM> FluxosCaixaVM { get; set; }

        public virtual Decimal TotalDebito //saída
        {
            get
            {
                return FluxosCaixaVM.Where(x => x.DebitoCredito == DebitoCredito.Débito).Select(x => x.Valor).Sum();
            }
        }
        public virtual Decimal TotalCredito //Entrada
        {
            get
            {
                return FluxosCaixaVM.Where(x => x.DebitoCredito == DebitoCredito.Crédito).Select(x => x.Valor).Sum();
            }
        }
        public virtual Decimal BalancoFinal
        {
            get
            {
                return ValorInicial + TotalCredito - TotalDebito;
            }
        }

    }
}
