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
    public class CommentRepository : ICommentRepository
    {
        private readonly SocialDbContext _context;
        public CommentRepository(SocialDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(PostComment comment)
        {
            await _context.PostComments.AddAsync(comment);
            await _context.SaveChangesAsync();
        }
    }
}
