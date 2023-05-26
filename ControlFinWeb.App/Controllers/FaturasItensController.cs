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
using ObjectsComparer;
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
        private readonly RepositorioPessoa RepositorioPessoa;
        private readonly RepositorioSubGasto RepositorioSubGasto;
        private readonly IMapper Mapper;
        public FaturasItensController(RepositorioFaturaItens repositorio, RepositorioFatura repositorioFatura,
            RepositorioParcela repositorioParcela, RepositorioPessoa repositorioPessoa, RepositorioSubGasto repositorioSubGasto,
            IMapper mapper)
        {
            Repositorio = repositorio;
            RepositorioFatura = repositorioFatura;
            RepositorioParcela = repositorioParcela;
            RepositorioPessoa = repositorioPessoa;
            RepositorioSubGasto = repositorioSubGasto;
            Mapper = mapper;
        }

        FaturaItens faturaItens = new FaturaItens();
        FaturaItensVM faturaItensVM = new FaturaItensVM();
        FaturaItens cloneFaturaItens;

        public IActionResult Index(Int64 IdFatura)
        {
            IEnumerable<FaturaItens> faturasItens = Repositorio.ObterPorParametros(x => x.Fatura.Id == IdFatura);
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
            return View(faturaItensVM);
        }

        [HttpPost]
        public IActionResult Editar(FaturaItensVM faturaItensVM)
        {
            if (ModelState.IsValid)
            {
                this.faturaItensVM = faturaItensVM;
                if (faturaItensVM.Id > 0)
                {
                    faturaItensVM.FaturaVM = Mapper.Map<FaturaVM>(RepositorioFatura.ObterPorId(faturaItensVM.FaturaId));
                    faturaItensVM.PessoaVM = Mapper.Map<PessoaVM>(RepositorioPessoa.ObterPorId(faturaItensVM.PessoaId));
                    faturaItensVM.SubGastoVM = Mapper.Map<SubGastoVM>(RepositorioSubGasto.ObterPorId(faturaItensVM.SubGastoId));
                    faturaItens = Repositorio.ObterPorId(faturaItensVM.Id);
                    faturaItens.UsuarioAlteracao = Configuracao.Usuario;
                    cloneFaturaItens = (FaturaItens)faturaItens.Clone();
                }
                else
                    faturaItens.UsuarioCriacao = Configuracao.Usuario;

                faturaItens = Mapper.Map(faturaItensVM, faturaItens);
                faturaItens.Fatura = RepositorioFatura.ObterPorId(faturaItens.Fatura.Id);
                CompararAlteracoes();
                return RedirectToAction("ConsultaItensRelacionados", new { codItemRelacionado = faturaItens.CodigoItemRelacionado });
            }
            return View(faturaItensVM);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public JsonResult Deletar(Int64 id)
        {
            Repositorio.ExcluirItemFaturaEAlterarParcela(id, Configuracao.Usuario);
            return Json("Excluído com sucesso");
        }

        [HttpGet("SituacaoFatura/{idCartao}")]
        public String SituacaoFatura(Int64 idCartao)
        {
            var fatura = RepositorioFatura.ObterPorId(idCartao);
            if (fatura == null)
                return String.Empty;

            return fatura.SituacaoFatura.ToString();
        }

        public IActionResult ConsultaItensRelacionados(String codItemRelacionado)
        {
            IList<FaturaItens> faturasItensFiltradas = new List<FaturaItens>();
            IEnumerable<FaturaItens> faturasItens = Repositorio.ObterPorParametros(x => x.CodigoItemRelacionado == codItemRelacionado);
            foreach (var item in faturasItens)
            {
                var fatura = RepositorioFatura.ObterPorId(item.Fatura.Id);
                if (fatura != null && fatura.SituacaoFatura == Dominio.ObjetoValor.SituacaoFatura.Aberta)
                    faturasItensFiltradas.Add(item);
            }
            List<FaturaItensVM> faturasVM = Mapper
                .Map<List<FaturaItensVM>>(faturasItensFiltradas);
            return View(faturasVM);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public JsonResult EditarPeloRegistro(String idFaturaItem, String novoValor)
        {
            var id = Int64.Parse(idFaturaItem);
            var valor = Decimal.Parse(novoValor);
            if (id > 0 && valor > 0)
            {
                var faturaItem = Repositorio.ObterPorId(id);
                var difference = new Difference("Valor", faturaItem.Valor.ToString("N2"), novoValor);
                IEnumerable<Difference> differences;
                differences = new Difference[] { difference };
                if (faturaItem != null)
                {
                    if (faturaItem.Valor != valor)
                        faturaItem.Valor = valor;
                   
                    Repositorio.SalvarAlterarFaturaItemEAlterarParcela(faturaItem, "1", faturaItensVM.Replicar, differences, Configuracao.Usuario);
                    return Json(new { result = true, message = "Alterado com sucesso! Novo valor " + faturaItem.Valor.ToString("C2") +" - ["+faturaItem.QuantidadeRelacionado+"]" });
                }
            }
            return Json(new { result = false, error = "Erro! Id da fatura ou valor igual a zero!" });
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public JsonResult DeletarPeloCodigoRelacionado(String codItemRelacionado)
        {
            var itens = Repositorio.ObterPorParametros(x => x.CodigoItemRelacionado == codItemRelacionado && x.UsuarioCriacao == Configuracao.Usuario);
            foreach (var item in itens)
            {
                if (item.Fatura.SituacaoFatura == Dominio.ObjetoValor.SituacaoFatura.Aberta)
                    Repositorio.ExcluirItemFaturaEAlterarParcela(item.Id, Configuracao.Usuario);
            }
            return Json("Excluído com sucesso");
        }

        public IActionResult Antecipar(Int64 id)
        {
            faturaItens = Repositorio.ObterPorId(id);
            IList<FaturaItens> faturasItensFiltradas = new List<FaturaItens>();
            IEnumerable<FaturaItens> faturasItens = Repositorio.ObterPorParametros(x => x.CodigoItemRelacionado == faturaItens.CodigoItemRelacionado && x.Fatura.MesAnoReferencia > faturaItens.Fatura.MesAnoReferencia);
      
            foreach (var item in faturasItens)
            {
                var fatura = RepositorioFatura.ObterPorId(item.Fatura.Id);
                if (fatura != null && fatura.SituacaoFatura == Dominio.ObjetoValor.SituacaoFatura.Aberta)
                    faturasItensFiltradas.Add(item);
            }
            List<FaturaItensVM> faturaItensVM = Mapper.Map<List<FaturaItensVM>>(faturasItensFiltradas);

            ViewBag.Aviso = $"ATENÇÃO: Os itens selecionados acima serão adicionados na fatura: {faturaItens.Fatura._DescricaoCompleta}";
            return View(faturaItensVM);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult Antecipar(List<Int64> ids, Int64 idFatura, Decimal desconto)
        {
            if (ids.Count == 0)
                return Json(new { result = false, error = "Nenhum item selecionado!" });

            Repositorio.Antecipar(ids, idFatura, desconto, Configuracao.Usuario);
            return new EmptyResult();
        }

        private void CompararAlteracoes()
        {
            var comparer = new ObjectsComparer.Comparer<FaturaItens>();
            comparer.AddComparerOverride<Usuario>(DoNotCompareValueComparer.Instance);
            comparer.AddComparerOverride<Fatura>(DoNotCompareValueComparer.Instance);
            comparer.AddComparerOverride<Int64>(DoNotCompareValueComparer.Instance, member => member.Name.Contains("Id"));
            comparer.AddComparerOverride<String>(DoNotCompareValueComparer.Instance, member => member.Name.StartsWith("_"));
            comparer.IgnoreMember("DataGeracao");
            comparer.IgnoreMember("DataAlteracao");
            var igual = comparer.Compare(cloneFaturaItens, faturaItens, out IEnumerable<Difference> diferencas);
            if (!igual)
                Repositorio.SalvarAlterarFaturaItemEAlterarParcela(faturaItens, faturaItensVM.NumeroParcelas, faturaItensVM.Replicar, diferencas, Configuracao.Usuario);
        }
    }
}
