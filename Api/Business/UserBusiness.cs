using AutoMapper;
using XVagas.Entity;
using XVagas.VO;
using XVagas.Helpers;
using Base.Business;
using XVagas.DAO;

namespace XVagas.Business
{
    public class UserBusiness: BaseBusiness<UserDAO, DatabaseContext, UserVO, User, AutoMapperProfile>
    {

    }
}