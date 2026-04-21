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

        public string? ErrorMessage { get; set; }
    }
}