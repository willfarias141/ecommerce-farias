using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceFarias.Model
{
    [Table("productcategory")]
    public class ProductCategory
    {
        [Key]
        [Column("id")]
        public int Id { get; private set; }

        [Column("productid")]
        public int ProductId { get; set; }
        public Product Product { get; set; }

        [Column("categoryid")]
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
