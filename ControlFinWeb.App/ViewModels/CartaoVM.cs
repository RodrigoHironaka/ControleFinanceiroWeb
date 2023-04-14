using ControlFinWeb.App.Utilitarios;
using ControlFinWeb.Dominio.ObjetoValor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ControlFinWeb.App.ViewModels
{
    public class CartaoVM
    {
        public CartaoVM()
        {
            if (VencimentoCartao == DateTime.MinValue)
                VencimentoCartao = DateTime.Now;
        }
        public Int64 Id { get; set; }

        [DisplayName("Nome")]
        [StringLength(70, ErrorMessage = "Limite máximo de 70 caracteres.")]
        [Required(ErrorMessage = "Campo Obrigatório")]
        public String Nome { get; set; }

        [DisplayName("Limite da Fatura")]
        [Range(1, Int64.MaxValue, ErrorMessage = "Valor precisa ser maior que zero!")]
        public Decimal LimiteFatura { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        [DisplayName("Qual dia de Vencimento?")]
        public Int32 DiaVencimento { get; set; }

        [DisplayName("Vencimento do Cartão")]
        [Required(ErrorMessage = "Campo Obrigatório")]
        public DateTime? VencimentoCartao { get; set; }

        [DisplayName("Situação")]
        public Situacao Situacao { get; set; }

        [Range(1, Int64.MaxValue, ErrorMessage = "Informe um Banco")]
        [DisplayName("Banco")]
        public Int64 BancoId { get; set; }

        public BancoVM BancoVM { get; set; }
        public SelectList ComboBancos
        {
            get
            {
                return PreencheCombo.Bancos();
            }
        }
    }
}
