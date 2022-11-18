﻿using ControlFinWeb.Dominio.Entidades;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace ControlFinWeb.Repositorio.Mapeamentos
{
    public class FaturaItensMAP : ClassMapping<FaturaItens>
    {
        public FaturaItensMAP()
        {
            Table("FaturasItens");
            Id(x => x.Id, m =>
            {
                m.Generator(Generators.HighLow, g => g.Params(new { max_lo = 0 }));
            });

            Property(x => x.Nome, m => m.Length(100));
            Property(x => x.DataCompra);
            Property(x => x.Valor);
            Property(x => x.DataGeracao);
            Property(x => x.DataAlteracao);
            ManyToOne(x => x.Pessoa, m => m.Column("Pessoa"));
            ManyToOne(x => x.SubGasto, m => m.Column("SubGasto"));
            ManyToOne(x => x.CartaoCredito, m => m.Column("CartaoCredito"));
            ManyToOne(x => x.UsuarioCriacao, m => m.Column("UsuarioCriacao"));
            ManyToOne(x => x.UsuarioAlteracao, m => m.Column("UsuarioAlteracao"));
        }
    }
}