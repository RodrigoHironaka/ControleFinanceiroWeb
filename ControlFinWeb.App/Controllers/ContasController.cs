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
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ObjectsComparer;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ControlFinWeb.App.Controllers
{
    public class ContasController : Controller
    {
        private readonly RepositorioConta Repositorio;
        private readonly RepositorioPessoa RepositorioPessoa;
        private readonly RepositorioSubGasto RepositorioSubGasto;
        private readonly IMapper Mapper;
        private readonly ILogger<ContasController> _logger;
        public ContasController(RepositorioConta repositorio, RepositorioPessoa repositorioPessoa, RepositorioSubGasto repositorioSubGasto, IMapper mapper, ILogger<ContasController> logger)
        {
            Repositorio = repositorio;
            RepositorioPessoa = repositorioPessoa;
            RepositorioSubGasto = repositorioSubGasto;
            Mapper = mapper;
            _logger = logger;
        }

        Conta conta = new Conta();
        ContaVM contaVM = new ContaVM();
        Conta cloneConta;

        public IActionResult Index()
        {
            var predicado = Repositorio.CriarPredicado();
            predicado = predicado.And(x => x.UsuarioCriacao.Id == Configuracao.Usuario.Id);
            predicado = predicado.And(x => x.Situacao == SituacaoConta.Aberto);

            IEnumerable<Conta> contas = Repositorio.ObterPorParametros(predicado);
            List<ContaVM> contasVM = Mapper
                .Map<List<ContaVM>>(contas);
            return View(contasVM);
        }
        public IActionResult Filtrar(Boolean somenteAbertos)
        {
            var predicado = Repositorio.CriarPredicado();
            predicado = predicado.And(x => x.UsuarioCriacao.Id == Configuracao.Usuario.Id);
            if (somenteAbertos)
                predicado = predicado.And(x => x.Situacao == SituacaoConta.Aberto);

            IEnumerable<Conta> contas = Repositorio.ObterPorParametros(predicado);
            List<ContaVM> contasVM = Mapper
                .Map<List<ContaVM>>(contas);
            return PartialView("_ListaContas",contasVM);
        }


        public IActionResult Editar(Int64 Id = 0)
        {
            if (Id > 0)
            {
                conta = Repositorio.ObterPorId(Id);
                contaVM = Mapper.Map<ContaVM>(conta);
                contaVM.JsonParcelas = JsonConvert.SerializeObject(contaVM.ParcelasVM);
            }
            return View(contaVM);
        }

        [HttpPost]
        public IActionResult Editar(ContaVM contaVM)
        {
            if (ModelState.IsValid)
            {
                if (contaVM.Id > 0)
                {
                    contaVM.SubGastoVM = Mapper.Map<SubGastoVM>(RepositorioSubGasto.ObterPorId(contaVM.SubGastoId));
                    contaVM.PessoaVM = Mapper.Map<PessoaVM>(RepositorioPessoa.ObterPorId(contaVM.PessoaId));
                    if (contaVM.JsonParcelas != null && !String.IsNullOrEmpty(contaVM.JsonParcelas))
                        contaVM.ParcelasVM = JsonConvert.DeserializeObject<IList<ParcelaVM>>(contaVM.JsonParcelas);
                    conta = Repositorio.ObterPorId(contaVM.Id);
                    cloneConta = (Conta)conta.Clone();

                    //limpando para não dar erro no mapping
                    conta.Parcelas.Clear();
                    conta.Arquivos.Clear();
                    //-------------------------------------
                    conta = Mapper.Map(contaVM, conta);
                    conta.UsuarioAlteracao = Configuracao.Usuario;
                    conta.Parcelas.ForEach(x => x.Conta = conta);
                    conta.Parcelas.Where(x => x.Id == 0).ForEach(x => { x.DataGeracao = DateTime.Now; x.UsuarioCriacao = Configuracao.Usuario; });
                    conta.Arquivos.ForEach(x => x.Conta = conta);
                    CompararAlteracoes();
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
            return View(contaVM);
        }
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public JsonResult Deletar(int id)
        {
            conta = Repositorio.ObterPorId(id);
            cloneConta = (Conta)conta.Clone();
            var parcelasDifPendente = conta.Parcelas.Where(x => x.SituacaoParcela != SituacaoParcela.Pendente).Count();
            if (conta.Parcelas.Where(x => x.SituacaoParcela != SituacaoParcela.Pendente).Count() > 0)
            {
                conta.Situacao = SituacaoConta.Cancelado;
                conta.Parcelas.Where(x => x.SituacaoParcela != SituacaoParcela.Pago && x.SituacaoParcela != SituacaoParcela.Cancelado).ForEach(x => x.SituacaoParcela = SituacaoParcela.Cancelado);
                conta.Observacao = "Houve uma tentativa de exclusão, mas havia parcelas que não estavam com situação pendente, neste caso a conta é cancelada!";
                CompararAlteracoes();
                return Json(conta.Nome + "cancelado com sucesso");
            }
            else
            {
                Repositorio.ExcluirRegistrarLog(conta, Configuracao.Usuario);
                return Json(conta.Nome + "excluído com sucesso");
            }
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult Finalizar(Int64 id)
        {
            try
            {
                conta = Repositorio.ObterPorId(id);
                cloneConta = (Conta)conta.Clone();
                if (conta != null)
                {
                    conta.Situacao = SituacaoConta.Finalizado;
                    CompararAlteracoes();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro finalizar conta");
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
                conta = Repositorio.ObterPorId(id);
                cloneConta = (Conta)conta.Clone();
                if (conta != null)
                {
                    conta.Situacao = SituacaoConta.Aberto;
                    CompararAlteracoes();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro reabrir conta");
                return Json(new { result = false, error = ex.Message });
            }
           
            return new EmptyResult();
        }

        private void CompararAlteracoes()
        {
            var comparer = new ObjectsComparer.Comparer<Conta>();
            comparer.AddComparerOverride<Usuario>(DoNotCompareValueComparer.Instance);
            comparer.AddComparerOverride<Int64>(DoNotCompareValueComparer.Instance, member => member.Name.Contains("Id"));
            comparer.AddComparerOverride<String>(DoNotCompareValueComparer.Instance, member => member.Name.StartsWith("_"));
            comparer.IgnoreMember("DataGeracao");
            comparer.IgnoreMember("DataAlteracao");
            comparer.IgnoreMember<IList<Parcela>>();
            comparer.IgnoreMember<IList<Arquivo>>();
            var igual = comparer.Compare(cloneConta, conta, out IEnumerable<Difference> diferencas);
            if (!igual)
                Repositorio.EditarRegistrarLog(conta, diferencas, Configuracao.Usuario, $"{conta.GetType().Name}[{conta.Id}]");
        }
    }
}
