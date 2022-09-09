

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
    public class FaturasCartaoCreditoController : Controller
    {
        private readonly RepositorioFaturaCartaoCredito Repositorio;
        private readonly RepositorioParcela RepositorioParcela;
        private readonly IMapper Mapper;
        public FaturasCartaoCreditoController(RepositorioFaturaCartaoCredito repositorio, RepositorioParcela repositorioParcela, IMapper mapper)
        {
            Repositorio = repositorio;
            RepositorioParcela = repositorioParcela;
            Mapper = mapper;
        }

        FaturaCartaoCredito faturaCartaoCredito = new FaturaCartaoCredito();
        FaturaCartaoCreditoVM faturaCartaoCreditoVM = new FaturaCartaoCreditoVM();

        public IActionResult Index()
        {
            IEnumerable<FaturaCartaoCredito> faturaCartaoCreditos = Repositorio.ObterTodos();
            List<FaturaCartaoCreditoVM> faturaCartaoCreditosVM = Mapper
                .Map<List<FaturaCartaoCreditoVM>>(faturaCartaoCreditos);
            return View(faturaCartaoCreditosVM);
        }

        public IActionResult Editar(Int64 Id = 0)
        {
            if (Id > 0)
            {
                faturaCartaoCredito = Repositorio.ObterPorId(Id);
                faturaCartaoCreditoVM = Mapper.Map<FaturaCartaoCreditoVM>(faturaCartaoCredito);
            }

            ViewBag.Cartoes = new SelectList(new RepositorioCartao(NHibernateHelper.ObterSessao()).ObterPorParametros(x => x.Situacao == Situacao.Ativo), "Id", "Nome", Id);
            return View(faturaCartaoCreditoVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(FaturaCartaoCreditoVM faturaCartaoCreditoVM)
        {
            if (ModelState.IsValid)
            {
                if (faturaCartaoCreditoVM.Id > 0)
                {
                    faturaCartaoCredito = Repositorio.ObterPorId(faturaCartaoCreditoVM.Id);
                    faturaCartaoCredito = Mapper.Map<FaturaCartaoCredito>(faturaCartaoCreditoVM);
                    faturaCartaoCredito.UsuarioAlteracao = Configuracao.Usuario;
                    Repositorio.Alterar(faturaCartaoCredito);
                }
                else
                {
                    faturaCartaoCredito = Mapper.Map(faturaCartaoCreditoVM, faturaCartaoCredito);
                    faturaCartaoCredito.UsuarioCriacao = Configuracao.Usuario;
                    Repositorio.SalvarEGerarNovaParcela(faturaCartaoCredito);
                }
                return new EmptyResult();
            }
            ViewBag.Cartoes = new SelectList(new RepositorioCartao(NHibernateHelper.ObterSessao()).ObterPorParametros(x => x.Situacao == Situacao.Ativo), "Id", "Nome", faturaCartaoCreditoVM.Id);
            return View(faturaCartaoCreditoVM);
        }


        [HttpPost]
        public JsonResult Deletar(int id)
        {
            Repositorio.ExcluirOuCancelarFaturaEParcela(id);
            return Json(faturaCartaoCredito.DescricaoCompleta + "excluído com sucesso");
        }
    }
}
