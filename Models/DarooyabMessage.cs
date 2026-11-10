
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using darooyab.Models;
#nullable disable

namespace darooyab.Models
{
    public enum MessageStatus
    {
        [Display(Name = "ارسال شده")]
        Sended,
        [Display(Name = "دریافت شده")]
        Recieved,
        [Display(Name = "مشاهده شده")]
        Visited,
        [Display(Name = "نمایش داده شود")]
        Displayed

    }
    public enum MessageType
    {
        Text,
        Voice,

        Image,
        Post
    }
    public class DarooyabMessage
    {
        public DarooyabMessage()
        {
            MessageChildrens = new HashSet<DarooyabMessage>();
        }
        public long Id { get; set; }
        public int UserSenderId { get; set; }
        public DarooyabUser SenderUser { get; set; }
        public int UserRecieveId { get; set; }
        public DarooyabUser ReceiveUser { get; set; }
        public MessageType MessageType { get; set; }
#nullable enable annotations
        public string? MessageTitle { get; set; }
        public string? MessageBody { get; set; }
#nullable disable
        public DateTime SendTime { get; set; }
        public long? ParentId { get; set; }
#nullable enable
        public DarooyabMessage? MessageParent { get; set; }
        public MessageStatus MessageStatus { get; set; } = MessageStatus.Sended;
        public virtual ICollection<DarooyabMessage> MessageChildrens { get; set; }
        public bool IsDeleted { get; set; }
        public int? NewsId { get; set; }
        public int? DrugId { get; set; }
        public int? BrandId { get; set; }
        public bool? IsGroupMessage { get; set; }
        public bool? IsSystemMessage { get; set; }
        public int? PrescriptionId { get; set; }
        [ForeignKey("PrescriptionId")]
        public DarooyabRecommandPrescription? DarooyabRecommandPrescription { get; set; }

    }
}