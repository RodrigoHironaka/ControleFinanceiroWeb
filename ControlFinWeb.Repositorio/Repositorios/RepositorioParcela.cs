using App1.Repositorio.Configuracao;
using ControlFinWeb.Dominio.Entidades;
using ControlFinWeb.Dominio.ObjetoValor;
using NHibernate;
using System;

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
    }
}
