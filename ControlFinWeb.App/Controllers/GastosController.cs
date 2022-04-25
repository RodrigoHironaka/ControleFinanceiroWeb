using ControlFinWeb.App.AutoMapper;
using ControlFinWeb.App.Utilitarios;
using ControlFinWeb.App.ViewModels;
using ControlFinWeb.Dominio.Entidades;
using ControlFinWeb.Repositorio.Repositorios;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ControlFinWeb.App.Controllers
{
    public class GastosController : Controller
    {
        private readonly RepositorioGasto Repositorio;

        public GastosController(RepositorioGasto repositorio)
        {
            Repositorio = repositorio;
        }

        Gasto gasto = new Gasto();
        GastoVM gastoVM = new GastoVM();

        public IActionResult Index()
        {
            var gastosVM = new List<GastoVM>();
            var gastos = Repositorio.ObterTodos();
            foreach (Gasto gasto in gastos)
                gastosVM.Add(AutoMapperConfig<Gasto, GastoVM>.Mapear(gasto));
            return View(gastosVM);
        }

        public IActionResult Editar(Int64 Id = 0)
        {
            if (Id > 0)
            {
                gasto = Repositorio.ObterPorId(Id);
                gastoVM = AutoMapperConfig<Gasto, GastoVM>.Mapear(gasto);
            }
            return View(gastoVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(GastoVM gastoVM)
        {
            if (ModelState.IsValid)
            {
                if (gastoVM.Id > 0)
                {
                    var gastoRep = Repositorio.ObterPorId(gastoVM.Id);
                    gasto = AutoMapperConfig<GastoVM, Gasto>.Mapear(gastoVM);
                    gasto.DataGeracao = gastoRep.DataGeracao;
                    gasto.UsuarioCriacao = gastoRep.UsuarioCriacao;
                    gasto.UsuarioAlteracao = Configuracao.Usuario;
                    Repositorio.Alterar(gasto);
                }
                else
                {
                    gasto = AutoMapperConfig<GastoVM, Gasto>.Mapear(gastoVM);
                    gasto.UsuarioCriacao = Configuracao.Usuario;
                    Repositorio.Salvar(gasto);
                }
                return RedirectToAction("Index");
            }
            return View(gastoVM);
        }

        [HttpPost]
        public JsonResult Deletar(Int64 Id)
        {
            gasto = Repositorio.ObterPorId(Id);
            Repositorio.Excluir(gasto);
            return Json(gasto.Nome + "excluído com sucesso");
        }
    }
}
