using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;
using RemoteCheckup.Models;
using RemoteCheckup.Server.DTOs;

namespace RemoteCheckup.Server.Controllers
{
    [Route("api/db/targets")] [ApiController] [Authorize]
    public class DatabaseTargetController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public DatabaseTargetController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("")]
        public ActionResult GetAll()
        {
            return Ok(new {
                data = 
                    from target
                    in _dbContext.DatabaseTargets
                    select new PublicDatabaseTarget(target)
            });
        }

    }
}
