

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
using ObjectsComparer;
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
        private readonly RepositorioCartao RepositorioCartao;
        private readonly RepositorioPessoa RepositorioPessoa;
        private readonly IMapper Mapper;
        public FaturasController(RepositorioFatura repositorio, RepositorioParcela repositorioParcela, 
            RepositorioCartao repositorioCartao, RepositorioPessoa repositorioPessoa, IMapper mapper)
        {
            Repositorio = repositorio;
            RepositorioParcela = repositorioParcela;
            RepositorioCartao = repositorioCartao;
            RepositorioPessoa = repositorioPessoa;
            Mapper = mapper;
        }

        Fatura fatura = new Fatura();
        FaturaVM faturaVM = new FaturaVM();
        Fatura cloneFatura;

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
            IEnumerable<Fatura> faturas = Repositorio.ObterPorParametros(x => x.UsuarioCriacao.Id == Configuracao.Usuario.Id && (x.SituacaoFatura == SituacaoFatura.Aberta || x.SituacaoFatura == SituacaoFatura.AbertaParcial));
            List<FaturaVM> faturasVM = Mapper.Map<List<FaturaVM>>(faturas);
            ViewBag.Cartoes = PreencheCombo.Cartoes(); //new SelectList(new RepositorioCartao(NHibernateHelper.ObterSessao()).ObterPorParametros(x => x.Situacao == Situacao.Ativo), "Id", "Nome", null);
            return View(faturasVM);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
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
            return PartialView("_TabelaFaturas", faturasVM);
        }

        public IActionResult Editar(Int64 Id = 0)
        {
            if (Id > 0)
            {
                fatura = Repositorio.ObterPorId(Id);
                faturaVM = Mapper.Map<FaturaVM>(fatura);
            }
            return View(faturaVM);
        }

        [HttpPost]
        public IActionResult Editar(FaturaVM faturaVM)
        {
            if (ModelState.IsValid)
            {
                if (faturaVM.Id > 0)
                {
                    faturaVM.CartaoVM = Mapper.Map<CartaoVM>(RepositorioCartao.ObterPorId(faturaVM.CartaoId));
                    faturaVM.PessoaVM = Mapper.Map<PessoaVM>(RepositorioPessoa.ObterPorId(faturaVM.PessoaId));
                    fatura = Repositorio.ObterPorId(faturaVM.Id);
                    cloneFatura = (Fatura)fatura.Clone();
                    fatura = Mapper.Map(faturaVM, fatura);
                    fatura.UsuarioAlteracao = Configuracao.Usuario;
                    CompararAlteracoes();
                }
                else
                {
                    fatura = Mapper.Map(faturaVM, fatura);
                    fatura.UsuarioCriacao = Configuracao.Usuario;
                    Repositorio.SalvarEGerarNovaParcela(fatura);
                }
                return new EmptyResult();
            }
            return View(faturaVM);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public JsonResult Deletar(int id)
        {
            Repositorio.ExcluirOuCancelarFaturaEParcela(id, Configuracao.Usuario);
            return Json(fatura._DescricaoCompleta + "excluído com sucesso");
        }

        [IgnoreAntiforgeryToken]
        public IActionResult FecharFatura(String obs, Int64 id = 0)
        {
            fatura = Repositorio.ObterPorId(id);
            if (fatura != null)
            {
                cloneFatura = (Fatura)fatura.Clone();
                fatura.SituacaoFatura = SituacaoFatura.Fechada;
                fatura.DataFechamento = DateTime.Now;
                if (!String.IsNullOrEmpty(obs))
                    fatura.Nome = obs;
                CompararAlteracoes();
            }
            return new EmptyResult();
        }

        public Boolean ExisteFatura(FaturaVM faturaVM)
        {
            var existeFatura = Repositorio.ObterPorParametros(x => x.Cartao.Id == faturaVM.CartaoId && x.MesAnoReferencia == faturaVM.MesAnoReferencia && x.Pessoa.Id == faturaVM.PessoaId && x.Id != faturaVM.Id).FirstOrDefault();
            if (existeFatura != null)
                return true;

            return false;
        }

        private void CompararAlteracoes()
        {
            var comparer = new ObjectsComparer.Comparer<Fatura>();
            comparer.AddComparerOverride<Usuario>(DoNotCompareValueComparer.Instance);
            comparer.AddComparerOverride<Int64>(DoNotCompareValueComparer.Instance, member => member.Name.Contains("Id"));
            comparer.AddComparerOverride<String>(DoNotCompareValueComparer.Instance, member => member.Name.StartsWith("_"));
            comparer.AddComparerOverride<Decimal>(DoNotCompareValueComparer.Instance, member => member.Name.StartsWith("_"));
            comparer.IgnoreMember("DataGeracao");
            comparer.IgnoreMember("DataAlteracao");
            comparer.IgnoreMember<IList<FaturaItens>>();
            var igual = comparer.Compare(cloneFatura, fatura, out IEnumerable<Difference> diferencas);
            if (!igual)
                Repositorio.EditarRegistrarLog(fatura, diferencas, Configuracao.Usuario, $"Fatura[{fatura.Id}]");
        }
    }
}
