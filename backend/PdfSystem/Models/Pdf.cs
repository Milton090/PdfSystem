using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace PdfSystem.Models
{
    public class Pdf
    {
        public Guid Id { get; set; }
        public String FileName { get; set; }
        public Byte[] FileData { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; } = null;
        public Boolean IsSigned { get; set; } = false;

        [ForeignKey("User")]
        public String UserId { get; set; }
        public virtual IdentityUser User { get; set; }
    }
}
