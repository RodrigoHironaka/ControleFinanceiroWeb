using App1.Repositorio.Configuracao;
using AutoMapper;
using ControlFinWeb.App.Utilitarios;
using ControlFinWeb.App.ViewModels;
using ControlFinWeb.Dominio.Entidades;
using ControlFinWeb.Dominio.ObjetoValor;
using ControlFinWeb.Repositorio.Repositorios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace ControlFinWeb.App.Controllers
{
    public class SubGastosController : Controller
    {
        private readonly RepositorioSubGasto Repositorio;
        private readonly IMapper Mapper;

        public SubGastosController(RepositorioSubGasto repositorio, IMapper mapper)
        {
            Repositorio = repositorio;
            Mapper = mapper;
        }

        SubGasto subGasto = new SubGasto();
        SubGastoVM subGastoVM = new SubGastoVM();

        public IActionResult Index()
        {
            IEnumerable<SubGasto> subGastos = Repositorio.ObterTodos();
            List<SubGastoVM> subGastosVM = Mapper.Map<List<SubGastoVM>>(subGastos);
            return View(subGastosVM);
        }

        public IActionResult Editar(Int64 Id = 0)
        {
            if (Id > 0)
            {
                subGasto = Repositorio.ObterPorId(Id);
                subGastoVM = Mapper.Map<SubGastoVM>(subGasto);
            }
            
            ViewBag.GastoId = new SelectList(new RepositorioGasto(NHibernateHelper.ObterSessao()).ObterPorParametros(x => x.Situacao == Situacao.Ativo), "Id", "Nome", Id);
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
                    subGasto = Repositorio.ObterPorId(subGastoVM.Id);
                    subGasto = Mapper.Map(subGastoVM, subGasto);
                    subGasto.UsuarioAlteracao = Configuracao.Usuario;
                    Repositorio.Alterar(subGasto);
                }
                else
                {
                    subGasto = Mapper.Map(subGastoVM,subGasto);
                    subGasto.UsuarioCriacao = Configuracao.Usuario;
                    Repositorio.Salvar(subGasto);
                }

                return new EmptyResult();
            }
            ViewBag.GastoId = new SelectList(new RepositorioGasto(NHibernateHelper.ObterSessao()).ObterPorParametros(x => x.Situacao == Situacao.Ativo), "Id", "Nome", subGastoVM.GastoId);
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
