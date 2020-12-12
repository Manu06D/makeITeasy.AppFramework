using System;

namespace makeITeasy.AppFramework.Models
{
    public interface ITimeTrackingEntity
    {
        DateTime? CreationDate { get; set; }
        DateTime? LastModificationDate { get; set; }
    }
}

