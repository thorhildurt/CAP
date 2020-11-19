using AutoMapper;
using CAcore.Dtos;
using CAcore.Models;

namespace CAcore.Profiles {
    public class UserCertificateProfile : Profile {
        public UserCertificateProfile () {
            CreateMap <UserCertificate, UserCertificateCreateDto>(); 
            CreateMap <UserCertificateCreateDto, UserCertificate>(); 
            CreateMap <UserCertificate, UserCertificateReadDto>(); 
        }
    }
}