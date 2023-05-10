using App1.Repositorio.Configuracao;
using AutoMapper;
using ControlFinWeb.App.Utilitarios;
using ControlFinWeb.App.ViewModels;
using ControlFinWeb.App.ViewModels.Acesso;
using ControlFinWeb.Dominio.Entidades;
using ControlFinWeb.Dominio.ObjetoValor;
using ControlFinWeb.Repositorio.Repositorios;
using K4os.Hash.xxHash;
using LinqKit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ObjectsComparer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Text;
using Utils.Extensions.Enums;

namespace ControlFinWeb.App.Controllers
{
    public class ParcelasController : Controller
    {
        private readonly RepositorioParcela Repositorio;
        private readonly RepositorioFormaPagamento RepositorioFormaPagamento;
        private readonly RepositorioConta RepositorioConta;
        private readonly RepositorioFatura RepositorioFatura;
        private readonly RepositorioFaturaItens RepositorioFaturaItens;
        private readonly RepositorioBanco RepositorioBanco;
        private readonly RepositorioCaixa RepositorioCaixa;
        private readonly RepositorioFluxoCaixa RepositorioFluxoCaixa;
        private readonly RepositorioSubGasto RepositorioSubGasto;
        private readonly IMapper Mapper;
        private readonly ILogger<ParcelasController> _logger;
        public ParcelasController(RepositorioParcela repositorio, RepositorioFormaPagamento repositorioFormaPagamento, RepositorioConta repositorioConta,
            RepositorioFatura repositorioFatura, RepositorioFaturaItens repositorioFaturaItens, RepositorioBanco repositorioBanco, RepositorioCaixa repositorioCaixa, RepositorioFluxoCaixa repositorioFluxoCaixa,
            RepositorioSubGasto repositorioSubGasto, IMapper mapper, ILogger<ParcelasController> logger)
        {
            Repositorio = repositorio;
            RepositorioFormaPagamento = repositorioFormaPagamento;
            RepositorioConta = repositorioConta;
            RepositorioFatura = repositorioFatura;
            RepositorioFaturaItens = repositorioFaturaItens;
            RepositorioBanco = repositorioBanco;
            RepositorioCaixa = repositorioCaixa;
            RepositorioFluxoCaixa = repositorioFluxoCaixa;
            RepositorioSubGasto = repositorioSubGasto;
            Mapper = mapper;
            _logger = logger;
        }

        Parcela parcela = new Parcela();
        ParcelaVM parcelaVM = new ParcelaVM();
        IList<Parcela> parcelas = new List<Parcela>();
        IList<ParcelaVM> parcelasVM = new List<ParcelaVM>();
        GerarParcelasVM gerarParcelasVM = new GerarParcelasVM();
        PagarParcelaVM pagarParcelaVM = new PagarParcelaVM();
        GerarRegistroFaturaVM gerarRegistroFaturaVM = new GerarRegistroFaturaVM();
        Parcela cloneParcela;
        public IList<Parcela> FiltroParcelas(FiltroParcelasVM filtrarParcelasVM)
        {
            var predicado = Repositorio.CriarPredicado();
            predicado = predicado.And(x => x.UsuarioCriacao.Id == Configuracao.Usuario.Id);
            if(filtrarParcelasVM.TipoConta == TipoConta.Pagar)
                predicado = predicado.And(x => x.Conta.TipoConta == filtrarParcelasVM.TipoConta || x.Fatura != null);
            else
                predicado = predicado.And(x => x.Conta.TipoConta == filtrarParcelasVM.TipoConta);// Não existe opção de receber de uma futura, sempre será "PAGAR"

            if (filtrarParcelasVM.DataInicio != null)
                predicado = predicado.And(x => x.DataVencimento >= filtrarParcelasVM.DataInicio);

            if (filtrarParcelasVM.DataFinal != null)
            {
                if (filtrarParcelasVM.DataFinal >= filtrarParcelasVM.DataInicio)
                    predicado = predicado.And(x => x.DataVencimento <= filtrarParcelasVM.DataFinal.Value.FinalDia());
            }

            if (filtrarParcelasVM.PessoaId > 0)
                predicado = predicado.And(x => x.Conta.Pessoa.Id == filtrarParcelasVM.PessoaId || x.Fatura.Pessoa.Id == filtrarParcelasVM.PessoaId);

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
                else if (filtrarParcelasVM.ContaFatura == ContaFatura.Fatura)
                    parcelas = parcelas.Where(x => x.Fatura != null && x.Fatura.SituacaoFatura != SituacaoFatura.Cancelada).ToList();
              
            }
            else
            {
                parcelas = parcelas.Where(x => x.Conta?.Situacao != SituacaoConta.Cancelado || x.Fatura?.SituacaoFatura != SituacaoFatura.Cancelada).ToList();
            }
            return parcelas;
        }

        public IActionResult Index(FiltroParcelasVM filtrarParcelasVM)
         {
            filtrarParcelasVM.Parcelas = Mapper.Map<List<ParcelaVM>>(FiltroParcelas(filtrarParcelasVM));
            return View(filtrarParcelasVM);
        }

        public IActionResult APagar()
        {
            var filtrarParcelasVM = new FiltroParcelasVM()
            {
                TipoConta = TipoConta.Pagar,
            };
            filtrarParcelasVM.Parcelas = Mapper.Map<List<ParcelaVM>>(FiltroParcelas(filtrarParcelasVM));

            return RedirectToAction("Index", filtrarParcelasVM);
        }

        public IActionResult AReceber()
        {
            var filtrarParcelasVM = new FiltroParcelasVM()
            {
                TipoConta = TipoConta.Receber,
            };
            filtrarParcelasVM.Parcelas = Mapper.Map<List<ParcelaVM>>(FiltroParcelas(filtrarParcelasVM));

            return RedirectToAction("Index", filtrarParcelasVM);
        }

        public IActionResult Atrasados()
        {
            var filtrarParcelasVM = new FiltroParcelasVM()
            {
                TipoConta = TipoConta.Pagar,
                SituacaoParcela = "2",
            };
            filtrarParcelasVM.Parcelas = Mapper.Map<List<ParcelaVM>>(FiltroParcelas(filtrarParcelasVM));

            return RedirectToAction("Index", filtrarParcelasVM);
        }

        public IActionResult Editar(Int64 Id = 0)
        {
            if (Id > 0)
            {
                parcela = Repositorio.ObterPorId(Id);
                parcelaVM = Mapper.Map(parcela, parcelaVM);
            }
            return View(parcelaVM);
        }

        [HttpPost]
        public IActionResult Editar(ParcelaVM parcelaVM)
        {
            if (ModelState.IsValid)
            {
                if (parcelaVM.Id > 0)
                {
                    parcela = Repositorio.ObterPorId(parcelaVM.Id);
                    cloneParcela = (Parcela)parcela.Clone();
                    parcela = Mapper.Map(parcelaVM, parcela);
                    parcela.UsuarioAlteracao = Configuracao.Usuario;
                    CompararAlteracoes();
                }
                else
                {
                    parcela = Mapper.Map(parcelaVM, parcela);
                    parcela.UsuarioCriacao = Configuracao.Usuario;
                    Repositorio.Salvar(parcela);
                }
                return new EmptyResult();
            }
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
                    DataVencimento = primeiroVencimento.AddMonths(i - 1),
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
                return Json(new { result = false, error = "Parcelas pagas, recebidas ou canceladas não podem ser selecionadas!" });

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
            return PartialView(pagarParcelaVM);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
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
                                if (parcelaVM.ContaVM != null && parcelaVM.ContaVM.TipoConta == TipoConta.Receber)
                                    parcelaVM.SituacaoParcela = SituacaoParcela.Recebido;
                                else
                                    parcelaVM.SituacaoParcela = SituacaoParcela.Pago;
                                parcelaVM.ValorReajustado = (parcelaVM.ValorParcela + parcelaVM.JurosValor) - parcelaVM.DescontoValor;
                            }


                            parcelasVM.Add(parcelaVM);
                        }
                    }

                    parcelas = Mapper.Map(parcelasVM, parcelas);

                    foreach (var parcelaConta in parcelas.Where(x => x.Conta != null))
                    {
                        parcelaConta.Conta = RepositorioConta.ObterPorId(parcelaConta.Conta.Id);
                    }
                    
                    foreach (var parcelaFatura in parcelas.Where(x => x.Fatura != null))
                    {
                        var fatura = RepositorioFatura.ObterPorId(parcelaFatura.Fatura.Id);
                        if (fatura != null)
                        {
                            if (fatura.SituacaoFatura == SituacaoFatura.Aberta)
                            {
                                fatura.DataFechamento = DateTime.Now;
                                fatura.Nome = "Fatura foi fechada na hora do pagamento!";
                            }
                            if (parcelaFatura.ValorAberto == 0)
                                fatura.SituacaoFatura = SituacaoFatura.Paga;
                            else if (parcelaFatura.ValorPago > 0)
                                fatura.SituacaoFatura = SituacaoFatura.AbertaParcial;
                            RepositorioFatura.AlterarLote(fatura);
                        }
                        parcelaFatura.Fatura = fatura;
                    }

                    var banco = parcelasVM.First().BancoId > 0 ? RepositorioBanco.ObterPorId(parcelasVM.First().BancoId) : null;

                    Repositorio.PagamentoParcela(parcelas, Configuracao.Usuario, banco, valoresPagoParcelas);

                    return new EmptyResult();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro pagar parcelas");
                    return Json(new { result = false, error = ex.Message.ToString() });
                }
            }
            return PartialView(pagarParcelaVM);
        }

        public IActionResult GerarRegistroFatura(Int64 IdParcela)
        {
            if (IdParcela > 0)
            {
                gerarRegistroFaturaVM.ParcelaId = IdParcela;
                gerarRegistroFaturaVM.ParcelaVM = Mapper.Map<ParcelaVM>(Repositorio.ObterPorId(IdParcela));
            }

            var faturas = RepositorioFatura.ObterPorParametros(x => x.SituacaoFatura == SituacaoFatura.Aberta && x.MesAnoReferencia >= DateTime.Now.PrimeiroDiaMes() && x.MesAnoReferencia <= DateTime.Now.AddMonths(1).UltimoDiaMes().FinalDia()).ToList();
            if (faturas != null && faturas.Count == 0)
                return RedirectToAction("Editar", "Faturas");

            ViewBag.FaturaId = new SelectList(faturas, "Id", "_DescricaoCompleta");
            return View(gerarRegistroFaturaVM);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult GerarRegistroFatura(GerarRegistroFaturaVM gerarRegistroFaturaVM)
        {
            var formaPagamento = RepositorioFormaPagamento.ObterPorId(gerarRegistroFaturaVM.FormaPagamentoId);
            var subgasto = RepositorioSubGasto.ObterPorId(gerarRegistroFaturaVM.SubGastoId);
            var fatura = RepositorioFatura.ObterPorId(gerarRegistroFaturaVM.FaturaId);
            var parcela = Repositorio.ObterPorId(gerarRegistroFaturaVM.ParcelaId);
            var caixaAberto = RepositorioCaixa.ObterCaixaAberto(Configuracao.Usuario.Id);
            parcela.FormaPagamento = formaPagamento;

            var faturaItem = new FaturaItens();
            faturaItem.Fatura = fatura;
            faturaItem.Valor = parcela.ValorAberto + gerarRegistroFaturaVM.Juros;
            faturaItem.DataCompra = DateTime.Now;
            faturaItem.DataGeracao = DateTime.Now;
            faturaItem.UsuarioCriacao = Configuracao.Usuario;
            faturaItem.SubGasto = subgasto;
            faturaItem.Pessoa = parcela.Conta != null ? parcela.Conta.Pessoa : parcela.Fatura.Pessoa;
            faturaItem.Nome = parcela.Conta != null ? parcela.Conta.Nome : parcela.Fatura._DescricaoCompleta;

            RepositorioFaturaItens.SalvarAlterarFaturaItemEAlterarParcela(faturaItem, gerarRegistroFaturaVM.Quantidade.ToString(), false, null, Configuracao.Usuario, caixaAberto, parcela, true);

            return RedirectToAction("Index", "Parcelas");

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
                if (sit == SituacaoParcela.Pago || sit == SituacaoParcela.Recebido || sit == SituacaoParcela.Cancelado)
                    return false;
            }
            return true;
        }

        public IActionResult ExcluirParcelas(List<Int64> ids)
        {
            if (!SituacaoDasParcelas(ids))
                return Json(new { result = false, error = "Parcelas pagas, recebidas ou canceladas não podem ser selecionadas!" });

            foreach (var id in ids)
            {
                parcela = Repositorio.ObterPorId(id);
                cloneParcela = (Parcela)parcela.Clone();
                if (parcela != null)
                    Repositorio.ExcluirRegistrarLog(parcela, Configuracao.Usuario);
            }

            return new EmptyResult();
        }

        public IActionResult CancelarParcelas(List<Int64> ids)
        {
            if (!SituacaoDasParcelas(ids))
                return Json(new { result = false, error = "Parcelas pagas, recebidas ou canceladas não podem ser selecionadas!" });

            foreach (var id in ids)
            {
                parcela = Repositorio.ObterPorId(id);
                cloneParcela = (Parcela)parcela.Clone();
                if (parcela != null)
                {
                    parcela.SituacaoParcela = SituacaoParcela.Cancelado;
                    CompararAlteracoes();
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
                ParcelaVM = new ParcelaVM() { Numero = parcela.Numero }
            });
            return PartialView("_HistoricoParcela", historicosVM);
        }

        public IActionResult AlterarParcelasSeguintesConta(ParcelaVM parcelaVM)
        {
            var parcela = Repositorio.ObterPorId(parcelaVM.Id);
            var parcelasAlteradas = new List<Parcela>();
            if(parcela != null)
            {
                try
                {
                    var parcelasConta = Repositorio.ObterPorParametros(x => x.Conta.Id == parcela.Conta.Id && x.Numero > parcela.Numero && x.SituacaoParcela == SituacaoParcela.Pendente).ToList();
                    Int16 i = 1;
                    foreach (var parc in parcelasConta)
                    {
                        parc.ValorParcela = parcela.ValorParcela;
                        parc.ValorAberto = parcela.ValorParcela;
                        parc.ValorReajustado = parcela.ValorParcela;
                        parc.DataVencimento = parcela.DataVencimento.Value.AddMonths(i);
                        parcelasAlteradas.Add(parc);
                        i++;
                    }
                    Repositorio.Alterar(parcelasAlteradas);
                    return Json(new { result = true, message = "Parcelas alteradas com sucesso!" });
                }
                catch (Exception ex)
                {
                    return Json(new { result = false, error = "Erro:" + ex.Message });
                }
               
            }
            else
                return Json(new { result = false, error="Parcela não foi encontrada!" });
        }

        private void CompararAlteracoes()
        {
            var comparer = new ObjectsComparer.Comparer<Parcela>();
            comparer.AddComparerOverride<Usuario>(DoNotCompareValueComparer.Instance);
            comparer.AddComparerOverride<Int64>(DoNotCompareValueComparer.Instance, member => member.Name.Contains("Id"));
            comparer.AddComparerOverride<String>(DoNotCompareValueComparer.Instance, member => member.Name.StartsWith("_"));
            comparer.AddComparerOverride<Conta>(DoNotCompareValueComparer.Instance);
            comparer.AddComparerOverride<Fatura>(DoNotCompareValueComparer.Instance);
            comparer.IgnoreMember("DataGeracao");
            comparer.IgnoreMember("DataAlteracao");
            var igual = comparer.Compare(cloneParcela, parcela, out IEnumerable<Difference> diferencas);
            var contaOuFatura = parcela.Conta != null ? "Conta" : "Fatura";
            var idContaOuFatura = parcela.Conta != null ? parcela.Conta.Id : parcela.Fatura.Id;
            if (!igual)
                Repositorio.EditarRegistrarLog(parcela, diferencas, Configuracao.Usuario, $"{contaOuFatura}[{idContaOuFatura}] - Parcela[{parcela.Numero}]");
        }
    }
}
