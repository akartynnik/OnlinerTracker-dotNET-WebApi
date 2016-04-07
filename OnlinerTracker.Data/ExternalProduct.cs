using System;
using System.Collections.Generic;

namespace OnlinerTracker.Data
{
    public class ExternalProduct
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        
        public string OnlinerId { get; set; }

        public string ImageUrl { get; set; }
        
        public string Name { get; set; }

        public string Description { get; set; }

        public bool Tracking { get; set; }

        public bool Compared { get; set; }


        public IEnumerable<Cost> Costs { get; set; }

        public decimal CurrentCost { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
