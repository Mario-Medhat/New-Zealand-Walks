using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NZWalks.API.Controllers
{
    // https://localhost:portNumber/api/students
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        // GET: api/students
        [HttpGet]
        public IActionResult GetAllStudents()
        {
            var students = new List<string> { "John", "Jane", "Jack" };
            return Ok(students);
        }
    }
}
