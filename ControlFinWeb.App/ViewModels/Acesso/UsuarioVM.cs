using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ControlFinWeb.App.ViewModels.Acesso
{
    public class UsuarioVM
    {
        public Int64 ID { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório!")]
        [StringLength(70, ErrorMessage = "Número de caracteres ultrapassou o permitido.")]
        public String Nome { get; set; }

        [StringLength(30, ErrorMessage = "Número de caracteres ultrapassou o permitido.")]
        public String Email { get; set; }

        [DataType(DataType.Password)]
        public String Senha { get; set; }

        
        [Compare("Senha")]
        [DisplayName("Confirmar Senha")]
        [DataType(DataType.Password)]
        public String ConfirmaSenha { get; set; }
    }
}
