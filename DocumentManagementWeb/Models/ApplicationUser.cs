using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace DocumentManagementWeb.Models
{
    public class ApplicationUser:IdentityUser
    {
        [Required]
        public string Name { get; set; }

    }
}
