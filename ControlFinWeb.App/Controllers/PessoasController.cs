using App1.Repositorio.Configuracao;
using AutoMapper;
using ControlFinWeb.App.Utilitarios;
using ControlFinWeb.App.ViewModels;
using ControlFinWeb.Dominio.Entidades;
using ControlFinWeb.Dominio.ObjetoValor;
using ControlFinWeb.Repositorio.Repositorios;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace ControlFinWeb.App.Controllers
{
    public class PessoasController : Controller
    {
        private readonly RepositorioPessoa Repositorio;
        private readonly IMapper Mapper;
        public PessoasController(RepositorioPessoa repositorio, IMapper mapper)
        {
            Repositorio = repositorio;
            Mapper = mapper;
        }

        Pessoa pessoa = new Pessoa();
        PessoaVM pessoaVM = new PessoaVM();
        PessoaRendas pessoaRendas = new PessoaRendas();
        PessoaRendasVM pessoaRendasVM = new PessoaRendasVM();

        public IActionResult Index()
        {
            IEnumerable<Pessoa> pessoas = Repositorio.ObterTodos();
            List<PessoaVM> pessoasVM = Mapper
                .Map<List<PessoaVM>>(pessoas);
            return View(pessoasVM);
        }

        public IActionResult Editar(Int64 Id = 0)
        {
            if (Id > 0)
            {
                pessoa = Repositorio.ObterPorId(Id);
                pessoaVM = Mapper.Map<PessoaVM>(pessoa);
            }
            pessoaVM.Rendas = new SelectList(new RepositorioRenda(NHibernateHelper.ObterSessao()).ObterPorParametros(x => x.Situacao == Situacao.Ativo), "Id", "Nome", Id);
            return View(pessoaVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(PessoaVM pessoaVM)
        {
            if (ModelState.IsValid)
            {
                if (pessoaVM.Id > 0)
                {
                    pessoa = Repositorio.ObterPorId(pessoaVM.Id);
                    pessoa = Mapper.Map(pessoaVM, pessoa);
                    pessoa.UsuarioAlteracao = Configuracao.Usuario;
                    pessoa.PessoaRendas.ForEach(x => x.Pessoa = pessoa);
                    Repositorio.Alterar(pessoa);
                }
                else
                {
                    pessoa = Mapper.Map(pessoaVM, pessoa);
                    pessoa.UsuarioCriacao = Configuracao.Usuario;
                    pessoa.PessoaRendas.ForEach(x => x.Pessoa = pessoa);
                    Repositorio.Salvar(pessoa);
                }
                return new EmptyResult();
            }
            pessoaVM.Rendas = new SelectList(new RepositorioRenda(NHibernateHelper.ObterSessao()).ObterPorParametros(x => x.Situacao == Situacao.Ativo), "Id", "Nome", pessoaVM.Id);
            return View(pessoaVM);
        }


        [HttpPost]
        public JsonResult Deletar(int id)
        {
            var pessoa = Repositorio.ObterPorId(id);
            Repositorio.Excluir(pessoa);
            return Json(pessoa.Nome + "excluído com sucesso");
        }
    }
}
