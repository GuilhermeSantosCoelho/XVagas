using System;
using System.IO;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using XVagas.Business;
using XVagas.Entity;
using XVagas.VO;
using Microsoft.EntityFrameworkCore;
using Base.Controller;
using XVagas.DAO;
using XVagas.Helpers;

namespace XVagas.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : BaseController<UserBusiness, UserDAO, DatabaseContext, UserVO, User, AutoMapperProfile>
    {
        
    }
}