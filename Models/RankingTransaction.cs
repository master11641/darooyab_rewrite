using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace darooyab.Models
{
    public class RankingTransaction
    {
        public int Id { get; set; }
        public DateTime RegisterDate { get; set; }
        public Int16 RankNumber { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int PharmacyAccountId { get; set; }
        public int PharmacyId { get; set; }
        public int Amount { get; set; }
        public long Irc { get; set; }
        public int ProvinceId { get; set; }
        public int DurationInDays { get; set; }
        public string? ModifyDescription { get; set; }
    }
}
