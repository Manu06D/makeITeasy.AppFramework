

using AutoMapper;

namespace makeITeasy.AppFramework.Core.Interfaces
{
    public interface IMapFrom<T>
    {
        void Mapping(Profile profile) => profile?.CreateMap(typeof(T), GetType()).ReverseMap();
    }
}
