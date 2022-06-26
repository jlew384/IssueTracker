using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IssueTracker.Models
{   
    
    public class Issue
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProjectId { get; set; }

        [Required]
        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; } = null!;

        public string? CreatorUserId { get; set; }

        [ForeignKey("CreatorUserId")]
        public virtual ApplicationUser? CreatorUser { get; set; }

        public string? AssignedUserId { get; set; }

        [ForeignKey("AssignedUserId")]
        public virtual ApplicationUser? AssignedUser { get; set; }

        [Required]
        [StringLength(50)]
        public string Title { get; set; } = null!;

        [Required]
        [StringLength(600)]
        public string Desc { get; set; } = null!;

        [Required]
        [StringLength(15)]
        public string Status { get; set; } = null!;

        [Required]
        [StringLength(15)]
        public string Priority { get; set; } = null!;

        [Required]
        [StringLength(15)]
        public string Type { get; set; } = null!;

        [Required]
        public DateTime Created { get; set; } = DateTime.Now;

        public DateTime? Modified { get; set; }
    }
}
