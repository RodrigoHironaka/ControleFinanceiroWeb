using ControlFinWeb.Dominio.Entidades;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace ControlFinWeb.Repositorio.Mapeamentos
{
    public class HoraExtraMAP : ClassMapping<HoraExtra>
    {
        public HoraExtraMAP()
        {
            Table("HorasExtra");

            Id(x => x.Id, m =>
            {
                m.Generator(Generators.HighLow, g => g.Params(new { max_lo = 0 }));
            });
            
            Property(x => x.Nome);
            Property(x => x.DataGeracao);
            Property(x => x.DataAlteracao);
            Property(x => x.DataHoraExtra);
            Property(x => x.HoraInicioManha);
            Property(x => x.HoraFinalManha);
            Property(x => x.HoraInicioTarde);
            Property(x => x.HoraFinalTarde);
            Property(x => x.HoraInicioNoite);
            Property(x => x.HoraFinalNoite);
            ManyToOne(x => x.Pessoa, m => m.Column("Pessoa"));
            ManyToOne(x => x.UsuarioCriacao, m => m.Column("UsuarioCriacao"));
            ManyToOne(x => x.UsuarioAlteracao, m => m.Column("UsuarioAlteracao"));


        }
    }
}
