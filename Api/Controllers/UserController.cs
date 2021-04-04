using System;
using System.IO;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using XVagas.Business;
using XVagas.Entity;
using XVagas.VO;
using Microsoft.EntityFrameworkCore;

namespace XVagas.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {

        private UserBusiness _businessInstance;
        public UserBusiness businessInstance
        {
            get
            {
                if (_businessInstance == null)
                {
                    _businessInstance = new UserBusiness();
                }

                return _businessInstance;
            }
        }

        [HttpPost("UploadFile"), DisableRequestSizeLimit]
        public IActionResult Upload()
        {
            try
            {
                var file = Request.Form.Files[0];
                var folderName = Path.Combine("Resources", "PDFs");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                if (file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    if(fileName.Substring(0).Split(".")[1] == "pdf"){
                        var fullPath = Path.Combine(pathToSave, DateTime.Now.ToString("yyyyMMddTHHmmssZ") + fileName);
                        var dbPath = Path.Combine(folderName, fileName);
                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }
                        return Ok(new { dbPath });
                    }else{
                        return StatusCode(500, $"O arquivo deve estar em formato PDF.");
                    }
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpPost]
        public ActionResult<UserVO> Save([FromBody] UserVO user)
        {
            try
            {   
                var entity = businessInstance.FromVOToEntity(user);
                using(DatabaseContext context = new DatabaseContext()){
                    context.Set<User>().Add(entity);
                    context.Entry(entity).State = EntityState.Added;
                    context.SaveChanges();
                }
                return Ok(businessInstance.FromEntityToVO(entity));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.GetBaseException());
            }
        }
    }
}