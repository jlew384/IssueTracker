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

        public string UpdatedByUserId { get; set; }

        [Required]
        [DisplayName("Updated By")]
        [ForeignKey("UpdatedByUserId")]
        public virtual ApplicationUser UpdatedBy { get; set; }

        public string? CreatorUserId { get; set; }

        [DisplayName("Creator")]
        [ForeignKey("CreatorUserId")]
        public virtual ApplicationUser? CreatorUser { get; set; }

        public string? AssignedUserId { get; set; }

        [DisplayName("Assigned")]
        [ForeignKey("AssignedUserId")]
        public virtual ApplicationUser? AssignedUser { get; set; }

        [StringLength(50)]
        public string? Title { get; set; }

        [DisplayName("Description")]
        [StringLength(600)]
        public string? Desc { get; set; }

        [StringLength(15)]
        public string? Status { get; set; }

        [StringLength(15)]
        public string? Priority { get; set; }

        [StringLength(15)]
        public string? Type { get; set; }

        [Required]
        public DateTime Updated { get; set; } = DateTime.UtcNow;

        public bool IsCreatorUpdated { get; set; } = false;
        public bool IsAssignedUserUpdated { get; set; } = false;
        public bool IsTitleUpdated { get; set; } = false;
        public bool IsDescUpdated { get; set; } = false;
        public bool IsStatusUpdated { get; set; } = false;
        public bool IsPriorityUpdated { get; set; } = false;
        public bool IsTypeUpdated { get; set; } = false;
    }
}
