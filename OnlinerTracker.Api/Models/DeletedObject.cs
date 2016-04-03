using System;
using System.ComponentModel.DataAnnotations;

namespace OnlinerTracker.Api.Models
{
    public class DeletedObject
    {
        [Required]
        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}