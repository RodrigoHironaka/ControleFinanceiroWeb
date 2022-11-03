using ControlFinWeb.Dominio.Entidades;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ControlFinWeb.Repositorio.Repositorios
{
    public class RepositorioFluxoCaixa : RepositorioBase<FluxoCaixa>
    {
        public RepositorioFluxoCaixa(ISession session) : base(session){}

        public void GerarFluxoCaixa(IList<Parcela> parcelas, Usuario usuario, Caixa caixa)
        {
            if (parcelas.Count > 0)
            {
                foreach (var parcela in parcelas)
                {
                    var novoFluxoCaixa = new FluxoCaixa();

                    if (parcela.Conta != null)
                    {
                        if (parcela.Conta.TipoConta == Dominio.ObjetoValor.TipoConta.Pagar)
                        {
                            novoFluxoCaixa.Nome = $"Pagamento efetuado no valor de {parcela.ValorPago:C2} via tela de conta {parcela.Conta.DescricaoCompleta}";
                            novoFluxoCaixa.DebitoCredito = Dominio.ObjetoValor.DebitoCredito.Débito;
                        }
                        else if (parcela.Conta.TipoConta == Dominio.ObjetoValor.TipoConta.Receber)
                        {
                            novoFluxoCaixa.Nome = $"Recebimento efetuado no valor de {parcela.ValorPago:C2} via tela de conta {parcela.Conta.DescricaoCompleta}";
                            novoFluxoCaixa.DebitoCredito = Dominio.ObjetoValor.DebitoCredito.Crédito;
                        }
                    }
                    else
                    {
                        novoFluxoCaixa.Nome = $"Pagamento efetuado no valor de {parcela.ValorPago:C2} via tela de fatura {parcela.Fatura.DescricaoCompleta}";
                        novoFluxoCaixa.DebitoCredito = Dominio.ObjetoValor.DebitoCredito.Débito;
                    }

                    novoFluxoCaixa.Valor = parcela.ValorPago;
                    novoFluxoCaixa.FormaPagamento = parcela.FormaPagamento;
                    novoFluxoCaixa.Parcela = parcela;
                    novoFluxoCaixa.Caixa = caixa;
                    novoFluxoCaixa.UsuarioCriacao = usuario;
                    novoFluxoCaixa.DataGeracao = DateTime.Now;
                    SalvarLote(novoFluxoCaixa);
                }
            }
        }

        public void GerarFluxoCaixa(Caixa caixa, Usuario usuario)
        {
            var novoFluxoCaixa = new FluxoCaixa();
            novoFluxoCaixa.Nome = $"Valor inicial via abertura de Caixa: {caixa.ValorInicial:C2}";
            novoFluxoCaixa.Valor = caixa.ValorInicial;
            novoFluxoCaixa.UsuarioCriacao = usuario;
            novoFluxoCaixa.DataGeracao = DateTime.Now;
            novoFluxoCaixa.DebitoCredito = Dominio.ObjetoValor.DebitoCredito.Crédito;
            novoFluxoCaixa.FormaPagamento = null;
            novoFluxoCaixa.Parcela = null;
            novoFluxoCaixa.Caixa = caixa;
            SalvarLote(novoFluxoCaixa);
        }

    }
}
