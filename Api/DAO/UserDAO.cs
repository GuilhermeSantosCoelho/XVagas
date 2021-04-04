using Base.DAO;
using XVagas.Entity;
using XVagas.VO;
using XVagas.Helpers;

namespace XVagas.DAO
{
	public class UserDAO : BaseDao<UserDAO, DatabaseContext, UserVO, User, AutoMapperProfile>
    {
	
	}
}