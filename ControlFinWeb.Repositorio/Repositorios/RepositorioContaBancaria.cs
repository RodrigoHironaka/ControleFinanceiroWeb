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

        public void RemoverOuAdicionar(IList<Parcela> parcelas, Usuario usuario, Banco banco)
        {
            var saldoContaBancaria = ObterSaldoContaBancaria(banco.Id);
            var contasBancarias = new List<ContaBancaria>();

            if (saldoContaBancaria < parcelas.Sum(x => x.ValorPago))
                throw new Exception("Saldo na conta bancária insulficiente para transação!");

            foreach (var parcela in parcelas)
            {
                var novoContaBancaria = new ContaBancaria();

                if(parcela.Conta != null)
                {
                    if(parcela.Conta.TipoConta == Dominio.ObjetoValor.TipoConta.Pagar)
                    {
                        novoContaBancaria.Nome = $"Saída para pagamento da conta {parcela.Conta.DescricaoCompleta}";
                        novoContaBancaria.Situacao = Dominio.ObjetoValor.EntradaSaida.Saída;
                    }
                    else
                    {
                        novoContaBancaria.Nome = $"Entrada para pagamento da conta {parcela.Conta.DescricaoCompleta}";
                        novoContaBancaria.Situacao = Dominio.ObjetoValor.EntradaSaida.Entrada;
                    }
                }
                else
                {
                    novoContaBancaria.Nome = $"Saída para pagamento da fatura {parcela.Fatura.DescricaoCompleta}";
                    novoContaBancaria.Situacao = Dominio.ObjetoValor.EntradaSaida.Saída;
                }

                novoContaBancaria.UsuarioCriacao = usuario;
                novoContaBancaria.Codigo = RetornaUltimoCodigo() + 1;
                novoContaBancaria.Valor = parcela.ValorPago;
                novoContaBancaria.DataRegistro = DateTime.Now;
                novoContaBancaria.TransacaoBancaria = Dominio.ObjetoValor.TransacaoBancaria.Outros;
                novoContaBancaria.Banco = banco;
                
                contasBancarias.Add(novoContaBancaria);
            }
            SalvarLote(contasBancarias);
        }
    }
}
