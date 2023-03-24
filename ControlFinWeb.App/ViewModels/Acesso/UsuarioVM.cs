using ControlFinWeb.Dominio.ObjetoValor;
using Microsoft.AspNetCore.Http;
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
        public Int64 Id { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório!")]
        [StringLength(70, ErrorMessage = "Número de caracteres ultrapassou o permitido.")]
        public String Nome { get; set; }

       
        [StringLength(30, ErrorMessage = "Número de caracteres ultrapassou o permitido.")]
        public String Email { get; set; }

        [DataType(DataType.Password)]
        public String Senha { get; set; }

        public SimNao Autorizado { get; set; }
        public String Imagem { get; set; }
        
        [DisplayName("Foto")]
        public IFormFile ImagemUpload { get; set; }

        [Compare("Senha", ErrorMessage = "Senhas diferentes!")]
        [DisplayName("Confirmar Senha")]
        [DataType(DataType.Password)]
        public String ConfirmaSenha { get; set; }

        [DisplayName("Situação")]
        public virtual Situacao Situacao { get; set; }

        [DisplayName("Tipo")]
        public virtual TipoUsuario TipoUsuario { get; set; }
    }
}
