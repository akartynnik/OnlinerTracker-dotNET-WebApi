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
        [Index("IX_UserId")]
        public Guid UserId { get; set; }

        [Required]
        [Index("IX_OnlinerId", IsUnique = true)]
        [MaxLength(50)]
        public string OnlinerId { get; set; }

        [MaxLength(200)]
        public string ImageUrl { get; set; }

        [Required]
        [MaxLength(500)]
        public string Name { get; set; }
    }
}
