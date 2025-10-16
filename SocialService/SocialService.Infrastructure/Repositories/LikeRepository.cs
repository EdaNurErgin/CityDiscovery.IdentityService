using SocialService.Application.Interfaces;
using SocialService.Domain.Entities;
using SocialService.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialService.Infrastructure.Repositories
{
    public class LikeRepository : ILikeRepository
    {
        private readonly SocialDbContext _context;

        public LikeRepository(SocialDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(PostLike like)
        {
            await _context.PostLikes.AddAsync(like);
            await _context.SaveChangesAsync();
        }
    }
}
