using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceFarias.Model
{
    [Table("product")]
    public class Product
    {
        [Key]
        [Column("id")]
        public int Id { get; private set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("ean")]
        public long Ean { get; set; }

        [Column("statusid")]
        public ProductStatusEnum StatusId { get; set; }

        [Column("imagemurl")]
        public string? ImagemUrl { get; set; }

        [Column("price")]
        public decimal? Price { get; set; }

        [Column("stock")]
        public int? Stock { get; set; }

        [Column("datecreated")]
        public DateTime? DateCreated { get; private set; }

        [Column("dateupdate")]
        public DateTime? DateUpdate { get; private set; }

        public ICollection<ProductCategory> ProductCategories { get; set; }

        public Product() { }

        public Product(string name, string? description, long ean, decimal price, int stock, List<ProductCategory> categories)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description;
            Ean = ean;
            Price = price == 0 ? null : price;
            Stock = stock == 0 ? null : stock;
            StatusId = ProductStatusEnum.PENDENTE;
            ProductCategories = categories;
        }
    }
}
