namespace EcommerceFarias.Model.Discount
{
    public class ClothingDiscount : IDiscountStrategy
    {
        public decimal ApplyDiscount(decimal price) => price * 0.20m;  // 20% desconto
    }
}
