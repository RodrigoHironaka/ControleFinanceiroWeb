﻿using App1.Repositorio.Configuracao;
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
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ControlFinWeb.App.Controllers
{
    public class ContasController : Controller
    {
        private readonly RepositorioConta Repositorio;
        private readonly IMapper Mapper;
        public ContasController(RepositorioConta repositorio, IMapper mapper)
        {
            Repositorio = repositorio;
            Mapper = mapper;
        }

        Conta conta = new Conta();
        ContaVM contaVM = new ContaVM();

        public IActionResult Index()
        {
            IEnumerable<Conta> contas = Repositorio.ObterPorParametros(x => x.UsuarioCriacao.Id == Configuracao.Usuario.Id);
            List<ContaVM> contasVM = Mapper
                .Map<List<ContaVM>>(contas);
            return View(contasVM);
        }

        public IActionResult Editar(Int64 Id = 0)
        {
            if (Id > 0)
            {
                conta = Repositorio.ObterPorId(Id);
                contaVM = Mapper.Map<ContaVM>(conta);
                contaVM.JsonParcelas = JsonConvert.SerializeObject(contaVM.ParcelasVM);
            }
            ViewBag.FormaPagamentoId = new SelectList(new RepositorioFormaPagamento(NHibernateHelper.ObterSessao()).ObterPorParametros(x => x.Situacao == Situacao.Ativo), "Id", "Nome", Id);
            ViewBag.PessoaId = new SelectList(new RepositorioPessoa(NHibernateHelper.ObterSessao()).ObterPorParametros(x => x.Situacao == Situacao.Ativo), "Id", "Nome", Id);
            ViewBag.SubGastoId = new SelectList(new RepositorioSubGasto(NHibernateHelper.ObterSessao()).ObterPorParametros(x => x.Situacao == Situacao.Ativo), "Id", "DescricaoCompleta", Id);

            return View(contaVM);
        }

        [HttpPost]
        public IActionResult Editar(ContaVM contaVM)
        {
            if (ModelState.IsValid)
            {
                if (contaVM.Id > 0)
                {
                    if (contaVM.JsonParcelas != null && !String.IsNullOrEmpty(contaVM.JsonParcelas))
                        contaVM.ParcelasVM = JsonConvert.DeserializeObject<IList<ParcelaVM>>(contaVM.JsonParcelas);
                    conta = Repositorio.ObterPorId(contaVM.Id);
                    //limpando para não dar erro no mapping
                    conta.Parcelas.Clear();
                    conta.Arquivos.Clear();
                    //-------------------------------------
                    conta = Mapper.Map(contaVM, conta);
                    conta.UsuarioAlteracao = Configuracao.Usuario;
                    conta.Parcelas.ForEach(x => x.Conta = conta);
                    conta.Parcelas.Where(x => x.Id == 0).ForEach(x => { x.DataGeracao = DateTime.Now; x.UsuarioCriacao = Configuracao.Usuario; });
                    conta.Arquivos.ForEach(x => x.Conta = conta);
                    Repositorio.Alterar(conta);
                }
                else
                {
                    if (contaVM.JsonParcelas != null && !String.IsNullOrEmpty(contaVM.JsonParcelas))
                        contaVM.ParcelasVM = JsonConvert.DeserializeObject<IList<ParcelaVM>>(contaVM.JsonParcelas);
                    conta = Mapper.Map(contaVM, conta);
                    conta.UsuarioCriacao = Configuracao.Usuario;
                    conta.Parcelas.ForEach(x => x.Conta = conta);
                    conta.Parcelas.Where(x => x.Id == 0).ForEach(x => { x.DataGeracao = DateTime.Now; x.UsuarioCriacao = Configuracao.Usuario; });
                    conta.Arquivos.ForEach(x => x.Conta = conta);
                    Repositorio.Salvar(conta);
                }

                return RedirectToAction("Editar", new { id = conta.Id });
            }
            ViewBag.FormaPagamentoId = new SelectList(new RepositorioFormaPagamento(NHibernateHelper.ObterSessao()).ObterPorParametros(x => x.Situacao == Situacao.Ativo), "Id", "Nome", contaVM.Id);
            ViewBag.PessoaId = new SelectList(new RepositorioPessoa(NHibernateHelper.ObterSessao()).ObterPorParametros(x => x.Situacao == Situacao.Ativo), "Id", "Nome", contaVM.Id);
            ViewBag.SubGastoId = new SelectList(new RepositorioSubGasto(NHibernateHelper.ObterSessao()).ObterPorParametros(x => x.Situacao == Situacao.Ativo), "Id", "DescricaoCompleta", contaVM.Id);
            return View(contaVM);
        }
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public JsonResult Deletar(int id)
        {
            var conta = Repositorio.ObterPorId(id);
            var parcelasDifPendente = conta.Parcelas.Where(x => x.SituacaoParcela != SituacaoParcela.Pendente).Count();
            if (conta.Parcelas.Where(x => x.SituacaoParcela != SituacaoParcela.Pendente).Count() > 0)
            {
                conta.Situacao = SituacaoConta.Cancelado;
                conta.Parcelas.Where(x => x.SituacaoParcela != SituacaoParcela.Pago && x.SituacaoParcela != SituacaoParcela.Cancelado).ForEach(x => x.SituacaoParcela = SituacaoParcela.Cancelado);
                conta.Observacao = "Houve uma tentativa de exclusão, mas havia parcelas que não estavam com situação pendente, neste caso a conta é cancelada!";
                Repositorio.Alterar(conta);
                return Json(conta.Nome + "cancelado com sucesso");
            }
            else
            {
                Repositorio.Excluir(conta);
                return Json(conta.Nome + "excluído com sucesso");
            }
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult Finalizar(Int64 id)
        {
            try
            {
                var conta = Repositorio.ObterPorId(id);
                if (conta != null)
                {
                    conta.Situacao = SituacaoConta.Finalizado;
                    Repositorio.Alterar(conta);
                }
            }
            catch (Exception ex)
            {
                return Json(new { result = false, error = ex.Message });
                
            }
            
            return new EmptyResult();
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult Reabrir(Int64 id)
        {
            try
            {
                var conta = Repositorio.ObterPorId(id);
                if (conta != null)
                {
                    conta.Situacao = SituacaoConta.Aberto;
                    Repositorio.Alterar(conta);
                }
            }
            catch (Exception ex)
            {
                return Json(new { result = false, error = ex.Message });
            }
           
            return new EmptyResult();
        }

    }
}
