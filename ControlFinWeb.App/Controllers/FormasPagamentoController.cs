using ControlFinWeb.App.Utilitarios;
using ControlFinWeb.App.ViewModels;
using ControlFinWeb.Dominio.Dominios;
using ControlFinWeb.Dominio.ObjetoValor;
using ControlFinWeb.Repositorio.Repositorios;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

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
            return View(Repositorio.ObterTodos());
        }

        public IActionResult Editar(Int64 Id = 0)
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

            return View(formaPagamentoVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(FormaPagamentoVM formaPagamentoVM)
        {
            if (ModelState.IsValid)
            {
                if (formaPagamentoVM.Id > 0)
                {
                    formaPagamento = Repositorio.ObterPorId(formaPagamentoVM.Id);
                    formaPagamento.UsuarioAlteracao = Configuracao.Usuario;
                }
                else
                    formaPagamento.UsuarioCriacao = Configuracao.Usuario;

                formaPagamento.Nome = formaPagamentoVM.Nome;
                formaPagamento.DebitoAutomatico = formaPagamentoVM.DebitoAutomatico;
                formaPagamento.Situacao = formaPagamentoVM.Situacao;

                if (formaPagamentoVM.Id == 0)
                    Repositorio.Salvar(formaPagamento);
                else
                    Repositorio.Alterar(formaPagamento);

                return RedirectToAction("Index");
            }

            return View(formaPagamentoVM);
        }

        [HttpPost]
        public JsonResult Deletar(int id)
        {
            var formaPagamento = Repositorio.ObterPorId(id);
            Repositorio.Excluir(formaPagamento);
            return Json(formaPagamento.Nome + "excluído com sucesso");
        }
    }
}
