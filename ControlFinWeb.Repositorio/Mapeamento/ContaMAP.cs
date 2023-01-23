using ControlFinWeb.Dominio.Entidades;
using ControlFinWeb.Dominio.ObjetoValor;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Type;

namespace ControlFinWeb.Repositorio.Mapeamentos
{
    public class ContaMAP : ClassMapping<Conta>
    {
        public ContaMAP()
        {
            Table("Contas");

            Id(x => x.Id, m => m.Generator(Generators.Identity));
            Property(x => x.Nome, m => m.Length(70));
            Property(x => x.Observacao, m => m.Length(400));
            Property(x => x.DataEmissao);
            Property(x => x.NumeroDocumento, m => m.Length(20));
            Property(x => x.DataGeracao);
            Property(x => x.DataAlteracao);
            Property(x => x.Situacao, m => m.Type<EnumType<SituacaoConta>>());
            Property(x => x.TipoConta, m => m.Type<EnumType<TipoConta>>());
            Property(x => x.TipoPeriodo, m => m.Type<EnumType<PeriodoConta>>());
            ManyToOne(x => x.FormaCompra, m => m.Column("FormaCompra"));
            ManyToOne(x => x.SubGasto, m => m.Column("SubGasto"));
            ManyToOne(x => x.Pessoa, m => m.Column("Pessoa"));
            ManyToOne(x => x.UsuarioCriacao, m => m.Column("UsuarioCriacao"));
            ManyToOne(x => x.UsuarioAlteracao, m => m.Column("UsuarioAlteracao"));
            
            Bag(x => x.Parcelas, m =>
            {
                m.Cascade(Cascade.All | Cascade.DeleteOrphans);
                m.Key(k => k.Column("Conta"));
                m.Inverse(true);
            }, map => map.OneToMany(a => a.Class(typeof(Parcela))));

            Bag(x => x.Arquivos, m =>
            {
                m.Cascade(Cascade.All | Cascade.DeleteOrphans);
                m.Key(k => k.Column("Conta"));
                m.Inverse(true);
            }, map => map.OneToMany(a => a.Class(typeof(Arquivo))));
        }
    }
}
