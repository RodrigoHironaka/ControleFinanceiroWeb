using ControlFinWeb.Dominio.Entidades;
using ControlFinWeb.Dominio.ObjetoValor;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Type;

namespace ControlFinWeb.Repositorio.Mapeamentos
{
    public class ParcelaMAP : ClassMapping<Parcela>
    {
        public ParcelaMAP()
        {
            Table("Parcelas");
            Id(x => x.Id, m =>
            {
                m.Generator(Generators.HighLow, g => g.Params(new { max_lo = 0 }));
                
            });
            Property(x => x.Numero);
            Property(x => x.ParcelaDe, m => m.Length(10));
            Property(x => x.ValorParcela);
            Property(x => x.DataVencimento);
            Property(x => x.DataPagamento);
            Property(x => x.JurosPorcentual);
            Property(x => x.JurosValor);
            Property(x => x.DescontoPorcentual);
            Property(x => x.DescontoValor);
            Property(x => x.ValorReajustado);
            Property(x => x.ValorPago);
            Property(x => x.ValorAberto);
            Property(x => x.Observacao, m => m.Length(500));
            Property(x => x.DataGeracao);
            Property(x => x.DataAlteracao);
            Property(x => x.SituacaoParcela, m => m.Type<EnumType<SituacaoParcela>>());
            ManyToOne(x => x.FormaPagamento, m => m.Column("FormaPagamento"));
            ManyToOne(x => x.Conta, m => m.Column("Conta"));
            ManyToOne(x => x.FaturaCartaoCredito, m => m.Column("FaturaCartaoCredito"));
            ManyToOne(x => x.UsuarioCriacao, m => m.Column("UsuarioCriacao"));
            ManyToOne(x => x.UsuarioAlteracao, m => m.Column("UsuarioAlteracao"));


        }
    }
}
