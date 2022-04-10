using ControlFinWeb.App.ViewModels;
using ControlFinWeb.Dominio.Dominios;
using ControlFinWeb.Dominio.ObjetoValor;
using ControlFinWeb.Repositorio.Repositorios;
using Microsoft.AspNetCore.Mvc;
using System;

namespace ControlFinWeb.App.Controllers
{

    public class FormasPagamentoController : Controller
    {
        private readonly RepositorioFormaPagamento Repositorio;
        public FormasPagamentoController(RepositorioFormaPagamento repositorio)
        {
            Repositorio = repositorio;
        }

        FormaPagamento formaPagamento = new FormaPagamento();
        FormaPagamentoVM formaPagamentoVM = new FormaPagamentoVM();

        public IActionResult Index()
        {
           return View(Repositorio.ObterPorParametros(x => x.Situacao == Situacao.Ativo));
        }

        public JsonResult Editar(Int64 Id = 0)
        {
            if (Id > 0)
            {
                formaPagamento = Repositorio.ObterPorId(Id);

                formaPagamentoVM = new FormaPagamentoVM
                {
                    Id = formaPagamento.Id,
                    Nome = formaPagamento.Nome,
                    DebitoAutomatico = formaPagamento.DebitoAutomatico,
                };
            }

            return Json(formaPagamentoVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Editar(FormaPagamentoVM formaPagamentoVM)
        {
            if (ModelState.IsValid)
            {
                if (formaPagamento.Id > 0)
                    formaPagamento = Repositorio.ObterPorId(formaPagamentoVM.Id);

                formaPagamento.Nome = formaPagamentoVM.Nome;
                formaPagamento.DebitoAutomatico = formaPagamentoVM.DebitoAutomatico;
                formaPagamento.Situacao = formaPagamentoVM.Situacao;

                if (formaPagamentoVM.Id == 0)
                    Repositorio.Salvar(formaPagamento);
                else
                    Repositorio.Alterar(formaPagamento);

                return Json(formaPagamentoVM);
            }

            return Json(formaPagamentoVM);
        }
    }
}
