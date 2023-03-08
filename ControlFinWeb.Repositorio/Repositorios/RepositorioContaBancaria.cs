using ControlFinWeb.Dominio.Entidades;
using ControlFinWeb.Dominio.ObjetoValor;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ControlFinWeb.Repositorio.Repositorios
{
    public class RepositorioContaBancaria : RepositorioBase<ContaBancaria>
    {
        private readonly RepositorioFluxoCaixa RepositorioFluxoCaixa;
        private readonly RepositorioCaixa RepositorioCaixa;
        private readonly RepositorioBanco RepositorioBanco;
        public RepositorioContaBancaria(RepositorioFluxoCaixa repositorioFluxoCaixa,
           RepositorioCaixa repositorioCaixa, RepositorioBanco repositorioBanco,
        ISession session) : base(session) 
        {
            RepositorioFluxoCaixa = repositorioFluxoCaixa;
            RepositorioCaixa = repositorioCaixa;
            RepositorioBanco = repositorioBanco;
        }

        public Decimal ObterSaldoContaBancaria(Int64 idBanco)
        {
            var valorSaida = ObterPorParametros(x => x.Banco.Id == idBanco && x.Situacao == Dominio.ObjetoValor.EntradaSaida.Saída).ToList().Sum(y => y.Valor);
            var valorEntrada = ObterPorParametros(x => x.Banco.Id == idBanco && x.Situacao == Dominio.ObjetoValor.EntradaSaida.Entrada).ToList().Sum(y => y.Valor);
            return valorEntrada - valorSaida;
        }

        public void RemoverOuAdicionar(Parcela parcela, Usuario usuario, Banco banco, decimal valorPagoParcela)
        {
            var saldoContaBancaria = ObterSaldoContaBancaria(banco.Id);

            if (saldoContaBancaria < valorPagoParcela)
                throw new Exception("Saldo na conta bancária insulficiente para transação!");

            var novoContaBancaria = new ContaBancaria();

            if (parcela.Conta != null)
            {
                if (parcela.Conta.TipoConta == Dominio.ObjetoValor.TipoConta.Pagar)
                {
                    novoContaBancaria.Nome = $"Saída para pagamento da conta: {parcela.Conta.DescricaoCompleta} - parcela N.º: [{parcela.Numero}]";
                    novoContaBancaria.Situacao = Dominio.ObjetoValor.EntradaSaida.Saída;
                }
                else
                {
                    novoContaBancaria.Nome = $"Entrada para pagamento da conta {parcela.Conta.DescricaoCompleta} - parcela N.º: [{parcela.Numero}]";
                    novoContaBancaria.Situacao = Dominio.ObjetoValor.EntradaSaida.Entrada;
                }
            }
            else
            {
                novoContaBancaria.Nome = $"Saída para pagamento da fatura {parcela.Fatura.DescricaoCompleta}";
                novoContaBancaria.Situacao = Dominio.ObjetoValor.EntradaSaida.Saída;
            }

            novoContaBancaria.UsuarioCriacao = usuario;
            novoContaBancaria.Valor = valorPagoParcela;
            novoContaBancaria.DataRegistro = DateTime.Now;
            novoContaBancaria.TransacaoBancaria = Dominio.ObjetoValor.TransacaoBancaria.Outros;
            novoContaBancaria.Banco = banco;

            SalvarLote(novoContaBancaria);
        }

        public void SalvarAddFluxoCaixa(ContaBancaria contaBancaria, Usuario usuario)
        {
            using var trans = Session.BeginTransaction();
            try
            {
                var banco = RepositorioBanco.ObterPorId(contaBancaria.Banco.Id);
                SalvarLote(contaBancaria);
                var fluxoCaixa = new FluxoCaixa()
                {
                    Valor = contaBancaria.Valor,
                    Data = DateTime.Now,
                    DebitoCredito = DebitoCredito.Débito,
                    FormaPagamento = null,
                    Parcela = null,
                    Caixa = RepositorioCaixa.ObterPorId(contaBancaria.Caixa.Id),
                    DataGeracao = DateTime.Now,
                    UsuarioCriacao = usuario,
                    Nome = $"Transferência para {banco.DadosCompletos}",
                };
                RepositorioFluxoCaixa.SalvarLote(fluxoCaixa);
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
