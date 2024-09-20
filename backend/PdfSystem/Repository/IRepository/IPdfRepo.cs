using PdfSystem.Models;

namespace PdfSystem.Repository.IRepository
{
    public interface IPdfRepo
    {
        Task<ResponseAPI> GetAllPdf();
        Task<ResponseAPI> SignPdf(Guid id, Byte[] signatureImage);
        Task<ResponseAPI> GetPdfByUserId(String userId);
        Task<ResponseAPI> GetPdfById(Guid id);
        Task<ResponseAPI> AddPdf(Pdf pdf);
        Task<ResponseAPI> DeletePdf(Guid id);
    }
}
