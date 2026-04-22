using System;
using System.ComponentModel.DataAnnotations;

namespace ReachMobiCaseStudy.Models
{
    public class NewsSearchViewModel
    {
        [Display(Name = "Keyword")]
        public string? Keyword { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "From Date")]
        public DateTime? FromDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "To Date")]
        public DateTime? ToDate { get; set; }

        [Display(Name = "Exact Phrase")]
        public string? ExactPhrase { get; set; }

        [Display(Name = "Exclude Words")]
        public string? ExcludeWords { get; set; }

        [Display(Name = "Title Only Search")]
        public string? TitleOnly { get; set; }

        [Display(Name = "Domains")]
        public string? Domains { get; set; }

        [Display(Name = "Exclude Domains")]
        public string? ExcludeDomains { get; set; }

        [Display(Name = "Sort By")]
        public string? SortBy { get; set; } = "publishedAt";

        public string? ErrorMessage { get; set; }
    }
}