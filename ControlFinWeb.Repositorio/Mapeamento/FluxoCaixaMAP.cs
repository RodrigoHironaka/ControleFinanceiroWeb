using ControlFinWeb.Dominio.Entidades;
using ControlFinWeb.Dominio.ObjetoValor;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Type;

namespace ControlFinWeb.Repositorio.Mapeamentos
{
    public class FluxoCaixaMAP : ClassMapping<FluxoCaixa>
    {
        public FluxoCaixaMAP()
        {
            Table("FluxoCaixas");

            Id(x => x.Id, m => {
                m.Generator(Generators.HighLow, g => g.Params(new { max_lo = 0 }));
            });
            Property(x => x.Nome, m => m.Length(150));
            Property(x => x.Valor);
            Property(x => x.DataGeracao, m => m.Column("DataMovimento"));
            Property(x => x.DataAlteracao, m => m.Column("DataMovimentoAlteracao"));
            Property(x => x.DebitoCredito, m => m.Type<EnumType<DebitoCredito>>());
            ManyToOne(x => x.Caixa, m => m.Column("Caixa"));
            ManyToOne(x => x.Parcela, m => m.Column("Parcela"));
            ManyToOne(x => x.FormaPagamento, m => m.Column("FormaPagamento"));
            ManyToOne(x => x.UsuarioCriacao, m => m.Column("UsuarioCriacao"));
            ManyToOne(x => x.UsuarioAlteracao, m => m.Column("UsuarioAlteracao"));
        }
    }
}
