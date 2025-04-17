using System.ComponentModel.DataAnnotations;

namespace EcommerceFarias.Model
{
    public class ProductRequest
    {
        public int Id { get; set; }

        /// <summary>
        /// Nome.
        /// </summary>
        [Required]
        public string? Name { get; set; }

        /// <summary>
        /// Descrição.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// EAN.
        /// </summary>
        [Required]
        public long Ean { get; set; }

        public decimal Price { get; set; }

        public int Stock { get; set; }

        public string? Status { get; set; }

        public List<int>? Categories { get; set; }
    }
}
