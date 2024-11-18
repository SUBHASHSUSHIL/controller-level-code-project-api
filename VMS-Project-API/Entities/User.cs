using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace VMS_Project_API.Entities
{
    [Index(nameof(EmailId), IsUnique = true)]
    [Index(nameof(Username), IsUnique = true)]
    [Index(nameof(MobileNo), IsUnique = true)]
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? MobileNo { get; set; }
        public string? EmailId { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public Role? Role { get; set; }
        public int RoleId { get; set; }
        public string? Image {  get; set; }
        public bool Status { get; set; } = true;
        public DateTime RegDate { get; set; } = DateTime.Now;
    }
}
