﻿using App1.Repositorio.Configuracao;
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

namespace ControlFinWeb.App.Controllers
{
    public class FaturasCartaoCreditoItensController : Controller
    {
        private readonly RepositorioFaturaCartaoCreditoItens Repositorio;
        private readonly IMapper Mapper;
        public FaturasCartaoCreditoItensController(RepositorioFaturaCartaoCreditoItens repositorio, IMapper mapper)
        {
            Repositorio = repositorio;
            Mapper = mapper;
        }

        FaturaCartaoCreditoItens faturaCartaoCreditoItens = new FaturaCartaoCreditoItens();
        FaturaCartaoCreditoItensVM faturaCartaoCreditoItensVM = new FaturaCartaoCreditoItensVM();

        public IActionResult Index(Int64 Id)//Id é da fatura do cartao
        {
            IEnumerable<FaturaCartaoCreditoItens> faturaCartaoCreditosItens = Repositorio.ObterPorParametros(x => x.CartaoCredito.Id == Id);
            List<FaturaCartaoCreditoItensVM> faturaCartaoCreditosItensVM = Mapper
                .Map<List<FaturaCartaoCreditoItensVM>>(faturaCartaoCreditosItens);
            return View(faturaCartaoCreditosItensVM);
        }

        public IActionResult Editar(Int64 Id = 0)
        {
            if (Id > 0)
            {
                faturaCartaoCreditoItens = Repositorio.ObterPorId(Id);
                faturaCartaoCreditoItensVM = Mapper.Map<FaturaCartaoCreditoItensVM>(faturaCartaoCreditoItens);
            }

            ViewBag.Pessoas = new SelectList(new RepositorioPessoa(NHibernateHelper.ObterSessao()).ObterPorParametros(x => x.Situacao == Situacao.Ativo), "Id", "Nome", Id);
            ViewBag.Subgastos = new SelectList(new RepositorioSubGasto(NHibernateHelper.ObterSessao()).ObterPorParametros(x => x.Situacao == Situacao.Ativo), "Id", "DescricaoCompleta", Id);
            return View(faturaCartaoCreditoItensVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(FaturaCartaoCreditoItensVM faturaCartaoCreditoItensVM)
        {
            if (ModelState.IsValid)
            {
                if (faturaCartaoCreditoItensVM.Id > 0)
                {
                    faturaCartaoCreditoItens = Repositorio.ObterPorId(faturaCartaoCreditoItensVM.Id);
                    faturaCartaoCreditoItens = Mapper.Map(faturaCartaoCreditoItensVM, faturaCartaoCreditoItens);
                    faturaCartaoCreditoItens.UsuarioAlteracao = Configuracao.Usuario;
                    Repositorio.Alterar(faturaCartaoCreditoItens);
                }
                else
                {
                    faturaCartaoCreditoItens = Mapper.Map(faturaCartaoCreditoItensVM, faturaCartaoCreditoItens);
                    faturaCartaoCreditoItens.UsuarioCriacao = Configuracao.Usuario;
                    Repositorio.Salvar(faturaCartaoCreditoItens);
                }
                return new EmptyResult();
            }
            ViewBag.Pessoas = new SelectList(new RepositorioPessoa(NHibernateHelper.ObterSessao()).ObterPorParametros(x => x.Situacao == Situacao.Ativo), "Id", "Nome", faturaCartaoCreditoItensVM.Id);
            ViewBag.Subgastos = new SelectList(new RepositorioSubGasto(NHibernateHelper.ObterSessao()).ObterPorParametros(x => x.Situacao == Situacao.Ativo), "Id", "DescricaoCompleta", faturaCartaoCreditoItensVM.Id);
            return View(faturaCartaoCreditoItensVM);
        }


        [HttpPost]
        public JsonResult Deletar(int id)
        {
            var faturaCartaoCreditoItem = Repositorio.ObterPorId(id);
            Repositorio.Excluir(faturaCartaoCreditoItem);
            return Json(faturaCartaoCreditoItem.Nome + "excluído com sucesso");
        }
    }
}
