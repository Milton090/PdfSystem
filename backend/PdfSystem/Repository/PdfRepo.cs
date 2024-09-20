using AutoMapper;
using iText.Forms;
using iText.Forms.Fields;
using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using iText.Signatures;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Pkcs;
using PdfSystem.Data;
using PdfSystem.Models;
using PdfSystem.Models.DTOs;
using PdfSystem.Repository.IRepository;

namespace PdfSystem.Repository
{
    public class PdfRepo : IPdfRepo
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;

        public PdfRepo(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }


        public async Task<ResponseAPI> SignPdf(Guid id, Byte[] signatureImage)
        {
            var pdf = await _db.Pdfs.FirstOrDefaultAsync(p => p.Id == id);
            if (pdf == null)
            {
                return new ResponseAPI
                {
                    Success = false,
                    Msg = "PDF not found",
                    Data = null
                };
            }

            if (pdf.IsSigned)
            {
                return new ResponseAPI
                {
                    Success = false,
                    Msg = "PDF is already signed!",
                    Data = null
                };
            }

            try
            {
                String baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                String certDirectory = Path.GetFullPath(Path.Combine(baseDirectory, @"..\..\..\Certificate"));
                String certFile = Environment.GetEnvironmentVariable("CERTIFICATE_PATH") ?? Path.Combine(certDirectory, "certificate.pfx");
                String certPwd = "MySup3rPassw0rd";

                using (var certStream = new FileStream(certFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    var cert = new Pkcs12Store(certStream, certPwd.ToCharArray());
                    var alias = cert.Aliases.Cast<String>().FirstOrDefault(a => cert.IsKeyEntry(a));
                    var privateKey = cert.GetKey(alias).Key;
                    var chain = cert.GetCertificateChain(alias).Select(c => c.Certificate).ToArray();

                    using (var pdfStream = new MemoryStream(pdf.FileData))
                    using (var signedPdfStream = new MemoryStream())
                    {
                        var reader = new PdfReader(pdfStream);
                        var writer = new PdfWriter(signedPdfStream);
                        var signer = new PdfSigner(reader, writer, new StampingProperties());

                        var pks = new PrivateKeySignature(privateKey, "SHA-256");


                        var form = PdfAcroForm.GetAcroForm(signer.GetDocument(), true);
                        var image = ImageDataFactory.Create(signatureImage);


                        var numberOfPages = signer.GetDocument().GetNumberOfPages();
                        var lastPage = signer.GetDocument().GetPage(numberOfPages);
                        var signatureField = PdfFormField.CreateSignature(signer.GetDocument());

                        var rect = new iText.Kernel.Geom.Rectangle(72, 72, 200, 100);

                        signatureField.SetFieldName($"sig");
                        signatureField.GetWidgets().Add(new PdfWidgetAnnotation(rect).SetFlag(PdfAnnotation.PRINT).SetPage(lastPage) as PdfWidgetAnnotation);
                        signatureField.SetPage(numberOfPages);
                        form.AddField(signatureField);

                        var pdfCanvas = new PdfCanvas(lastPage);
                        pdfCanvas.AddImageAt(image, rect.GetLeft(), rect.GetBottom(), false);


                        signer.GetSignatureAppearance()
                            .SetReason($"Esto es solo una firma de prueba")
                            .SetLocation("Medellin")
                            .SetPageRect(rect)
                            .SetPageNumber(numberOfPages);

                        signer.SignDetached(pks, chain, null, null, null, 0, PdfSigner.CryptoStandard.CADES);



                        pdf.FileData = signedPdfStream.ToArray();
                        pdf.IsSigned = true;
                        pdf.UpdatedDate = DateTime.Now;

                        _db.Pdfs.Update(pdf);
                        await _db.SaveChangesAsync();
                    }

                    return new ResponseAPI
                    {
                        Success = true,
                        Msg = "PDF signed successfully",
                        Data = pdf
                    };
                }
            }
            catch (Exception ex)
            {
                return new ResponseAPI
                {
                    Success = false,
                    Msg = $"Error signing PDF: {ex.Message}",
                    Data = null
                };
            }
        }


        public async Task<ResponseAPI> GetAllPdf()
        {
            var pdfs = await _db.Pdfs.ToListAsync();
            var pdfsDto = _mapper.Map<List<GetPdfsDto>>(pdfs);
            return new ResponseAPI
            {
                Success = true,
                Msg = "PDFs found",
                Data = pdfsDto
            };
        }

        public async Task<ResponseAPI> GetPdfByUserId(String userId)
        {
            var pdfs = await _db.Pdfs.Where(p => p.UserId == userId).ToListAsync();
            var pdfsDto = _mapper.Map<List<GetPdfsDto>>(pdfs);
            return new ResponseAPI
            {
                Success = true,
                Msg = "Data retrieved successfully",
                Data = pdfsDto
            };
        }

        public async Task<ResponseAPI> AddPdf(Pdf pdf)
        {
            try
            {
                await _db.Pdfs.AddAsync(pdf);
                await _db.SaveChangesAsync();

                return new ResponseAPI
                {
                    Success = true,
                    Msg = "PDF saved successfully",
                    Data = new { PdfId = pdf.Id }
                };
            }
            catch (Exception ex)
            {
                return new ResponseAPI
                {
                    Success = false,
                    Msg = $"Error saving PDF: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<ResponseAPI> GetPdfById(Guid id)
        {
            var pdf = await _db.Pdfs.FirstOrDefaultAsync(p => p.Id == id);
            if (pdf == null)
            {
                return new ResponseAPI
                {
                    Success = false,
                    Msg = "PDF not found",
                    Data = null
                };
            }

            return new ResponseAPI
            {
                Success = true,
                Msg = "PDF found",
                Data = pdf
            };
        }

        public async Task<ResponseAPI> DeletePdf(Guid id)
        {
            var pdf = await _db.Pdfs.FirstOrDefaultAsync(p => p.Id == id);
            if (pdf == null)
            {
                return new ResponseAPI
                {
                    Success = false,
                    Msg = "PDF not found",
                    Data = null
                };
            }

            try
            {
                _db.Pdfs.Remove(pdf);
                await _db.SaveChangesAsync();

                return new ResponseAPI
                {
                    Success = true,
                    Msg = "PDF deleted successfully",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                return new ResponseAPI
                {
                    Success = false,
                    Msg = $"Error deleting PDF: {ex.Message}",
                    Data = null
                };
            }
        }
    }
}
