using System;

namespace makeITeasy.AppFramework.Models
{
    public interface ITimeTrackingEntity
    {
        DateTime? CreationDate { get; }
        DateTime? LastModificationDate { get; }
    }
}