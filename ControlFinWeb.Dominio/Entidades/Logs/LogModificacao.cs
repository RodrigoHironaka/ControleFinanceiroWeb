using System;

namespace ControlFinWeb.Dominio.Entidades.Logs
{
    public class LogModificacao : Base
    {
        public virtual String Chave { get; set; }
        public virtual String Historico { get; set; }
    }
}
