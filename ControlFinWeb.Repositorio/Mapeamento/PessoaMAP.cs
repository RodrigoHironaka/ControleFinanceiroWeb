using ControlFinWeb.Dominio.Entidades;
using ControlFinWeb.Dominio.ObjetoValor;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Type;

namespace ControlFinWeb.Repositorio.Mapeamentos
{
    public class PessoaMAP : ClassMapping<Pessoa>
    {
        public PessoaMAP()
        {
            Table("Pessoas");

            Id(x => x.Id, m => m.Generator(Generators.Identity));
            Property(x => x.Nome, m => m.Length(70));
            Property(x => x.DataGeracao);
            Property(x => x.DataAlteracao);
            Property(x => x.Renda);
            Property(x => x.Situacao, m => m.Type<EnumType<Situacao>>());
            ManyToOne(x => x.UsuarioCriacao, m => m.Column("UsuarioCriacao"));
            ManyToOne(x => x.UsuarioAlteracao, m => m.Column("UsuarioAlteracao"));

           
        }
    }
}
