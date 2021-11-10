using ChocAn.DataCenterConsole.Models;
using ChocAn.MemberRepository;
using ChocAn.ProviderRepository;
using ChocAn.ProviderServiceRepository;
using AutoMapper;

namespace ChocAn.DataCenterConsole.Infrastructure
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Member, MemberEditViewModel>();
            CreateMap<Member, MemberDetailsViewModel>();
            CreateMap<MemberCreateViewModel, Member>();
            CreateMap<MemberEditViewModel, Member>();

            CreateMap<Provider, ProviderEditViewModel>();
            CreateMap<Provider, ProviderDetailsViewModel>();
            CreateMap<ProviderCreateViewModel, Provider>();
            CreateMap<ProviderEditViewModel, Provider>();

            CreateMap<ProviderService, ProviderServiceEditViewModel>();
            CreateMap<ProviderService, ProviderServiceDetailsViewModel>();
            CreateMap<ProviderServiceCreateViewModel, ProviderService>();
            CreateMap<ProviderServiceEditViewModel, ProviderService>();
        }
    }
}
