using Domain.Entities;

namespace Application.DTOs.User
{
    public class UserDto
    {
        public int IdCardNit { get; set; }
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int Identification { get; set; }
        public int RoleId { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string LastJti { get; set; } = string.Empty;
    }


}
