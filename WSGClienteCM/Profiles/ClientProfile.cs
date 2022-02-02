using AutoMapper;
using WSGClienteCM.Models;

namespace WSGClienteCM.Profiles

{
    public class ClientProfile :Profile
    {
        public ClientProfile() {
            CreateMap<ClientViewModel, ClientBindingModel>();
            //.ForMember(dest => dest., opts => opts.MapFrom(src => src.NCODE_CONF))
            CreateMap<CiiuViewModel, CiiuBindingModel>(); 
            CreateMap<AddressViewModel, AddressBindingModel>()
                .ForMember(dest=>dest.P_ADDRESSTYPE,opts => opts.MapFrom(src=>src.P_STI_DIRE));
            CreateMap<PhoneViewModel, PhoneBindingModel>();
            CreateMap<EmailViewModel, EmailBindingModel>();
        }
    }
}
