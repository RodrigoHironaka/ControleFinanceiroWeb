using ControlFinWeb.Dominio.Entidades.Logs;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace ControlFinWeb.Repositorio.Mapeamento.Logs
{
    public class EventoMAP : ClassMapping<Evento>
    {
        public EventoMAP()
        {
            Table("Eventos");

            Id(x => x.Id, m => m.Generator(Generators.Identity));
            Property(x => x.Historico, m => m.Length(1000));
            Property(x => x.Data);
            ManyToOne(x => x.Usuario, m => m.Column("Usuario"));
        }
    }
}
