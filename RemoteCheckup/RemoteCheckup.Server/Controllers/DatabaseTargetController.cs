using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;
using RemoteCheckup.Models;
using RemoteCheckup.Server.DTOs;
using RemoteCheckup.Services;

namespace RemoteCheckup.Server.Controllers
{
    [Route("api/db/targets")] [ApiController] [Authorize]
    public class DatabaseTargetController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly PeriodicDatabaseCheckupService _service;

        public DatabaseTargetController(ApplicationDbContext dbContext, PeriodicDatabaseCheckupService service)
        {
            _dbContext = dbContext;
            _service = service;
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

        [HttpPost("")]
        public ActionResult AddNew([FromBody] PublicDatabaseTarget info)
        {
            _dbContext.DatabaseTargets.Add(new () {
                Name = info.Name,
                DatabaseType = info.DatabaseType,
                ConnectionString = info.ConnectionString,
                ConnectionSecret = info.ConnectionSecret ?? ""
            });
            _dbContext.SaveChanges();
            return Ok(new {
                message = "Done"
            });
        }

        [HttpPut("")]
        public ActionResult Edit([FromBody] PublicDatabaseTarget info)
        {
            var target = _dbContext.DatabaseTargets.Find(info.Id);
            if (target == null) 
            {
                return NotFound(new {
                    message = "Database target not found"
                });
            }

            target.Name = info.Name;
            target.DatabaseType = info.DatabaseType;
            target.ConnectionString = info.ConnectionString;
            if (!string.IsNullOrEmpty(info.ConnectionSecret))
                target.ConnectionSecret = info.ConnectionSecret;
            
            _dbContext.SaveChanges();
            _service.UpdateDbProbes();

            return Ok(new {
                message = "Done"
            });
        }


        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var target = _dbContext.DatabaseTargets.Find(id);
            if (target == null) 
            {
                return NotFound(new {
                    message = "Database target not found"
                });
            }

            _dbContext.DatabaseTargets.Remove(target);
            _dbContext.SaveChanges();
            _service.UpdateDbProbes();

            return Ok(new {
                message = "Done"
            });
        }

    }
}
