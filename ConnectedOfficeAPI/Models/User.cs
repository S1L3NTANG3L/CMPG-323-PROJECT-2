namespace ConnectedOfficeAPI.Models
{
    public partial class User
    {
        public string UserId { get; set; } = null!;
        public string? Name { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
    }
}
