using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class LikesController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly ILikeRepository _likeRepository;
        public LikesController(IUserRepository userRepository, ILikeRepository likeRepository)
        {
            _likeRepository = likeRepository;
            _userRepository = userRepository;
        }

        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username)
        {
            var sourceUserId = User.GetUserID();
            var likedUser = await _userRepository.GetUserByUsernameAsync(username);
            var sourceUser = await _likeRepository.GetUserWithLikes(sourceUserId);

            if (likedUser == null) return NotFound();

            if (sourceUser.UserName == username) return BadRequest("You cannot like yourself");

            var userLike = await _likeRepository.GetUserLike(sourceUserId, likedUser.Id);

            if (userLike != null) 
            {
                sourceUser.LikedUsers.Remove(userLike);
                
                if (await _userRepository.SaveAllAsync()) return Ok(false);
            }
            else 
            {
                userLike = new UserLike
                {
                    SourceUserId = sourceUserId,
                    LikedUserId = likedUser.Id
                };

                sourceUser.LikedUsers.Add(userLike);
                
                if (await _userRepository.SaveAllAsync()) return Ok(true);
            }

            return BadRequest("Failed to like user");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LikeDto>>> GetUserLIkes(string predicate)
        {
            var users = await _likeRepository.GetUserLikes(predicate, User.GetUserID());
            return Ok(users);
        }
    }
}