using App1.Repositorio.Configuracao;
using AutoMapper;
using ControlFinWeb.App.Utilitarios;
using ControlFinWeb.App.ViewModels;
using ControlFinWeb.Dominio.Entidades;
using ControlFinWeb.Dominio.ObjetoValor;
using ControlFinWeb.Repositorio.Repositorios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ObjectsComparer;
using System;
using System.Collections.Generic;

namespace ControlFinWeb.App.Controllers
{
    public class BancosController : Controller
    {
        private readonly RepositorioBanco Repositorio;
        private readonly RepositorioPessoa RepositorioPessoa;
        private readonly IMapper Mapper;

        public BancosController(RepositorioBanco repositorio, RepositorioPessoa repositorioPessoa, IMapper mapper)
        {
            Repositorio = repositorio;
            RepositorioPessoa = repositorioPessoa;
            Mapper = mapper;
        }

        Banco banco = new Banco();
        BancoVM bancoVM = new BancoVM();
        Banco cloneBanco;

        public IActionResult Index()
        {
            IEnumerable<Banco> bancos = Repositorio.ObterPorParametros(x => x.UsuarioCriacao.Id == Configuracao.Usuario.Id); 
            List<BancoVM> bancosVM = Mapper.Map<List<BancoVM>>(bancos);
            return View(bancosVM);
        }

        public IActionResult Editar(Int64 Id = 0)
        {
            if (Id > 0)
            {
                banco = Repositorio.ObterPorId(Id);
                bancoVM = Mapper.Map<BancoVM>(banco);
            }

            ViewBag.Pessoas = new SelectList(new RepositorioPessoa(NHibernateHelper.ObterSessao()).ObterPorParametros(x => x.Situacao == Situacao.Ativo), "Id", "Nome", Id);
            return View(bancoVM);
        }

        [HttpPost]
        public IActionResult Editar(BancoVM bancoVM)
        {
            if (ModelState.IsValid)
            {
                if (bancoVM.Id > 0)
                {
                    bancoVM.PessoaRefBancoVM = Mapper.Map<PessoaVM>(RepositorioPessoa.ObterPorId(bancoVM.PessoaRefBancoId));
                    banco = Repositorio.ObterPorId(bancoVM.Id);
                    cloneBanco = (Banco)banco.Clone();
                    banco = Mapper.Map(bancoVM, banco);
                    banco.UsuarioAlteracao = Configuracao.Usuario;
                    CompararAlteracoes();
                }
                else
                {
                    banco = Mapper.Map(bancoVM, banco);
                    banco.UsuarioCriacao = Configuracao.Usuario;
                    Repositorio.Salvar(banco);
                }

                return new EmptyResult();
            }
            ViewBag.Pessoas = new SelectList(new RepositorioPessoa(NHibernateHelper.ObterSessao()).ObterPorParametros(x => x.Situacao == Situacao.Ativo), "Id", "Nome", bancoVM.Id);
            return View(bancoVM);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public JsonResult Deletar(Int64 Id)
        {
            banco = Repositorio.ObterPorId(Id);
            Repositorio.ExcluirRegistrarLog(banco, Configuracao.Usuario);
            return Json(banco.Nome + "excluído com sucesso");
        }

        private void CompararAlteracoes()
        {
            var comparer = new ObjectsComparer.Comparer<Banco>();
            comparer.AddComparerOverride<Usuario>(DoNotCompareValueComparer.Instance);
            comparer.AddComparerOverride<Int64>(DoNotCompareValueComparer.Instance, member => member.Name.Contains("Id"));
            comparer.AddComparerOverride<String>(DoNotCompareValueComparer.Instance, member => member.Name.StartsWith("_"));
            var igual = comparer.Compare(cloneBanco, banco, out IEnumerable<Difference> diferencas);
            if (!igual)
                Repositorio.EditarRegistrarLog(banco, diferencas, Configuracao.Usuario, String.Format("Banco[{0}]", banco.Id));
        }
    }
}
