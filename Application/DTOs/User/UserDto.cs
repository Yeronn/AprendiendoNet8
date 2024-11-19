using Domain.Entities;

namespace Application.DTOs.User
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Fullname { get; set; } = string.Empty;
        public List<RoleEntity> Roles { get; set; } = [];
    }
}
