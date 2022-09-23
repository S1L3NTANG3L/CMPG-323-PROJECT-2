namespace ConnectedOfficeAPI.Models
{
    public partial class Refreshtoken
    {
        public string UserId { get; set; } = null!;
        public string? TokenId { get; set; }
        public string? RefreshToken { get; set; }
        public byte? IsActive { get; set; }
    }
}
