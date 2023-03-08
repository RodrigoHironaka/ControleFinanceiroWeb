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
    public class FluxoCaixasController : Controller
    {
        private readonly RepositorioFluxoCaixa Repositorio;
        private readonly IMapper Mapper;

        public FluxoCaixasController(RepositorioFluxoCaixa repositorio, IMapper mapper)
        {
            Repositorio = repositorio;
            Mapper = mapper;
        }

        FluxoCaixa fluxoCaixa = new FluxoCaixa();
        FluxoCaixaVM fluxoCaixaVM = new FluxoCaixaVM();

        //public IActionResult Index()
        //{
        //    IEnumerable<FluxoCaixa> fluxos = Repositorio.ObterTodos(); // pegar por caixa
        //    List<FluxoCaixaVM> fluxosVM = Mapper.Map<List<FluxoCaixaVM>>(fluxos);
        //    return View(fluxosVM);
        //}

        public IActionResult Editar(Int64 Id = 0)
        {
            if (Id > 0)
            {
                fluxoCaixa = Repositorio.ObterPorId(Id);
                fluxoCaixaVM = Mapper.Map<FluxoCaixaVM>(fluxoCaixa);
            }

            ViewBag.FormaPagamentoId = new SelectList(new RepositorioFormaPagamento(NHibernateHelper.ObterSessao()).ObterPorParametros(x => x.Situacao == Situacao.Ativo), "Id", "Nome", Id);
            return View(fluxoCaixaVM);
        }

        [HttpPost]
        public IActionResult Editar(FluxoCaixaVM fluxoCaixaVM)
        {
            if (ModelState.IsValid)
            {
                if (fluxoCaixaVM.Id > 0)
                {
                    fluxoCaixa = Repositorio.ObterPorId(fluxoCaixaVM.Id);
                    fluxoCaixa = Mapper.Map(fluxoCaixaVM, fluxoCaixa);
                    fluxoCaixa.UsuarioAlteracao = Configuracao.Usuario;
                    Repositorio.Alterar(fluxoCaixa);
                }
                else
                {
                    fluxoCaixa = Mapper.Map(fluxoCaixaVM, fluxoCaixa);
                    fluxoCaixa.UsuarioCriacao = Configuracao.Usuario;
                    Repositorio.Salvar(fluxoCaixa);
                }

                return new EmptyResult();
            }

            ViewBag.FormaPagamentoId = new SelectList(new RepositorioFormaPagamento(NHibernateHelper.ObterSessao()).ObterPorParametros(x => x.Situacao == Situacao.Ativo), "Id", "Nome", fluxoCaixaVM.Id);
            return View(fluxoCaixaVM);
        }

        [HttpPost]
        public JsonResult Deletar(Int64 Id)
        {
            fluxoCaixa = Repositorio.ObterPorId(Id);
            Repositorio.Excluir(fluxoCaixa);
            return Json(fluxoCaixa.Nome + "excluído com sucesso");
        }
    }
}
