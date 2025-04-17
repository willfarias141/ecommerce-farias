namespace EcommerceFarias.Model.Discount
{
    public class NoDiscount : IDiscountStrategy
    {
        public decimal ApplyDiscount(decimal price) => price * 0;  // Sem desconto
    }
}
