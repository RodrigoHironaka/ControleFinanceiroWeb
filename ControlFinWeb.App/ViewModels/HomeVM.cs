namespace ControlFinWeb.App.ViewModels
{
    public class HomeVM
    {
        public decimal TotalPagar { get; set; }
        public decimal TotalReceber { get; set; }
        public decimal TotalAtrasadas { get; set; }
        public decimal TotalBalanco { get; set; }


        #region Graficos
        public string Descricao { get; set; }
        public decimal Valor { get; set; }
        public string Cor { get; set; }
        #endregion

    }
}
