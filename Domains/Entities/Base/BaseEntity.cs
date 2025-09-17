using System.ComponentModel.DataAnnotations;

namespace Domains.Entities.Base
{
    //Base class for entities common properties
    public class BaseEntity
    {
        [Key]
        public Guid Id { get; set; }

        public int CurrentState { get; set; } = 1;

        public Guid CreatedBy { get; set; }

        public DateTime CreatedDateUtc { get; set; } = DateTime.UtcNow;    

        public Guid? UpdatedBy { get; set; }

        public DateTime? UpdatedDateUtc { get; set; }
    }
}
