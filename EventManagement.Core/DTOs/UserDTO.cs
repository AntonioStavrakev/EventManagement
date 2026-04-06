namespace EventManagement.Core.DTOs
{
    public class UserBaseDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
    }

    public class UserCreateDto : UserBaseDto
    {
    }

    public class UserUpdateDto : UserBaseDto
    {
        public int UserId { get; set; }
    }

    public class UserResponseDto : UserBaseDto
    {
        public int UserId { get; set; }
    }
}
