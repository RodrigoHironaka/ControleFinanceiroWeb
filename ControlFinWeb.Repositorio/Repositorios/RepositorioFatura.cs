using ControlFinWeb.Dominio.Entidades;
using ControlFinWeb.Dominio.ObjetoValor;
using NHibernate;
using ObjectsComparer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ControlFinWeb.Repositorio.Repositorios
{
    public class RepositorioFatura : RepositorioBase<Fatura>
    {
        private readonly RepositorioParcela RepositorioParcela;
        private readonly RepositorioCartao RepositorioCartao;
        private readonly RepositorioLogModificacao RepositorioLog;
        public RepositorioFatura(ISession session, RepositorioParcela repositorioParcela, RepositorioCartao repositorioCartao, RepositorioLogModificacao repositorioLog) : base(session)
        {
            RepositorioParcela = repositorioParcela;
            RepositorioCartao = repositorioCartao;
            RepositorioLog = repositorioLog;
        }

        public void SalvarEGerarNovaParcela(Fatura entidade)
        {
            using (var trans = Session.BeginTransaction())
            {
                try
                {
                    entidade.DataGeracao = DateTime.Now;
                    SalvarLote(entidade);

                    var diaVenc = RepositorioCartao.ObterPorId(entidade.Cartao.Id).DiaVencimento;
                    var dataVenc = new DateTime(entidade.MesAnoReferencia.Year, entidade.MesAnoReferencia.AddMonths(1).Month, diaVenc);
                    RepositorioParcela.AdicionarNovaParcela(0, dataVenc, entidade.UsuarioCriacao, null, entidade);

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new Exception(ex.ToString());
                }
            }
        }

        public void SalvarEGerarNovaParcelaLote(Fatura entidade)
        {
            entidade.DataGeracao = DateTime.Now;
            SalvarLote(entidade);

            var diaVenc = RepositorioCartao.ObterPorId(entidade.Cartao.Id).DiaVencimento;
            var dataVenc = new DateTime(entidade.MesAnoReferencia.Year, entidade.MesAnoReferencia.Month, diaVenc).AddMonths(1);
            RepositorioParcela.AdicionarNovaParcela(0, dataVenc, entidade.UsuarioCriacao, null, entidade);
        }

        public void ExcluirOuCancelarFaturaEParcela(Int64 Id, Usuario usuario)
        {
            using (var trans = Session.BeginTransaction())
            {
                try
                {
                    var fatura = ObterPorId(Id);
                    var existeParcela = RepositorioParcela.ObterPorParametros(x => x.Fatura.Id.Equals(fatura.Id)).FirstOrDefault();

                    if (existeParcela != null)
                        RepositorioParcela.ExcluirLote(existeParcela);
                    ExcluirLote(fatura);

                    RepositorioLog.RegistrarLog($"Fatura {fatura._DescricaoCompleta}\nParcela[{existeParcela.Id}] nº {existeParcela?.Numero} - valor {existeParcela.ValorAberto.ToString("C2")}\nforam excluídas", usuario, $"Fatura[{Id}]");

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new Exception(ex.ToString());
                }
            }
        }

        public void EditarRegistrarLog(Fatura fatura, IEnumerable<Difference> diferencas, Usuario usuario, String chave)
        {
            using (var trans = Session.BeginTransaction())
            {
                try
                {
                    RepositorioLog.RegistrarLogModificacao(diferencas, usuario, chave);
                    AlterarLote(fatura);
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new Exception(ex.Message);
                }
            }
        }
    }
}
