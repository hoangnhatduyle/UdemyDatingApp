using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class LikesController : BaseApiController
    {
        private readonly IUnitOfWOrk _unitOfWOrk;
        public LikesController(IUnitOfWOrk unitOfWOrk)
        {
            _unitOfWOrk = unitOfWOrk;
        }

        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username)
        {
            var sourceUserId = User.GetUserID();
            var likedUser = await _unitOfWOrk.UserRepository.GetUserByUsernameAsync(username);
            var sourceUser = await _unitOfWOrk.LikeRepository.GetUserWithLikes(sourceUserId);

            if (likedUser == null) return NotFound();

            if (sourceUser.UserName == username) return BadRequest("You cannot like yourself");

            var userLike = await _unitOfWOrk.LikeRepository.GetUserLike(sourceUserId, likedUser.Id);

            if (userLike != null) 
            {
                sourceUser.LikedUsers.Remove(userLike);
                
                if (await _unitOfWOrk.Complete()) return Ok(false);
            }
            else 
            {
                userLike = new UserLike
                {
                    SourceUserId = sourceUserId,
                    LikedUserId = likedUser.Id
                };

                sourceUser.LikedUsers.Add(userLike);
                
                if (await _unitOfWOrk.Complete()) return Ok(true);
            }

            return BadRequest("Failed to like user");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LikeDto>>> GetUserLIkes([FromQuery]LikesParams likesParams)
        {
            likesParams.UserId = User.GetUserID();
            var users = await _unitOfWOrk.LikeRepository.GetUserLikes(likesParams);

            Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

            return Ok(users);
        }
    }
}