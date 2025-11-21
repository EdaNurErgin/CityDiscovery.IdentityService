using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Common.DTOs.Identity
{
    // Shared.Common/DTOs/Identity/UserDto.cs

        public class UserDto
        {
            public Guid Id { get; set; }
            public string UserName { get; set; } = default!;
            public string Email { get; set; } = default!;
            public string Role { get; set; } = default!;

            public string? Bio { get; set; }
            public string? City { get; set; }
            public string? AvatarUrl { get; set; }
            public DateTime? DateOfBirth { get; set; }
    }
    

}
