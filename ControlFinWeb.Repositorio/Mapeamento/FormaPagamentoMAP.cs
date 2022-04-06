using ControlFinWeb.Dominio.Dominios;
using ControlFinWeb.Dominio.ObjetoValor;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Type;

namespace ControlFinWeb.Repositorio.Mapeamentos
{
    public class FormaPagamentoMAP : ClassMapping<FormaPagamento>
    {
        public FormaPagamentoMAP()
        {
            Table("FormasPagamento");
            Id(x => x.Id, m =>
            {
                m.Generator(Generators.HighLow, g => g.Params(new { max_lo = 0 }));
            });

            Property(x => x.Nome, m => m.Length(70));
            Property(x => x.DataGeracao);
            Property(x => x.DataAlteracao);
            Property(x => x.Situacao, m => m.Type<EnumType<Situacao>>());
            Property(x => x.DebitoAutomatico, m => m.Type<EnumType<SimNao>>());
            ManyToOne(x => x.UsuarioCriacao, m => m.Column("UsuarioCriacao"));
            ManyToOne(x => x.UsuarioAlteracao, m => m.Column("UsuarioAlteracao"));
        }
    }
}
