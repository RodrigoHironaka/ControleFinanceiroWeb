using ControlFinWeb.Dominio.Dominios;
using ControlFinWeb.Dominio.Entidades;
using ControlFinWeb.Dominio.Interfaces;
using ControlFinWeb.Dominio.ObjetoValor;
using System;

namespace ControlFinWeb.Repositorio.Mapeamentos
{
    public class Configuracao : Base
    {
        #region Sistema
        public virtual String CaminhoArquivos { get; set; }
        public virtual String CaminhoBackup { get; set; }
        public virtual Int32 DiasAlertaVencimento { get; set; }
        public virtual FormaPagamento FormaPagamentoPadraoConta { get; set; }
        public virtual SubGasto GastoFaturaPadrao { get; set; }
        #endregion
    }
}
