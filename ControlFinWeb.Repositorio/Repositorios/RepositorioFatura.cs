﻿using ControlFinWeb.Dominio.Entidades;
using ControlFinWeb.Dominio.ObjetoValor;
using NHibernate;
using System;
using System.Linq;

namespace ControlFinWeb.Repositorio.Repositorios
{
    public class RepositorioFatura : RepositorioBase<Fatura>
    {
        private readonly RepositorioParcela RepositorioParcela;
        private readonly RepositorioCartao RepositorioCartao;
        public RepositorioFatura(ISession session, RepositorioParcela repositorioParcela, RepositorioCartao repositorioCartao) : base(session)
        {
            RepositorioParcela = repositorioParcela;
            RepositorioCartao = repositorioCartao;
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
            var dataVenc = new DateTime(entidade.MesAnoReferencia.Year, entidade.MesAnoReferencia.AddMonths(1).Month, diaVenc);
            RepositorioParcela.AdicionarNovaParcela(0, dataVenc, entidade.UsuarioCriacao, null, entidade);
        }

        public void ExcluirOuCancelarFaturaEParcela(Int64 Id)
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
}