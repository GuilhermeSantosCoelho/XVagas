using AutoMapper;
using XVagas.Entity;
using XVagas.VO;
using XVagas.Helpers;
using Base.Business;
using XVagas.DAO;

namespace XVagas.Business
{
    public class FilePDFBusiness : BaseBusiness<FilePDFDAO, DatabaseContext, FilePDFVO, FilePDF, AutoMapperProfile>
    {
        
    }
}