﻿using ControlFinWeb.Dominio.Interfaces;
using LinqKit;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Expression = NHibernate.Criterion.Expression;

namespace ControlFinWeb.Repositorio.Repositorios
{
    public class RepositorioBase<T> where T : class, IEntidade
    {
        public RepositorioBase(ISession session)
        {
            if (session == null)
                throw new ArgumentException("A session deve existir.", "session");
            _session = session;
        }
        private readonly ISession _session;

        protected ISession Session
        {
            get { return _session; }
        }

        public void Salvar(T entidade)
        {
            using (var trans = Session.BeginTransaction())
            {
                try
                {
                    entidade.DataGeracao = DateTime.Now;
                    this.Session.Save(entidade);
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new Exception(ex.ToString());
                }
            }
        }

        public void SalvarLote(T entidade)
        {
            entidade.DataGeracao = DateTime.Now;
            Session.Save(entidade);
        }

        public void Salvar(IList<T> entidades)
        {
            using (var trans = Session.BeginTransaction())
            {
                foreach (var entidade in entidades)
                {
                    try
                    {
                        entidade.DataGeracao = DateTime.Now;
                        this.Session.Save(entidade);
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

        public void SalvarLote(IList<T> entidades)
        {
            foreach (var entidade in entidades)
            {
                entidade.DataGeracao = DateTime.Now;
                Session.Save(entidade);
            }
        }

        public void Alterar(T entidade)
        {
            using (var trans = Session.BeginTransaction())
            {
                try
                {
                    entidade.DataAlteracao = DateTime.Now;
                    this.Session.Merge<T>(entidade);
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new Exception(ex.ToString());
                }
            }
        }

        public void Alterar(IList<T> entidades)
        {
            using (var trans = Session.BeginTransaction())
            {
                try
                {
                    foreach (var entidade in entidades)
                    {
                        entidade.DataAlteracao = DateTime.Now;
                        this.Session.Merge<T>(entidade);
                    }
                  
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new Exception(ex.ToString());
                }
            }
        }

        public void AlterarLote(T entidade)
        {
            entidade.DataAlteracao = DateTime.Now;
            Session.Merge<T>(entidade);
        }
        public void AlterarLote(IList<T> entidades)
        {
            foreach (var entidade in entidades)
            {
                entidade.DataAlteracao = DateTime.Now;
                Session.Merge<T>(entidade);
            }
        }

        public void Excluir(T entidade)
        {
            using (var trans = Session.BeginTransaction())
            {
                try
                {
                    Session.Delete(entidade);
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new Exception(ex.ToString());
                }
            }
        }

        public void ExcluirLote(T entidade)
        {
            Session.Delete(entidade);
        }

        public T ObterPorId(Int64 id)
        {
            var entidade = this.Session.Get<T>(id);
            return entidade;
        }

        public IQueryable<T> ObterTodos()
        {
            var entidades = this
                .Session
                .CreateCriteria(typeof(T))
                .List<T>()
                .AsQueryable<T>();
            return entidades;
        }

        public IQueryable<T> ObterPorParametros(Expression<Func<T, bool>> predicado)
        {
            var a = from u in Session.Query<T>().Where(predicado)
                    select u;
            return a.AsQueryable<T>();
        }

        public Expression<Func<T, bool>> CriarPredicado()
        {
            return PredicateBuilder.New<T>(true);
        }

        public Int64 RetornaUltimoId() //retorna ultimo id da Tabela mas nao é o ultimo id Usado, ele nao pega da tabela uniquekey
        {
            var criterio = Session.CreateCriteria<T>().SetProjection(Projections.Max("Id"));
            try
            {
                return criterio.UniqueResult<Int64>();
            }
            catch
            {
                throw;
            }
        }

        public Int64 RetornaUltimoCodigo() //retorna ultimo codigo da Tabela
        {
            var criterio = Session.CreateCriteria<T>().SetProjection(Projections.Max("Codigo"));
            try
            {
                return criterio.UniqueResult<Int64>();
            }
            catch
            {
                throw;
            }
        }

        public Int64 RetornaUltimoCampo(String campo) //retorna ultimo campo da Tabela
        {
            var criterio = Session.CreateCriteria<T>().SetProjection(Projections.Max(campo));
            try
            {
                return criterio.UniqueResult<Int64>();
            }
            catch
            {
                throw;
            }
        }

        public T ObterPorCodigo(Int64 cod)
        {
            var criterio = Session.CreateCriteria<T>();
            criterio.Add(Expression.Eq("Codigo", cod));

            return criterio.UniqueResult<T>();
        }
    }
}
