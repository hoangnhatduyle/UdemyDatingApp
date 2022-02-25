using System.Security.Claims;
using API.DTOs;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize] //ensure this endpoint is protected with authentication
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public UsersController(IUserRepository userRepository, IMapper mapper)
        {
            _mapper = mapper;
            _userRepository = userRepository;
        }

        //api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()    //ActionResult is usually used to wrap IEnumerable
        {            
            var users = await _userRepository.GetMembersAsync();                            //make this asynchronous - multithreaded to database
            return Ok(users);
            //when request goes to database, it waits, deferred to the task (query to database)
        }

        //api/users/username
        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetSpecificUser(string username)    //ActionResult is usually used to wrap IEnumerable
        {
            return await _userRepository.GetMemberAsync(username);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;        //give the username from the token that the API uses to authenticate the user
            var user = await _userRepository.GetUserByUsernameAsync(username);

            _mapper.Map(memberUpdateDto, user);
            _userRepository.Update(user);

            if (await _userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Failed to update the user");
        } 
    }
}