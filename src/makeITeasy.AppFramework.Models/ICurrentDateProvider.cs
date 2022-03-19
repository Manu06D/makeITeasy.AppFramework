using System;

namespace makeITeasy.AppFramework.Models
{
    public interface ICurrentDateProvider
    {
        DateTime Now { get; }
    }
}
