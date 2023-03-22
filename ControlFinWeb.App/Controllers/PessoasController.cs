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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public IActionResult Index()
        {
            IEnumerable<Pessoa> pessoas = Repositorio.ObterPorParametros(x => x.UsuarioCriacao.Id == Configuracao.Usuario.Id);
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
                return View(pessoaVM);
        }

        [HttpPost]
        public IActionResult Editar(PessoaVM pessoaVM)
        {
            if (ModelState.IsValid)
            {
                if (pessoaVM.Id > 0)
                {
                   
                    pessoa = Repositorio.ObterPorId(pessoaVM.Id);
                    pessoa = Mapper.Map(pessoaVM, pessoa);
                    pessoa.UsuarioAlteracao = Configuracao.Usuario;
                    Repositorio.Alterar(pessoa);
                }
                else
                {
                    
                    pessoa = Mapper.Map(pessoaVM, pessoa);
                    pessoa.UsuarioCriacao = Configuracao.Usuario;
                    Repositorio.Salvar(pessoa);
                }
                return new EmptyResult();
            }
            return View(pessoaVM);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public JsonResult Deletar(int id)
        {
            try
            {
                var pessoa = Repositorio.ObterPorId(id);
                Repositorio.Excluir(pessoa);
                return Json(pessoa.Nome + "excluído com sucesso");
            }
            catch
            {
                throw;
            }
        }
    }
}
