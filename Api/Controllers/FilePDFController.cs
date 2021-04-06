using System;
using System.IO;
using System.Net.Http.Headers;
using Base.Controller;
using Microsoft.AspNetCore.Mvc;
using XVagas.DAO;
using XVagas.Entity;
using XVagas.Helpers;
using XVagas.VO;
using XVagas.Business;

namespace XVagas.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FilePDFController : BaseController<FilePDFBusiness, FilePDFDAO, DatabaseContext, FilePDFVO, FilePDF, AutoMapperProfile>
    {
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
    }
}