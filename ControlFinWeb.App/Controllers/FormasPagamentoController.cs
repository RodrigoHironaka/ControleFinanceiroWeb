using ControlFinWeb.Dominio.Dominios;
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
        ViewModels.FormaPagamentoVM formaPagamentoVM = new ViewModels.FormaPagamentoVM();

        public IActionResult Index()
        {
            return View(formaPagamentoVM);
            //return View(Repositorio.ObterPorParametros(x => x.Situacao == Situacao.Ativo));
        }

        public IActionResult Editar(Int64 Id = 0)
        {
            if (Id > 0)
            {
                formaPagamento = Repositorio.ObterPorId(Id);

                formaPagamentoVM = new ViewModels.FormaPagamentoVM
                {
                    Id = formaPagamento.Id,
                    Nome = formaPagamento.Nome,
                    DebitoAutomatico = formaPagamento.DebitoAutomatico,
                };
            }

            return View(formaPagamentoVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(ViewModels.FormaPagamentoVM formaPagamentoVM)
        {
            if (ModelState.IsValid)
            {
                if (formaPagamentoVM.Id > 0)
                    formaPagamento = Repositorio.ObterPorId(formaPagamentoVM.Id);

                formaPagamento.Nome = formaPagamentoVM.Nome;
                formaPagamento.DebitoAutomatico = formaPagamentoVM.DebitoAutomatico;

                if (formaPagamentoVM.Id == 0)
                    Repositorio.Salvar(formaPagamento);
                else
                    Repositorio.Alterar(formaPagamento);

                return RedirectToAction("Index");
            }

            return View(formaPagamentoVM);
        }
    }
}
