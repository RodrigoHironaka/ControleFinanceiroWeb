

using App1.Repositorio.Configuracao;
using AutoMapper;
using ControlFinWeb.App.Utilitarios;
using ControlFinWeb.App.ViewModels;
using ControlFinWeb.Dominio.Entidades;
using ControlFinWeb.Dominio.ObjetoValor;
using ControlFinWeb.Repositorio.Repositorios;
using LinqKit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Org.BouncyCastle.Asn1.Cmp;
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

        public IActionResult Index(Int64 idFatura = 0)
        {
            if (idFatura > 0)
            {
                fatura = Repositorio.ObterPorId(idFatura);
                if (fatura != null)
                    faturaVM = Mapper.Map<FaturaVM>(fatura);
            }
            return View(faturaVM);
        }

        public IActionResult Pesquisa()
        {
            IEnumerable<Fatura> faturas = Repositorio.ObterPorParametros(x => x.UsuarioCriacao.Id == Configuracao.Usuario.Id && x.SituacaoFatura == SituacaoFatura.Aberta);
            List<FaturaVM> faturasVM = Mapper.Map<List<FaturaVM>>(faturas);
            ViewBag.Cartoes = new SelectList(new RepositorioCartao(NHibernateHelper.ObterSessao()).ObterPorParametros(x => x.Situacao == Situacao.Ativo), "Id", "Nome", null);
            return View(faturasVM);
        }

        [HttpPost]
        public IActionResult Pesquisa(Int64 cartaoId = 0, bool somenteAtivos = true)
        {
            var predicado = Repositorio.CriarPredicado();
            predicado = predicado.And(x => x.UsuarioCriacao.Id == Configuracao.Usuario.Id);
            if (cartaoId > 0)
                predicado = predicado.And(x => x.Cartao.Id == cartaoId);
            if (somenteAtivos)
                predicado = predicado.And(x => x.SituacaoFatura == SituacaoFatura.Aberta);

            IEnumerable<Fatura> faturas = Repositorio.ObterPorParametros(predicado);
            List<FaturaVM> faturasVM = Mapper.Map<List<FaturaVM>>(faturas);
            ViewBag.Cartoes = new SelectList(new RepositorioCartao(NHibernateHelper.ObterSessao()).ObterPorParametros(x => x.Situacao == Situacao.Ativo), "Id", "Nome", null);
            return PartialView("_TabelaFaturas", faturasVM);
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
                    fatura = Mapper.Map(faturaVM, fatura);
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
            if (fatura != null)
            {
                fatura.SituacaoFatura = SituacaoFatura.Fechada;
                fatura.DataFechamento = DateTime.Now;
                if (!String.IsNullOrEmpty(obs))
                    fatura.Nome = obs;
                Repositorio.Alterar(fatura);
            }
            return new EmptyResult();
        }

        public Boolean ExisteFatura(FaturaVM faturaVM)
        {
            var existeFatura = Repositorio.ObterPorParametros(x => x.Cartao.Id == faturaVM.CartaoId && x.MesAnoReferencia == faturaVM.MesAnoReferencia).FirstOrDefault();
            if (existeFatura != null)
                return true;

            return false;
        }
    }
}
