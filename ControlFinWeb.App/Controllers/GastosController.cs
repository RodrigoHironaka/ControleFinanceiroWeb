using AutoMapper;
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
        private readonly IMapper Mapper;

        public GastosController(RepositorioGasto repositorio, IMapper mapper)
        {
            Repositorio = repositorio;
            Mapper = mapper;
        }

        Gasto gasto = new Gasto();
        GastoVM gastoVM = new GastoVM();

        public IActionResult Index()
        {
            IEnumerable<Gasto> gasto = Repositorio.ObterTodos();
            List<GastoVM> gastoVM = Mapper
                .Map<List<GastoVM>>(gasto);
            return View(gastoVM);
        }

        public IActionResult Editar(Int64 Id = 0)
        {
            if (Id > 0)
            {
                gasto = Repositorio.ObterPorId(Id);
                gastoVM = Mapper.Map<GastoVM>(gasto);
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
                    gasto = Repositorio.ObterPorId(gastoVM.Id);
                    gasto = Mapper.Map(gastoVM, gasto);
                    gasto.UsuarioAlteracao = Configuracao.Usuario;
                    Repositorio.Alterar(gasto);
                }
                else
                {
                    gasto = Mapper.Map(gastoVM, gasto);
                    gasto.UsuarioCriacao = Configuracao.Usuario;
                    Repositorio.Salvar(gasto);
                }
                return new EmptyResult();
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
