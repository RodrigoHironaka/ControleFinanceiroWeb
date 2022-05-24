using ControlFinWeb.Dominio.Entidades;
using ControlFinWeb.Dominio.ObjetoValor;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlFinWeb.Repositorio.Mapeamento
{
    public class CartaoMAP : ClassMapping<Cartao>
    {
        public CartaoMAP()
        {
            Table("Cartoes");
            Id(x => x.Id, m =>
            {
                m.Generator(Generators.HighLow, g => g.Params(new { max_lo = 0 }));
            });

            Property(x => x.Nome, m => m.Length(70));
            Property(x => x.LimiteFatura);
            Property(x => x.DiaVencimento);
            Property(x => x.VencimentoCartao);
            Property(x => x.DataGeracao);
            Property(x => x.DataAlteracao);
            Property(x => x.Situacao, m => m.Type<EnumType<Situacao>>());
            ManyToOne(x => x.Banco, m => m.Column("Banco"));
            ManyToOne(x => x.UsuarioCriacao, m => m.Column("UsuarioCriacao"));
            ManyToOne(x => x.UsuarioAlteracao, m => m.Column("UsuarioAlteracao"));
        }
    }
}
