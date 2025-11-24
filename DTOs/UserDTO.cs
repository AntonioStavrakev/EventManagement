namespace EventManagement.Core.DTOs
{
    public class UserBaseDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
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
