﻿using App1.Repositorio.Configuracao;
using ControlFinWeb.Dominio.Entidades;
using ControlFinWeb.Dominio.ObjetoValor;
using NHibernate;
using ObjectsComparer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ControlFinWeb.Repositorio.Repositorios
{
    public class RepositorioParcela : RepositorioBase<Parcela>
    {
        private readonly RepositorioFluxoCaixa RepositorioFluxoCaixa;
        private readonly RepositorioCaixa RepositorioCaixa;
        private readonly RepositorioContaBancaria RepositorioContaBancaria;
        private readonly RepositorioLogModificacao RepositorioLog;
        public RepositorioParcela(ISession session, RepositorioFluxoCaixa repositorioFluxoCaixa,
           RepositorioCaixa repositorioCaixa,
           RepositorioContaBancaria repositorioContaBancaria,
           RepositorioLogModificacao repositorioLog) : base(session)
        {
            RepositorioFluxoCaixa = repositorioFluxoCaixa;
            RepositorioCaixa = repositorioCaixa;
            RepositorioContaBancaria = repositorioContaBancaria;
            RepositorioLog = repositorioLog;
        }

        public void AdicionarNovaParcela(Decimal valor, DateTime? dataVencimento, Usuario usuario, Conta conta = null, Fatura fatura = null)
        {
            var parcelaNova = new Parcela()
            {
                Numero = 1,
                ParcelaDe = "1/1",
                ValorParcela = valor,
                ValorReajustado = valor,
                ValorAberto = valor,
                DataVencimento = dataVencimento,
                Conta = conta,
                Fatura = fatura,
                SituacaoParcela = SituacaoParcela.Pendente,
                DataGeracao = DateTime.Now,
                UsuarioCriacao = usuario,
            };
            SalvarLote(parcelaNova);
        }

        public void AlterarParcelaFatura(FaturaItens faturaItens, Boolean RemovendoItemFatura = false, Boolean AlterandoItemFatura = false)
        {
            var existeParcela = ObterPorParametros(x => x.Fatura.Id.Equals(faturaItens.Fatura.Id)).FirstOrDefault();

            if (RemovendoItemFatura)
            {
                existeParcela.ValorParcela -= faturaItens.Valor;
                existeParcela.ValorReajustado -= faturaItens.Valor;
                existeParcela.ValorAberto -= faturaItens.Valor;
            }
            else if (AlterandoItemFatura)
            {
                existeParcela.ValorParcela = faturaItens.Fatura._ValorFatura;
                existeParcela.ValorReajustado = existeParcela.CalculaValorReajustado();
                existeParcela.ValorAberto = existeParcela.CalculaValorAberto();
            }
            else
            {
                existeParcela.ValorParcela = faturaItens.Fatura._ValorFatura + faturaItens.Valor;
                existeParcela.ValorReajustado = existeParcela.CalculaValorReajustado();
                existeParcela.ValorAberto += faturaItens.Valor;
            }

            AlterarLote(existeParcela);
        }

        public void PagamentoParcela(IList<Parcela> parcelas, Usuario usuario, Banco banco, IDictionary<Int64, decimal> valoresPagoParcelas)
        {
            if (parcelas == null)
                throw new ArgumentException("Nenhuma parcela para pagar!");

            using (var trans = Session.BeginTransaction())
            {
                try
                {
                    AlterarLote(parcelas);
                    var caixa = RepositorioCaixa.ObterCaixaAberto(usuario.Id);
                    if (caixa == null)
                        throw new ArgumentException("Nenhum caixa aberto!");

                    foreach (var parcela in parcelas)
                    {
                        var valorPago = valoresPagoParcelas[parcela.Id];
                        if (valorPago > 0)
                        {
                            RepositorioFluxoCaixa.GerarFluxoCaixa(parcela, usuario, caixa, valorPago, null);

                            if (banco != null)
                            {
                                RepositorioContaBancaria.RemoverOuAdicionar(parcela, usuario, banco, valorPago, caixa);
                                RepositorioFluxoCaixa.GerarFluxoCaixa(parcela, usuario, caixa, valorPago, banco);
                            }
                        }
                    }

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new Exception(ex.Message.ToString());
                }
            }
        }

        public void EditarRegistrarLog(Parcela parcela, IEnumerable<Difference> diferencas, Usuario usuario, String chave)
        {
            using (var trans = Session.BeginTransaction())
            {
                try
                {
                    RepositorioLog.RegistrarLogModificacao(diferencas, usuario, chave);
                    AlterarLote(parcela);
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new Exception(ex.Message);
                }
            }
        }

        public void ExcluirRegistrarLog(Parcela parcela, Usuario usuario)
        {
            using (var trans = Session.BeginTransaction())
            {
                try
                {
                    var contaOuFatura = parcela.Conta != null ? "Conta" : "Fatura";
                    var idContaOuFatura = parcela.Conta != null ? parcela.Conta.Id : parcela.Fatura.Id;
                    RepositorioLog.RegistrarLog($"Parcela Nº {parcela.Numero} foi excluída!", usuario, $"{contaOuFatura}[{idContaOuFatura}]");
                    ExcluirLote(parcela);
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new Exception(ex.Message);
                }
            }


        }
       
    }
}
