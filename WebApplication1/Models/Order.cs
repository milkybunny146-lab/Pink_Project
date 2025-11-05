using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    [Table("pinkshop_orders")]
    public class Order
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("order_number")]
        public string OrderNumber { get; set; } = string.Empty;

        [Column("member_id")]
        public int? MemberId { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("customer_name")]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        [Column("customer_phone")]
        public string CustomerPhone { get; set; } = string.Empty;

        [MaxLength(100)]
        [Column("customer_email")]
        public string? CustomerEmail { get; set; }

        [MaxLength(500)]
        [Column("customer_address")]
        public string? CustomerAddress { get; set; }

        [MaxLength(20)]
        [Column("delivery_type")]
        public string? DeliveryType { get; set; }

        [MaxLength(1000)]
        [Column("notes")]
        public string? Notes { get; set; }

        [Required]
        [Column("total_amount", TypeName = "decimal(10, 2)")]
        public decimal TotalAmount { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("order_status")]
        public string OrderStatus { get; set; } = "待處理";

        [Required]
        [MaxLength(20)]
        [Column("payment_status")]
        public string PaymentStatus { get; set; } = "未付款";

        [MaxLength(50)]
        [Column("payment_method")]
        public string? PaymentMethod { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        // 導航屬性
        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
