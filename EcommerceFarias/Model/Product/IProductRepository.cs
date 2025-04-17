namespace EcommerceFarias.Model
{
    public interface IProductRepository
    {
        Product Create(Product produto);

        Product? GetByEan(long ean);

        List<Product> GetAll(int? categoryId = null, ProductStatusEnum? status = null, decimal? minPrice = null, decimal? maxPrice = null);

        void Update(Product product);

        void DeleteByEan(long ean);
    }
}
