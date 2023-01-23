using ControlFinWeb.Dominio.Entidades;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ControlFinWeb.Repositorio.Repositorios
{
    public class RepositorioContaBancaria : RepositorioBase<ContaBancaria>
    {

        public RepositorioContaBancaria(ISession session) : base(session) { }

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
    }
}
