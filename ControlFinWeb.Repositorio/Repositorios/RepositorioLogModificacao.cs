using ControlFinWeb.Dominio.Entidades;
using ControlFinWeb.Dominio.Entidades.Logs;
using NHibernate;
using ObjectsComparer;
using System;
using System.Collections.Generic;
using System.Text;

namespace ControlFinWeb.Repositorio.Repositorios
{
    public class RepositorioLogModificacao : RepositorioBase<LogModificacao>
    {
        public RepositorioLogModificacao(ISession session) : base(session) {}

        public void RegistrarLogModificacao(IEnumerable<Difference> diferencas, Usuario usuario, String chave)
        {
            var historico = new StringBuilder();
            foreach (var item in diferencas)
                historico.AppendLine(String.Format("Campo: {0} | Antes: {1} | Depois: {2}", item.MemberPath, item.Value1, item.Value2));

            var log = new LogModificacao
            {
                DataGeracao = DateTime.Now,
                UsuarioCriacao = usuario,
                Historico = historico.ToString(),
                Chave = chave,
            };

            SalvarLote(log);
        }

    }
}
