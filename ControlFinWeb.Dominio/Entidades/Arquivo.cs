using System;

namespace ControlFinWeb.Dominio.Entidades
{
    public class Arquivo : Base
    {
        public override string ToString()
        {
            return Nome;
        }

        public virtual Conta Conta { get; set; }
        public virtual FaturaCartaoCredito FaturaCartaoCredito { get; set; }
        public virtual String Caminho { get; set; }
        public virtual String Extensao { get; set; }

    }
}
