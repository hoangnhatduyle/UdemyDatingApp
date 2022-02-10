using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController] //this indicates this class is an API controller
    [Route("api/[controller]")] //this (controller) will be replaced by weatherforecast
    public class UsersController : ControllerBase
    {
        private readonly DataContext _context;
        public UsersController(DataContext context)
        {
            _context = context;
        }

        //api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()    //ActionResult is usually used to wrap IEnumerable
        {                                                       //make this asynchronous - multithreaded to database
            return await _context.Users.ToListAsync();
            //when request goes to database, it waits, deferred to the task (query to database)
        }

        //api/users/id
        [HttpGet("{id}")]
        public async Task<ActionResult<AppUser>> GetSpecificUser(int id)    //ActionResult is usually used to wrap IEnumerable
        {
            return await _context.Users.FindAsync(id);
        }
    }
}