using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Notification.Models;

namespace mtdgtpp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HealthCheckController : ControllerBase
    {
        private DBContext db;
        private IHttpContextAccessor _IPAccess;

        public HealthCheckController(DBContext context, IHttpContextAccessor accessor)
        {
            _IPAccess = accessor;
            db = context;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                using (MySqlConnection conn = db.GetConnection())
                {
                    conn.Open();
                    MySqlCommand cmd;
                    string cmdQuery = " SELECT 'DatabaseQueryOK' ";
                    cmd = new MySqlCommand(cmdQuery, conn);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }

            }
            catch (Exception)
            {
                return BadRequest("Unable to connect to Database.");
            }

            return Ok("Service is up and running");
        }
    }
}

