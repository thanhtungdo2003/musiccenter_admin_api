using Microsoft.AspNetCore.Mvc;
using MuscicCenter.Storage;
using MusicBusniess;
using MusicCenterAPI.Data;
using MusicCenterAPI.ProcedureStorage;
using System.Data;
using System.Data.SqlClient;

namespace MusicCenterAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArtistController : ControllerBase
    {
        public IMusicCenterAPI api;
        public ArtistController(IMusicCenterAPI api)
        {
            this.api = api;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Dictionary<string, object>> aritsts = api.CoventToDictionarysWithDataTable(api.ProcedureCall(ProcedureName.GET_ALL_ARTISTS, null));
            return Ok(aritsts);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            if (Guid.TryParse(id, out Guid uid))
            {
                var artist = new ArtistData(api, uid);
                if (artist != null)
                {
                    return Ok(artist);
                }
                else
                {
                    return NotFound();
                }
            }
            return NotFound();
        }
        [HttpPost("visit/userName={userName}artistUid={id}")]
        public IActionResult addVisit(string userName, string id)
        {
            if (Guid.TryParse(id, out Guid uid))
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@ArtistUid", System.Data.SqlDbType.NVarChar){Value=uid.ToString()}
                };
                api.ProcedureCall(ProcedureName.ADD_VISIT_BY_ARTISTUID, parameters);
                return Ok();
            }
            return NotFound();
        }
        [HttpPost]
        public IActionResult Add(string stageName, IFormFile? avataFile)
        {
            if (avataFile != null && avataFile.Length > 0)
            {
                var _artist = new ArtistData(api)
                {
                    ArtistUid = Guid.NewGuid().ToString(),
                    StageName = stageName,
                    Avata = avataFile.FileName,
                    Visits = 0
                };
                api.FileAdd("artist-avata", avataFile);
                _artist.Save();
                return Ok();
            }
            return BadRequest("Không có tệp nào được chọn");
        }

        [HttpPut("{id}")]
        public IActionResult SetById(string id, ArtistData artist)
        {
            if (Guid.TryParse(id, out Guid uid))
            {
                var artistEditor = new ArtistData(api, uid);
                if (artistEditor != null)
                {
                    artistEditor.StageName = artist.StageName;
                    artistEditor.Save();
                    return NoContent();
                }
                else
                {
                    return NotFound();
                }
            }
            return NotFound();
        }
        [HttpGet("getAvata/{id}")]
        public IActionResult getAvata(string id)
        {
            var avataPath = (string)api.getValueByKey(DatabaseStruct.ArtistTable, "Avata", "ArtistUid", id);
            IFormFile file = api.FileExport("artist-avata", avataPath);
            return File(file.OpenReadStream(), file.ContentType, file.FileName);
        }
        [HttpGet("records/{id}")]
        public IActionResult getRecord(string id)
        {
            if (Guid.TryParse(id, out Guid artistUid))
            {
                ArtistData artist = new ArtistData(api, artistUid);
                return Ok(artist.getRecords());
            }
            return NotFound();
        }
        [HttpGet("page/{page}&{rows}")]
        public IActionResult GetByPage(int page, int rows)
        {
            SqlParameter[] parameters = new SqlParameter[]
           {
                new SqlParameter("@Page", SqlDbType.Int) { Value = page },
                new SqlParameter("@Rows", SqlDbType.Int) { Value = rows }
           };
            return Ok(api.CoventToDictionarysWithDataTable(api.ProcedureCall(ProcedureName.GET_ARTISTS_BY_PAGE, parameters)));
        }
    }
}
