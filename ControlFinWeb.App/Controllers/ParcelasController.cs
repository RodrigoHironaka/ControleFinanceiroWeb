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
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Text;

namespace ControlFinWeb.App.Controllers
{
    public class ParcelasController : Controller
    {
        private readonly RepositorioParcela Repositorio;
        private readonly RepositorioFormaPagamento RepositorioFormaPagamento;
        private readonly RepositorioConta RepositorioConta;
        private readonly RepositorioFatura RepositorioFatura;
        private readonly RepositorioBanco RepositorioBanco;
        private readonly RepositorioFluxoCaixa RepositorioFluxoCaixa;
        private readonly IMapper Mapper;
        public ParcelasController(RepositorioParcela repositorio, RepositorioFormaPagamento repositorioFormaPagamento, RepositorioConta repositorioConta,
            RepositorioFatura repositorioFatura, RepositorioBanco repositorioBanco, RepositorioFluxoCaixa repositorioFluxoCaixa, IMapper mapper)
        {
            Repositorio = repositorio;
            RepositorioFormaPagamento = repositorioFormaPagamento;
            RepositorioConta = repositorioConta;
            RepositorioFatura = repositorioFatura;
            RepositorioBanco = repositorioBanco;
            RepositorioFluxoCaixa = repositorioFluxoCaixa;
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

            if (filtrarParcelasVM.DataInicio != null)
                predicado = predicado.And(x => x.DataVencimento >= filtrarParcelasVM.DataInicio);

            if (filtrarParcelasVM.DataFinal != null)
            {
                if (filtrarParcelasVM.DataFinal >= filtrarParcelasVM.DataInicio)
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

        public IActionResult Editar(Int64 Id = 0)
        {
            if (Id > 0)
            {
                parcela = Repositorio.ObterPorId(Id);
                parcelaVM = Mapper.Map(parcela, parcelaVM);
            }
            ViewBag.FormaPagamentoId = new SelectList(new RepositorioFormaPagamento(NHibernateHelper.ObterSessao()).ObterPorParametros(x => x.Situacao == Situacao.Ativo), "Id", "Nome", Id);

            return View(parcelaVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(ParcelaVM parcelaVM)
        {
            if (ModelState.IsValid)
            {
                if (parcelaVM.Id > 0)
                {
                    parcela = Repositorio.ObterPorId(parcelaVM.Id);
                    parcela = Mapper.Map(parcelaVM, parcela);
                    parcela.UsuarioAlteracao = Configuracao.Usuario;
                    Repositorio.Alterar(parcela);
                }
                else
                {
                    parcela = Mapper.Map(parcelaVM, parcela);
                    parcela.UsuarioCriacao = Configuracao.Usuario;
                    Repositorio.Salvar(parcela);
                }
                return new EmptyResult();
            }
            ViewBag.FormaPagamentoId = new SelectList(new RepositorioFormaPagamento(NHibernateHelper.ObterSessao()).ObterPorParametros(x => x.Situacao == Situacao.Ativo), "Id", "Nome", parcelaVM.Id);
            return View(parcelaVM);
        }

        public IActionResult GerarParcelas()
        {
            return View(gerarParcelasVM);
        }

        [HttpPost]
        public IActionResult GerarParcelas(GerarParcelasVM gerarParcelasVM)
        {
            Decimal valor = gerarParcelasVM.Valor;
            Int32 qtd = gerarParcelasVM.Qtd;
            DateTime primeiroVencimento = (DateTime)gerarParcelasVM.PrimeiroVencimento;
            Boolean replicar = gerarParcelasVM.Replicar;
            Decimal valorParcela = 0, restante = 0;
            Int64 numero = gerarParcelasVM.UltimoNumero;
            Int64 contaId = gerarParcelasVM.ContaId;
            var conta = RepositorioConta.ObterPorId(contaId);

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
                    Numero = conta.Parcelas.Count > 0 ? conta.Parcelas.OrderBy(x => x.Numero).Last().Numero + i : i,
                    ParcelaDe = $"{i}/{qtd}",
                    ValorParcela = valorParcela,
                    ValorAberto = valorParcela,
                    ValorReajustado = valorParcela,
                    DataVencimento = primeiroVencimento.AddMonths(i),
                    SituacaoParcela = Dominio.ObjetoValor.SituacaoParcela.Pendente,
                    ContaId = contaId,
                    UsuarioCriacaoId = Configuracao.Usuario.Id,
                    DataGeracao = DateTime.Now,
                };

                parcelasVM.Add(novaParcela);
            }

            parcelas = Mapper.Map<List<Parcela>>(parcelasVM);
            parcelas.ForEach(x => conta.Parcelas.Add(x));
            RepositorioConta.Alterar(conta);
            return new EmptyResult();
        }

        public IActionResult PagarParcelas(List<Int64> ids)
        {
            if (!SituacaoDasParcelas(ids))
                return Json(new { result = false, error = "Parcelas Pagas e Canceladas não podem ser selecionadas!" });

            var parcelas = new List<Parcela>();
            foreach (var id in ids)
                parcelas.Add(Repositorio.ObterPorId(id));

            pagarParcelaVM.ValorAPagar = parcelas.Sum(x => x.ValorAberto);
            pagarParcelaVM.ValorReajustado = parcelas.Sum(x => x.ValorAberto);
            pagarParcelaVM.DataPagamento = DateTime.Now;
            if (parcelas.Sum(x => x.JurosValor) > 0)
                pagarParcelaVM.Mensagens.Add(String.Format("Valor à pagar já possui juros de {0:C2}", parcelas.Sum(x => x.JurosValor)));
            if (parcelas.Sum(x => x.DescontoValor) > 0)
                pagarParcelaVM.Mensagens.Add(String.Format("Valor à pagar já possui desconto de {0:C2}", parcelas.Sum(x => x.DescontoValor)));

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
                        decimal juros = Math.Round(parcelaVM.ValorAberto * (pagarParcelaVM.JurosPorcentual / 100), 2);
                        decimal desconto = Math.Round(parcelaVM.ValorAberto * (pagarParcelaVM.DescontoPorcentual / 100), 2);
                        if (valorPagoGeral > 0)
                        {
                            parcelaVM.UsuarioAlteracaoId = Configuracao.Usuario.Id;
                            parcelaVM.DataPagamento = pagarParcelaVM.DataPagamento;
                            parcelaVM.JurosPorcentual += pagarParcelaVM.JurosPorcentual;
                            parcelaVM.JurosValor += juros;
                            parcelaVM.DescontoPorcentual += pagarParcelaVM.DescontoPorcentual;
                            parcelaVM.DescontoValor += desconto;
                            parcelaVM.FormaPagamentoId = pagarParcelaVM.FormaPagamentoId;
                            parcelaVM.BancoId = pagarParcelaVM.BancoId;
                            parcelaVM.ValorReajustado = (parcelaVM.ValorAberto + juros) - desconto;
                            valoresPagoParcelas.Add(parcelaVM.Id, parcelaVM.ValorReajustado <= valorPagoGeral ? parcelaVM.ValorReajustado : valorPagoGeral);//add no Dicitionary para usar depois os valores de cada parcela paga
                            parcelaVM.ValorPago += valoresPagoParcelas[parcelaVM.Id];
                            parcelaVM.ValorAberto = (parcelaVM.ValorAberto + juros) - desconto - valoresPagoParcelas[parcelaVM.Id];

                            valorPagoGeral -= parcelaVM.ValorReajustado;
                            if (parcelaVM.ValorAberto > 0)
                                parcelaVM.SituacaoParcela = SituacaoParcela.PendenteParcial;
                            else
                            {
                                parcelaVM.SituacaoParcela = SituacaoParcela.Pago;
                                parcelaVM.ValorReajustado = (parcelaVM.ValorParcela + parcelaVM.JurosValor) - parcelaVM.DescontoValor;
                            }


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
                            var fatura = RepositorioFatura.ObterPorId(faturaId);
                            if (fatura != null)
                            {
                                if (fatura.SituacaoFatura == SituacaoFatura.Aberta)
                                {
                                    fatura.DataFechamento = DateTime.Now;
                                    fatura.Nome = "Fatura foi fechada na hora do pagamento!";
                                }
                                fatura.SituacaoFatura = SituacaoFatura.Paga;
                                RepositorioFatura.AlterarLote(fatura);
                            }
                            parcelaFatura.Fatura = fatura;
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
                if (sit == SituacaoParcela.Pago || sit == SituacaoParcela.Cancelado)
                    return false;
            }
            return true;
        }

        public IActionResult ExcluirParcelas(List<Int64> ids)
        {
            if (!SituacaoDasParcelas(ids))
                return Json(new { result = false, error = "Parcelas Pagas e Canceladas não podem ser selecionadas!" });

            foreach (var id in ids)
            {
                var parcela = Repositorio.ObterPorId(id);
                if (parcela != null)
                    Repositorio.Excluir(parcela);
            }

            return new EmptyResult();
        }

        public IActionResult CancelarParcelas(List<Int64> ids)
        {
            if (!SituacaoDasParcelas(ids))
                return Json(new { result = false, error = "Parcelas Pagas e Canceladas não podem ser selecionadas!" });

            foreach (var id in ids)
            {
                var parcela = Repositorio.ObterPorId(id);
                if (parcela != null)
                {
                    parcela.SituacaoParcela = SituacaoParcela.Cancelado;
                    Repositorio.Alterar(parcela);
                }
            }
            return new EmptyResult();

        }

        public IActionResult HistoricoParcela(Int64 Id = 0)
        {
            var historicos = new List<FluxoCaixa>(); 
            var historicosVM = new List<FluxoCaixaVM>();
            parcela = Repositorio.ObterPorId(Id);

            if (Id > 0)
            {
                historicos = RepositorioFluxoCaixa.ObterPorParametros(x => x.Parcela.Id == Id).ToList();
                if (historicos.Count > 0)
                {
                    historicosVM = Mapper.Map<List<FluxoCaixaVM>>(historicos);
                    return PartialView("_HistoricoParcela", historicosVM);
                }
            }

            historicosVM.Add(new FluxoCaixaVM()
            {
                Data = null,
                Valor = 0,
                FormaPagamentoVM = new FormaPagamentoVM() { Nome = "" },
                ParcelaVM= new ParcelaVM() { Numero = parcela.Numero}
            });
            return PartialView("_HistoricoParcela", historicosVM);
        }
    }
}
