using System.ComponentModel.DataAnnotations;

namespace ReachMobiCaseStudy.Models;

public class NewsSearchViewModel
{
    [StringLength(100, ErrorMessage = "Keyword cannot exceed 100 characters.")]
    public string? Keyword { get; set; }

    [DataType(DataType.Date)]
    public DateTime? Date { get; set; }
}
