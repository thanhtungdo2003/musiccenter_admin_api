using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MusicBusniess;
using MusicCenterAPI.Data;
using MusicCenterAPI.ProcedureStorage;
using System.Data.SqlClient;

namespace MusicCenterAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private IMusicCenterAPI api;
        public CategoryController(IMusicCenterAPI api)
        {
            this.api = api;
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(api.CoventToDictionarysWithDataTable(api.ProcedureCall(ProcedureName.GET_ALL_CATEGORY, null)));
        }
        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            if (Guid.TryParse(id, out Guid uid))
            {
                return Ok(new CategoryData(api, uid));
            }
            return NotFound();
        }
        [HttpGet("records/{id}")]
        public IActionResult GetRecords(string id)
        {
            if (Guid.TryParse(id, out Guid uid))
            {
                if (new CategoryData(api, uid).categoryUid != null)
                {
                    SqlParameter[] parameters = new SqlParameter[]
                    {
                        new SqlParameter("@CategoryUid", System.Data.SqlDbType.NVarChar) {Value = uid.ToString()}
                    };
                    return Ok(api.CoventToDictionarysWithDataTable(api.ProcedureCall(ProcedureName.GET_RECORDS_BY_CATEGORYUID, parameters)));
                }
            }
            return NotFound();
        }
        [HttpPost("add/{displayName}")]
        public IActionResult Create(string displayName)
        {
            var categoryNew = new CategoryData(api)
            {
                categoryUid = Guid.NewGuid().ToString(),
                displayName = displayName,
            };
            categoryNew.Save();
            return Ok();
        }
        [HttpGet("top-cate")]
        public IActionResult getTop()
        {
            return Ok(api.CoventToDictionarysWithDataTable(api.ProcedureCall(ProcedureName.GET_TOP_CATEGORY, null)));
        }
    }
}
