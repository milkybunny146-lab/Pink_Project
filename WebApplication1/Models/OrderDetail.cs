using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    [Table("pinkshop_order_details")]
    public class OrderDetail
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("order_id")]
        public int OrderId { get; set; }

        [Column("product_id")]
        public int? ProductId { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("product_name")]
        public string ProductName { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        [Column("size_name")]
        public string SizeName { get; set; } = string.Empty;

        [Required]
        [Column("quantity")]
        public int Quantity { get; set; }

        [Required]
        [Column("unit_price", TypeName = "decimal(10, 2)")]
        public decimal UnitPrice { get; set; }

        [Required]
        [Column("subtotal", TypeName = "decimal(10, 2)")]
        public decimal Subtotal { get; set; }

        [MaxLength(20)]
        [Column("sweetness_level")]
        public string? SweetnessLevel { get; set; }

        [MaxLength(20)]
        [Column("ice_level")]
        public string? IceLevel { get; set; }

        [MaxLength(500)]
        [Column("notes")]
        public string? Notes { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // 導航屬性
        [ForeignKey("OrderId")]
        public Order Order { get; set; } = null!;
    }
}
