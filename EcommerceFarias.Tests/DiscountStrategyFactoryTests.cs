using EcommerceFarias.Model.Discount;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceFarias.Tests
{
    [TestClass]
    public class DiscountStrategyFactoryTests
    {
        private ServiceProvider _serviceProvider;
        private IDiscountStrategyFactory _factory;

        [TestInitialize]
        public void Setup()
        {
            var services = new ServiceCollection();
            services.AddScoped<ElectronicsDiscount>();
            services.AddScoped<ClothingDiscount>();
            services.AddScoped<NoDiscount>();
            services.AddScoped<IDiscountStrategyFactory, DiscountStrategyFactory>();

            _serviceProvider = services.BuildServiceProvider();
            _factory = _serviceProvider.GetRequiredService<IDiscountStrategyFactory>();
        }

        [TestMethod]
        public void GetStrategy_ShouldReturnElectronicsDiscount_WhenCategoryIsElectronics()
        {
            var strategy = _factory.GetStrategy("electronics");
            Assert.IsInstanceOfType(strategy, typeof(ElectronicsDiscount));
        }

        [TestMethod]
        public void GetStrategy_ShouldReturnClothingDiscount_WhenCategoryIsClothing()
        {
            var strategy = _factory.GetStrategy("clothing");
            Assert.IsInstanceOfType(strategy, typeof(ClothingDiscount));
        }

        [TestMethod]
        public void GetStrategy_ShouldReturnNoDiscount_WhenCategoryIsUnknown()
        {
            var strategy = _factory.GetStrategy("unknown");
            Assert.IsInstanceOfType(strategy, typeof(NoDiscount));
        }
    }
}
