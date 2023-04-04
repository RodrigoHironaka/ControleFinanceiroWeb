using ControlFinWeb.Dominio.Entidades;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ControlFinWeb.Repositorio.Repositorios
{
    public class RepositorioFluxoCaixa : RepositorioBase<FluxoCaixa>
    {
        public RepositorioFluxoCaixa(ISession session) : base(session) { }

        public void GerarFluxoCaixa(Parcela parcela, Usuario usuario, Caixa caixa, decimal valorPagoParcela, Banco banco = null, decimal valorSecundarioParcela = 0)
        {
            if (parcela != null)
            {
                var descricaoParcela = String.Format("Parcela Nº: {0} - {1}", parcela.Numero, parcela.Conta != null ? parcela.Conta._DescricaoCompleta : parcela.Fatura._DescricaoCompleta);
                var novoFluxoCaixa = new FluxoCaixa();
                var valor = valorPagoParcela > 0 ? valorPagoParcela : valorSecundarioParcela;

                if (parcela.Conta != null)
                {
                    if (parcela.Conta.TipoConta == Dominio.ObjetoValor.TipoConta.Pagar)
                    {
                        if(banco != null)
                        {
                            novoFluxoCaixa.Nome = $"Entrada de valor via o banco {banco._DadosCompletos} ref. {descricaoParcela}";
                            novoFluxoCaixa.DebitoCredito = Dominio.ObjetoValor.DebitoCredito.Crédito;
                           
                        }
                        else
                        {
                            novoFluxoCaixa.Nome = $"Pagamento efetuado no valor de {valor:C2} referente a conta: {parcela.Conta._DescricaoCompleta} - parcela N.º: [{parcela.Numero}] {parcela.DataVencimento.Value.ToShortDateString()}";
                            novoFluxoCaixa.DebitoCredito = Dominio.ObjetoValor.DebitoCredito.Débito;
                        }
                       
                    }
                    else if (parcela.Conta.TipoConta == Dominio.ObjetoValor.TipoConta.Receber)
                    {
                        if( banco != null)
                        {
                            novoFluxoCaixa.Nome = $"Transferência de valor para o banco {banco._DadosCompletos} ref. {descricaoParcela}";
                            novoFluxoCaixa.DebitoCredito = Dominio.ObjetoValor.DebitoCredito.Débito;
                        }
                        else
                        {
                            novoFluxoCaixa.Nome = $"Recebimento efetuado no valor de {valor:C2} referente a conta {parcela.Conta._DescricaoCompleta} - parcela N.º: [{parcela.Numero}] {parcela.DataVencimento.Value.ToShortDateString()}";
                            novoFluxoCaixa.DebitoCredito = Dominio.ObjetoValor.DebitoCredito.Crédito;
                        }
                    }
                }
                else
                {
                    if(banco != null)
                    {
                        novoFluxoCaixa.Nome = $"Entrada de valor via banco {banco._DadosCompletos} ref. {descricaoParcela}";
                        novoFluxoCaixa.DebitoCredito = Dominio.ObjetoValor.DebitoCredito.Crédito;
                    }
                    else
                    {
                        novoFluxoCaixa.Nome = $"Pagamento efetuado no valor de {valor:C2} referente a fatura {parcela.Fatura._DescricaoCompleta}";
                        novoFluxoCaixa.DebitoCredito = Dominio.ObjetoValor.DebitoCredito.Débito;
                    }
                }

                novoFluxoCaixa.Valor = valorPagoParcela;
                novoFluxoCaixa.FormaPagamento = parcela.FormaPagamento;
                novoFluxoCaixa.Parcela = parcela;
                novoFluxoCaixa.Caixa = caixa;
                novoFluxoCaixa.UsuarioCriacao = usuario;
                novoFluxoCaixa.Data = DateTime.Now;
                SalvarLote(novoFluxoCaixa);

            }
        }

        public void GerarFluxoCaixa(Caixa caixa, Usuario usuario)
        {
            var novoFluxoCaixa = new FluxoCaixa();
            novoFluxoCaixa.Nome = $"Valor inicial via abertura de Caixa: {caixa.ValorInicial:C2}";
            novoFluxoCaixa.Valor = caixa.ValorInicial;
            novoFluxoCaixa.UsuarioCriacao = usuario;
            novoFluxoCaixa.Data = DateTime.Now;
            novoFluxoCaixa.DebitoCredito = Dominio.ObjetoValor.DebitoCredito.Crédito;
            novoFluxoCaixa.FormaPagamento = null;
            novoFluxoCaixa.Parcela = null;
            novoFluxoCaixa.Caixa = caixa;
            SalvarLote(novoFluxoCaixa);
        }

    }
}
