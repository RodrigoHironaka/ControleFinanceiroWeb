using App1.Repositorio.Configuracao;
using ControlFinWeb.Dominio.Entidades;
using ControlFinWeb.Dominio.ObjetoValor;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ControlFinWeb.Repositorio.Repositorios
{
    public class RepositorioParcela : RepositorioBase<Parcela>
    {
        private readonly RepositorioFluxoCaixa RepositorioFluxoCaixa;
        private readonly RepositorioCaixa RepositorioCaixa;
        public RepositorioParcela(ISession session, RepositorioFluxoCaixa repositorioFluxoCaixa, RepositorioCaixa repositorioCaixa) : base(session)
        {
            RepositorioFluxoCaixa = repositorioFluxoCaixa;
            RepositorioCaixa = repositorioCaixa;
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

        public void AlterarParcelaFatura(FaturaItens faturaItens)
        {
            var existeParcela = ObterPorParametros(x => x.Fatura.Id.Equals(faturaItens.CartaoCredito.Id)).FirstOrDefault();

            if (existeParcela != null)
            {
                if (faturaItens.Id > 0)
                    existeParcela.ValorParcela = faturaItens.CartaoCredito.ValorFatura;
                else
                    existeParcela.ValorParcela = faturaItens.CartaoCredito.ValorFatura + faturaItens.Valor;

                AlterarLote(existeParcela);
            }
        }

        public void PagamentoParcela(IList<Parcela> parcelas, Usuario usuario)
        {
            if(parcelas == null)
                throw new ArgumentException("Nenhuma parcela para pagar!");

            using (var trans = Session.BeginTransaction())
            {
                try
                {
                    AlterarLote(parcelas);
                    var caixa = RepositorioCaixa.ObterCaixaAberto(usuario.Id);
                    if (caixa == null)
                        throw new ArgumentException("Nenhum caixa aberto!");

                    RepositorioFluxoCaixa.GerarFluxoCaixa(parcelas, usuario, caixa);
                    //log aqui

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new Exception(ex.ToString());
                }
            }
            

        }
    }
}
