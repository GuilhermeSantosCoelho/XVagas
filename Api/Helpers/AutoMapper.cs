using System.Collections.Generic;
using AutoMapper;
using XVagas.VO;
using XVagas.Entity;

namespace XVagas.Helpers
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserVO>()
                .ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}