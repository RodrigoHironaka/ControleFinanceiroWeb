using ControlFinWeb.Dominio.Entidades;
using ControlFinWeb.Dominio.ObjetoValor;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Type;

namespace ControlFinWeb.Repositorio.Mapeamentos
{
    public class UsuarioMAP : ClassMapping<Usuario>
    {
        public UsuarioMAP()
        {
            Table("Usuarios");

            Id(x => x.Id, m => m.Generator(Generators.Identity));
            Property(x => x.Nome, m => m.Length(70));
            Property(x => x.Email, m => m.Length(70));
            Property(x => x.DataGeracao);
            Property(x => x.DataAlteracao);
            Property(x => x.Senha, m => m.Length(255));
            Property(x => x.Autorizado, m => m.Type<EnumType<SimNao>>());
            Property(x => x.TipoUsuario, m => m.Type<EnumType<TipoUsuario>>());
            Property(x => x.Situacao, m => m.Type<EnumType<Situacao>>());
        }
    }
}
