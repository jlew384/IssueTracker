using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace IssueTracker.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
    }

}
