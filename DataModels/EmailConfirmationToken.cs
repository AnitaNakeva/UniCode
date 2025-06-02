namespace UniCodeProject.API.DataModels
{
    public class EmailConfirmationToken
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string Token { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
