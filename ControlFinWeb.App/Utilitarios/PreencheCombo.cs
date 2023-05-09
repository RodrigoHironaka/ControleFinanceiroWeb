using App1.Repositorio.Configuracao;
using ControlFinWeb.Dominio.Dominios;
using ControlFinWeb.Dominio.Entidades;
using Microsoft.AspNetCore.Mvc.Rendering;
using NHibernate;
using NHibernate.Mapping;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace ControlFinWeb.App.Utilitarios
{
    public static class PreencheCombo
    {
        public static SelectList Pessoas()
        {
            var lista = new List<Pessoa>();
            using (var session = NHibernateHelper.ObterSessao())
            {
                try
                {
                    lista.AddRange(from p in session.Query<Pessoa>().Where(x => x.Situacao == Dominio.ObjetoValor.Situacao.Ativo && x.UsuarioCriacao.Id == Configuracao.Usuario.Id)
                                   select new Pessoa
                                   {
                                       Id = p.Id,
                                       Nome = p.Nome
                                   });
                    return new SelectList(lista, "Id", "Nome");

                }
                finally
                {
                    session.Close();
                }
            }
        }
        public static SelectList Pessoas2()
        {
            var lista = new List<Pessoa>();
            lista.Add(new Pessoa { Id = 0, Nome = "---Selecione um item---" });
            using (var session = NHibernateHelper.ObterSessao())
            {
                try
                {
                    lista.AddRange(from p in session.Query<Pessoa>().Where(x => x.Situacao == Dominio.ObjetoValor.Situacao.Ativo && x.UsuarioCriacao.Id == Configuracao.Usuario.Id)
                                   select new Pessoa
                                   {
                                       Id = p.Id,
                                       Nome = p.Nome
                                   });
                    return new SelectList(lista, "Id", "Nome");

                }
                finally
                {
                    session.Close();
                }
            }
        }
        public static SelectList Bancos()
        {
            var lista = new List<Banco>();
            lista.Add(new Banco { Id = 0, Nome = "---Selecione um item---" });
            using (var session = NHibernateHelper.ObterSessao())
            {
                try
                {
                    lista.AddRange(from p in session.Query<Banco>().Where(x => x.Situacao == Dominio.ObjetoValor.Situacao.Ativo && x.UsuarioCriacao.Id == Configuracao.Usuario.Id)
                                   select new Banco
                                   {
                                       Id = p.Id,
                                       Nome = p.Nome
                                   });
                    return new SelectList(lista, "Id", "Nome");

                }
                finally
                {
                    session.Close();
                }
            }
        }
        public static SelectList BancosDadosCompletos()
        {
            var lista = new List<Banco>();
            lista.Add(new Banco { Id = 0, Nome = "---Selecione um item---" });
            using (var session = NHibernateHelper.ObterSessao())
            {
                try
                {
                    lista.AddRange(from p in session.Query<Banco>().Where(x => x.Situacao == Dominio.ObjetoValor.Situacao.Ativo && x.UsuarioCriacao.Id == Configuracao.Usuario.Id)
                                   select new Banco
                                   {
                                       Id = p.Id,
                                       Nome = p._DadosCompletos
                                   });
                    return new SelectList(lista, "Id", "Nome");

                }
                finally
                {
                    session.Close();
                }
            }
        }
       
        public static SelectList FormasPagamento()
        {
            var lista = new List<FormaPagamento>();
            lista.Add(new FormaPagamento { Id = 0, Nome = "---Selecione um item---" });
            using (var session = NHibernateHelper.ObterSessao())
            {
                try
                {
                    lista.AddRange(from p in session.Query<FormaPagamento>().Where(x => x.Situacao == Dominio.ObjetoValor.Situacao.Ativo)
                                   select new FormaPagamento
                                   {
                                       Id = p.Id,
                                       Nome = p.Nome
                                   });
                    return new SelectList(lista, "Id", "Nome");

                }
                finally
                {
                    session.Close();
                }
            }
        }
        public static SelectList FormasPagamentoGeraFatura()
        {
            var lista = new List<FormaPagamento>();
            lista.Add(new FormaPagamento { Id = 0, Nome = "---Selecione um item---" });
            using (var session = NHibernateHelper.ObterSessao())
            {
                try
                {
                    lista.AddRange(from p in session.Query<FormaPagamento>().Where(x => x.Situacao == Dominio.ObjetoValor.Situacao.Ativo && x.GerarRegistroFatura == true)
                                   select new FormaPagamento
                                   {
                                       Id = p.Id,
                                       Nome = p.Nome
                                   });
                    return new SelectList(lista, "Id", "Nome");

                }
                finally
                {
                    session.Close();
                }
            }
        }
        public static SelectList Gastos()
        {
            var lista = new List<Gasto>();
            lista.Add(new Gasto { Id = 0, Nome = "---Selecione um item---" });
            using (var session = NHibernateHelper.ObterSessao())
            {
                try
                {
                    lista.AddRange(from p in session.Query<Gasto>().Where(x => x.Situacao == Dominio.ObjetoValor.Situacao.Ativo).OrderBy(x => x.Nome)
                                   select new Gasto
                                   {
                                       Id = p.Id,
                                       Nome = p.Nome
                                   });
                    return new SelectList(lista, "Id", "Nome");

                }
                finally
                {
                    session.Close();
                }
            }
        }
        public static SelectList SubGastos()
        {
            var lista = new List<SubGasto>();
            lista.Add(new SubGasto { Id = 0, Nome = "---Selecione um item---" });
            using (var session = NHibernateHelper.ObterSessao())
            {
                try
                {
                    lista.AddRange(from p in session.Query<SubGasto>().Where(x => x.Situacao == Dominio.ObjetoValor.Situacao.Ativo).OrderBy(x => x.Gasto.Nome)
                                   select new SubGasto
                                   {
                                       Id = p.Id,
                                       Nome = p._DescricaoCompleta
                                   });
                    return new SelectList(lista, "Id", "Nome");

                }
                finally
                {
                    session.Close();
                }
            }
        }
        public static SelectList Cartoes()
        {
            var lista = new List<Cartao>();
            lista.Add(new Cartao { Id = 0, Nome = "---Selecione um item---" });
            using (var session = NHibernateHelper.ObterSessao())
            {
                try
                {
                    lista.AddRange(from p in session.Query<Cartao>().Where(x => x.Situacao == Dominio.ObjetoValor.Situacao.Ativo && x.UsuarioCriacao.Id == Configuracao.Usuario.Id)
                                   select new Cartao
                                   {
                                       Id = p.Id,
                                       Nome = p.Nome
                                   });
                    return new SelectList(lista, "Id", "Nome");

                }
                finally
                {
                    session.Close();
                }
            }
        }
    }
}
