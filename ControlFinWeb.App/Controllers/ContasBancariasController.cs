using App1.Repositorio.Configuracao;
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
using Microsoft.Extensions.Options;
using ObjectsComparer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.Extensions.Enums;

namespace ControlFinWeb.App.Controllers
{
    public class ContasBancariasController : Controller
    {
        private readonly RepositorioContaBancaria Repositorio;
        private readonly RepositorioCaixa RepositorioCaixa;
        private readonly RepositorioBanco RepositorioBanco;
        private readonly IMapper Mapper;

        public ContasBancariasController(RepositorioContaBancaria repositorio, RepositorioCaixa repositorioCaixa, RepositorioBanco repositorioBanco, IMapper mapper)
        {
            Repositorio = repositorio;
            RepositorioCaixa = repositorioCaixa;
            RepositorioBanco = repositorioBanco;
            Mapper = mapper;
        }

        ContaBancaria contaBancaria = new ContaBancaria();
        ContaBancariaVM contaBancariaVM = new ContaBancariaVM();
        ContaBancaria cloneContaBancaria;

        public IActionResult Index(FiltroContaBancariaVM filtroContaBancariaVM)
        {
            var predicado = Repositorio.CriarPredicado();
            var inicioMes = new DateTime(filtroContaBancariaVM.Ano, filtroContaBancariaVM.Mes, 1);
            var finalMes = inicioMes.UltimoDiaMes().FinalDia();

            predicado = predicado.And(x => x.DataRegistro >= inicioMes);
            predicado = predicado.And(x => x.DataRegistro <= finalMes);

            if(filtroContaBancariaVM.BancoId > 0)
                predicado = predicado.And(x => x.Banco.Id == filtroContaBancariaVM.BancoId);

            filtroContaBancariaVM.ContasBancarias = Mapper.Map<List<ContaBancariaVM>>(Repositorio.ObterPorParametros(predicado));

            return View(filtroContaBancariaVM);
        }

        public IActionResult Filtrar(FiltroContaBancariaVM filtroContaBancariaVM)
        {
            var predicado = Repositorio.CriarPredicado();
            var inicioMes = new DateTime(filtroContaBancariaVM.Ano, filtroContaBancariaVM.Mes, 1);
            var finalMes = inicioMes.UltimoDiaMes().FinalDia();

            predicado = predicado.And(x => x.DataRegistro >= inicioMes);
            predicado = predicado.And(x => x.DataRegistro <= finalMes);

            if (filtroContaBancariaVM.BancoId > 0)
                predicado = predicado.And(x => x.Banco.Id == filtroContaBancariaVM.BancoId);

            filtroContaBancariaVM.ContasBancarias = Mapper.Map<List<ContaBancariaVM>>(Repositorio.ObterPorParametros(predicado));

            return PartialView("_RegistrosBancarios", filtroContaBancariaVM.ContasBancarias);
        }

        public IActionResult Editar(Int64 Id = 0, decimal valorTransferencia = 0)
        {
            if (Id > 0)
            {
                contaBancaria = Repositorio.ObterPorId(Id);
                contaBancariaVM = Mapper.Map<ContaBancariaVM>(contaBancaria);
            }

            if (valorTransferencia > 0)
            {
                contaBancariaVM.Valor = valorTransferencia;
                contaBancariaVM.GerarFluxoCaixa = true;
            }
            return View(contaBancariaVM);
        }

        [HttpPost]
        public IActionResult Editar(ContaBancariaVM contaBancariaVM)
        {
            if (ModelState.IsValid)
            {
                if (contaBancariaVM.TransfereParaCaixa == true)
                {
                    var saldo = Repositorio.Saldo(DateTime.Now, contaBancariaVM.BancoId);
                    if(saldo < contaBancariaVM.Valor)
                        return Json(new { result = false, error = "Saldo insuficiente na conta bancária!" });
                }

                if (contaBancariaVM.Id > 0)
                {
                    contaBancariaVM.CaixaVM = Mapper.Map<CaixaVM>(RepositorioCaixa.ObterPorId(contaBancariaVM.CaixaId));
                    contaBancariaVM.BancoVM = Mapper.Map<BancoVM>(RepositorioBanco.ObterPorId(contaBancariaVM.BancoId));
                    contaBancaria = Repositorio.ObterPorId(contaBancariaVM.Id);
                    cloneContaBancaria = (ContaBancaria)contaBancaria.Clone();
                    contaBancaria = Mapper.Map(contaBancariaVM, contaBancaria);
                    contaBancaria.UsuarioAlteracao = Configuracao.Usuario;
                    CompararAlteracoes();
                }
                else
                {
                    contaBancaria = Mapper.Map(contaBancariaVM, contaBancaria);
                    contaBancaria.UsuarioCriacao = Configuracao.Usuario;
                    if (contaBancariaVM.GerarFluxoCaixa)
                        Repositorio.SalvarAddFluxoCaixa(contaBancaria, Configuracao.Usuario);
                    else
                        Repositorio.Salvar(contaBancaria);
                }

                return new EmptyResult();
            }
            return View(contaBancariaVM);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public JsonResult Deletar(Int64 Id)
        {
            contaBancaria = Repositorio.ObterPorId(Id);
            Repositorio.ExcluirRegistrarLog(contaBancaria, Configuracao.Usuario);
            return Json(contaBancaria.Nome + "excluído com sucesso");
        }

        private void CompararAlteracoes()
        {
            var comparer = new ObjectsComparer.Comparer<ContaBancaria>();
            comparer.AddComparerOverride<Usuario>(DoNotCompareValueComparer.Instance);
            comparer.AddComparerOverride<Caixa>(DoNotCompareValueComparer.Instance);
            comparer.AddComparerOverride<Int64>(DoNotCompareValueComparer.Instance, member => member.Name.Contains("Id"));
            comparer.AddComparerOverride<String>(DoNotCompareValueComparer.Instance, member => member.Name.StartsWith("_"));
            comparer.IgnoreMember("DataGeracao");
            comparer.IgnoreMember("DataAlteracao");
            var igual = comparer.Compare(cloneContaBancaria, contaBancaria, out IEnumerable<Difference> diferencas);
            if (!igual)
                Repositorio.EditarRegistrarLog(contaBancaria, diferencas, Configuracao.Usuario, String.Format("ContaBancaria[{0}]", contaBancaria.Id));
        }
    }
}
