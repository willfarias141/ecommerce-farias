namespace EcommerceFarias.Model.Discount
{
    public class ElectronicsDiscount : IDiscountStrategy
    {
        public decimal ApplyDiscount(decimal price) => price * 0.10m;  // 10% desconto
    }
}
