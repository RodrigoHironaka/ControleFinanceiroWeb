using AutoMapper;
using ControlFinWeb.App.Utilitarios;
using ControlFinWeb.App.ViewModels;
using ControlFinWeb.Dominio.Entidades;
using ControlFinWeb.Repositorio.Repositorios;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

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
            var caixa = Repositorio.ObterCaixaAberto(Configuracao.Usuario.Id);
            if(caixa != null)
                caixaVM = Mapper.Map(caixa, caixaVM);
            return View(caixaVM);
        }

        public IActionResult Editar(Int64 Id = 0)
        {
            if (Id > 0)
            {
                caixa = Repositorio.ObterPorId(Id);
                caixaVM = Mapper.Map(caixa, caixaVM);
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
                    caixa.Situacao = Dominio.ObjetoValor.SituacaoCaixa.Fechado;
                    Repositorio.Alterar(caixa);
                }
                else
                {
                    caixa = Mapper.Map(caixaVM, caixa);
                    caixa.Codigo = Repositorio.RetornaUltimoCodigo() + 1;
                    caixa.UsuarioCriacao = Configuracao.Usuario;
                    Repositorio.SalvarCaixa(caixa, Configuracao.Usuario);
                    //Repositorio.Salvar(caixa);
                }

                return RedirectToAction("Index");
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
