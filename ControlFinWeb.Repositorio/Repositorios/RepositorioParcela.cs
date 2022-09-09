using App1.Repositorio.Configuracao;
using ControlFinWeb.Dominio.Entidades;
using ControlFinWeb.Dominio.ObjetoValor;
using NHibernate;
using System;
using System.Linq;

namespace ControlFinWeb.Repositorio.Repositorios
{
    public class RepositorioParcela : RepositorioBase<Parcela>
    {
        public RepositorioParcela(ISession session) : base(session) { }

        public void AdicionarNovaParcela(Decimal valor, DateTime? dataVencimento, Usuario usuario, Conta conta = null, FaturaCartaoCredito faturaCartaoCredito = null)
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
                FaturaCartaoCredito = faturaCartaoCredito,
                SituacaoParcela = SituacaoParcela.Pendente,
                DataGeracao = DateTime.Now,
                UsuarioCriacao = usuario,
            };
            SalvarLote(parcelaNova);
        }

        public void AlterarParcelaFatura(FaturaCartaoCreditoItens faturaCartaoCreditoItens)
        {
            var existeParcela = ObterPorParametros(x => x.FaturaCartaoCredito.Id.Equals(faturaCartaoCreditoItens.CartaoCredito.Id)).FirstOrDefault();

            if (existeParcela != null)
            {
                if (faturaCartaoCreditoItens.Id > 0)
                    existeParcela.ValorParcela = faturaCartaoCreditoItens.CartaoCredito.ValorFatura;
                else
                    existeParcela.ValorParcela = faturaCartaoCreditoItens.CartaoCredito.ValorFatura + faturaCartaoCreditoItens.Valor;

                AlterarLote(existeParcela);
            }
        }

        public void ExcluirOuCancelarParcelaFatura(FaturaCartaoCredito faturaCartaoCredito, bool cancelar = false)
        {
            var existeParcela = ObterPorParametros(x => x.FaturaCartaoCredito.Id.Equals(faturaCartaoCredito.Id)).FirstOrDefault();

            if (existeParcela != null)
            {
                if (cancelar)
                {
                    existeParcela.SituacaoParcela = SituacaoParcela.Cancelado;
                    AlterarLote(existeParcela);
                }
                else
                    ExcluirLote(existeParcela);
            }
        }
    }
}
