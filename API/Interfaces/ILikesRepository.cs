using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using API.DTO;
using API.Helpers;

namespace API.Interfaces
{
    public interface ILikesRepository
    {
        Task<UserLike> GetUserLikeAsync(int sourceUserId, int likedUserId);
        Task<AppUser> GetUserWithLikesAsync(int userId);
        Task<PagedList<LikeDto>> GetUserLikesAsync(LikesParams likesParams);
    }
}