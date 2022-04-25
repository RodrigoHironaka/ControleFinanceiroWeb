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
    public class SubGastosController : Controller
    {
        private readonly RepositorioSubGasto Repositorio;

        public SubGastosController(RepositorioSubGasto repositorio)
        {
            Repositorio = repositorio;
        }

        SubGasto subGasto = new SubGasto();
        SubGastoVM subGastoVM = new SubGastoVM();

        public IActionResult Index()
        {
            var subGastosVM = new List<SubGastoVM>();
            var subGastos = Repositorio.ObterTodos();
            foreach (SubGasto subGasto in subGastos)
                subGastosVM.Add(AutoMapperConfig<SubGasto, SubGastoVM>.Mapear(subGasto));
            return View(subGastosVM);
        }

        public IActionResult Editar(Int64 Id = 0)
        {
            if (Id > 0)
            {
                subGasto = Repositorio.ObterPorId(Id);
                subGastoVM = AutoMapperConfig<SubGasto, SubGastoVM>.Mapear(subGasto);
            }
            return View(subGastoVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(SubGastoVM subGastoVM)
        {
            if (ModelState.IsValid)
            {
                if (subGastoVM.Id > 0)
                {
                    var gastoRep = Repositorio.ObterPorId(subGastoVM.Id);
                    subGasto = AutoMapperConfig<SubGastoVM, SubGasto>.Mapear(subGastoVM);
                    subGasto.DataGeracao = gastoRep.DataGeracao;
                    subGasto.UsuarioCriacao = gastoRep.UsuarioCriacao;
                    subGasto.UsuarioAlteracao = Configuracao.Usuario;
                    Repositorio.Alterar(subGasto);
                }
                else
                {
                    subGasto = AutoMapperConfig<SubGastoVM, SubGasto>.Mapear(subGastoVM);
                    subGasto.UsuarioCriacao = Configuracao.Usuario;
                    Repositorio.Salvar(subGasto);
                }
                return RedirectToAction("Index");
            }
            return View(subGastoVM);
        }

        [HttpPost]
        public JsonResult Deletar(Int64 Id)
        {
            subGasto = Repositorio.ObterPorId(Id);
            Repositorio.Excluir(subGasto);
            return Json(subGasto.Nome + "excluído com sucesso");
        }
    }
}
