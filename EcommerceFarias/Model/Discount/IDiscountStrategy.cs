namespace EcommerceFarias.Model.Discount
{
    public interface IDiscountStrategy
    {
        decimal ApplyDiscount(decimal price);
    }
}
