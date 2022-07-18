using ControlFinWeb.Dominio.Entidades;
using ControlFinWeb.Dominio.ObjetoValor;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ControlFinWeb.DTO.Contas
{
    public class ConsultaContasDTO
    {
        public String TipoConta { get; set; }
        public Int64 CodigoConta { get; set; }
        public String NomeConta { get; set; }
        public String NumeroParcela { get; set; }
        public Decimal ValorParcela { get; set; }
        public Decimal ValorReajustado { get; set; }
        public DateTime? DataVencimento { get; set; }
        public String FormaCompra { get; set; }
        public String NumeroDocumento { get; set; }
        public String PessoaNome { get; set; }
        public String SituacaoParcelas { get; set; }

        public IList<ConsultaContasDTO> ToList(ISession session)
        {
            var v = Query(null, session);
            return v.ToList();
        }

        public IList<ConsultaContasDTO> ToList(System.Linq.Expressions.Expression<Func<Parcela, bool>> predicado, ISession session)
        {
            var v = Query(predicado, session);
            return v.ToList();
        }

        public ConsultaContasDTO Load(System.Linq.Expressions.Expression<Func<Parcela, bool>> predicado, ISession session)
        {
            var v = Query(predicado, session);
            return v.SingleOrDefault();
        }

        private static IQueryable<ConsultaContasDTO> Query(System.Linq.Expressions.Expression<Func<Parcela, bool>> predicado, ISession session)
        {
            var query = session.Query<Parcela>();
            if (predicado != null)
                query = query.Where(predicado);

            var consulta = from x in query select new ConsultaContasDTO
            {
                NumeroParcela = x.NumeroParcela,
                ValorParcela = x.ValorParcela,
                ValorReajustado = x.ValorReajustado,
                DataVencimento = x.DataVencimento,
                SituacaoParcelas = Enum.GetName(typeof(SituacaoParcela), x.SituacaoParcela),
                TipoConta = Enum.GetName(typeof(TipoConta), x.Conta.TipoConta), 
                CodigoConta = x.Conta.Codigo,
                NomeConta = x.Conta.Nome,
                FormaCompra = x.Conta.FormaCompra.Nome,
                NumeroDocumento = x.Conta.NumeroDocumento,
                PessoaNome = x.Conta.Pessoa != null ? x.Conta.Pessoa.Nome : null
            };
            return consulta;
        }

        public override string ToString()
        {
            return string.Format("{0} - {1}", CodigoConta, NomeConta);
        }



    }
}
