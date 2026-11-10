public class ConversationViewModel
{
    public int OtherUserId { get; set; }
    public int TotalMessages { get; set; }
    public int LastSenderId { get; set; }
    public int UnreadMessages { get; set; }
    public DateTime LastSendTime { get; set; }
    public string LastMessageBody { get; set; }
    public string Mobile { get; set; }
    public string? ProfileImageUrl { get; set; }
    public string Name { get; set; }
    public string ActivityTitle { get; set; }
    public int TotalCount { get; set; }
}
