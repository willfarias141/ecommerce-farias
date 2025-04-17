using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceFarias.Model
{
    [Table("category")]
    public class Category
    {
        [Key]
        [Column("id")]
        public int Id { get; private set; }

        [Column("name")]
        public string Name { get; private set; }

        [Column("parentcategoryid")]
        public int? ParentCategoryId { get; private set; }

        public ICollection<ProductCategory> ProductCategories { get; set; }

        public Category() { }

        public Category(string name, int? parentCategoryId)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            ParentCategoryId = parentCategoryId == 0 ? null : parentCategoryId;
        }
    }
}
