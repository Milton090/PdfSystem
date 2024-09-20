namespace PdfSystem.Models.DTOs
{
    public class GetPdfsDto
    {
        public Guid Id { get; set; }
        public String FileName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public Boolean IsSigned { get; set; }
        public String UserId { get; set; }
    }
}
