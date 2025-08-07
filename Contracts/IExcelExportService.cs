using UniCodeProject.API.DataModels;

namespace UniCodeProject.API.Contracts;

public interface IExcelExportService
{
    Task<(byte[] fileData, string? taskName)> GenerateTaskResultsExcelAsync(int taskId, string lecturerId);
}

