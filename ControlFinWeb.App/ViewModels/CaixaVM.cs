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
       
        public Int64 Id { get; set; }

        [DisplayName("Número")]
        public Int64 Numero { get; set; }

        [DisplayName("Valor Inicial")]
        public Decimal ValorInicial { get; set; }

        [DisplayName("Situação")]
        public SituacaoCaixa Situacao { get; set; }

        [DisplayName("Data Abertura")]
        public DateTime DataGeracao { get; set; }

        public IList<FluxoCaixaVM> FluxosCaixaVM { get; set; }

        public Decimal TotalDebito //saída
        {
            get
            {
                return FluxosCaixaVM != null ? FluxosCaixaVM.Where(x => x.DebitoCredito == DebitoCredito.Débito).Select(x => x.Valor).Sum() : 0;
            }
        }
        public Decimal TotalCredito //Entrada
        {
            get
            {
                return FluxosCaixaVM != null ? FluxosCaixaVM.Where(x => x.DebitoCredito == DebitoCredito.Crédito).Select(x => x.Valor).Sum() : 0;
            }
        }
        public Decimal BalancoFinal
        {
            get
            {
                return TotalCredito - TotalDebito;
            }
        }

        public String DescricaoCaixa
        {
            get
            {
                return String.Format("Caixa Nº {0:00000} - {1}", Numero, Situacao.ToString().ToUpper());
            }
        }
    }
}
