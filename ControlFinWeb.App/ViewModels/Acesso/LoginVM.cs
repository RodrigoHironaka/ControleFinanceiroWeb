using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ControlFinWeb.App.ViewModels.Acesso
{
    public class LoginVM
    {
        [Required(ErrorMessage = "Campo Obrigatório!")]
        [DataType(DataType.EmailAddress)]
        public String Email { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório!")]
        [DataType(DataType.Password)]
        public String Senha { get; set; }

        public String ReturnUrl { get; set; }
    }
}
