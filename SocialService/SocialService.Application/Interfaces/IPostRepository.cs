using SocialService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialService.Application.Interfaces
{
    public interface IPostRepository
    {
        Task AddAsync(Post post);
        Task<Post> GetByIdAsync(Guid id);
    }
}
