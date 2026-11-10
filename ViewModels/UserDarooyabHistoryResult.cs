namespace darooyab.ViewModels
{
    public sealed class UserDarooyabHistoryResult
    {
        public long Id { get; set; }
        public string? ActionType { get; set; }
        public int? BrandId { get; set; }
        public DateTime CreateDate { get; set; }
        public int? DrugId { get; set; }
        public string? IP { get; set; }
        public Int16? LogSourceType { get; set; }
        public int? NewsId { get; set; }
        public string? Page { get; set; }
        public int? PrescriptionId { get; set; }
        public int? UserId { get; set; }
    }
}
