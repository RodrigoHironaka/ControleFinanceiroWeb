using ControlFinWeb.Dominio.Entidades;
using ControlFinWeb.Dominio.ObjetoValor;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Type;

namespace ControlFinWeb.Repositorio.Mapeamentos
{
    public class CartaoCreditoMAP : ClassMapping<CartaoCredito>
    {
        public CartaoCreditoMAP()
        {
            Table("CartoesCredito");
            Id(x => x.Id, m =>
            {
                m.Generator(Generators.HighLow, g => g.Params(new { max_lo = 0 }));
            });
            Property(x => x.Nome, m => m.Length(300));
            Property(x => x.DataGeracao);
            Property(x => x.DataAlteracao);
            Property(x => x.MesAnoReferencia);
            Property(x => x.SituacaoFatura, m => m.Type<EnumType<SituacaoFatura>>());
            
            Property(x => x.ValorFatura, m =>
            {
                m.Precision(10);
                m.Scale(2);
            });

            ManyToOne(x => x.Cartao, m => m.Column("Cartao"));
            ManyToOne(x => x.UsuarioCriacao, m => m.Column("UsuarioCriacao"));
            ManyToOne(x => x.UsuarioAlteracao, m => m.Column("UsuarioAlteracao"));

            Bag(x => x.CartoesCreditoItens, m =>
            {
                m.Cascade(Cascade.All);
                m.Key(k => k.Column("CartaoCredito"));
                m.Inverse(true);
            }, map => map.OneToMany(a => a.Class(typeof(CartaoCreditoItens))));
        }
    }
}
