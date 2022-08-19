using System.ComponentModel;
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

        [DisplayName("Description")]
        [Required]
        [StringLength(600)]
        public string Desc { get; set; } = null!;

        public virtual ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();

        [DisplayName("Created")]
        [Required]
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        [DisplayName("Modified")]
        public DateTime? DateModified { get; set; }

        public string OwnerId { get; set; }
        
    }
}
