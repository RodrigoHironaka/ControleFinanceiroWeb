using System;

namespace ControlFinWeb.Dominio.Entidades
{
    public class HoraExtra : Base
    {
        public virtual Pessoa Pessoa { get; set; }
        public virtual TimeSpan HorasTrabalhoManha { get; set; }
        public virtual TimeSpan HorasTrabalhoTarde { get; set; }
        public virtual TimeSpan HorasTrabalhoNoite { get; set; }
        public virtual DateTime? Data { get; set; }
        public virtual TimeSpan HoraInicioManha { get; set; }
        public virtual TimeSpan HoraFinalManha { get; set; }
        public virtual TimeSpan HoraInicioTarde { get; set; }
        public virtual TimeSpan HoraFinalTarde { get; set; }
        public virtual TimeSpan HoraInicioNoite { get; set; }
        public virtual TimeSpan HoraFinalNoite { get; set; }
        public virtual TimeSpan TotalManha { get; set; }
        public virtual TimeSpan TotalTarde { get; set; } 
        public virtual TimeSpan TotalNoite { get; set; }
        public virtual TimeSpan HoraFinalDia { get; set; }
        
    }
}
