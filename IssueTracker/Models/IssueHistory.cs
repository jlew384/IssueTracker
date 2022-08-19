using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IssueTracker.Models
{
    public class IssueHistory
    {
        [Key]
        public int Id { get; set; }

        public int IssueId { get; set; }

        [ForeignKey("IssueId")]
        public virtual Issue Issue { get; set; }

        public string? CreatorUserId { get; set; }

        [DisplayName("Creator")]
        [ForeignKey("CreatorUserId")]
        public virtual ApplicationUser? CreatorUser { get; set; }

        public string? AssignedUserId { get; set; }

        [DisplayName("Assigned")]
        [ForeignKey("AssignedUserId")]
        public virtual ApplicationUser? AssignedUser { get; set; }

        [Required]
        [StringLength(50)]
        public string Title { get; set; } = null!;

        [DisplayName("Description")]
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
        public DateTime Created { get; set; } = DateTime.UtcNow;

        public bool IsCreatorUpdated { get; set; }
        public bool IsAssignedUserUpdated { get; set; }
        public bool IsDescUpdated { get; set; }
        public bool IsStatusUpdated { get; set; }
        public bool IsPriorityUpdated { get; set; }
        public bool IsTypeUpdated { get; set; }
    }
}
