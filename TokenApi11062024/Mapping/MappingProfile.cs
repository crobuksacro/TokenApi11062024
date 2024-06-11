using AutoMapper;
using TokenApi11062024.Model.Dbo;
using TokenApi11062024.Model.ViewModel;

namespace TokenApi11062024.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ApplicationUser, ApplicationUserViewModel>();
        }
    }
}
