using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class CallNumber
    {
        public int Id { get; set; }
        /// <summary>
        /// نوع تماس از جدول category، مثلا mobile یا phone
        /// </summary>
        public int? CallTypeIdFk { get; set; }
        /// <summary>
        /// مقدار تماس
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// کلید خارجی کاربر
        /// </summary>
        public int? WebSiteUserAccountIdFk { get; set; }

        public virtual Category CallTypeIdFkNavigation { get; set; }
        public virtual WebSiteUserAccount WebSiteUserAccountIdFkNavigation { get; set; }
    }
}
