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
        public String Nome { get; set; }

        [Range(1, Int64.MaxValue, ErrorMessage = "Informe uma Pessoa")]
        public Int64 PessoaId { get; set; }
        public PessoaVM PessoaVM { get; set; }

        [DisplayName("Manhã")]
        [Required(ErrorMessage = "Campo Obrigatório")]
        //[Range(typeof(TimeSpan), "00:01", "23:59", ErrorMessage = "Campo não pode ser Zero!")]
        public TimeSpan HorasTrabalhoManha { get; set; }

        [DisplayName("Tarde")]
        [Required(ErrorMessage = "Campo Obrigatório")]
        public TimeSpan HorasTrabalhoTarde { get; set; }

        [DisplayName("Noite")]
        [Required(ErrorMessage = "Campo Obrigatório")]
        public TimeSpan HorasTrabalhoNoite { get; set; }

        //[Required(ErrorMessage = "Campo Obrigatório")]
        [Range(typeof(DateTime), "01/01/1980", "31/12/3000", ErrorMessage = "Data Inválida!")]
        public DateTime Data { get; set; }

        [DisplayName("I.Manhã")]
        public TimeSpan HoraInicioManha { get; set; }

        [DisplayName("F.Manhã")]
        public TimeSpan HoraFinalManha { get; set; }

        [DisplayName("I.Tarde")]
        public TimeSpan HoraInicioTarde { get; set; }

        [DisplayName("F.Tarde")]
        public TimeSpan HoraFinalTarde { get; set; }

        [DisplayName("I.Noite")]
        public TimeSpan HoraInicioNoite { get; set; }

        [DisplayName("F.Noite")]
        public TimeSpan HoraFinalNoite { get; set; }

        
        [DisplayName("T.Manhã")]
        public TimeSpan TotalManha
        {
            get { return HoraFinalManha - HoraInicioManha; }
        }

        [DisplayName("T.Tarde")]
        public TimeSpan TotalTarde
        {
            get { return HoraFinalTarde - HoraInicioTarde; }
        }

        [DisplayName("Total Noite")]
        public TimeSpan TotalNoite 
        { 
            get { return HoraFinalNoite - HoraInicioNoite; }
        }

        [DisplayName("Total horas do dia")]
        public TimeSpan HorasTrabalhoDia
        {
            get { return HorasTrabalhoManha + HorasTrabalhoTarde + HorasTrabalhoNoite; }
        }

        [DisplayName("T.Dia")]
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
