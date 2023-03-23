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

        public void RemoverOuAdicionar(Parcela parcela, Usuario usuario, Banco banco, decimal valorPagoParcela, Caixa caixa)
        {
            var saldoContaBancaria = ObterSaldoContaBancaria(banco.Id);

            var novoRegistro = new ContaBancaria();

            if (parcela.Conta != null)
            {
                if (parcela.Conta.TipoConta == Dominio.ObjetoValor.TipoConta.Pagar)
                {
                    if (saldoContaBancaria < valorPagoParcela)
                        throw new Exception("Saldo na conta bancária insulficiente para transação!");

                    novoRegistro.Nome = $"Saída para pagamento da conta: {parcela.Conta.DescricaoCompleta} - parcela N.º: [{parcela.Numero}]";
                    novoRegistro.Situacao = Dominio.ObjetoValor.EntradaSaida.Saída;
                }
                else
                {
                    novoRegistro.Nome = $"Entrada para pagamento da conta {parcela.Conta.DescricaoCompleta} - parcela N.º: [{parcela.Numero}]";
                    novoRegistro.Situacao = Dominio.ObjetoValor.EntradaSaida.Entrada;
                }
            }
            else
            {
                novoRegistro.Nome = $"Saída para pagamento da fatura {parcela.Fatura.DescricaoCompleta}";
                novoRegistro.Situacao = Dominio.ObjetoValor.EntradaSaida.Saída;
            }

            novoRegistro.UsuarioCriacao = usuario;
            novoRegistro.Valor = valorPagoParcela;
            novoRegistro.DataRegistro = DateTime.Now;
            novoRegistro.TransacaoBancaria = Dominio.ObjetoValor.TransacaoBancaria.Outros;
            novoRegistro.Banco = banco;
            novoRegistro.Caixa = caixa;

            SalvarLote(novoRegistro);
        }

        public void SalvarAddFluxoCaixa(ContaBancaria contaBancaria, Usuario usuario)
        {
            using var trans = Session.BeginTransaction();
            try
            {
                var banco = RepositorioBanco.ObterPorId(contaBancaria.Banco.Id);
                var caixa = RepositorioCaixa.ObterPorId(contaBancaria.Caixa.Id);
                SalvarLote(contaBancaria);
                var fluxoCaixa = new FluxoCaixa();
                if(contaBancaria.Situacao == EntradaSaida.Entrada)
                {
                    fluxoCaixa.DebitoCredito = DebitoCredito.Débito;
                    fluxoCaixa.Nome = $"Transferência do caixa para {banco._DadosCompletos}";
                }
                else
                {
                    //VERIFICAR SALDO CONTA AQUI - TRATAR CONTABANCARIA INICIO E FIM DE MES
                    fluxoCaixa.DebitoCredito = DebitoCredito.Crédito;
                    fluxoCaixa.Nome = $"Transferência do {banco._DadosCompletos} para o caixa";
                }

                fluxoCaixa.Valor = contaBancaria.Valor;
                fluxoCaixa.Data = DateTime.Now;
                fluxoCaixa.FormaPagamento = null;
                fluxoCaixa.Parcela = null;
                fluxoCaixa.Caixa = caixa;
                fluxoCaixa.DataGeracao = DateTime.Now;
                fluxoCaixa.UsuarioCriacao = usuario;
               
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
