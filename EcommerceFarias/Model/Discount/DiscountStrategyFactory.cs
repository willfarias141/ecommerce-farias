namespace EcommerceFarias.Model.Discount
{
    public class DiscountStrategyFactory : IDiscountStrategyFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public DiscountStrategyFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IDiscountStrategy GetStrategy(string category)
        {
            return category.ToLower() switch
            {
                "electronics" => _serviceProvider.GetRequiredService<ElectronicsDiscount>(),
                "clothing" => _serviceProvider.GetRequiredService<ClothingDiscount>(),
                _ => _serviceProvider.GetRequiredService<NoDiscount>()
            };
        }
    }
}
