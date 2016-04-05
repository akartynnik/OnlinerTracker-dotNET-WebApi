using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlinerTracker.Data
{
    public class JobLog
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Index("IX_JobTypeAndCheckedAt", 0, IsUnique = true)]
        public JobType Type { get; set; }

        [Index("IX_JobTypeAndCheckedAt", 1, IsUnique = true)]
        public DateTime CheckedAt { get; set; }

        public bool IsSuccessed { get; set; }


        public string Info { get; set; }
    }
}
