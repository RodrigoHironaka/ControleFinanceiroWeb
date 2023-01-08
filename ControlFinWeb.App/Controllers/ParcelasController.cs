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
using System.Collections;
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

        public IActionResult Index(FiltroParcelasVM filtrarParcelasVM)
        {
            var predicado = Repositorio.CriarPredicado();

            if(filtrarParcelasVM.DataInicio != null)
                predicado = predicado.And(x => x.DataVencimento >= filtrarParcelasVM.DataInicio);

            if (filtrarParcelasVM.DataFinal != null)
            {
                if(filtrarParcelasVM.DataFinal >= filtrarParcelasVM.DataInicio)
                    predicado = predicado.And(x => x.DataVencimento <= filtrarParcelasVM.DataFinal.Value.AddHours(23).AddMinutes(59).AddSeconds(59));
            }

            if (filtrarParcelasVM.TipoConta != null)
                predicado = predicado.And(x => x.Conta.TipoConta == filtrarParcelasVM.TipoConta);

            if (filtrarParcelasVM.PeriodoConta != null)
                predicado = predicado.And(x => x.Conta.TipoPeriodo == filtrarParcelasVM.PeriodoConta);

            if (filtrarParcelasVM.PessoaId > 0)
                predicado = predicado.And(x => x.Conta.Pessoa.Id == filtrarParcelasVM.PessoaId);

            if (!String.IsNullOrEmpty(filtrarParcelasVM.SituacaoParcela))
            {
                var situacaoesParcelas = new List<SituacaoParcela>();
                var situacoes = filtrarParcelasVM.SituacaoParcela.Split(",");
                foreach (var num in situacoes)
                {
                    var numero = Int32.Parse(num);
                    situacaoesParcelas.Add((SituacaoParcela)Int32.Parse(num));
                }
                predicado = predicado.And(x => situacaoesParcelas.Contains(x.SituacaoParcela));
            }

            var parcelas = Repositorio.ObterPorParametros(predicado).ToList();

            if (filtrarParcelasVM.ContaFatura != null)
            {
                if (filtrarParcelasVM.ContaFatura == ContaFatura.Conta)
                    parcelas = parcelas.Where(x => x.Conta?.Situacao == SituacaoConta.Aberto).ToList();
                else
                    parcelas = parcelas.Where(x => x.Fatura?.SituacaoFatura == SituacaoFatura.Aberta || x.Fatura?.SituacaoFatura == SituacaoFatura.Fechada).ToList();
            }
            else
            {
                parcelas = parcelas.Where(x => x.Conta?.Situacao == SituacaoConta.Aberto || x.Fatura?.SituacaoFatura == SituacaoFatura.Aberta || x.Fatura?.SituacaoFatura == SituacaoFatura.Fechada).ToList();
            }
          
            filtrarParcelasVM.Parcelas = Mapper.Map<List<ParcelaVM>>(parcelas);
            
            ViewBag.PessoaId = new SelectList(new RepositorioPessoa(NHibernateHelper.ObterSessao()).ObterPorParametros(x => x.Situacao == Situacao.Ativo), "Id", "Nome", filtrarParcelasVM.PessoaId);
            return View(filtrarParcelasVM);
        }

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
            pagarParcelaVM.JsonParcelasPagar = JsonConvert.SerializeObject(Mapper.Map<List<ParcelaVM>>(parcelas));

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

                    decimal valorPagoGeral = pagarParcelaVM.ValorPago;
                    IDictionary<Int64, decimal> valoresPagoParcelas = new Dictionary<Int64, decimal>();
   

                    foreach (var parcelaVM in parcelasVMPagas)
                    {
                        if (valorPagoGeral > 0)
                        {
                            parcelaVM.DataPagamento = pagarParcelaVM.DataPagamento;
                            parcelaVM.JurosPorcentual = pagarParcelaVM.JurosPorcentual;
                            parcelaVM.JurosValor = pagarParcelaVM.JurosValor;
                            parcelaVM.DescontoPorcentual = pagarParcelaVM.DescontoPorcentual;
                            parcelaVM.DescontoValor = pagarParcelaVM.DescontoValor;
                            parcelaVM.FormaPagamentoId = pagarParcelaVM.FormaPagamentoId;
                            parcelaVM.BancoId = pagarParcelaVM.BancoId;
                            parcelaVM.ValorReajustado = (parcelaVM.ValorAberto + parcelaVM.JurosValor) - parcelaVM.DescontoValor;
                            valoresPagoParcelas.Add(parcelaVM.Id, parcelaVM.ValorReajustado <= valorPagoGeral ? parcelaVM.ValorReajustado : valorPagoGeral);//add no Dicitionary para usar depois os valores de cada parcela paga
                            parcelaVM.ValorPago += valoresPagoParcelas[parcelaVM.Id];
                            parcelaVM.ValorAberto = (parcelaVM.ValorAberto + parcelaVM.JurosValor) - parcelaVM.DescontoValor - valoresPagoParcelas[parcelaVM.Id];
                            if (parcelaVM.ValorAberto > 0)
                                parcelaVM.SituacaoParcela = SituacaoParcela.PendenteParcial;
                            else
                                parcelaVM.SituacaoParcela = SituacaoParcela.Pago;
                            parcelaVM.UsuarioAlteracaoId = Configuracao.Usuario.Id;

                            valorPagoGeral -= parcelaVM.ValorReajustado;
                            parcelasVM.Add(parcelaVM);
                        }
                    }

                    parcelas = Mapper.Map(parcelasVM, parcelas);

                    if (parcelasVM.First().ContaId > 0)
                        parcelas.ForEach(x => x.Conta = RepositorioConta.ObterPorId(parcelasVM.First().ContaId));
                    else
                    {
                        foreach (var parcelaFatura in parcelas)
                        {
                            var faturaId = parcelasVM.Where(x => x.Id == parcelaFatura.Id).First().FaturaId;
                            parcelaFatura.Fatura = RepositorioFatura.ObterPorId(faturaId);
                        }
                    }

                    var banco = parcelasVM.First().BancoId > 0 ? RepositorioBanco.ObterPorId(parcelasVM.First().BancoId) : null;

                    Repositorio.PagamentoParcela(parcelas, Configuracao.Usuario, banco, valoresPagoParcelas);
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

        public bool SituacaoDasParcelas(List<Int64> ids)
        {
            foreach (var id in ids)
            {
               var sit = Repositorio.ObterPorId(id).SituacaoParcela;
                if(sit == SituacaoParcela.Pago || sit == SituacaoParcela.Cancelado)
                    return false;
            }
            return true;
        }
    }
}
