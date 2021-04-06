using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Base.Entities;
using Base.ViewObject;
using Base.Helpers;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using X.PagedList;
using System.Diagnostics;


namespace Base.DAO
{
    public abstract class BaseDao<DAO, TContext, VO, Entity, AutoMapProfile>
        where VO : BaseVO, new()
        where DAO : class, new()
        where Entity : BaseEntity, new()
        where AutoMapProfile: Profile, new()
        where TContext : DbContext, new()
    {

        protected IMapper _mapper;

        public BaseDao()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapProfile());
            });

            _mapper = new Mapper(config);
        }

        private static DAO Instance;
        public static DAO GetInstance()
        {
            if (Instance == null)
            {
                Instance = new DAO();
            }
            return Instance;
        }

        public virtual void SetIncludes(TContext ctx) { }

        public virtual VO GetOne(long id)
        {
            Entity entity;
            using (TContext context = new TContext())
            {
                entity = context.Set<Entity>().LoadRelated().Where(m => m.Id == id).FirstOrDefault();
                SetIncludes(context);
                return entity != null ? FromEntityToVO(entity) : null;
            }
        }

        public virtual BaseResponseVO<VO> GetAllFilter(int pageNumber, int pageSize, VO filter,
             params Expression<Func<Entity, object>>[] navigationProperties)
        {            
            
            using (TContext context = new TContext())
            {
                int totalPages = 1;
                int currentPage = pageNumber == 0 ? pageNumber = 1 : pageNumber;

                IQueryable<Entity> dbQuery = context.Set<Entity>();
               

                dbQuery = GetCustomWhere(dbQuery, filter);
                //dbQuery = dbQuery.DefaultWhere<Entity, VO>(filter);
                dbQuery = dbQuery.DefaulSort<Entity, VO>(filter);
                //dbQuery = dbQuery.OrderByDescending(m => m.Id);//.DefaultWhere<Entity, VO>(filter);
                int totalItems = dbQuery.Count();
                
                Console.WriteLine("---------------------------------------------------\n\n\n"+totalItems);

                int itemsPerPage = pageSize == 0 ? totalItems : pageSize;
                
                dbQuery = dbQuery
                .Skip( (currentPage - 1) * pageSize)
                .Take(itemsPerPage)
                .LoadRelated<Entity>();

                SetIncludes(context);                

                IEnumerable<Entity> filteredItems = dbQuery;

                var filteredItensVO = new List<VO>();
                
                if(itemsPerPage>0)
                {
                    filteredItensVO = filteredItems
                    .Select(m => FromEntityToVO(m))
                    .ToList();

                    totalPages = (int)Math.Ceiling(decimal.Divide(totalItems, itemsPerPage));

                };

                return new BaseResponseVO<VO>()
                {
                    dataList = filteredItensVO,
                    summary = new SummaryData(currentPage, itemsPerPage, totalItems, totalPages)
                };
            }
        }

        // Save or Update
        public virtual VO Save(VO vo)
        {
            Entity entity = null;

            using (TContext context = new TContext())
            {
                entity = context.Set<Entity>().LoadRelated().Where(m => m.Id == vo.id).FirstOrDefault();

                if (entity != null)
                {
                    SetIncludes(context);
                    context.Entry(entity).State = EntityState.Detached;
                }

                entity = FromVOToEntity(vo, entity);

                if (entity.Id <= 0)
                {
                    context.Set<Entity>().Add(entity);
                    context.Entry(entity).State = EntityState.Added;
                }
                else
                {
                    context.Set<Entity>().Update(entity);
                    context.Entry(entity).State = EntityState.Modified;
                }

                context.SaveChanges();
                return FromEntityToVO(entity);
            }
        }

        public virtual List<VO> SaveAll(List<VO> vos, bool flag = true)
        {
            Entity entity = null;
            List<VO> savedVOs = new List<VO>();
            using (TContext context = new TContext())
            {
                for(int i = 0; i < vos.Count; i++) {
                    VO vo = vos[i];
                    entity = context.Set<Entity>().LoadRelated().Where(m => m.Id == vo.id).FirstOrDefault();

                    if (entity != null)
                    {
                        SetIncludes(context);
                        context.Entry(entity).State = EntityState.Detached;
                    }

                    entity = FromVOToEntity(vo, entity); 

                    if (entity.Id <= 0 || !flag)
                    {
                        context.Set<Entity>().Add(entity);
                        context.Entry(entity).State = EntityState.Added;
                    }
                    else
                    {
                        context.Set<Entity>().Update(entity);
                        context.Entry(entity).State = EntityState.Modified;
                    }
                    savedVOs.Add(FromEntityToVO(entity));
                }
                try{
                    context.SaveChanges();
                }catch(Exception e){
                    Console.WriteLine(e.GetBaseException());
                }
                
                return savedVOs;
            }
        }

        public virtual VO Update(VO vo)
        {
            Entity entity = null;

            using (TContext context = new TContext())
            {
                // entity = context.Set<Entity>().LoadRelated().Where(m => m.Id == vo.id).FirstOrDefault();
                entity = context.Set<Entity>().Where(m => m.Id == vo.id).FirstOrDefault();
                // SetIncludes(context);

                if (entity != null)
                {
                    context.Entry(entity).State = EntityState.Detached;
                    entity = FromVOToEntity(vo, entity);

                    context.Set<Entity>().Update(entity);
                    context.Entry(entity).State = EntityState.Modified;

                    context.SaveChanges();
                    return FromEntityToVO(entity);
                }

                return null;
            }
        }

        public virtual VO Patch(VO vo)
        {
            Entity entity = null;

            using (TContext context = new TContext())
            {
                entity = context.Set<Entity>().LoadRelated().Where(m => m.Id == vo.id).FirstOrDefault();

                if (entity != null)
                {
                    // context.Entry(entity).State = EntityState.Detached;
                    entity = FromVOToEntity(vo, entity);

                    // context.Set<Entity>().Update(entity);
                    context.Attach(entity);
                    context.Entry(entity).State = EntityState.Modified;

                    context.SaveChanges();
                    return FromEntityToVO(entity);
                }

                return null;
            }
        }

        public virtual void Delete(long id)
        {
            using (TContext context = new TContext())
            {
                var entity = context.Set<Entity>().Where(m => m.Id == id).FirstOrDefault();

                if (entity != null)
                {
                    context.Set<Entity>().Remove(entity);
                    context.SaveChanges();
                }
            }
        }

        public virtual VO FromEntityToVO(Entity entity)
        {
            return _mapper.Map<VO>(entity);
        }

        public virtual Entity FromVOToEntity(VO vo, Entity entity = null)
        {
            return _mapper.Map<VO,Entity>(vo, entity);
        }

        public virtual IQueryable<Entity> DefaultWhere(IQueryable<Entity> originalQuery, VO filter, List<string> attributes)
        {            
            Expression queryExpr = originalQuery.Expression;
            var parameter = Expression.Parameter(typeof(Entity),"o"); 
            foreach (var attribute in attributes)
            {
                Type typeNullable = null;
                Expression val;
                try
                {
                    var valueFilter = filter.GetType().GetProperty(attribute).GetValue(filter, null);
                    if(valueFilter!=null && attribute!="way" && attribute!="orderBy")
                    {
                        if(typeof(VO).GetProperty(attribute).PropertyType.ToString().Contains("Nullable"))
                        {                        
                            typeNullable = filter.GetType().GetProperty(attribute).GetValue(filter, null).GetType().GetNullableType();                                        
                            val = Expression.Constant(valueFilter, typeNullable);
                        }    
                        else
                        {
                            val = Expression.Constant(valueFilter);
                        }  
                        var propr = Expression.Property(parameter, attribute);   
                        var body = Expression.Equal(propr, val);                    
                        var selectorExp =  Expression.Lambda<Func<Entity, bool>>(body, parameter);                    
                        originalQuery = System.Linq.Queryable.Where(originalQuery, selectorExp);                                                                   
                    }
                }
                catch
                {

                }
            }           
            return originalQuery;                      
        }        

        public virtual List<string> IgnoreAttributes(VO filter, List<string> attributes)
        {
            List<string> list = new List<string>();
            foreach (var prop in typeof(VO).GetProperties())      
            {
                bool flag = false;
                foreach(var attr in attributes)
                {
                    if(prop.Name.Equals(attr))
                    {
                       flag = true; 
                       break;
                    }
                }
                if(!flag)
                {
                    var value = filter.GetType().GetProperty(prop.Name).GetValue(filter, null);
                    if(value!=null)
                        list.Add(prop.Name);
                }

            }
            return list;
        }

        /// <summary>
        /// Used in "getAllFilter" method to create custom filter by interacting trought each item of the entity. 
        ///
        /// The "iterableItem" parameter represents each element of the interaction;
        /// 
        /// The "filter" parameter represents the body object passed by the user.
        /// 
        /// Returns true case the condition to filter is valid and false if not;
        /// </summary>
        public virtual IQueryable<Entity> GetCustomWhere(IQueryable<Entity> query, VO filter)
        {
            return query;
        }
    }
}
