namespace Group1.BackEnd.Application
{
    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public List<string> Roles { get; set; }
    }
}
