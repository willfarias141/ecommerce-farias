using EcommerceFarias.Controllers;
using EcommerceFarias.Model;
using EcommerceFarias.Model.Discount;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceFarias.Tests
{
    [TestClass]
    public class ProductControllerTests
    {
        private Mock<IProductRepository> _mockRepository;
        private Mock<IDiscountService> _mockDiscountService;
        private ProductController _controller;
        private List<Product> _allProducts;

        [TestInitialize]
        public void Setup()
        {
            _mockRepository = new Mock<IProductRepository>();
            _mockDiscountService = new Mock<IDiscountService>();
            _controller = new ProductController(_mockRepository.Object, _mockDiscountService.Object);

            List<ProductCategory> electronics = new List<ProductCategory>()
            {
                new ProductCategory { Category = new Category("electronics", null) }
            };

            List<ProductCategory> kitchen = new List<ProductCategory>()
            {
                new ProductCategory { Category = new Category("kitchen", null) }
            };

            _allProducts = new List<Product>();

            var product = new Product("Product 1", "description", 5116820331341, (decimal)149.99, 200, null);
            _allProducts.Add(product);

            product = new Product("Product 2", "description", 5116820331342, (decimal)149.99, 200, electronics);
            _allProducts.Add(product);

            product = new Product("Product 3", "description", 5116820331343, (decimal)149.99, 200, electronics);
            product.StatusId = ProductStatusEnum.ATIVO;
            _allProducts.Add(product);

            product = new Product("Product 4", "description", 5116820331344, (decimal)149.99, 200, electronics);
            product.StatusId = ProductStatusEnum.ATIVO;
            _allProducts.Add(product);

            product = new Product("Product 5", "description", 5116820331345, (decimal)149.99, 200, kitchen);
            product.StatusId = ProductStatusEnum.INATIVO;
            _allProducts.Add(product);
        }

        [TestMethod]
        public void GetAll_ShouldReturnAllProducts_WhenStatusIsNull()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetAll(null, null, null, null)).Returns(_allProducts);

            // Act
            var result = _controller.GetAll(null, null, null, null) as OkObjectResult;
            var products = result?.Value as List<ProductViewModel>;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual(5, products?.Count ?? 0);
        }

        [TestMethod]
        public void GetAll_ShouldReturnOnlyActiveProducts_WhenStatusIsActive()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetAll(null, ProductStatusEnum.ATIVO, null, null)).Returns(_allProducts.Where(p => p.StatusId == ProductStatusEnum.ATIVO).ToList());

            // Act
            var result = _controller.GetAll("ativo", null, null, null) as OkObjectResult;
            var products = result?.Value as List<ProductViewModel>;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual(2, products?.Count ?? 0);
            Assert.IsTrue(products?.TrueForAll(p => p.Status == "ATIVO"));
        }
    }
}
