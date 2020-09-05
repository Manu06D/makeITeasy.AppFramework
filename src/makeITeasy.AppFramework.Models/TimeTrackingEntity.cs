using System;

namespace makeITeasy.AppFramework.Models
{
    public class TimeTrackingEntity : ITimeTrackingEntity
    {
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
    }
}
