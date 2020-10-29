using AutoMapper;
using CAcore.Models;
using CAcore.Dtos;

namespace CAcore.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        { 
            // mapping User to UserReadDto
            CreateMap<User, UserReadDto>();
            // mapping UserCreateDto to User
            CreateMap<UserCreateDto, User>();
        }
    }
}