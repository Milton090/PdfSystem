using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PdfSystem.Models;
using PdfSystem.Repository.IRepository;
using System.Security.Claims;

namespace PdfSystem.Controllers
{
    [Authorize]
    [Route("api/pdf")]
    [ApiController]
    public class PdfController : ControllerBase
    {
        private readonly IPdfRepo _pdfRepo;

        public PdfController(IPdfRepo pdfRepo)
        {
            _pdfRepo = pdfRepo;
        }



        [HttpPost("sign/{id}")]
        public async Task<IActionResult> SignPdf(Guid id, IFormFile signatureImage)
        {
            if (signatureImage == null || signatureImage.Length == 0)
            {
                return BadRequest(new ResponseAPI
                {
                    Success = false,
                    Msg = "No signature image provided",
                    Data = null
                });
            }

            using (var memoryStream = new MemoryStream())
            {
                await signatureImage.CopyToAsync(memoryStream);
                var signatureBytes = memoryStream.ToArray();

                var response = await _pdfRepo.SignPdf(id, signatureBytes);

                if (!response.Success)
                {
                    return BadRequest(response);
                }

                var pdf = (Pdf)response.Data;
                return File(pdf.FileData, "application/pdf", pdf.FileName);
            }
        }



        [HttpGet]
        public async Task<IActionResult> GetAllPdf()
        {
            var response = await _pdfRepo.GetAllPdf();
            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);
        }


        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetPdfByUserId(String userId)
        {
            var response = await _pdfRepo.GetPdfByUserId(userId);
            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);
        }


        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new ResponseAPI
                {
                    Success = false,
                    Msg = "No file uploaded",
                    Data = null
                });
            }

            
            if (!file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase) ||
                file.ContentType != "application/pdf")
            {
                return BadRequest(new ResponseAPI
                {
                    Success = false,
                    Msg = "Invalid file type. Only PDF files are allowed.",
                    Data = null
                });
            }

            
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new ResponseAPI
                {
                    Success = false,
                    Msg = "User ID not found in token",
                    Data = null
                });
            }

            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);

                    var pdf = new Pdf
                    {
                        Id = Guid.NewGuid(),
                        FileName = file.FileName,
                        FileData = memoryStream.ToArray(),
                        CreatedDate = DateTime.UtcNow,
                        UpdatedDate = null,
                        UserId = userId
                    };

                    var response = await _pdfRepo.AddPdf(pdf);
                    if (!response.Success)
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, response);
                    }

                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseAPI
                {
                    Success = false,
                    Msg = $"Error uploading file: {ex.Message}",
                    Data = null
                });
            }
        }



        [HttpGet("download/{id}")]
        public async Task<IActionResult> Download(Guid id)
        {
            var response = await _pdfRepo.GetPdfById(id);
            if (!response.Success)
            {
                return NotFound(response);
            }

            var pdf = (Pdf)response.Data;
            return File(pdf.FileData, "application/pdf", pdf.FileName);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePdf(Guid id)
        {
            var response = await _pdfRepo.DeletePdf(id);
            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);
        }
    }
}
