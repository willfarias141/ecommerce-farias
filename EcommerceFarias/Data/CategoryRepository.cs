using EcommerceFarias.Model;
using Microsoft.EntityFrameworkCore;

namespace EcommerceFarias.Data
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public CategoryRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Category Create(Category category)
        {
            _dbContext.Categories.Add(category);
            _dbContext.SaveChanges();
            return category;
        }

        public void Delete(int id)
        {
            var category = GetById(id);

            if (category == null)
                return;

            _dbContext.Categories.Remove(category);
            _dbContext.SaveChanges();
        }

        public List<Category> GetAll()
        {
            return _dbContext.Categories.ToList();
        }

        public Category? GetById(int id)
        {
            return _dbContext.Categories.FirstOrDefault(x => x.Id == id);
        }

        public List<Category> GetChildCategories(int id)
        {
            return _dbContext.Categories.Where(x => x.ParentCategoryId == id).ToList();
        }

        public void Update(Category category)
        {
            _dbContext.Entry(category).State = EntityState.Modified;
            _dbContext.SaveChanges();
        }
    }
}
