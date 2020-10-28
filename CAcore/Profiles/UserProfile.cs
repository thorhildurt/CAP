using AutoMapper;
using CAcore.Models;
using CAcore.Dtos;

namespace CAcore.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserReadDto>();
        }
    }
}