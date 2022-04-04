using ControlFinWeb.Dominio.Entidades;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace ControlFinWeb.Repositorio.Mapeamentos
{
    public class PessoaRendasMAP : ClassMapping<PessoaRendas>
    {
        public PessoaRendasMAP()
        {
            Table("PessoaRendas");

            Id(x => x.ID, m => {
                m.Generator(Generators.HighLow, g => g.Params(new { max_lo = 0}));
            });

            Property(x => x.RendaBruta);
            Property(x => x.RendaLiquida);
            ManyToOne(x => x.TipoRenda, m=>m.Column("TipoRenda"));
            ManyToOne(x => x.Pessoa, m => m.Column("Pessoa"));
        }
    }
}
