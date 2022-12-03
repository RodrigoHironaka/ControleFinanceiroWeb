using ControlFinWeb.Dominio.ObjetoValor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ControlFinWeb.App.ViewModels
{
    public class FiltroParcelasVM
    {
        public FiltroParcelasVM()
        {
            DataInicio = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DataFinal = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
        }

        [DisplayName("Início")]
        //[Range(typeof(DateTime), "01/01/1980", "31/12/5000", ErrorMessage = "Data Inválida!")]
        //[Required(ErrorMessage = "Campo Obrigatório")]
        public DateTime? DataInicio { get; set; }

        [DisplayName("Início")]
        //[Range(typeof(DateTime), "01/01/1980", "31/12/5000", ErrorMessage = "Data Inválida!")]
        //[Required(ErrorMessage = "Campo Obrigatório")]
        public DateTime? DataFinal { get; set; }

        [DisplayName("Tipo Conta")]
        public TipoConta? TipoConta { get; set; }

        [DisplayName("Período da Conta")]
        public PeriodoConta? PeriodoConta { get; set; }

        [DisplayName("Conta ou Fatura")]
        public ContaFatura? ContaFatura { get; set; }

        [DisplayName("Situação Parcela")]
        public String SituacaoParcela { get; set; }

        //[DisplayName("Pesquisa Geral")]
        //public String PesquisaGeral { get; set; }

        [DisplayName("Pessoa Referenciada")]
        public Int64 PessoaId { get; set; }
        public PessoaVM PessoaVM { get; set; }

        public IList<ParcelaVM> Parcelas { get; set; }
    }
}
