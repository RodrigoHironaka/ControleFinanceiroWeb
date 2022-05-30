using ControlFinWeb.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ControlFinWeb.App.ViewModels
{
    public class HoraExtraVM
    {
        public Int64 Id { get; set; }

        [DisplayName("Descrição")]
        [StringLength(255, ErrorMessage = "Limite máximo de 255 caracteres.")]
        [Required(ErrorMessage = "Campo Obrigatório")]
        public String Nome { get; set; }

        [Range(1, Int64.MaxValue, ErrorMessage = "Informe uma Pessoa")]
        public Int64 PessoaId { get; set; }
        public PessoaVM PessoaVM { get; set; }

        [DisplayName("Hora Padrão Manhã")]
        public TimeSpan HorasTrabalhoManha { get; set; }

        [DisplayName("Hora Padrão Tarde")]
        public TimeSpan HorasTrabalhoTarde { get; set; }

        [DisplayName("Hora Padrão Noite")]
        public TimeSpan HorasTrabalhoNoite { get; set; }
        public DateTime Data { get; set; }

        [DisplayName("Início Manhã")]
        public TimeSpan HoraInicioManha { get; set; }

        [DisplayName("Final Manhã")]
        public TimeSpan HoraFinalManha { get; set; }

        [DisplayName("Total Manhã")]
        public TimeSpan TotalManha { get; set; }

        [DisplayName("Início Tarde")]
        public TimeSpan HoraInicioTarde { get; set; }

        [DisplayName("Final Tarde")]
        public TimeSpan HoraFinalTarde { get; set; }

        [DisplayName("Início Noite")]
        public TimeSpan HoraInicioNoite { get; set; }

        [DisplayName("Final Noite")]
        public TimeSpan HoraFinalNoite { get; set; }

        [DisplayName("Total Tarde")]
        public TimeSpan TotalTarde { get; set; }

        [DisplayName("Total Manhã")]
        public TimeSpan TotalNoite { get; set; }

        [DisplayName("Total horas do dia")]
        public TimeSpan HorasTrabalhoDia
        {
            get
            {
                return HorasTrabalhoManha + HorasTrabalhoTarde + HorasTrabalhoNoite;
            }
        }

        [DisplayName("Total Dia")]
        public TimeSpan HoraFinalDia 
        {
            get
            {
                if (TotalManha == TimeSpan.Zero)
                    return TotalManha + TotalTarde + TotalNoite - HorasTrabalhoManha;
                else if (TotalTarde == TimeSpan.Zero)
                    return TotalManha + TotalTarde + TotalNoite - HorasTrabalhoTarde;
                else if (TotalNoite == TimeSpan.Zero)
                    return TotalManha + TotalTarde + TotalNoite - HorasTrabalhoNoite;

                else
                    return TotalManha + TotalTarde + TotalNoite - HorasTrabalhoDia;

            }
        }
    }
}
