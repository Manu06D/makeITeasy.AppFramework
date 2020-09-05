using System;

namespace makeITeasy.AppFramework.Models
{
    public interface ITimeTrackingEntity
    {
        DateTime CreatedDate { get; set; }
        DateTime LastModifiedDate { get; set; }
    }
}