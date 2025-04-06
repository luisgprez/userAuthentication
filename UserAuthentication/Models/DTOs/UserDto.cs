namespace UserAuthentication.Models.DTOs
{
    public class UserDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Token { get; set; }
        public UserDto(int userId, string userName, string token)
        {
            UserId = userId;
            UserName = userName;
            Token = token;
        }
    }
}
