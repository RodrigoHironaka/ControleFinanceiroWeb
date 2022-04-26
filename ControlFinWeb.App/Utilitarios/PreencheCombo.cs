using App1.Repositorio.Configuracao;
using ControlFinWeb.App.ViewModels.Utilidades;
using ControlFinWeb.Dominio.Entidades;
using Microsoft.AspNetCore.Mvc.Rendering;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ControlFinWeb.App.Utilitarios
{
  
    public class PreencheCombo
    {
        #region INSTANCE
        private static PreencheCombo _instance;
        public static PreencheCombo Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new PreencheCombo();

                return _instance;
            }
        }
        #endregion

        #region Session
        private ISession _session;
        private ISession Session
        {
            get
            {
                if (_session == null || !_session.IsOpen)
                {
                    if (_session != null)
                        _session.Dispose();

                    _session = NHibernateHelper.ObterSessao();
                }
                return _session;
            }
        }
        #endregion

        public SelectList PreencheComboGasto()
        {
            var lista = new List<SelectListVM>();
            lista.Add(new SelectListVM { Id = "0", Descricao = "Selecione..." });
            lista.AddRange(from a in Session.Query<Gasto>().Where(x => x.Situacao == Dominio.ObjetoValor.Situacao.Ativo)
                           select new SelectListVM
                           {
                               Id = a.Id.ToString(),
                               Descricao = a.Nome
                           });
            return new SelectList(lista, "Id", "Descricao");
        }
    }
}
