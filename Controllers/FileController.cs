using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MusicBusniess;

namespace MusicCenterAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        public IMusicCenterAPI api;
        public FileController(IMusicCenterAPI api)
        {
            this.api = api;
        }
        [HttpGet("get/folder={folder}&file={fileName}")]
        public IActionResult getAvata(string folder, string fileName)
        {
            IFormFile file = api.FileExport(api.GetFilePathConfig("Appsettings", folder), fileName);
            if (file != null)
            {
                return File(file.OpenReadStream(), file.ContentType, file.FileName);
            }
            return BadRequest();
        }
    }
}
