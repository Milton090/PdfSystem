using AutoMapper;
using PdfSystem.Models;
using PdfSystem.Models.DTOs;

namespace PdfSystem.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Pdf, GetPdfsDto>();
        }
    }
}
