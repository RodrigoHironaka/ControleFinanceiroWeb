using ControlFinWeb.Repositorio.Mapeamentos;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NHibernate.Tool.hbm2ddl;
using System;

namespace App1.Repositorio.Configuracao
{
    public class NHibernateHelper
    {
        private static ISessionFactory _sessionFactory;

        //private static String connectionString = "Server=localhost;Port=3306;Database=ControlFin;Uid=root;Pwd=123456;SslMode=none";
        private static String connectionString = "Server=MYSQL8003.site4now.net;Database=db_a97813_finan;Uid=a97813_finan;Pwd=q1w2e3r4";

        public static ISession ObterSessao()
        {
            if (_sessionFactory == null)
            {
                lock (typeof(NHibernateHelper))
                {
                    if (_sessionFactory == null)
                    {
                        var configure = new Configuration();
                        configure.DataBaseIntegration(db =>
                        {
                            db.Dialect<NHibernate.Dialect.MySQL5InnoDBDialect>();
                            db.Driver<NHibernate.Driver.MySqlDataDriver>();

                            db.ConnectionString = connectionString;

                            db.Timeout = 10;

                            db.LogFormattedSql = false;
                            db.LogSqlInConsole = false;
                            db.AutoCommentSql = false;
                        });

                        var mapper = new ModelMapper();
                        mapper.AddMappings(typeof(UsuarioMAP).Assembly.GetTypes());

                        HbmMapping mapping = mapper.CompileMappingForAllExplicitlyAddedEntities();
                        configure.AddMapping(mapping);

                        _sessionFactory = configure.BuildSessionFactory();
                        //BuildSchema(configure);
                    }
                }
            }

            return _sessionFactory.OpenSession();
        }

        private static void BuildSchema(Configuration config)
        {
            new SchemaExport(config).SetOutputFile(@"D:\Projetos\ControlFinWeb\Doc\Script\ControlFinWeb.sql").SetDelimiter(";")
                .Create(false, false);
        }
    }
}
