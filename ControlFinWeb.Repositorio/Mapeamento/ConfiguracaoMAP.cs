using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace ControlFinWeb.Repositorio.Mapeamentos
{
    public class ConfiguracaoMAP : ClassMapping<Configuracao>
    {
        public ConfiguracaoMAP()
        {
            Table("Configuracoes");
            Id(x => x.Id, m =>
            {
                m.Generator(Generators.HighLow, g => g.Params(new { max_lo = 0 }));
            });

            Property(x => x.CaminhoArquivos, m => m.Length(250));
            Property(x => x.CaminhoBackup, m => m.Length(250));
            Property(x => x.DiasAlertaVencimento);
            Property(x => x.DataGeracao);
            Property(x => x.DataAlteracao);
            ManyToOne(x => x.FormaPagamentoPadraoConta, m => m.Column("FormaPagamentoPadraoConta"));
            ManyToOne(x => x.TransacaoBancariaPadrao, m => m.Column("TransacaoBancariaPadrao"));
            ManyToOne(x => x.UsuarioCriacao, m => m.Column("UsuarioCriacao"));
        }
    }
}
