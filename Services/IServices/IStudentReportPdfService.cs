using Models.Dto;

namespace Services.IServices;

public interface IStudentReportPdfService
{
    Task<byte[]> GenerateAsync(
        StudentRecordDto student,
        IEnumerable<StudentDisabilityItemDto> disabilities,
        IEnumerable<StudentAttentionAreaItemDto> attentionAreas);
}
