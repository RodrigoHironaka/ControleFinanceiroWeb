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
    public class CaixasController : Controller
    {
        private readonly RepositorioCaixa Repositorio;
        private readonly IMapper Mapper;

        public CaixasController(RepositorioCaixa repositorio, IMapper mapper)
        {
            Repositorio = repositorio;
            Mapper = mapper;
        }

        Caixa caixa = new Caixa();
        CaixaVM caixaVM = new CaixaVM();
        Caixa cloneCaixa;

        public IActionResult Index(Int64 idCaixa = 0)
        {
            if (idCaixa == 0)
            {
                caixa = Repositorio.ObterCaixaAberto(Configuracao.Usuario.Id);
                if (caixa != null)
                    caixaVM = Mapper.Map(caixa, caixaVM);
            }
            else
            {
                caixa = Repositorio.ObterPorId(idCaixa);
                if (caixa != null)
                    caixaVM = Mapper.Map<CaixaVM>(caixa);
            }
            return View(caixaVM);
        }

        public IActionResult Pesquisa()
        {
            IEnumerable<Caixa> caixas = Repositorio.ObterPorParametros(x => x.UsuarioCriacao.Id == Configuracao.Usuario.Id);
            List<CaixaVM> caixasVM = Mapper.Map<List<CaixaVM>>(caixas);
            return View(caixasVM);

        }

        public IActionResult Editar(Int64 Id = 0)
        {
            if (Id > 0)
            {
                caixa = Repositorio.ObterPorId(Id);
                caixaVM = Mapper.Map(caixa, caixaVM);
            }

            return View(caixaVM);
        }

        [HttpPost]
        public IActionResult Editar(CaixaVM caixaVM)
        {
            if (ModelState.IsValid)
            {
                if (caixaVM.Id > 0)
                {
                    caixa = Repositorio.ObterPorId(caixaVM.Id);
                    cloneCaixa = (Caixa)caixa.Clone();
                    caixa = Mapper.Map(caixaVM, caixa);
                    caixa.UsuarioAlteracao = Configuracao.Usuario;
                    caixa.Situacao = Dominio.ObjetoValor.SituacaoCaixa.Fechado;
                    CompararAlteracoes();
                }
                else
                {
                    caixa = Mapper.Map(caixaVM, caixa);
                    caixa.UsuarioCriacao = Configuracao.Usuario;
                    var ultimoCaixa = Repositorio.ObterPorParametros(x => x.UsuarioCriacao.Id == Configuracao.Usuario.Id).OrderByDescending(x => x.Id).FirstOrDefault();
                    caixa.Numero = ultimoCaixa != null ? ultimoCaixa.Numero + 1 : 1;
                    Repositorio.SalvarCaixa(caixa, Configuracao.Usuario);
                }

                return RedirectToAction("Index");
            }
            return View(caixaVM);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public JsonResult Deletar(Int64 Id)
        {
            caixa = Repositorio.ObterPorId(Id);
            Repositorio.ExcluirRegistrarLog(caixa, Configuracao.Usuario);
            return Json(caixa.Id + "excluído com sucesso");
        }

        private void CompararAlteracoes()
        {
            var comparer = new ObjectsComparer.Comparer<Caixa>();
            comparer.AddComparerOverride<Usuario>(DoNotCompareValueComparer.Instance);
            comparer.AddComparerOverride<Int64>(DoNotCompareValueComparer.Instance, member => member.Name.Contains("Id"));
            comparer.AddComparerOverride<Decimal>(DoNotCompareValueComparer.Instance, member => member.Name.StartsWith("_"));
            comparer.IgnoreMember("DataGeracao");
            comparer.IgnoreMember("DataAlteracao");
            comparer.IgnoreMember<IList<FluxoCaixa>>();
            var igual = comparer.Compare(cloneCaixa, caixa, out IEnumerable<Difference> diferencas);
            if (!igual)
                Repositorio.EditarRegistrarLog(caixa, diferencas, Configuracao.Usuario, $"Caixa[{caixa.Id}]");
        }
    }
}
