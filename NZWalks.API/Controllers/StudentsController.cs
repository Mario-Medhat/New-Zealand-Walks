using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NZWalks.API.Controllers
{
    // https://localhost:portNumber/api/students
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly ILogger<StudentsController> logger;

        public StudentsController(ILogger<StudentsController> logger)
        {
            this.logger = logger;
        }
        // GET: api/students
        [HttpGet]
        public IActionResult GetAllStudents()
        {
            var students = new List<string> { "John", "Jane", "Jack" };
            logger.LogInformation("Retrieved all students successfully. Students: {Students}", string.Join(", ", students));
            return Ok(students);
        }
    }
}
