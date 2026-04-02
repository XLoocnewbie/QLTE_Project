namespace Backend_QLTE.ChildService.Application.DTOs.Client.Message
{
    public class MessageDTO
    {
        public Guid MessageID { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public bool Seen { get; set; }
        public string Type { get; set; }
        public UserShortDto FromUser { get; set; }
        public UserShortDto ToUser { get; set; }
    }

    public class UserShortDto
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Avatar { get; set; }
        public string UserName { get; set; }
    }
}
