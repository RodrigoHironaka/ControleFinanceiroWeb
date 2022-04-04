using ControlFinWeb.Dominio.Entidades;
using ControlFinWeb.Dominio.ObjetoValor;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Type;

namespace ControlFinWeb.Repositorio.Mapeamentos
{
    public class BancoFinanceiroMAP : ClassMapping<BancoFinanceiro>
    {
        public BancoFinanceiroMAP()
        {
            Table("BancosFinanceiro");
            Id(x => x.Id, m =>
            {
                m.Generator(Generators.HighLow, g => g.Params(new { max_lo = 0 }));
            });
            Property(x => x.Codigo);
            Property(x => x.Nome, m => m.Length(200));
            Property(x => x.DataGeracao);
            Property(x => x.DataAlteracao);
            Property(x => x.Valor, m =>
            {
                m.Precision(10);
                m.Scale(2);
            });
            Property(x => x.Situacao, m => m.Type<EnumType<EntradaSaida>>());
            Property(x => x.TransacaoBancaria, m => m.Type<EnumType<TransacaoBancaria>>());
            ManyToOne(x => x.Caixa, m => m.Column("Caixa"));
            ManyToOne(x => x.Banco, m => m.Column("Banco"));
            ManyToOne(x => x.UsuarioCriacao, m => m.Column("UsuarioCriacao"));
            ManyToOne(x => x.UsuarioAlteracao, m => m.Column("UsuarioAlteracao"));

        }
    }
}
