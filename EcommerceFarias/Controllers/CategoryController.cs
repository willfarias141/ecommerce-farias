using EcommerceFarias.Model;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceFarias.Controllers
{
    [ApiController]
    [Route("api/v1/category")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        /// <summary>
        /// Cadastrar uma categoria.
        /// </summary>
        /// <param name="category">Informações da categoria.</param>
        /// <remarks>
        /// Exemplo:
        ///
        ///     POST /api/v1/category
        ///     {
        ///        "name": "electronics",
        ///        "parentcategoryid": 1
        ///     }
        ///
        /// </remarks>
        [HttpPost()]
        public IActionResult Create(CategoryViewModel category)
        {
            if (category == null)
                return BadRequest();

            if (category.ParentCategoryId != null && category.ParentCategoryId > 0)
            {
                var parentCategory = _categoryRepository.GetById((int)category.ParentCategoryId);

                if (parentCategory == null)
                    return NotFound(new ApiResponse<CategoryViewModel>
                    {
                        Success = false,
                        Message = $"Não foi possível criar a categoria, pois ParentCategoryId {category.ParentCategoryId} não existe.",
                        Data = category
                    });
            }

            var categoryDb = new Category(category.Name ?? string.Empty, category.ParentCategoryId);

            category.Id = _categoryRepository.Create(categoryDb).Id;
            return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
        }

        /// <summary>
        /// Consultar lista de categorias.
        /// </summary>
        [HttpGet]
        public IActionResult GetAll()
        {
            var categories = _categoryRepository.GetAll();

            var categoriesViewModel = categories?.Select(s => new CategoryViewModel
            {
                Id = s.Id,
                Name = s.Name ?? string.Empty,
                ParentCategoryId = s.ParentCategoryId
            }).ToList();

            return Ok(categoriesViewModel);
        }

        /// <summary>
        /// Consultar uma categoria pelo Id.
        /// </summary>
        /// <param name="id">Id da categoria.</param>
        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var category = _categoryRepository.GetById(id);

            if (category == null)
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message = $"Categoria com ID {id} não encontrada."
                });

            var categoryViewModel = new CategoryViewModel
            {
                Id = category.Id,
                Name = category.Name ?? string.Empty,
                ParentCategoryId = category.ParentCategoryId
            };

            return Ok(categoryViewModel);
        }

        /// <summary>
        /// Excluir uma categoria.
        /// </summary>
        /// <param name="id">Id da Categoria.</param>
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var categoryDb = _categoryRepository.GetById(id);

            if (categoryDb == null)
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message = $"Categoria com ID {id} não encontrada."
                });

            var childCategories = _categoryRepository.GetChildCategories(id);

            if (childCategories != null && childCategories.Count > 0)
                return Conflict(new ApiResponse<List<Category>>
                {
                    Success = false,
                    Message = "Não é possível excluir a categoria, pois existem subcategorias vinculadas. Para prosseguir com a operação é preciso excluir as subcategorias ou desfazer o vínculo delas.",
                    Data = childCategories,
                    Errors = new List<string> { "Categoria possui dependências." }
                });

            _categoryRepository.Delete(id);
            return NoContent();
        }
    }
}
