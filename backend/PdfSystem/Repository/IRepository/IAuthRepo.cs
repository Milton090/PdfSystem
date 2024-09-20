using PdfSystem.Models;

namespace PdfSystem.Repository.IRepository
{
    public interface IAuthRepo
    {
        Task<ResponseAPI> Register (Auth user);
        Task<ResponseAPI> Login(Auth user);
    }
}
