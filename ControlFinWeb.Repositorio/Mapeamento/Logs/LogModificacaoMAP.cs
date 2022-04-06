using ControlFinWeb.Dominio.Entidades.Logs;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace ControlFinWeb.Repositorio.Mapeamento.Logs
{
    public class LogModificacaoMAP : ClassMapping<LogModificacao>
    {
        public LogModificacaoMAP()
        {
            Table("LogsModificacoes");

            Id(x => x.Id, m =>
            {
                m.Generator(Generators.HighLow, g => g.Params(new { max_lo = 0 }));
            });

            Property(x => x.Nome, m => m.Length(200));
            Property(x => x.Chave, m => m.Length(200));
            Property(x => x.Historico, m => m.Length(2000));

            Property(x => x.DataGeracao);
            Property(x => x.DataAlteracao);
            ManyToOne(x => x.UsuarioCriacao, m => m.Column("UsuarioCriacao"));
            ManyToOne(x => x.UsuarioAlteracao, m => m.Column("UsuarioAlteracao"));
        }
    }
}
