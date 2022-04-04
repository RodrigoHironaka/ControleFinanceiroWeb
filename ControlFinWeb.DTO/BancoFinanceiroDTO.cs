using ControlFinWeb.Dominio.Entidades;
using ControlFinWeb.Dominio.ObjetoValor;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CFP.DTO.Cofres
{
    public class BancoFinanceiroDTO
    {
        public String Situacao { get; set; }
        public Int64 Codigo { get; set; }
        public String Nome { get; set; }
        public String Banco { get; set; }
        public Decimal ValorGuardado { get; set; }
        public DateTime? DataGeracao { get; set; }
        

        public IList<BancoFinanceiroDTO> ToList(ISession session)
        {
            var v = Query(null, session);
            return v.ToList();
        }

        public IList<BancoFinanceiroDTO> ToList(System.Linq.Expressions.Expression<Func<BancoFinanceiro, bool>> predicado, ISession session)
        {
            var v = Query(predicado, session);
            return v.ToList();
        }

        public BancoFinanceiroDTO Load(System.Linq.Expressions.Expression<Func<BancoFinanceiro, bool>> predicado, ISession session)
        {
            var v = Query(predicado, session);
            return v.SingleOrDefault();
        }

        private static IQueryable<BancoFinanceiroDTO> Query(System.Linq.Expressions.Expression<Func<BancoFinanceiro, bool>> predicado, ISession session)
        {
            var query = session.Query<BancoFinanceiro>();
            if (predicado != null)
                query = query.Where(predicado);

            var consulta = from x in query
                           select new BancoFinanceiroDTO
                           {
                               Situacao = Enum.GetName(typeof(SituacaoBanco), x.Situacao),
                               Codigo = x.Codigo,
                               Nome = x.Nome,
                               Banco = x.Banco.Nome,
                               ValorGuardado = x.Valor,
                               DataGeracao = x.DataGeracao
                           };
            return consulta;
        }

        public override string ToString()
        {
            return string.Format("{0}", Banco);
        }

    }
}
