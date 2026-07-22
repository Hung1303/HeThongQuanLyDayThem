namespace BusinessObjects
{
    public partial class Token
    {
        public Guid Id { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? ExpiredTime { get; set; }
        public int Status { get; set; }
        public Guid UserId { get; set; }
    }
}
