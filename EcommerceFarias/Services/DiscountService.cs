using EcommerceFarias.Model;
using EcommerceFarias.Model.Discount;

namespace EcommerceFarias.Services
{
    public class DiscountService : IDiscountService
    {
        private readonly IDiscountStrategyFactory _factory;

        public DiscountService(IDiscountStrategyFactory factory)
        {
            _factory = factory;
        }

        public decimal GetDiscountedPrice(Category category, decimal price)
        {
            var strategy = _factory.GetStrategy(category.Name);
            var discountedPrice = strategy.ApplyDiscount(price);
            return discountedPrice;
        }

        public decimal GetDiscountedPrice(List<Category> categories, decimal price)
        {
            decimal totalDiscount = 0;

            foreach (var category in categories)
            {
                totalDiscount += GetDiscountedPrice(category, price);
            }

            return totalDiscount;
        }
    }
}
