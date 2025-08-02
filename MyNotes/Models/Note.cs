using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace MyNotes.Models
{
    public class Note
    {
        public int Id { get; set; }
        [Display(Name = "Topic Name")]
        public string? Name { get; set; }
        public string? Description { get; set; }
        [Display(Name = "Links")]
        public string? links { get; set; }
        public string? Tags { get; set; }
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        [Display(Name = "Updated Date")]
        public DateTime UpdatedDate { get; set; } = DateTime.Now;


    }
}
