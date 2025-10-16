using Microsoft.EntityFrameworkCore;
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
    public class PostRepository : IPostRepository
    {
        private readonly SocialDbContext _context;
        public PostRepository(SocialDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Post post)
        {
            // DbContext'in Posts setine yeni post'u ekle
            await _context.Posts.AddAsync(post);
            // Değişiklikleri veritabanına kaydet
            await _context.SaveChangesAsync();
        }

        public async Task<Post> GetByIdAsync(Guid id)
        {
            // Post'u bulurken, ilişkili olduğu yorumları ve beğenileri de getirmesini istiyoruz.
            return await _context.Posts
                .Include(p => p.Comments)
                .Include(p => p.Likes)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}
