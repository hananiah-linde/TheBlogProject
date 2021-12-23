using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace TheBlogProject.Models;

public class BlogUser : IdentityUser
{
    [Required]
    [StringLength(50, ErrorMessage = "The {0} must be at least {2} and no more than {1} charaters long", MinimumLength = 2)]
    [Display(Name = "First Name")]
    public string FirstName { get; set; }

    [Required]
    [StringLength(50, ErrorMessage = "The {0} must be at least {2} and no more than {1} charaters long", MinimumLength = 2)]
    [Display(Name = "Last Name")]
    public string LastName { get; set; }

    [Required]
    [StringLength(50, ErrorMessage = "The {0} must be at least {2} and no more than {1} charaters long", MinimumLength = 2)]
    [Display(Name = "Last Name")]
    public string DisplayName { get; set; }


    public byte[] ImageData { get; set; }
    public string ContentType { get; set; }

    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and no more than {1} charaters long", MinimumLength = 2)]
    public string GitHubUrl { get; set; }

    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and no more than {1} charaters long", MinimumLength = 2)]
    public string LinkedInUrl { get; set; }

    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and no more than {1} charaters long", MinimumLength = 2)]
    public string PersonalUrl { get; set; }

    [NotMapped]
    public string FullName { get => $"{FirstName} {LastName}"; }

    public virtual ICollection<Blog> Blogs { get; set; } = new HashSet<Blog>();
    public virtual ICollection<Post> Posts { get; set; } = new HashSet<Post>();

}
