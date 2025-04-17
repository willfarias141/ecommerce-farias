using Amazon.S3;
using Amazon.S3.Model;
using EcommerceFarias.Model;
using EcommerceFarias.Model.Discount;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace EcommerceFarias.Controllers
{
    [ApiController]
    [Route("api/v1/products")]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        private readonly IDiscountService _discountService;

        public ProductController(IProductRepository productRepository, IDiscountService discountService)
        {
            _productRepository = productRepository;
            _discountService = discountService;
        }

        /// <summary>
        /// Cadastrar um produto.
        /// </summary>
        /// <param name="product">Informações do produto (nome, descrição, ean, preço, estoque, categorias).</param>
        /// <remarks>
        /// Exemplo:
        ///
        ///     POST /api/v1/products
        ///     {
        ///        "name": "Nome do produto",
        ///        "description": "Descricao do produto",
        ///        "ean": 3846347129351,
        ///        "price": 3699.01,
        ///        "stock": 200,
        ///        "status": "pendente",
        ///        "categories": [
        ///             5
        ///        ]
        ///     }
        ///
        /// </remarks>
        [HttpPost()]
        public IActionResult Create(ProductRequest product)
        {
            Product productDb = new(product.Name ?? string.Empty, product.Description, product.Ean, product.Price, product.Stock
                , product.Categories?.Select(c => new ProductCategory { CategoryId = c }).ToList() ?? new List<ProductCategory>());

            product.Id = _productRepository.Create(productDb).Id;

            return CreatedAtAction(nameof(GetByEan), new { ean = product.Ean }, product);
        }

        /// <summary>
        /// Cadastrar uma imagem para o produto.
        /// </summary>
        [HttpPost("{ean:long}/imagem")]
        public async Task<IActionResult> UploadImagem(long ean, IFormFile arquivo, [FromServices] IAmazonS3 s3Client, [FromServices] IOptions<AwsSettings> awsOptions)
        {
            var product = _productRepository.GetByEan(ean);

            if (product == null)
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message = $"Produto EAN {ean} não encontrado."
                });

            if (arquivo == null || arquivo.Length == 0)
                return BadRequest("Arquivo inválido.");

            var aws = awsOptions.Value;
            var nomeArquivo = $"products/{Guid.NewGuid()}_{arquivo.FileName}";

            using var stream = arquivo.OpenReadStream();

            var uploadRequest = new PutObjectRequest
            {
                BucketName = aws.BucketName,
                Key = nomeArquivo,
                InputStream = stream,
                ContentType = arquivo.ContentType
            };

            var response = await s3Client.PutObjectAsync(uploadRequest);
            var publicUrl = $"https://{aws.BucketName}.s3.{aws.Region}.amazonaws.com/{nomeArquivo}";
            product.ImagemUrl = publicUrl;

            _productRepository.Update(product);

            return Ok(new { imagemUrl = publicUrl });
        }



        /// <summary>
        /// Editar um produto.
        /// </summary>
        /// <param name="ean">Ean.</param>
        /// <param name="productRequest">Informações do produto que podem ser editadas: nome, descrição, ean, status, preço e estoque.</param>
        [HttpPut("{ean:long}")]
        public IActionResult Edit(long ean, ProductRequest productRequest)
        {
            if (productRequest == null || ean == 0)
                return BadRequest();

            var productDb = _productRepository.GetByEan(ean);

            if (productDb == null || productDb.Ean == 0)
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message = $"Produto EAN {ean} não encontrado."
                });

            productDb.Name = productRequest.Name ?? string.Empty;
            productDb.Description = productRequest.Description;
            productDb.Ean = productRequest.Ean;
            productDb.Price = productRequest.Price;
            productDb.Stock = productRequest.Stock;

            if (!string.IsNullOrWhiteSpace(productRequest.Status))
            {
                if (!Enum.TryParse<ProductStatusEnum>(productRequest.Status, true, out var statusEnum))
                    return BadRequest("Status inválido. Use 'ATIVO', 'INATIVO', 'PENDENTE' ou 'REPROVADO'.");

                productDb.StatusId = statusEnum;
            }

            _productRepository.Update(productDb);
            return NoContent();
        }

        /// <summary>
        /// Excluir um produto.
        /// </summary>
        /// <param name="ean">Ean.</param>
        [HttpDelete("{ean:long}")]
        public IActionResult Delete(long ean)
        {
            var productDb = _productRepository.GetByEan(ean);

            if (productDb == null)
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message = $"Produto EAN {ean} não encontrado."
                });

            _productRepository.DeleteByEan(ean);
            return NoContent();
        }

        /// <summary>
        /// Consultar lista de produtos.
        /// </summary>
        /// <remarks>
        /// Exemplo de uso:
        ///     
        ///     GET /api/v1/products → lista todos os produtos
        ///     
        ///     GET /api/v1/products?status=ativo → lista apenas os produtos ativos
        ///     
        ///     GET /api/v1/products?categoryId=7 → lista apenas os produtos da categoria 7
        ///     
        ///     GET /api/v1/products?minPrice=100 → lista apenas os produtos com preço acima de 100
        ///     
        ///     GET /api/v1/products?maxPrice=100 → lista apenas os produtos com preço abaixo de 100
        ///     
        /// </remarks>
        [HttpGet]
        public IActionResult GetAll([FromQuery] string? status, int? categoryId, decimal? minPrice, decimal? maxPrice)
        {
            ProductStatusEnum? statusValid = null;

            if (!string.IsNullOrEmpty(status))
            {
                if (!Enum.TryParse<ProductStatusEnum>(status, true, out var productStatusEnum))
                    return BadRequest("Status inválido. Use 'ATIVO', 'INATIVO', 'PENDENTE' ou 'REPROVADO'.");

                statusValid = productStatusEnum;
            }

            var products = _productRepository.GetAll(categoryId, statusValid, minPrice, maxPrice);
            var productsViewModel = products?.Select(p => new ProductViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Ean = p.Ean,
                Price = p.Price ?? 0,
                Discount = _discountService.GetDiscountedPrice(p.ProductCategories?.Select(pc => pc.Category).ToList(), p.Price ?? 0),
                Stock = p.Stock ?? 0,
                Status = p.StatusId.ToString(),
                Categories = p.ProductCategories?.Select(pc => new CategoryViewModel
                    {
                        Id = pc.Category.Id,
                        Name = pc.Category.Name
                    }).ToList()
            }).ToList();

            return Ok(productsViewModel);
        }

        /// <summary>
        /// Consultar um produto.
        /// </summary>
        /// <param name="ean">Ean.</param>
        [HttpGet("{ean:long}")]
        public IActionResult GetByEan(long ean)
        {
            var product = _productRepository.GetByEan(ean);

            if (product == null)
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message = $"Produto EAN {ean} não encontrado."
                });

            var productViewModel = new ProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Ean = product.Ean,
                Price = product.Price ?? 0,
                Discount = _discountService.GetDiscountedPrice(product.ProductCategories.Select(pc => pc.Category).ToList(), product.Price ?? 0),
                Stock = product.Stock ?? 0,
                Status = product.StatusId.ToString(),
                Categories = product.ProductCategories
                    .Select(pc => new CategoryViewModel
                    {
                        Id = pc.Category.Id,
                        Name = pc.Category.Name
                    }).ToList()
            };

            return Ok(productViewModel);
        }
    }
}
