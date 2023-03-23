using ControlFinWeb.Dominio.Entidades;
using ControlFinWeb.Dominio.Entidades.Logs;
using Microsoft.AspNetCore.Mvc;
using ObjectsComparer;
using System;
using System.Collections.Generic;
using System.Text;
using ControlFinWeb.Repositorio.Repositorios;

namespace ControlFinWeb.App.Controllers
{
    public class LogModificacoesController : Controller
    {
        private readonly RepositorioLogModificacao Repositorio;

        public LogModificacoesController(RepositorioLogModificacao repositorio)
        {
            Repositorio = repositorio;
        }
        public ActionResult Index()
        {
            return View();
        }

    }
}
