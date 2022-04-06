using ControlFinWeb.Dominio.Entidades;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace ControlFinWeb.Repositorio.Mapeamentos
{
    public class PessoaRendasMAP : ClassMapping<PessoaRendas>
    {
        public PessoaRendasMAP()
        {
            Table("PessoasRendas");

            Id(x => x.Id, m => {
                m.Generator(Generators.HighLow, g => g.Params(new { max_lo = 0}));
            });
            Property(x => x.RendaBruta);
            Property(x => x.RendaLiquida);
            Property(x => x.DataGeracao);
            Property(x => x.DataAlteracao);
            ManyToOne(x => x.TipoRenda, m=>m.Column("TipoRenda"));
            ManyToOne(x => x.Pessoa, m => m.Column("Pessoa"));
            ManyToOne(x => x.UsuarioCriacao, m => m.Column("UsuarioCriacao"));
            ManyToOne(x => x.UsuarioAlteracao, m => m.Column("UsuarioAlteracao"));
        }
    }
}
