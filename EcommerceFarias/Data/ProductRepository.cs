using EcommerceFarias.Model;
using Microsoft.EntityFrameworkCore;

namespace EcommerceFarias.Data
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ProductRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Product Create(Product produto)
        {
            _dbContext.Products.Add(produto);
            _dbContext.SaveChanges();
            return produto;
        }

        public Product? GetByEan(long ean)
        {
            return _dbContext.Products.Include(p => p.ProductCategories).ThenInclude(pc => pc.Category).FirstOrDefault(x => x.Ean == ean);
        }

        public List<Product> GetAll(int? categoryId = null, ProductStatusEnum? status = null, decimal? minPrice = null, decimal? maxPrice = null)
        {
            var query = _dbContext.Products.Include(p => p.ProductCategories).ThenInclude(pc => pc.Category).AsQueryable();

            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice.Value);

            if (categoryId.HasValue)
                query = query.Where(p => p.ProductCategories.Any(pc => pc.CategoryId == categoryId.Value));

            if (status != null)
                query = query.Where(p => p.StatusId == status);

            return query.ToList();
        }

        public void Update(Product product)
        {
            _dbContext.Entry(product).State = EntityState.Modified;
            _dbContext.SaveChanges();
        }

        public void DeleteByEan(long ean)
        {
            var product = GetByEan(ean);

            if (product == null)
                return;

            _dbContext.Products.Remove(product);
            _dbContext.SaveChanges();
        }
    }
}
