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

            Id(x => x.Id, m => m.Generator(Generators.Identity));
            Property(x => x.Nome);
            Property(x => x.HorasTrabalhoManha);
            Property(x => x.HorasTrabalhoTarde);
            Property(x => x.HorasTrabalhoNoite);
            Property(x => x.Data);
            Property(x => x.HoraInicioManha);
            Property(x => x.HoraFinalManha);
            Property(x => x.TotalManha);
            Property(x => x.HoraInicioTarde);
            Property(x => x.HoraFinalTarde);
            Property(x => x.TotalTarde);
            Property(x => x.HoraInicioNoite);
            Property(x => x.HoraFinalNoite);
            Property(x => x.TotalNoite);
            Property(x => x.HoraFinalDia);
            Property(x => x.AjusteManual);
            Property(x => x.DataGeracao);
            Property(x => x.DataAlteracao);
            ManyToOne(x => x.Pessoa, m => m.Column("Pessoa"));
            ManyToOne(x => x.UsuarioCriacao, m => m.Column("UsuarioCriacao"));
            ManyToOne(x => x.UsuarioAlteracao, m => m.Column("UsuarioAlteracao"));


        }
    }
}
