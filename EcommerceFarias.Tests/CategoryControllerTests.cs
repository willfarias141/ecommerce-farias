using EcommerceFarias.Controllers;
using EcommerceFarias.Model;
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
    public class CategoryControllerTests
    {
        private Mock<ICategoryRepository> _mockRepository;
        private CategoryController _controller;
        private CategoryViewModel _categoryViewModel;
        private CategoryViewModel _categoryWithParentCategoryIdViewModel;
        private List<CategoryViewModel> _categoriesViewModel;
        private Category _category;
        private Category _categoryWithParentCategoryId;
        private List<Category> _categories;

        [TestInitialize]
        public void Setup()
        {
            _mockRepository = new Mock<ICategoryRepository>();
            _controller = new CategoryController(_mockRepository.Object);
            _category = new Category("Eletrônicos", null);
            _categoryWithParentCategoryId = new Category("TV", 2);
            _categories = new List<Category>();
            _categories.Add(_categoryWithParentCategoryId);

            _categoryViewModel = new CategoryViewModel() { Name = "Eletrônicos" };
            _categoryWithParentCategoryIdViewModel = new CategoryViewModel() { Name = "TV", ParentCategoryId = 2 };
            _categoriesViewModel = new List<CategoryViewModel>();
            _categoriesViewModel.Add(_categoryWithParentCategoryIdViewModel);
        }

        [TestMethod]
        public void CreateCategory_ReturnCreatedAtAction_WhenSavedSuccessfully()
        {
            // Arrange
            _mockRepository.Setup(r => r.Create(It.IsAny<Category>())).Returns(_category);

            // Act
            var resultado = _controller.Create(_categoryViewModel);

            // Assert
            Assert.IsInstanceOfType(resultado, typeof(CreatedAtActionResult));
        }

        [TestMethod]
        public void CreateCategory_ReturnBadRequest_WhenBodyIsNull()
        {
            // Arrange
            CategoryViewModel categoryViewModel = null;

            // Act
            var resultado = _controller.Create(categoryViewModel);

            // Assert
            Assert.IsInstanceOfType(resultado, typeof(BadRequestResult));
        }

        [TestMethod]
        public void CreateCategory_ReturnNotFound_WhenParentCategoryIdDoesNotExist()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetById(2)).Returns((Category)null);

            // Act
            var resultado = _controller.Create(_categoryWithParentCategoryIdViewModel);

            // Assert
            Assert.IsInstanceOfType(resultado, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public void DeleteCategory_ReturnConflict_WhenCategoryHasDependencies()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetById(1)).Returns(_category);
            _mockRepository.Setup(r => r.GetChildCategories(1)).Returns(_categories);

            // Act
            var resultado = _controller.Delete(1);

            // Assert
            Assert.IsInstanceOfType(resultado, typeof(ConflictObjectResult));
        }

        [TestMethod]
        public void DeleteCategory_ReturnNoContent_Successfully()
        {
            // Arrange
            var categories = new List<Category>();
            _mockRepository.Setup(r => r.GetById(1)).Returns(_category);
            _mockRepository.Setup(r => r.GetChildCategories(1)).Returns(categories);

            // Act
            var resultado = _controller.Delete(1);

            // Assert
            Assert.IsInstanceOfType(resultado, typeof(NoContentResult));
        }
    }
}
