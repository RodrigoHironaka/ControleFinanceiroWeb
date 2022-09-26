

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
using System.Linq;

namespace ControlFinWeb.App.Controllers
{
    public class FaturasController : Controller
    {
        private readonly RepositorioFatura Repositorio;
        private readonly RepositorioParcela RepositorioParcela;
        private readonly IMapper Mapper;
        public FaturasController(RepositorioFatura repositorio, RepositorioParcela repositorioParcela, IMapper mapper)
        {
            Repositorio = repositorio;
            RepositorioParcela = repositorioParcela;
            Mapper = mapper;
        }

        Fatura fatura = new Fatura();
        FaturaVM faturaVM = new FaturaVM();

        public IActionResult Index()
        {
            IEnumerable<Fatura> faturas = Repositorio.ObterTodos();
            List<FaturaVM> faturaVM = Mapper
                .Map<List<FaturaVM>>(faturas);
            return View(faturaVM);
        }

        public IActionResult Editar(Int64 Id = 0)
        {
            if (Id > 0)
            {
                fatura = Repositorio.ObterPorId(Id);
                faturaVM = Mapper.Map<FaturaVM>(fatura);
            }

            ViewBag.Cartoes = new SelectList(new RepositorioCartao(NHibernateHelper.ObterSessao()).ObterPorParametros(x => x.Situacao == Situacao.Ativo), "Id", "Nome", Id);
            return View(faturaVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(FaturaVM faturaVM)
        {
            if (ModelState.IsValid)
            {
                if (faturaVM.Id > 0)
                {
                    fatura = Repositorio.ObterPorId(faturaVM.Id);
                    fatura = Mapper.Map<Fatura>(faturaVM);
                    fatura.UsuarioAlteracao = Configuracao.Usuario;
                    Repositorio.Alterar(fatura);
                }
                else
                {
                    fatura = Mapper.Map(faturaVM, fatura);
                    fatura.UsuarioCriacao = Configuracao.Usuario;
                    Repositorio.SalvarEGerarNovaParcela(fatura);
                }
                return new EmptyResult();
            }
            ViewBag.Cartoes = new SelectList(new RepositorioCartao(NHibernateHelper.ObterSessao()).ObterPorParametros(x => x.Situacao == Situacao.Ativo), "Id", "Nome", fatura.Id);
            return View(faturaVM);
        }

        [HttpPost]
        public JsonResult Deletar(int id)
        {
            Repositorio.ExcluirOuCancelarFaturaEParcela(id);
            return Json(fatura.DescricaoCompleta + "excluído com sucesso");
        }

      
        public IActionResult FecharFatura(String obs, Int64 id = 0)
        {
            var fatura = Repositorio.ObterPorId(id);
            if(fatura != null)
            {
                fatura.SituacaoFatura = SituacaoFatura.Fechada;
                fatura.DataFechamento = DateTime.Now;
                if (!String.IsNullOrEmpty(obs))
                    fatura.Nome = obs;
                Repositorio.Alterar(fatura);
                
            }

            return new EmptyResult();
        }
    }
}
