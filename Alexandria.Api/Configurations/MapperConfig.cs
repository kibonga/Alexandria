using Alexandria.Api.Data;
using Alexandria.Api.Models.Author;
using AutoMapper;

namespace Alexandria.Api.Configurations
{
    public class MapperConfig: Profile
    {
        public MapperConfig()
        {
            #region Author
            CreateMap<AuthorCreateDto, Author>().ReverseMap(); 
            CreateMap<AuthorUpdateDto, Author>().ReverseMap(); 
            CreateMap<AuthorReadOnlyDto, Author>().ReverseMap(); 
            #endregion
        }
    }
}
