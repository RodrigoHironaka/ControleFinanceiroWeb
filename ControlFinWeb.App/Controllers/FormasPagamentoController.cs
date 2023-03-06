using AutoMapper;
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
        private readonly IMapper Mapper;
        public FormasPagamentoController(RepositorioFormaPagamento repositorio, IMapper mapper)
        {
            Repositorio = repositorio;
            Mapper = mapper;
        }

        FormaPagamento formaPagamento = new FormaPagamento();
        FormaPagamentoVM formaPagamentoVM = new FormaPagamentoVM();

        public IActionResult Index()
        {
            IEnumerable<FormaPagamento> formasPagamento = Repositorio.ObterTodos();
            List<FormaPagamentoVM> formasPagamentoVM = Mapper
                .Map<List<FormaPagamentoVM>>(formasPagamento);
            return View(formasPagamentoVM);
        }

        public IActionResult Editar(Int64 Id = 0)
        {
            if (Id > 0)
            {
                formaPagamento = Repositorio.ObterPorId(Id);
                formaPagamentoVM = Mapper.Map<FormaPagamentoVM>(formaPagamento);
            }
            return View(formaPagamentoVM);
        }

        [HttpPost]
        public IActionResult Editar(FormaPagamentoVM formaPagamentoVM)
        {
            if (ModelState.IsValid)
            {
                if (formaPagamentoVM.Id > 0)
                {
                    formaPagamento = Repositorio.ObterPorId(formaPagamentoVM.Id);
                    formaPagamento = Mapper.Map(formaPagamentoVM, formaPagamento);
                    formaPagamento.UsuarioAlteracao = Configuracao.Usuario;
                    Repositorio.Alterar(formaPagamento);
                }
                else
                {
                    formaPagamento = Mapper.Map(formaPagamentoVM, formaPagamento);
                    formaPagamento.UsuarioCriacao = Configuracao.Usuario;
                    Repositorio.Salvar(formaPagamento);
                }
                return new EmptyResult();
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
