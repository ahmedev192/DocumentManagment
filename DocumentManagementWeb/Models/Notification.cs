using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocumentManagementWeb.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string Msg { get; set; }

        public IdentityUser applicationUser { get; set; }

        public DateTime CreatedDateTime { get; set; } = DateTime.Now;
    }
}
