using ControlFinWeb.Dominio.ObjetoValor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ControlFinWeb.App.ViewModels
{
    public class FaturaCartaoCreditoVM
    {
        public FaturaCartaoCreditoVM()
        {
            SituacaoFatura = SituacaoFatura.Aberta;
        }
        public Int64 Id { get; set; }

        [DisplayName("Observação")]
        [StringLength(100, ErrorMessage = "Limite máximo de 500 caracteres.")]
        [Required(ErrorMessage = "Campo Obrigatório")]
        public String Nome { get; set; }

        [DisplayName("Mês e Ano de Referência")]
        [Range(typeof(DateTime), "01/01/1980", "31/12/5000", ErrorMessage = "Data Inválida!")]
        [Required(ErrorMessage = "Campo Obrigatório")]
        public DateTime MesAnoReferencia { get; set; }

        public SituacaoFatura SituacaoFatura { get; set; }

        #region Campos vinculados a outras classes
        [DisplayName("Cartão")]
        [Range(1, Int64.MaxValue, ErrorMessage = "Informe um Cartão")]
        public Int64 CartaoId { get; set; }
        public CartaoVM CartaoVM { get; set; }
        #endregion

        public IList<FaturaCartaoCreditoItensVM> FaturaItensVM { get; set; }

        public string DescricaoCompleta
        {
            get
            {
                if (CartaoVM != null)
                    return string.Format("{0} - {1:MMyyyy}", CartaoVM.Nome, MesAnoReferencia);
                else
                    return string.Empty;
            }
        }

        public Decimal ValorFatura
        {
            get
            {
                
                return (FaturaItensVM != null) ? FaturaItensVM.Select(x => x.Valor).Sum() : 0;
            }
        }
    }
}
