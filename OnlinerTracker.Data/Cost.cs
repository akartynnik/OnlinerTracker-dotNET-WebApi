using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlinerTracker.Data
{
    public class Cost
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public Guid ProductId { get; set; }

        [Required]
        public decimal Value { get; set; }

        [Required]
        public DateTime CratedAt { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }
}
