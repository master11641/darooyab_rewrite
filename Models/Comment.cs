using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class Comment
    {
        public Comment()
        {
            InverseParent = new HashSet<Comment>();
        }

        public int Id { get; set; }
        /// <summary>
        /// نمایش داده شود/بولین
        /// </summary>
        public bool? IsAproved { get; set; }
        /// <summary>
        /// متن نظر 
        /// </summary>
        public string? CommentText { get; set; }
        /// <summary>
        /// کد نظر اصلی-جهت ثبت جواب/ریپلای
        /// </summary>
        public int? ParentId { get; set; }
        /// <summary>
        /// تاریخ و ساعت ثبت نظر
        /// </summary>
        public DateTime InsertDateTime { get; set; }
        /// <summary>
        /// نام و نام خانوادگی ثبت کننده نظر
        /// </summary>
        public string? NameFamily { get; set; }
        /// <summary>
        /// ایمیل ثبت کننده نظر
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// تلفن ثبت کننده نظر
        /// </summary>
        public string Tel { get; set; }
        public long? IrcFk { get; set; }
        public int? DrugIdFk { get; set; }
        public int? RespByDoctorIdFk { get; set; }
        public int? CommentForWebSiteAccountUserFk { get; set; }
        public int? ReferredbyWebSiteAccountUserFk { get; set; }

        public virtual WebSiteUserAccount CommentForWebSiteAccountUserFkNavigation { get; set; }
        public virtual Comment Parent { get; set; }
        public virtual WebSiteUserAccount ReferredbyWebSiteAccountUserFkNavigation { get; set; }
        public virtual WebSiteUserAccount RespByDoctorIdFkNavigation { get; set; }
        public virtual ICollection<Comment> InverseParent { get; set; }
    }
}
