using ControlFinWeb.Dominio.Entidades;
using ControlFinWeb.Dominio.Entidades.Logs;
using Microsoft.AspNetCore.Mvc;
using ObjectsComparer;
using System;
using System.Collections.Generic;
using System.Text;
using ControlFinWeb.Repositorio.Repositorios;
using LinqKit;
using ControlFinWeb.App.Utilitarios;
using ControlFinWeb.App.ViewModels.Acesso;
using System.Linq;

namespace ControlFinWeb.App.Controllers
{
    public class LogModificacoesController : Controller
    {
        private readonly RepositorioLogModificacao Repositorio;

        public LogModificacoesController(RepositorioLogModificacao repositorio)
        {
            Repositorio = repositorio;
        }
        public ActionResult Logs(LogModificacaoVM logModificacaoVM)
        {
            var predicado = Repositorio.CriarPredicado();
            predicado = predicado.And(x => x.UsuarioCriacao.Id == Configuracao.Usuario.Id);
            predicado = predicado.And(x => x.DataGeracao >= logModificacaoVM.DataInicio);
            predicado = predicado.And(x => x.DataGeracao <= logModificacaoVM.DataFinal);
            if(!String.IsNullOrEmpty(logModificacaoVM.Chave))
                predicado = predicado.And(x => x.Chave.Contains(logModificacaoVM.Chave));

            logModificacaoVM.Logs = Repositorio.ObterPorParametros(predicado).ToList();
            return View(logModificacaoVM);
        }

    }
}
