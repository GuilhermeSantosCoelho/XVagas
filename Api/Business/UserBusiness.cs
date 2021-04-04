using AutoMapper;
using XVagas.Entity;
using XVagas.VO;
using XVagas.Helpers;
using Base.Business;
using XVagas.DAO;

namespace XVagas.Business
{
    public class UserBusiness : BaseBusiness<UserDAO, DatabaseContext, UserVO, User, AutoMapperProfile>
    {
        protected IMapper _mapper;

        public UserBusiness()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });
            _mapper = new Mapper(config);
        }


        public virtual User FromVOToEntity(UserVO vo, User entity = null)
        {
            return _mapper.Map<UserVO, User>(vo, entity);
        }

        public virtual UserVO FromEntityToVO(User entity)
        {
            return _mapper.Map<UserVO>(entity);
        }
    }
}