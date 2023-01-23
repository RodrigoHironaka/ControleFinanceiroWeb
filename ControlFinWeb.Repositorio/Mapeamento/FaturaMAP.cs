using ControlFinWeb.Dominio.Entidades;
using ControlFinWeb.Dominio.ObjetoValor;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Type;

namespace ControlFinWeb.Repositorio.Mapeamentos
{
    public class FaturaMAP : ClassMapping<Fatura>
    {
        public FaturaMAP()
        {
            Table("Faturas");

            Id(x => x.Id, m => m.Generator(Generators.Identity));
            Property(x => x.Nome, m => m.Length(500));
            Property(x => x.MesAnoReferencia);
            Property(x => x.DataFechamento);
            Property(x => x.DataGeracao);
            Property(x => x.DataAlteracao);
            Property(x => x.SituacaoFatura, m => m.Type<EnumType<SituacaoFatura>>());
            ManyToOne(x => x.Cartao, m => m.Column("Cartao"));
            ManyToOne(x => x.UsuarioCriacao, m => m.Column("UsuarioCriacao"));
            ManyToOne(x => x.UsuarioAlteracao, m => m.Column("UsuarioAlteracao"));

            Bag(x => x.FaturaItens, m =>
            {
                m.Cascade(Cascade.All);
                m.Key(k => k.Column("CartaoCredito"));
                m.Inverse(true);
            }, map => map.OneToMany(a => a.Class(typeof(FaturaItens))));

            Bag(x => x.Parcelas, m =>
            {
                m.Cascade(Cascade.All);
                m.Key(k => k.Column("Parcela"));
                m.Inverse(true);
            }, map => map.OneToMany(a => a.Class(typeof(Parcela))));

        }
    }
}
