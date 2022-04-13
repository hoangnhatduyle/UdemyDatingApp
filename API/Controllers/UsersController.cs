using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize] //ensure this endpoint is protected with authentication
    public class UsersController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;
        private readonly IUnitOfWOrk _unitOfWork;
        public UsersController(IUnitOfWOrk unitOfWOrk, IMapper mapper, IPhotoService photoService)
        {
            _unitOfWork = unitOfWOrk;
            _photoService = photoService;
            _mapper = mapper;
        }

        //api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers([FromQuery] UserParams userParams)    //ActionResult is usually used to wrap IEnumerable
        {
            var gender = await _unitOfWork.UserRepository.GetUserGender(User.GetUsername());
            userParams.CurrentUsername = User.GetUsername();

            if (string.IsNullOrEmpty(userParams.Gender))
                userParams.Gender = gender == "male" ? "female" : "male";

            var users = await _unitOfWork.UserRepository.GetMembersAsync(userParams);                            //make this asynchronous - multithreaded to database

            Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

            return Ok(users);
            //when request goes to database, it waits, deferred to the task (query to database)
        }

        //api/users/username
        [HttpGet("{username}", Name = "GetUser")]
        public async Task<ActionResult<MemberDto>> GetSpecificUser(string username)    //ActionResult is usually used to wrap IEnumerable
        {
            var currentUsername = User.GetUsername();
            return await _unitOfWork.UserRepository.GetMemberAsync(username, isCurrentUser: currentUsername == username);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername()); //give the username from the token that the API uses to authenticate the user

            _mapper.Map(memberUpdateDto, user);
            _unitOfWork.UserRepository.Update(user);

            if (await _unitOfWork.Complete()) return NoContent();

            return BadRequest("Failed to update the user");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());

            var result = await _photoService.AddPhotoAsync(file);

            if (result.Error != null) return BadRequest(result.Error.Message);

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            // if (user.Photos.Count == 0)
            // {
            //     photo.IsMain = true;
            // }

            user.Photos.Add(photo);

            if (await _unitOfWork.Complete())
            {
                return CreatedAtRoute("GetUser", new { username = user.UserName }, _mapper.Map<PhotoDto>(photo));
            }

            return BadRequest("Problem adding photo");
        }

        [HttpPut("set-main-photo/{photoID}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());

            var photo = user.Photos.FirstOrDefault(photo => photo.Id == photoId);

            if (photo.IsMain) return BadRequest("This is already your main photo");

            var currentMain = user.Photos.FirstOrDefault(photo => photo.IsMain);
            if (currentMain != null) currentMain.IsMain = false;
            photo.IsMain = true;

            if (await _unitOfWork.Complete()) return NoContent();

            return BadRequest("Failed to set main photo");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());

            var photo = user.Photos.FirstOrDefault(photo => photo.Id == photoId);

            if (photo == null) return NotFound();
            if (photo.IsMain) return BadRequest("You cannot delete your main photo");
            if (photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null) return BadRequest(result.Error.Message);
            }

            user.Photos.Remove(photo);
            if (await _unitOfWork.Complete()) return Ok();

            return BadRequest("Failed to delete the photo");
        }
    }
}