using ControlFinWeb.Dominio.ObjetoValor;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;


namespace ControlFinWeb.App.ViewModels
{
    public class PessoaVM
    {
        public PessoaVM()
        {
            PessoaRendasVM = new List<PessoaRendasVM>();
        }

        public Int64 Id { get; set; }

        [DisplayName("Descrição")]
        [StringLength(70, ErrorMessage = "Limite máximo de 70 caracteres.")]
        [Required(ErrorMessage = "Campo Obrigatório")]
        public String Nome { get; set; }

        [DisplayName("Situação")]
        public Situacao Situacao { get; set; }

        public IList<PessoaRendasVM> PessoaRendasVM { get; set; }

        public SelectList Rendas { get; set; }

        #region Campos Apenas para digitar valores e add a lista PessaRendasVM
        [DisplayName("R.Bruta")]
        public Decimal RendaBruta { get; set; }

        [DisplayName("R.Líquida")]
        public Decimal RendaLiquida { get; set; }

        [DisplayName("Renda")]
        public Int64 TipoRendaId { get; set; }
        public RendaVM TipoRendaVM { get; set; }
        #endregion

        #region Totalizadores
        [DisplayName("Total Renda Bruta")]
        public String TotalRendaBruta
        {
            get
            {
                return PessoaRendasVM.Select(x => x.RendaBruta).Sum().ToString("C2");
            }
        }

        [DisplayName("Total Renda Líquida")]
        public String TotalRendaLiquida
        {
            get
            {
                return PessoaRendasVM.Select(x => x.RendaLiquida).Sum().ToString("C2");
            }
        }
        #endregion


        public override string ToString()
        {
            return Nome;
        }
    }
}
