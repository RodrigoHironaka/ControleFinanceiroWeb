using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlFinWeb.Dominio.Entidades.Logs
{
    public class Evento
    {
        public virtual Int64 Id { get; set; }
        public virtual String Historico { get; set; }
        public virtual Usuario Usuario { get; set; }
        public virtual DateTime Data { get; set; }
    }
}
