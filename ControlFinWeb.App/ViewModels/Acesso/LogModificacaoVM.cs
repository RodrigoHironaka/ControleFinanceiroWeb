using ControlFinWeb.Dominio.Entidades.Logs;
using NHibernate.Mapping;
using System;
using System.Collections.Generic;

namespace ControlFinWeb.App.ViewModels.Acesso
{
    public class LogModificacaoVM
    {
        public LogModificacaoVM()
        {
            DataInicio = DateTime.Now;
            DataFinal = DateTime.Now;
            Logs = new List<LogModificacao>();      
        }

        public DateTime DataInicio { get; set; }
        public DateTime DataFinal { get; set; }
        public String Chave { get; set; }
        public IList<LogModificacao> Logs { get; set; }
    }
}
