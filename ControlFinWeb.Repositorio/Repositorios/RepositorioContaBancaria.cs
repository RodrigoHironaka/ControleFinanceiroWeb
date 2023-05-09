using ControlFinWeb.Dominio.Entidades;
using ControlFinWeb.Dominio.ObjetoValor;
using LinqKit;
using NHibernate;
using ObjectsComparer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Utils.Extensions.Enums;

namespace ControlFinWeb.Repositorio.Repositorios
{
    public class RepositorioContaBancaria : RepositorioBase<ContaBancaria>
    {
        private readonly RepositorioFluxoCaixa RepositorioFluxoCaixa;
        private readonly RepositorioCaixa RepositorioCaixa;
        private readonly RepositorioBanco RepositorioBanco;
        private readonly RepositorioLogModificacao RepositorioLog;
        public RepositorioContaBancaria(RepositorioFluxoCaixa repositorioFluxoCaixa,
           RepositorioCaixa repositorioCaixa, RepositorioBanco repositorioBanco,
           RepositorioLogModificacao repositorioLog,
        ISession session) : base(session)
        {
            RepositorioFluxoCaixa = repositorioFluxoCaixa;
            RepositorioCaixa = repositorioCaixa;
            RepositorioBanco = repositorioBanco;
            RepositorioLog = repositorioLog;
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

                    novoRegistro.Nome = $"Saída para pagamento da conta: {parcela.Conta._DescricaoCompleta} - parcela N.º: [{parcela.Numero}]";
                    novoRegistro.Situacao = Dominio.ObjetoValor.EntradaSaida.Saída;
                }
                else
                {
                    novoRegistro.Nome = $"Entrada para pagamento da conta {parcela.Conta._DescricaoCompleta} - parcela N.º: [{parcela.Numero}]";
                    novoRegistro.Situacao = Dominio.ObjetoValor.EntradaSaida.Entrada;
                }
            }
            else
            {
                novoRegistro.Nome = $"Saída para pagamento da fatura {parcela.Fatura._DescricaoCompleta}";
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
                if (contaBancaria.Situacao == EntradaSaida.Entrada)
                {
                    fluxoCaixa.DebitoCredito = DebitoCredito.Débito;
                    fluxoCaixa.Nome = $"Transferência do caixa para {banco._DadosCompletos}";
                }
                else
                {
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

        public void EditarRegistrarLog(ContaBancaria contaBancaria, IEnumerable<Difference> diferencas, Usuario usuario, String chave)
        {
            using (var trans = Session.BeginTransaction())
            {
                try
                {
                    RepositorioLog.RegistrarLogModificacao(diferencas, usuario, chave);
                    AlterarLote(contaBancaria);
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new Exception(ex.Message);
                }
            }
        }

        public void ExcluirRegistrarLog(ContaBancaria contaBancaria, Usuario usuario)
        {
            using (var trans = Session.BeginTransaction())
            {
                try
                {
                    RepositorioLog.RegistrarLog($"Registro excluído!", usuario, $"ContaBancaria[{contaBancaria.Id}]");
                    ExcluirLote(contaBancaria);
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new Exception(ex.Message);
                }
            }

        }

        public Decimal Saldo(DateTime data, Int64 bancoId)
        {
            decimal saldo = 0;
            var predicado = CriarPredicado();
            predicado = predicado.And(x => x.Banco.Id == bancoId);
            predicado = predicado.And(x => x.DataRegistro >= data.PrimeiroDiaMes());
            predicado = predicado.And(x => x.DataRegistro <= data.UltimoDiaMes().FinalDia());

            var registros = ObterPorParametros(predicado).ToList();
            if (registros.Count > 0)
            {
                var saldoEntrada = registros.Where(x => x.Situacao == EntradaSaida.Entrada).Sum(x => x.Valor);
                var saldoSaida = registros.Where(x => x.Situacao == EntradaSaida.Saída).Sum(x => x.Valor);
                saldo = saldoEntrada - saldoSaida;
            }
            return saldo;
        }

        public void TransferenciaentreContas(Int64 bancoSaidaId, Int64 bancoEntradaId, Decimal valor, TransacaoBancaria transBancaria, Usuario usuario)
        {
            using (var trans = Session.BeginTransaction())
            {
                try
                {
                    var bancoSaida = RepositorioBanco.ObterPorId(bancoSaidaId);
                    if (bancoSaida == null)
                        throw new Exception("Houve um erro: Nenhuma conta saída foi definida!");

                    var bancoEntrada = RepositorioBanco.ObterPorId(bancoEntradaId);
                    if (bancoEntrada == null)
                        throw new Exception("Houve um erro: Nenhuma conta entrada foi definida!");

                    GeraNovoRegistro(bancoSaida, EntradaSaida.Saída, valor, transBancaria, $"Saída via transferência entre bancos: enviado para {bancoEntrada._DadosCompletos}", usuario);
                    GeraNovoRegistro(bancoEntrada, EntradaSaida.Entrada, valor, transBancaria, $"Entrada via transferência entre bancos: recebido de {bancoSaida._DadosCompletos}", usuario);

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new Exception(ex.Message);
                }
            }
        }

        public void GeraNovoRegistro(Banco banco, EntradaSaida entradaSaida, Decimal valor, TransacaoBancaria transBancaria, String descricao, Usuario usuario)
        {
            var novoRegistro = new ContaBancaria
            {
                Valor = valor,
                DataRegistro = DateTime.Now,
                TransacaoBancaria = transBancaria,
                Situacao = entradaSaida,
                Caixa = null,
                Banco = banco,
                UsuarioCriacao = usuario,
                Nome = descricao
            };
            SalvarLote(novoRegistro);
        }
    }
}
