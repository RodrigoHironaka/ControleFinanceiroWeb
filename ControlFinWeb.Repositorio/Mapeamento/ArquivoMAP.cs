using ControlFinWeb.Dominio.Entidades;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace ControlFinWeb.Repositorio.Mapeamento
{
    public class ArquivoMAP : ClassMapping<Arquivo>
    {
        public ArquivoMAP()
        {
            Table("Arquivos");
            Id(x => x.Id, m =>
            {
                m.Generator(Generators.HighLow, g => g.Params(new { max_lo = 0 }));

            });
           
            Property(x => x.Nome, m => m.Length(255));
            Property(x => x.DataGeracao);
            Property(x => x.DataAlteracao);
            Property(x => x.Caminho, m => m.Length(255));
            Property(x => x.Extensao, m => m.Length(255));
            ManyToOne(x => x.Conta, m => m.Column("Conta"));
            ManyToOne(x => x.UsuarioCriacao, m => m.Column("UsuarioCriacao"));
            ManyToOne(x => x.UsuarioAlteracao, m => m.Column("UsuarioAlteracao"));



        }
    }
}
