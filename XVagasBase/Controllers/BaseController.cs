using AutoMapper;
using Base.Business;
using Base.DAO;
using Base.Entities;
using Base.ViewObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace Base.Controller
{
    [Produces("application/json")]
    [ApiController]    
    public class BaseController<Business, DAO, TContext, VO, Entity, AutoMapProfile> : ControllerBase
        where VO : BaseVO, new()
        where TContext : DbContext, new()
        where Entity : BaseEntity, new()
        where AutoMapProfile: Profile, new()
        where DAO : BaseDao<DAO, TContext, VO, Entity, AutoMapProfile>, new()
        where Business : BaseBusiness<DAO, TContext, VO, Entity, AutoMapProfile>, new()
    {
        public BaseController(){ }

        private Business _businessInstance;
        public Business businessInstance
        {
            get
            {
                if (_businessInstance == null)
                {
                    _businessInstance = new Business();
                }

                return _businessInstance;
            }
        }


        // GET api/{controller}/ggetOne/{id}
        [HttpGet("{id}")]
        public virtual ActionResult<VO> GetOne(long id)
        {
            try
            {
                return Ok(businessInstance.GetOne(id));
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpGet]
        [Route("allFilter/{pageNumber}/{listSize}/")]
        public virtual ActionResult<BaseResponseVO<VO>> GetAllFilter2(int pageNumber, int listSize, [FromQuery] VO filter = null)
        {
            try
            {
                return Ok(businessInstance.GetAllFilter(pageNumber, listSize, filter));
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }



        // POST api/{controller}
        [HttpPost]
        public virtual ActionResult<VO> Save([FromBody] VO vo)
        {
            if (vo == null)
            {
                return BadRequest("A non-empty request body is required.");
            }

            try
            {
                return Ok(businessInstance.Save(vo));
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        // PUT api/{controller} || PUT api/{controller}/{id}
        // Can either pass the id trought the url or body
        [HttpPut("{id?}")]
        public virtual ActionResult<VO> Update([FromBody] VO vo, long id)
        {
            if (vo == null)
            {
                return BadRequest("A non-empty request body is required.");
            }
            
            try
            {
                return Ok(businessInstance.Update(vo, id));
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        // PATCH api/{controller} || PATCH api/{controller}/{id}
        // Can either pass the id trought the url or body
        [HttpPatch("{id?}")]
        public virtual ActionResult<VO> Patch([FromBody] VO vo, long id)
        {
            if (vo == null)
            {
                return BadRequest("A non-empty request body is required.");
            }
            
            try
            {
                return Ok(businessInstance.Patch(vo, id));
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        // DELETE api/{controller}/{id}
        [HttpDelete("{id}")]
        public virtual ActionResult Delete(long id)
        {
            try
            {
                businessInstance.Delete(id);
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [Route("Register")]
        [AcceptVerbs("OPTIONS")]
        [EnableCors("AllowSpecificOrigin")]
        public IActionResult Register() => Ok();
    }
}
