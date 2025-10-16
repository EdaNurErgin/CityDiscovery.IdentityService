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
    public class PostSavedRepository : IPostSavedRepository
    {
        private readonly SocialDbContext _context;
        public PostSavedRepository(SocialDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(PostSaved postSaved)
        {
            await _context.PostSaveds.AddAsync(postSaved);
            await _context.SaveChangesAsync();
        }
    }
}
