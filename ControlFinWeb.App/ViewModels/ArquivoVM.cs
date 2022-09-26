using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ControlFinWeb.App.ViewModels
{
    public class ArquivoVM
    {
        public Int64 Id { get; set; }

        [DisplayName("Descrição")]
        [StringLength(100, ErrorMessage = "Limite máximo de 100 caracteres.")]
        [Required(ErrorMessage = "Campo Obrigatório")]
        public String Nome { get; set; }

        [DisplayName("Caminho")]
        [StringLength(200, ErrorMessage = "Limite máximo de 200 caracteres.")]
        [Required(ErrorMessage = "Campo Obrigatório")]
        public virtual String Caminho { get; set; }

        [DisplayName("Extensão")]
        [StringLength(10, ErrorMessage = "Limite máximo de 10 caracteres.")]
        [Required(ErrorMessage = "Campo Obrigatório")]
        public virtual String Extensao { get; set; }

        public Int64 ContaId { get; set; }
        public virtual ContaVM ContaVM { get; set; }

        public Int64 FaturaId { get; set; }
        public virtual FaturaVM FaturaVM { get; set; }
        
    }
}
