using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PdfSystem.Controllers;
using PdfSystem.Models;
using PdfSystem.Repository.IRepository;

namespace PdfSystemTest.Controllers
{
    public class PdfControllerTests
    {
        private readonly Mock<IPdfRepo> _mockPdfRepo;
        private readonly PdfController _pdfController;
        private readonly Mock<IFormFile> _mockFile;
        private readonly Mock<HttpContext> _mockHttpContext;

        public PdfControllerTests()
        {
            _mockPdfRepo = new Mock<IPdfRepo>();
            _pdfController = new PdfController(_mockPdfRepo.Object);
            _mockFile = new Mock<IFormFile>();

            _mockFile.Setup(f => f.Length).Returns(1);
            _mockFile.Setup(f => f.FileName).Returns("test.pdf");
            _mockFile.Setup(f => f.ContentType).Returns("application/pdf");
            _mockFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), CancellationToken.None))
                     .Returns(Task.CompletedTask);
        }

        [Fact]
        public async Task SignPdf_ReturnsBadRequest_WhenNoSignatureImageProvided()
        {
            var result = await _pdfController.SignPdf(Guid.NewGuid(), null);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ResponseAPI>(badRequestResult.Value);

            Assert.False(response.Success);
            Assert.Equal("No signature image provided", response.Msg);
        }


        [Fact]
        public async Task SignPdf_ReturnsFileResult_WhenSignatureIsSuccessful()
        {
            var fakeId = Guid.NewGuid();
            var fakeFileBytes = new byte[] { 0x1, 0x2, 0x3 };
            var fakePdf = new Pdf
            {
                FileData = fakeFileBytes,
                FileName = "signed.pdf"
            };

            var fakeResponse = new ResponseAPI
            {
                Success = true,
                Msg = "PDF signed successfully",
                Data = fakePdf
            };

            var mockPdfRepo = new Mock<IPdfRepo>();
            mockPdfRepo.Setup(repo => repo.SignPdf(It.IsAny<Guid>(), It.IsAny<byte[]>()))
                .ReturnsAsync(fakeResponse);

            var pdfController = new PdfController(mockPdfRepo.Object);

            var fileContent = new FormFile(new MemoryStream(fakeFileBytes), 0, fakeFileBytes.Length, "signatureImage", "signature.png");

            var result = await pdfController.SignPdf(fakeId, fileContent);

            var fileResult = Assert.IsType<FileContentResult>(result);

            Assert.Equal("signed.pdf", fileResult.FileDownloadName);
            Assert.Equal("application/pdf", fileResult.ContentType);
            Assert.Equal(fakeFileBytes, fileResult.FileContents);
        }



        [Fact]
        public async Task Upload_ReturnsBadRequest_WhenFileIsNull()
        {
            IFormFile file = null;

            var result = await _pdfController.Upload(file);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ResponseAPI>(badRequestResult.Value);

            Assert.False(response.Success);
            Assert.Equal("No file uploaded", response.Msg);
        }

        [Fact]
        public async Task Upload_ReturnsBadRequest_WhenFileIsNotPdf()
        {

            _mockFile.Setup(f => f.ContentType).Returns("image/png");

            var result = await _pdfController.Upload(_mockFile.Object);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ResponseAPI>(badRequestResult.Value);

            Assert.False(response.Success);
            Assert.Equal("Invalid file type. Only PDF files are allowed.", response.Msg);
        }

        [Fact]
        public async Task Upload_ReturnsUnauthorized_WhenUserIdNotFound()
        {
            _pdfController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            var result = await _pdfController.Upload(_mockFile.Object);

            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var response = Assert.IsType<ResponseAPI>(unauthorizedResult.Value);

            Assert.False(response.Success);
            Assert.Equal("User ID not found in token", response.Msg);
        }
    }
}
