using System.ComponentModel.DataAnnotations.Schema;

namespace makeITeasy.AppFramework.Models
{
    public interface IBaseEntity
    {
        [NotMapped]
        object DatabaseID { get; }
    }
}