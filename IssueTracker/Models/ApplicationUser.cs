using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace IssueTracker.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<ApplicationUserProject>? Projects { get; set; }
        public virtual ICollection<ApplicationUser>? Issues { get; set; }
    }
}
