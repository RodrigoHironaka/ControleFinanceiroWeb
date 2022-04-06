﻿using ControlFinWeb.Dominio.Entidades;
using ControlFinWeb.Dominio.ObjetoValor;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Type;

namespace ControlFinWeb.Repositorio.Mapeamentos
{
    public class PessoaMAP : ClassMapping<Pessoa>
    {
        public PessoaMAP()
        {
            Table("Pessoas");

            Id(x => x.Id, m => {
                m.Generator(Generators.HighLow, g => g.Params(new { max_lo = 0}));
            });

            Property(x => x.Nome, m => m.Length(70));
            Property(x => x.DataGeracao);
            Property(x => x.DataAlteracao);
            Property(x => x.Situacao, m => m.Type<EnumType<Situacao>>());
            ManyToOne(x => x.UsuarioCriacao, m => m.Column("UsuarioCriacao"));
            ManyToOne(x => x.UsuarioAlteracao, m => m.Column("UsuarioAlteracao"));

            Bag(x => x.PessoaRendas, m =>
            {
                m.Cascade(Cascade.All);
                m.Key(k => k.Column("Pessoa"));
                m.Inverse(true);
            }, map => map.OneToMany(a => a.Class(typeof(PessoaRendas))));
        }
    }
}
