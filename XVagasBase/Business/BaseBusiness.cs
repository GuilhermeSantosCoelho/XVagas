using AutoMapper;
using Base.DAO;
using Base.Entities;
using Base.ViewObject;
using Microsoft.EntityFrameworkCore;

namespace Base.Business
{
    public class BaseBusiness<DAO, TContext, VO, Entity, AutoMapProfile>
        where TContext : DbContext, new()
        where VO : BaseVO, new()
        where Entity : BaseEntity, new()
        where AutoMapProfile : Profile, new()
        where DAO : BaseDao<DAO, TContext, VO, Entity, AutoMapProfile>, new()
    {
        private DAO _daoInstance;
        public DAO daoInstance
        {
            get
            {
                if (_daoInstance == null)
                {
                    _daoInstance = new DAO();
                }

                return _daoInstance;
            }
        }


        public virtual VO GetOne(long id)
        {
            return daoInstance.GetOne(id);
        }

        public virtual BaseResponseVO<VO> GetAllFilter(int page, int listSize, VO filter)
        {
            return daoInstance.GetAllFilter(page, listSize, filter);
        }

        public virtual VO Save(VO vo)
        {
            return daoInstance.Save(vo);
        }

        public virtual VO Update(VO vo, long id)
        {

            if (id > 0 && vo.id <= 0)
            {
                vo.id = id;
            }

            return daoInstance.Update(vo);
        }

        public virtual VO Patch(VO vo, long id)
        {

            if (id > 0 && vo.id <= 0)
            {
                vo.id = id;
            }

            return daoInstance.Patch(vo);
        }

        public virtual void Delete(long id)
        {
            daoInstance.Delete(id);
        }

    }
}
