using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlinerTracker.Data
{
    public class Product
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        [Index("IX_OnlinerIdAndUserId", 0, IsUnique = true)]
        public Guid UserId { get; set; }

        [Required]
        [Index("IX_OnlinerIdAndUserId", 1, IsUnique = true)]
        [MaxLength(50)]
        public string OnlinerId { get; set; }

        [MaxLength(200)]
        public string ImageUrl { get; set; }

        [Required]
        [MaxLength(500)]
        public string Name { get; set; }
    }
}
