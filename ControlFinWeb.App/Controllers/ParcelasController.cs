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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ControlFinWeb.App.Controllers
{
    public class ParcelasController : Controller
    {
        private readonly RepositorioParcela Repositorio;
        private readonly RepositorioFormaPagamento RepositorioFormaPagamento;
        private readonly RepositorioConta RepositorioConta;
        private readonly RepositorioFatura RepositorioFatura;
        private readonly RepositorioBanco RepositorioBanco;
        private readonly IMapper Mapper;
        public ParcelasController(RepositorioParcela repositorio, RepositorioFormaPagamento repositorioFormaPagamento, RepositorioConta repositorioConta, RepositorioFatura repositorioFatura, RepositorioBanco repositorioBanco, IMapper mapper)
        {
            Repositorio = repositorio;
            RepositorioFormaPagamento = repositorioFormaPagamento;
            RepositorioConta = repositorioConta;
            RepositorioFatura = repositorioFatura;
            RepositorioBanco = repositorioBanco;
            Mapper = mapper;
        }

        Parcela parcela = new Parcela();
        ParcelaVM parcelaVM = new ParcelaVM();
        IList<Parcela> parcelas = new List<Parcela>();
        IList<ParcelaVM> parcelasVM = new List<ParcelaVM>();
        GerarParcelasVM gerarParcelasVM = new GerarParcelasVM();
        PagarParcelaVM pagarParcelaVM = new PagarParcelaVM();

        public IActionResult ModalGerarParcelas()
        {
            return View(gerarParcelasVM);
        }

        [HttpPost]
        public String GerarNovasParcelas(GerarParcelasVM gerarParcelasVM)
        {
            Decimal valor = gerarParcelasVM.Valor;
            Int32 qtd = gerarParcelasVM.Qtd;
            DateTime primeiroVencimento = (DateTime)gerarParcelasVM.PrimeiroVencimento;
            Boolean replicar = gerarParcelasVM.Replicar;
            Decimal valorParcela = 0, restante = 0;
            Int64 numero = gerarParcelasVM.UltimoNumero;

            if (replicar)
                valorParcela = valor;
            else
            {
                valorParcela = Math.Round(valor / qtd, 2);
                restante = Math.Round(valor - (valorParcela * qtd), 2);
            }

            for (int i = 1; i <= qtd; i++)
            {
                if (restante > 0 && i == qtd)
                {
                    valorParcela += restante;
                    restante = 0;
                }
                else if (restante < 0 && i == qtd)
                {
                    valorParcela += restante;
                    restante = 0;
                }

                var novaParcela = new ParcelaVM()
                {
                    Numero = numero + i,
                    ParcelaDe = $"{i}/{qtd}",
                    ValorParcela = valorParcela,
                    ValorAberto = valorParcela,
                    ValorReajustado = valorParcela,
                    DataVencimento = primeiroVencimento.AddMonths(i),
                    SituacaoParcela = Dominio.ObjetoValor.SituacaoParcela.Pendente,
                };

                parcelasVM.Add(novaParcela);
            }

            var novasParcelas = JsonConvert.SerializeObject(parcelasVM);
            return novasParcelas;
        }

        public IActionResult PagarParcelas(List<Int64> ids)
        {
            if (ids.Count <= 0)
                return new EmptyResult();

            var parcelas = new List<Parcela>();
            foreach (var id in ids)
                parcelas.Add(Repositorio.ObterPorId(id));

            pagarParcelaVM.ValorAPagar = parcelas.Sum(x => x.ValorAberto);
            pagarParcelaVM.ValorReajustado = parcelas.Sum(x => x.ValorAberto);
            pagarParcelaVM.DataPagamento = DateTime.Now;
            pagarParcelaVM.JurosValor = parcelas.Sum(x => x.JurosValor);
            pagarParcelaVM.JurosPorcentual = parcelas.Sum(x => x.JurosPorcentual);
            pagarParcelaVM.DescontoValor = parcelas.Sum(x => x.DescontoValor);
            pagarParcelaVM.DescontoPorcentual = parcelas.Sum(x => x.DescontoPorcentual);
            pagarParcelaVM.ValorPago = parcelas.Sum(x => x.ValorPago);
            pagarParcelaVM.JsonParcelasPagar = JsonConvert.SerializeObject(Mapper.Map<List<ParcelaVM>>(parcelas)); //Mapper.Map<List<ParcelaVM>>(parcelas);

            ViewBag.FormaPagamentoId = new SelectList(new RepositorioFormaPagamento(NHibernateHelper.ObterSessao()).ObterPorParametros(x => x.Situacao == Situacao.Ativo), "Id", "Nome");
            ViewBag.BancoId = new SelectList(new RepositorioBanco(NHibernateHelper.ObterSessao()).ObterPorParametros(x => x.Situacao == Situacao.Ativo), "Id", "DadosCompletos");
            return PartialView(pagarParcelaVM);
        }

        [HttpPost]
        public IActionResult PagarParcelas(PagarParcelaVM pagarParcelaVM)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    var parcelasVMPagas = JsonConvert.DeserializeObject<IList<ParcelaVM>>(pagarParcelaVM.JsonParcelasPagar);

                    var valorPago = pagarParcelaVM.ValorPago;
                    foreach (var parcelaVM in parcelasVMPagas)
                    {
                        if (valorPago > 0 || valorPago <= parcelaVM.ValorReajustado)
                        {

                            parcelaVM.DataPagamento = pagarParcelaVM.DataPagamento;
                            parcelaVM.JurosPorcentual = pagarParcelaVM.JurosPorcentual;
                            parcelaVM.JurosValor = pagarParcelaVM.JurosValor;
                            parcelaVM.DescontoPorcentual = pagarParcelaVM.DescontoPorcentual;
                            parcelaVM.DescontoValor = pagarParcelaVM.DescontoValor;
                            parcelaVM.FormaPagamentoId = pagarParcelaVM.FormaPagamentoId;
                            parcelaVM.BancoId = pagarParcelaVM.BancoId;
                            parcelaVM.ValorReajustado = (parcelaVM.ValorParcela + parcelaVM.JurosValor) - parcelaVM.DescontoValor;
                            parcelaVM.ValorPago = parcelaVM.ValorReajustado <= valorPago ? parcelaVM.ValorReajustado : valorPago;
                            parcelaVM.ValorAberto = (parcelaVM.ValorParcela + parcelaVM.JurosValor) - parcelaVM.DescontoValor - parcelaVM.ValorPago;
                            if (parcelaVM.ValorAberto > 0)
                                parcelaVM.SituacaoParcela = SituacaoParcela.PendenteParcial;
                            else
                                parcelaVM.SituacaoParcela = SituacaoParcela.Pago;
                            parcelaVM.UsuarioAlteracaoId = Configuracao.Usuario.Id;
                            valorPago -= parcelaVM.ValorPago;
                        }
                        parcelasVM.Add(parcelaVM);
                    }

                    parcelas = Mapper.Map(parcelasVM, parcelas);

                    if (parcelasVM.First().ContaId > 0)
                        parcelas.ForEach(x => x.Conta = RepositorioConta.ObterPorId(parcelasVM.First().ContaId));
                    else
                        parcelas.ForEach(x => x.Fatura = RepositorioFatura.ObterPorId(parcelasVM.First().FaturaId));

                    var banco = parcelasVM.First().BancoId > 0 ? RepositorioBanco.ObterPorId(parcelasVM.First().BancoId) : null;

                    Repositorio.PagamentoParcela(parcelas, Configuracao.Usuario, banco);
                    return new EmptyResult();
            }
                catch (Exception ex)
            {
                return Json(new { result = false, error = ex.Message.ToString() });
            }
        }

            ViewBag.FormaPagamentoId = new SelectList(new RepositorioFormaPagamento(NHibernateHelper.ObterSessao()).ObterPorParametros(x => x.Situacao == Situacao.Ativo), "Id", "Nome");
            ViewBag.BancoId = new SelectList(new RepositorioBanco(NHibernateHelper.ObterSessao()).ObterPorParametros(x => x.Situacao == Situacao.Ativo), "Id", "DadosCompletos");
            return PartialView(pagarParcelaVM);
        }

        [HttpGet("FormaPagamentoDebito/{idFormaPagamento}")]
        public bool FormaPagamentoDebito(int idFormaPagamento)
        {
            return RepositorioFormaPagamento.ObterPorId(idFormaPagamento).DebitoAutomatico;
        }
    }
}
