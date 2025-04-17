namespace EcommerceFarias.Model.Discount
{
    public interface IDiscountStrategyFactory
    {
        IDiscountStrategy GetStrategy(string category);
    }
}
