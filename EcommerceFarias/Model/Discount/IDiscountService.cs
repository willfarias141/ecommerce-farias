namespace EcommerceFarias.Model.Discount
{
    public interface IDiscountService
    {
        decimal GetDiscountedPrice(Category category, decimal price);

        decimal GetDiscountedPrice(List<Category> categories, decimal price);
    }
}
