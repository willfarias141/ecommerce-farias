namespace EcommerceFarias.Model
{
    public interface ICategoryRepository
    {
        Category Create(Category category);

        List<Category> GetAll();

        Category? GetById(int id);

        List<Category> GetChildCategories(int id);

        void Update(Category category);

        void Delete(int id);
    }
}
