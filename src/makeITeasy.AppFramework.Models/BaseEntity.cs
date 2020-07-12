using System.ComponentModel.DataAnnotations.Schema;

namespace makeITeasy.AppFramework.Models
{
    public abstract class BaseEntity : IBaseEntity
    {
        [NotMapped]
        public abstract object DatabaseID { get; set; }
    }
}
