using ControlFinWeb.App.AutoMapper;
using ControlFinWeb.App.Utilitarios;
using ControlFinWeb.App.ViewModels;
using ControlFinWeb.Dominio.Dominios;
using ControlFinWeb.Dominio.ObjetoValor;
using ControlFinWeb.Repositorio.Repositorios;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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
            var formasPagamentoVM = new List<FormaPagamentoVM>();
            var formasPagamento = Repositorio.ObterTodos();
            foreach (var forma in formasPagamento)
                formasPagamentoVM.Add(AutoMapperConfig<FormaPagamento, FormaPagamentoVM>.Mapear(forma));
            
            return View(formasPagamentoVM);
        }

        public IActionResult Editar(Int64 Id = 0)
        {
            if (Id > 0)
            {
                formaPagamento = Repositorio.ObterPorId(Id);
                formaPagamentoVM = AutoMapperConfig<FormaPagamento, FormaPagamentoVM>.Mapear(formaPagamento);
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
                    var formaPagamentoRep = Repositorio.ObterPorId(formaPagamentoVM.Id);
                    formaPagamento = AutoMapperConfig<FormaPagamentoVM, FormaPagamento>.Mapear(formaPagamentoVM);
                    formaPagamento.DataGeracao = formaPagamentoRep.DataGeracao;
                    formaPagamento.UsuarioCriacao = formaPagamentoRep.UsuarioCriacao;
                    formaPagamento.UsuarioAlteracao = Configuracao.Usuario;
                    Repositorio.Alterar(formaPagamento);
                }
                else
                {
                    formaPagamento = AutoMapperConfig<FormaPagamentoVM, FormaPagamento>.Mapear(formaPagamentoVM);
                    formaPagamento.UsuarioCriacao = Configuracao.Usuario;
                    Repositorio.Salvar(formaPagamento);
                }
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
