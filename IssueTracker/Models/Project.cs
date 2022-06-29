using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IssueTracker.Models
{
    public class Project
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Title { get; set; } = null!;
        [Required]
        [StringLength(600)]
        public string Desc { get; set; } = null!;

        public string? ProjectManagerId { get; set; }

        [ForeignKey("ProjectManagerId")]
        public virtual ApplicationUser? ProjectManager { get; set; }

        [Required]
        public virtual ICollection<ApplicationUserProject> Users { get; set; } = null!;

        [Required]
        public DateTime DateCreated { get; set; }

        public DateTime? DateModified { get; set; }
    }
}
