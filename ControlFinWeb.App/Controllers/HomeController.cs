using ControlFinWeb.App.Models;
using ControlFinWeb.App.Utilitarios;
using ControlFinWeb.App.ViewModels;
using ControlFinWeb.Dominio.Entidades;
using ControlFinWeb.Dominio.ObjetoValor;
using ControlFinWeb.Repositorio.Repositorios;
using LinqKit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Utils.Extensions.Enums;

namespace ControlFinWeb.App.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly RepositorioParcela RepositorioParcela;
        private readonly RepositorioCaixa RepositorioCaixa;
        private readonly RepositorioContaBancaria RepositorioContaBancaria;
        private readonly RepositorioCartao RepositorioCartao;
        public HomeController(RepositorioParcela repositorioParcela,
            RepositorioCaixa repositorioCaixa,
            RepositorioContaBancaria repositorioContaBancaria,
            RepositorioCartao repositorioCartao,
            ILogger<HomeController> logger)
        {
            RepositorioParcela = repositorioParcela;
            RepositorioCaixa = repositorioCaixa;
            RepositorioContaBancaria = repositorioContaBancaria;
            RepositorioCartao = repositorioCartao;
            _logger = logger;
        }

        [Route("Principal")]
        public IActionResult Index()
        {
            ParcelasAtrasadas();

            var predicado = RepositorioParcela.CriarPredicado();
            predicado = predicado.And(x => x.SituacaoParcela != Dominio.ObjetoValor.SituacaoParcela.Pago && x.SituacaoParcela != Dominio.ObjetoValor.SituacaoParcela.Cancelado);
            predicado = predicado.And(x => x.DataVencimento >= DateTime.Now.PrimeiroDiaMes());
            predicado = predicado.And(x => x.DataVencimento <= DateTime.Now.UltimoDiaMes().FinalDia());

            var parcelas = RepositorioParcela.ObterPorParametros(predicado).ToList();
            var caixaAberto = RepositorioCaixa.ObterCaixaAberto(Configuracao.Usuario.Id);

            decimal pagar = parcelas.Where(x => x.Conta != null && x.Conta.TipoConta == Dominio.ObjetoValor.TipoConta.Pagar).Sum(x => x.ValorAberto) + parcelas.Where(x => x.Fatura != null).Sum(x => x.ValorAberto);
            decimal receber = parcelas.Where(x => x.Conta != null && x.Conta.TipoConta == Dominio.ObjetoValor.TipoConta.Receber).Sum(x => x.ValorAberto);
            decimal atrasados = parcelas.Where(x => x.SituacaoParcela == Dominio.ObjetoValor.SituacaoParcela.Atrasado).Sum(x => x.ValorAberto);
            decimal balancoMes = 0;

            if (caixaAberto == null)
                ViewData["Mensagem"] = "Nenhum caixa aberto!";
            else
                balancoMes = caixaAberto._BalancoFinal;

            var homeVM = new HomeVM();
            homeVM.TotalPagar = pagar;
            homeVM.TotalReceber = receber;
            homeVM.TotalBalanco = balancoMes;
            homeVM.TotalAtrasadas = atrasados;

            GraficoGasto12Meses();
            GastoPorGrupo();

            return View(homeVM);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private void ParcelasAtrasadas()
        {
            var predicado = RepositorioParcela.CriarPredicado();
            predicado = predicado.And(x => x.UsuarioCriacao.Id == Configuracao.Usuario.Id);
            predicado = predicado.And(x => x.SituacaoParcela != Dominio.ObjetoValor.SituacaoParcela.Pago && x.SituacaoParcela != Dominio.ObjetoValor.SituacaoParcela.Cancelado && x.SituacaoParcela != Dominio.ObjetoValor.SituacaoParcela.Atrasado);
            predicado = predicado.And(x => x.DataVencimento.Value.Date < DateTime.Now.Date);
            var parcelas = RepositorioParcela.ObterPorParametros(predicado).ToList();

            if (parcelas.Count > 0)
            {
                foreach (var parcela in parcelas)
                    parcela.SituacaoParcela = Dominio.ObjetoValor.SituacaoParcela.Atrasado;
                RepositorioParcela.Alterar(parcelas);
            }
        }

        private void GraficoGasto12Meses()
        {

            var descricao = string.Empty;
            var total = string.Empty;
            var cor = string.Empty;

            for (int i = 12; i >= 0; i--)
            {
                var primeiroDiaMes = DateTime.Now.AddMonths(-i).PrimeiroDiaMes();
                var ultimoDiaMes = DateTime.Now.AddMonths(-i).UltimoDiaMes().FinalDia();
                var auxLista = RepositorioParcela.ObterPorParametros(x =>
                x.DataVencimento >= primeiroDiaMes &&
                x.DataVencimento <= ultimoDiaMes &&
                 x.SituacaoParcela != Dominio.ObjetoValor.SituacaoParcela.Cancelado &&
                x.UsuarioCriacao.Id == Configuracao.Usuario.Id).ToList();

                var random = new Random();
                if (auxLista.Count > 0)
                {
                    var nomeMes = String.Format("{0}/{1}", auxLista.First().DataVencimento.Value.ToString("MMM").ToUpper(), auxLista.First().DataVencimento.Value.ToString("yyyy"));
                    total += auxLista.Sum(x => x.ValorReajustado).ToString("N2").Replace(".", "").Replace(",", ".") + ",";
                    descricao += "'" + nomeMes + "',";
                    cor += "'" + String.Format("#{0:X6}", random.Next(0x1000000)) + "',";
                }

            }
            ViewBag.Descricoes = descricao;
            ViewBag.Totais = total;
            ViewBag.Cores = cor;
        }

        private void GastoPorGrupo()
        {
            var descricoes = string.Empty;
            var totais = string.Empty;
            var cores = string.Empty;
            var random = new Random();

            var auxListaConta = RepositorioParcela.ObterPorParametros(x =>
            x.DataVencimento >= DateTime.Now.PrimeiroDiaMes() &&
            x.DataVencimento <= DateTime.Now.UltimoDiaMes().FinalDia() &&
             x.SituacaoParcela != Dominio.ObjetoValor.SituacaoParcela.Cancelado &&
            x.UsuarioCriacao.Id == Configuracao.Usuario.Id &&
            x.Conta != null &&
            x.Conta.TipoConta == Dominio.ObjetoValor.TipoConta.Pagar).ToList();

            var auxListaFaturas = RepositorioParcela.ObterPorParametros(x =>
           x.DataVencimento >= DateTime.Now.PrimeiroDiaMes() &&
           x.DataVencimento <= DateTime.Now.UltimoDiaMes().FinalDia() &&
            x.SituacaoParcela != Dominio.ObjetoValor.SituacaoParcela.Cancelado &&
           x.UsuarioCriacao.Id == Configuracao.Usuario.Id &&
           x.Fatura != null).ToList();


            var contas = auxListaConta.GroupBy(x => x.Conta.SubGasto).Select(gp => new
            {
                SubGasto = gp.Key,
                Total = gp.Sum(x => x.ValorReajustado)
            }).ToList();

            var itensFaturas = new List<HomeVM>();
            foreach (var fat in auxListaFaturas)
            {
                foreach (var item in fat.Fatura.FaturaItens)
                {
                    itensFaturas.Add(new HomeVM
                    {
                        Descricao = item.SubGasto._DescricaoCompleta,
                        Valor = item.Valor
                    });
                }
            }

            var faturas = itensFaturas.GroupBy(x => x.Descricao).Select(gp => new
            {
                Descricao = gp.Key,
                Total = gp.Sum(x => x.Valor)
            }).ToList();

            var listateste = new List<HomeVM>();
            if (contas.Count > 0)
            {
                for (int i = 0; i < contas.Count; i++)
                {
                    listateste.Add(new HomeVM()
                    {
                        Valor = contas[i].Total,
                        Descricao = contas[i].SubGasto._DescricaoCompleta
                    });
                }
            }

            if (faturas.Count > 0)
            {
                for (int i = 0; i < faturas.Count; i++)
                {
                    listateste.Add(new HomeVM()
                    {
                        Valor = faturas[i].Total,
                        Descricao = faturas[i].Descricao
                    });

                }
            }
            var listaTeste2 = listateste.GroupBy(x => x.Descricao).Select(gp => new
            {
                Descricao = gp.Key,
                Total = gp.Sum(x => x.Valor)
            }).ToList();

            foreach (var item in listaTeste2)
            {
                totais += item.Total.ToString("N2").Replace(".", "").Replace(",", ".") + ",";
                descricoes += "'" + item.Descricao + "',";
                cores += "'" + String.Format("#{0:X6}", random.Next(0x1000000)) + "',";
            }

            ViewBag.Descricoes1 = descricoes;
            ViewBag.Totais1 = totais;
            ViewBag.Cores1 = cores;
            ViewBag.Mes1 = $"({DateTime.Now.ToString("MMM")})";
        }

        public String TodosAvisos()
        {
            var aviso = new StringBuilder();

            var avisoCaixa = AvisoGastos();
            if (!String.IsNullOrEmpty(avisoCaixa))
                aviso.AppendLine(avisoCaixa);

            var avisoCartao = AvisoCartao();
            if (!String.IsNullOrEmpty(avisoCartao))
                aviso.AppendLine(avisoCartao);

           

            return aviso.ToString();

        }

        private String AvisoCartao()
        {
            var avisoCartao = new StringBuilder();

            var predicadoCartao = RepositorioCartao.CriarPredicado();
            predicadoCartao = predicadoCartao.And(x => x.UsuarioCriacao.Id == Configuracao.Usuario.Id);
            predicadoCartao = predicadoCartao.And(x => x.Situacao == Situacao.Ativo);
            predicadoCartao = predicadoCartao.And(x => x.VencimentoCartao.Value >= DateTime.Now.AddMonths(-2));
            predicadoCartao = predicadoCartao.And(x => x.VencimentoCartao.Value <= DateTime.Now.FinalDia());
            var cartoesProximosVencimento = RepositorioCartao.ObterPorParametros(predicadoCartao);
            cartoesProximosVencimento.ForEach(x => avisoCartao.AppendLine(String.Format("<p>Cartão {0} próximo do vencimento {1:MM/yyyy}</p>", x.Nome, x.VencimentoCartao.Value)));

            var predicadoCartaoAposVencimento = RepositorioCartao.CriarPredicado();
            predicadoCartaoAposVencimento = predicadoCartaoAposVencimento.And(x => x.UsuarioCriacao.Id == Configuracao.Usuario.Id);
            predicadoCartaoAposVencimento = predicadoCartaoAposVencimento.And(x => x.Situacao == Situacao.Ativo);
            predicadoCartaoAposVencimento = predicadoCartaoAposVencimento.And(x => x.VencimentoCartao.Value < DateTime.Now.FinalDia());
            var cartoesVencidos = RepositorioCartao.ObterPorParametros(predicadoCartaoAposVencimento);
            cartoesVencidos.ForEach(x => avisoCartao.AppendLine(String.Format("<p>Cartão {0}  vencido em {1:MM/yyyy}</p>", x.Nome, x.VencimentoCartao.Value)));


            return avisoCartao.ToString();
        }

        private String AvisoGastos()
        {
            var avisoGasto = new StringBuilder();
            Decimal totalMesAtual = 0, totalMesAnterior = 0;

            var predicadoMesAtual = RepositorioParcela.CriarPredicado();
            predicadoMesAtual = predicadoMesAtual.And(x => x.UsuarioCriacao.Id == Configuracao.Usuario.Id);
            predicadoMesAtual = predicadoMesAtual.And(x => x.DataVencimento >= DateTime.Now.PrimeiroDiaMes());
            predicadoMesAtual = predicadoMesAtual.And(x => x.DataVencimento <= DateTime.Now.UltimoDiaMes().FinalDia());
            var MesAtual = RepositorioParcela.ObterPorParametros(predicadoMesAtual).ToList();
            totalMesAtual = MesAtual.Sum(x => x.ValorAberto);

            var predicadoMesAterior = RepositorioParcela.CriarPredicado();
            predicadoMesAterior = predicadoMesAterior.And(x => x.UsuarioCriacao.Id == Configuracao.Usuario.Id);
            predicadoMesAterior = predicadoMesAterior.And(x => x.DataVencimento >= DateTime.Now.AddMonths(-1).PrimeiroDiaMes());
            predicadoMesAterior = predicadoMesAterior.And(x => x.DataVencimento <= DateTime.Now.AddMonths(-1).UltimoDiaMes().FinalDia());
            var MesAnterior = RepositorioParcela.ObterPorParametros(predicadoMesAterior).ToList();
            totalMesAnterior = MesAnterior.Sum(x => x.ValorPago);

            var resultado = (totalMesAtual * 100) / totalMesAnterior;

            if(resultado != 0)
                avisoGasto.Append(String.Format("Gasto entre o mês atual e o anterior: {0:N2}%", resultado));

            return avisoGasto.ToString();
        }
    }
}
