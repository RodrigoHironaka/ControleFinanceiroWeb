using ControlFinWeb.Dominio.Entidades;
using ControlFinWeb.Dominio.ObjetoValor;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Type;

namespace ControlFinWeb.Repositorio.Mapeamentos
{
    public class CaixaMAP : ClassMapping<Caixa> 
    {
        public CaixaMAP()
        {
            Table("Caixas");

            Id(x => x.Id, m => m.Generator(Generators.Identity));
            Property(x => x.Numero);
            Property(x => x.ValorInicial);
            Property(x => x.DataGeracao);
            Property(x => x.DataAlteracao);
            Property(x => x.Situacao, m => m.Type<EnumType<SituacaoCaixa>>());
            ManyToOne(x => x.UsuarioCriacao, m => m.Column("UsuarioAbertura"));
            ManyToOne(x => x.UsuarioAlteracao, m => m.Column("UsuarioFechamento"));

            Bag(x => x.FluxosCaixa, m =>
            {
                m.Cascade(Cascade.All);
                m.Key(k => k.Column("Caixa"));
                m.Inverse(true);
            }, map => map.OneToMany(a => a.Class(typeof(FluxoCaixa))));
        }
    }
}
