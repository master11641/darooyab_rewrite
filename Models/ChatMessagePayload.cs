using System;

namespace darooyab.Models
{
    public class SendMessageRealtimeInput
    {
        public int ReceiveUserId { get; set; }
        public MessageType MessageType { get; set; }
        public string? Title { get; set; }
        public string? Body { get; set; }
        public long? ReplyToId { get; set; }
    }

    public class ChatMessagePayload
    {
        public long Id { get; set; }
        public int UserSenderId { get; set; }
        public int UserRecieveId { get; set; }
        public MessageType MessageType { get; set; }
        public string? MessageTitle { get; set; }
        public string? MessageBody { get; set; }
        public long? ParentId { get; set; }
        public DateTime SendTime { get; set; }
        public MessageStatus MessageStatus { get; set; }
    }
}
