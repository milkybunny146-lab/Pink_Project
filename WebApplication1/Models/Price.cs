using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    [Table("pinkshop_prices")]
    public class Price
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("product_id")]
        public int ProductId { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("size_name")]
        public string SizeName { get; set; } = string.Empty;

        [Required]
        [Column("price", TypeName = "decimal(10, 2)")]
        public decimal PriceAmount { get; set; }

        [Column("display_order")]
        public int DisplayOrder { get; set; } = 0;

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        // 導航屬性
        [ForeignKey("ProductId")]
        public Product Product { get; set; } = null!;
    }
}
