using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace darooyab.Models
{
    public enum QuestionType
    {
        SingleChoice = 1,
        MultipleChoice = 2,
        ShortText = 3,
        LongText = 4,
        Number = 5,
        Date = 6,
        YesNo = 7
    }

    // مدل اصلی برای سرویس‌های دیتابیس (Domain Models)
    public class Questionnaire
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ImageUrl { get; set; }
        public string? MetaDescription { get; set; }

        public virtual ICollection<Question> Questions { get; set; }
    }

    public class Question
    {
        [Key]
        public int Id { get; set; }
        public int QuestionnaireId { get; set; }
        [Required]
        public string Text { get; set; }
        public int QuestionType { get; set; }
        public bool IsRequired { get; set; } = true;
        public int SortOrder { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        [ForeignKey("QuestionnaireId")]
        public virtual Questionnaire Questionnaire { get; set; }
        public virtual ICollection<QuestionOption> QuestionOptions { get; set; }
    }

    public class QuestionOption
    {
        [Key]
        public int Id { get; set; }
        public int QuestionId { get; set; }
        [Required]
        public string Text { get; set; }
        public string Value { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        [ForeignKey("QuestionId")]
        public virtual Question Question { get; set; }
    }

    public class QuestionnaireResponse
    {
        [Key]
        public int Id { get; set; }
        public int QuestionnaireId { get; set; }
        public int? UserId { get; set; }
        public DateTime StartedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        [ForeignKey("QuestionnaireId")]
        public virtual Questionnaire Questionnaire { get; set; }
        public virtual ICollection<Answer> Answers { get; set; }
    }

    public class Answer
    {
        [Key]
        public int Id { get; set; }
        public int QuestionnaireResponseId { get; set; }
        public int QuestionId { get; set; }
        public int? QuestionOptionId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? AnswerText { get; set; }

        [ForeignKey("QuestionnaireResponseId")]
        public virtual QuestionnaireResponse QuestionnaireResponse { get; set; }

        [ForeignKey("QuestionId")]
        public virtual Question Question { get; set; }

        [ForeignKey("QuestionOptionId")]
        public virtual QuestionOption? QuestionOption { get; set; }
    }

    // ==========================================
    // ViewModels (خالص برای استفاده در ویوها)
    // ==========================================

    public class QuestionnaireSummaryViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public string PersianCreatedDate { get; set; }
        public List<QuestionStatsViewModel> Questions { get; set; } = new List<QuestionStatsViewModel>();
    }

    public class QuestionStatsViewModel
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public List<OptionStat> Options { get; set; } = new List<OptionStat>();
        public int TotalResponses => Options.Sum(x => x.Count);
    }

    public class QuestionnaireReportViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int ParticipantCount { get; set; }
        public int CompletedResponseCount { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<QuestionReportViewModel> Questions { get; set; } = new List<QuestionReportViewModel>();
    }

    public class QuestionReportViewModel
    {
        public int QuestionId { get; set; }
        public string Text { get; set; }
        public QuestionType Type { get; set; }
        public bool IsRequired { get; set; }
        public int ResponseCount { get; set; }
        public int UnansweredCount { get; set; }
        public decimal ResponseRate { get; set; }
        public decimal? MinimumNumber { get; set; }
        public decimal? MaximumNumber { get; set; }
        public decimal? AverageNumber { get; set; }
        public List<OptionReportViewModel> Options { get; set; } = new List<OptionReportViewModel>();
        public List<AnswerReportViewModel> Answers { get; set; } = new List<AnswerReportViewModel>();
    }

    public class OptionReportViewModel
    {
        public string Text { get; set; }
        public int Count { get; set; }
        public decimal Percentage { get; set; }
    }

    public class AnswerReportViewModel
    {
        public int ResponseId { get; set; }
        public string AnswerText { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class OptionStat
    {
        public string OptionText { get; set; }
        public int Count { get; set; }
    }

    public class QuestionnaireCreateViewModel
    {
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        public string? ImageUrl { get; set; }
        public string? MetaDescription { get; set; }
        public bool IsActive { get; set; } = true;
        public List<QuestionCreateViewModel> Questions { get; set; } = new List<QuestionCreateViewModel>();
    }

    public class QuestionCreateViewModel
    {
        public int Id { get; set; }
        public bool HasResponses { get; set; }
        [Required]
        public string Text { get; set; }
        public int QuestionType { get; set; } // 1: Binary, 2: Single Choice, 3: Multi Choice, 4: Text
        public bool IsRequired { get; set; } = true;
        public int SortOrder { get; set; }
        public List<QuestionOptionCreateViewModel> Options { get; set; } = new List<QuestionOptionCreateViewModel>();
    }

    public class QuestionOptionCreateViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Text { get; set; }
        [Required]
        public string Value { get; set; }
        public int SortOrder { get; set; }
    }
}
