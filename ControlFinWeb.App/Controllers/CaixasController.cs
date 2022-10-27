using AutoMapper;
using ControlFinWeb.App.Utilitarios;
using ControlFinWeb.App.ViewModels;
using ControlFinWeb.Dominio.Entidades;
using ControlFinWeb.Repositorio.Repositorios;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace ControlFinWeb.App.Controllers
{
    public class CaixasController : Controller
    {
        private readonly RepositorioCaixa Repositorio;
        private readonly IMapper Mapper;

        public CaixasController(RepositorioCaixa repositorio, IMapper mapper)
        {
            Repositorio = repositorio;
            Mapper = mapper;
        }

        Caixa caixa = new Caixa();
        CaixaVM caixaVM = new CaixaVM();

        public IActionResult Index()
        {
            IEnumerable<Caixa> caixas = Repositorio.ObterTodos();
            List<CaixaVM> caixasVM = Mapper.Map<List<CaixaVM>>(caixas);
            return View(caixasVM);
        }

        public IActionResult Editar(Int64 Id = 0)
        {
            if (Id > 0)
            {
                caixa = Repositorio.ObterPorId(Id);
                caixaVM = Mapper.Map<CaixaVM>(caixa);
            }

            return View(caixaVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(CaixaVM caixaVM)
        {
            if (ModelState.IsValid)
            {
                if (caixaVM.Id > 0)
                {
                    caixa = Repositorio.ObterPorId(caixaVM.Id);
                    caixa = Mapper.Map(caixaVM, caixa);
                    caixa.UsuarioAlteracao = Configuracao.Usuario;
                    Repositorio.Alterar(caixa);
                }
                else
                {
                    caixa = Mapper.Map(caixaVM, caixa);
                    caixa.UsuarioCriacao = Configuracao.Usuario;
                    Repositorio.Salvar(caixa);
                }

                return new EmptyResult();
            }
            return View(caixaVM);
        }

        [HttpPost]
        public JsonResult Deletar(Int64 Id)
        {
            caixa = Repositorio.ObterPorId(Id);
            Repositorio.Excluir(caixa);
            return Json(caixa.Id + "excluído com sucesso");
        }

    }
}
