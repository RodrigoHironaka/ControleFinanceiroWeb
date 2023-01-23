using ControlFinWeb.Dominio.Entidades;
using ControlFinWeb.Dominio.ObjetoValor;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Type;

namespace ControlFinWeb.Repositorio.Mapeamentos
{
    public class ContaBancariaMAP : ClassMapping<ContaBancaria>
    {
        public ContaBancariaMAP()
        {
            Table("contasBancarias");

            Id(x => x.Id, m => m.Generator(Generators.Identity));
            Property(x => x.Nome, m => m.Length(200));
            Property(x => x.Valor);
            Property(x => x.DataRegistro);
            Property(x => x.Situacao, m => m.Type<EnumType<EntradaSaida>>());
            Property(x => x.TransacaoBancaria, m => m.Type<EnumType<TransacaoBancaria>>());
            Property(x => x.DataGeracao);
            Property(x => x.DataAlteracao);
            ManyToOne(x => x.Caixa, m => m.Column("Caixa"));
            ManyToOne(x => x.Banco, m => m.Column("Banco"));
            ManyToOne(x => x.UsuarioCriacao, m => m.Column("UsuarioCriacao"));
            ManyToOne(x => x.UsuarioAlteracao, m => m.Column("UsuarioAlteracao"));

        }
    }
}
