using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MusicCenterAPI.Data;
using System.Data.SqlClient;
using System.Data;
using MusicCenterAPI.ProcedureStorage;
using MusicBusniess;
using MusicCenterAPI.Models;

namespace MusicCenterAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecordController : Controller
    {

        public static Dictionary<string, ViralMusic> viewsStatic = new Dictionary<string, ViralMusic>();
        public IMusicCenterAPI _api;
        public RecordController(IMusicCenterAPI api)
        {
            _api = api;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_api.CoventToDictionarysWithDataTable(_api.ProcedureCall(ProcedureName.GET_ALL_RECORDS, null)));
        }
        [HttpGet("page/{page}&{rows}")]
        public IActionResult GetByPage(int page, int rows)
        {
            SqlParameter[] parameters = new SqlParameter[]
           {
                new SqlParameter("@Page", SqlDbType.Int) { Value = page },
                new SqlParameter("@Rows", SqlDbType.Int) { Value = rows }
           };
            return Ok(_api.CoventToDictionarysWithDataTable(_api.ProcedureCall(ProcedureName.GET_RECORD_BY_PAGE, parameters)));
        }
        [HttpGet("top_audio")]
        public IActionResult GetTop()
        {
            SqlParameter[] parameters = new SqlParameter[]
           {
                new SqlParameter("@TopRows", SqlDbType.Int) { Value = 6 }
           };
            return Ok(_api.CoventToDictionarysWithDataTable(_api.ProcedureCall(ProcedureName.GET_TOP_RECORD_BY_VIEW, parameters)));
        }
        [HttpGet("new_audio")]
        public IActionResult GetNew()
        {
            SqlParameter[] parameters = new SqlParameter[]
           {
                new SqlParameter("@TopRows", SqlDbType.Int) { Value = 6 }
           };
            return Ok(_api.CoventToDictionarysWithDataTable(_api.ProcedureCall(ProcedureName.GET_NEW_RECORDS, parameters)));
        }
        [HttpGet("search_query={keyword}")]
        public IActionResult Search(string keyword)
        {
            SqlParameter[] parameters = new SqlParameter[]
           {
                new SqlParameter("@keyword", SqlDbType.NVarChar) { Value = keyword }
           };
            return Ok(_api.CoventToDictionarysWithDataTable(_api.ProcedureCall(ProcedureName.RECORD_SEARCH_BY_KEYWORD, parameters)));
        }
        [HttpGet("{id}")]
        public RecordData Record(string id)
        {
            Guid idValue;
            if (Guid.TryParse(id, out idValue))
            {
                var record = new RecordData(_api, idValue.ToString());
                if (record != null)
                {
                    return record;
                }
            }
            return null;
        }
        [HttpPost("Create")]
        public async Task<IActionResult> Add(string artistUid, string categoryUid, string displayName, string payfee, IFormFile? posterFile, IFormFile? coverPhotoFile, IFormFile? audioFile, string lyric, string description)
        {
            try
            {

                if (posterFile == null)
                {
                    return BadRequest("File Poster không hợp lệ");
                }
                if (coverPhotoFile == null)
                {
                    return BadRequest("File ảnh bìa không hợp lệ");
                }
                if (audioFile == null)
                {
                    return BadRequest("File âm thanh không hợp lệ");
                }
                var recordAdd = new RecordData(_api)
                {
                    RecordUid = Guid.NewGuid().ToString(),
                    ArtistUid = artistUid,
                    CategoryUid = categoryUid,
                    DisplayName = displayName,
                    Record = audioFile.FileName,
                    Poster = posterFile.FileName,
                    CoverPhoto = coverPhotoFile.FileName,
                    Lyrics = lyric,
                    Description = description,
                    Payfee = payfee,
                    Views = 0
                };
                _api.FileAdd(_api.GetFilePathConfig("Appsettings", "record-poster"), posterFile);
                _api.FileAdd(_api.GetFilePathConfig("Appsettings", "record-coverphoto"), coverPhotoFile);
                _api.FileAdd(_api.GetFilePathConfig("Appsettings", "record-audio"), audioFile);
                recordAdd.Save();
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpPut("edit-record")]
        public IActionResult SetById(RecordData record)
        {
            if (Guid.TryParse(record.RecordUid.ToString(), out Guid idValue))
            {
                var recordEditor = new RecordData(_api, idValue.ToString());
                if (recordEditor != null)
                {
                    recordEditor.ArtistUid = record.ArtistUid;
                    recordEditor.DisplayName = record.DisplayName;
                    recordEditor.CategoryUid = record.CategoryUid;
                    recordEditor.Views = record.Views;
                    recordEditor.CoverPhoto = record.CoverPhoto;
                    recordEditor.Poster = record.Poster;
                    recordEditor.Record = record.Record;
                    recordEditor.Payfee = record.Payfee;
                    recordEditor.Description = record.Description;
                    recordEditor.Lyrics = record.Lyrics;
                    recordEditor.Save();
                    return Ok();
                }
            }
            return NotFound();
        }
        [HttpDelete("remove-record/{id}")]
        public bool RemoveByID(string id)
        {
            Guid.TryParse(id, out Guid rid);
            return new RecordData(_api, rid.ToString()).Delete();
        }

        [HttpPut("AddViews/{id}")]
        public IActionResult addViews(string id, int amount)
        {
            if (Guid.TryParse(id, out Guid idValue))
            {
                var recordEditor = new RecordData(_api, idValue.ToString());
                if (recordEditor != null)
                {
                    int oldAmount = recordEditor.Views;
                    recordEditor.Views = oldAmount + amount;
                    if (viewsStatic.ContainsKey(id))
                    {
                        viewsStatic[id].views = viewsStatic[id].views + amount;
                    }
                    else
                    {
                        viewsStatic[id] = new ViralMusic(id, amount, recordEditor);
                    }
                    recordEditor.Save();
                    return Ok();
                }
            }
            return NotFound();
        }
        [HttpGet("Viral")]
        public IActionResult GetViral()
        {
            return Ok(viewsStatic.Values);
        }
    }
}
