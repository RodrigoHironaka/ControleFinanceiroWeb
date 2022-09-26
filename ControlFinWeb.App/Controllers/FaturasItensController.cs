using App1.Repositorio.Configuracao;
using AutoMapper;
using ControlFinWeb.App.Utilitarios;
using ControlFinWeb.App.ViewModels;
using ControlFinWeb.Dominio.Entidades;
using ControlFinWeb.Dominio.ObjetoValor;
using ControlFinWeb.Repositorio.Repositorios;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ControlFinWeb.App.Controllers
{
    public class FaturasItensController : Controller
    {
        private readonly RepositorioFaturaItens Repositorio;
        private readonly RepositorioFatura RepositorioFatura;
        private readonly RepositorioParcela RepositorioParcela;
        private readonly IMapper Mapper;
        public FaturasItensController(RepositorioFaturaItens repositorio, RepositorioFatura repositorioFatura, RepositorioParcela repositorioParcela, IMapper mapper)
        {
            Repositorio = repositorio;
            RepositorioFatura = repositorioFatura;
            RepositorioParcela = repositorioParcela;
            Mapper = mapper;
        }

        FaturaItens faturaItens = new FaturaItens();
        FaturaItensVM faturaItensVM = new FaturaItensVM();

        public IActionResult Index(Int64 Id)//Id é da fatura do cartao
        {
            IEnumerable<FaturaItens> faturasItens = Repositorio.ObterPorParametros(x => x.CartaoCredito.Id == Id);
            List<FaturaItensVM> faturasVM = Mapper
                .Map<List<FaturaItensVM>>(faturasItens);
            return View(faturasVM);
        }

        public IActionResult Editar(Int64 Id = 0)
        {
            if (Id > 0)
            {
                faturaItens = Repositorio.ObterPorId(Id);
                faturaItensVM = Mapper.Map<FaturaItensVM>(faturaItens);
            }

            ViewBag.Pessoas = new SelectList(new RepositorioPessoa(NHibernateHelper.ObterSessao()).ObterPorParametros(x => x.Situacao == Situacao.Ativo), "Id", "Nome", Id);
            ViewBag.Subgastos = new SelectList(new RepositorioSubGasto(NHibernateHelper.ObterSessao()).ObterPorParametros(x => x.Situacao == Situacao.Ativo), "Id", "DescricaoCompleta", Id);
            return View(faturaItensVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(FaturaItensVM faturaItensVM)
        {
            if (ModelState.IsValid)
            {
                if (faturaItensVM.Id > 0)
                {
                    faturaItens = Repositorio.ObterPorId(faturaItensVM.Id);
                    faturaItens.UsuarioAlteracao = Configuracao.Usuario;
                }
                else
                    faturaItens.UsuarioCriacao = Configuracao.Usuario;

                faturaItens = Mapper.Map(faturaItensVM, faturaItens);
                faturaItens.CartaoCredito = RepositorioFatura.ObterPorId(faturaItens.CartaoCredito.Id);
                Repositorio.SalvarAlterarFaturaItemEAlterarParcela(faturaItens, faturaItensVM.NumeroParcelas, faturaItensVM.Replicar);
                return new EmptyResult();
            }
            ViewBag.Pessoas = new SelectList(new RepositorioPessoa(NHibernateHelper.ObterSessao()).ObterPorParametros(x => x.Situacao == Situacao.Ativo), "Id", "Nome", faturaItensVM.Id);
            ViewBag.Subgastos = new SelectList(new RepositorioSubGasto(NHibernateHelper.ObterSessao()).ObterPorParametros(x => x.Situacao == Situacao.Ativo), "Id", "DescricaoCompleta", faturaItensVM.Id);
            return View(faturaItensVM);
        }

        [HttpPost]
        public JsonResult Deletar(Int64 id)
        {
            Repositorio.ExcluirItemFaturaEAlterarParcela(id);
            return Json("Excluído com sucesso");
        }

        [HttpGet("SituacaoFatura/{idCartao}")]
        public String SituacaoFatura(Int64 idCartao)
        {
            var fatura = RepositorioFatura.ObterPorId(idCartao);
            if(fatura == null)
               return String.Empty;

            return fatura.SituacaoFatura.ToString();
        }
    }
}
