using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using UniCodeProject.API.Contracts;
using UniCodeProject.API.Data;

public class ExcelExportService : IExcelExportService
{
    private readonly ApplicationDbContext _context;

    public ExcelExportService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<(byte[] fileData, string? taskName)> GenerateTaskResultsExcelAsync(int taskId, string lecturerId)
    {
        var task = await _context.TaskModels
            .Include(t => t.Submissions)
            .FirstOrDefaultAsync(t => t.Id == taskId && t.LecturerId == lecturerId);

        if (task == null)
            return (null!, null);

        var studentIds = task.Submissions.Select(s => s.StudentId).Distinct().ToList();
        var studentProfiles = await _context.StudentProfiles
            .Where(sp => studentIds.Contains(sp.UserId))
            .ToDictionaryAsync(sp => sp.UserId, sp => sp);

        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Results");

        worksheet.Cells[1, 1].Value = "Full Name";
        worksheet.Cells[1, 2].Value = "Faculty Number";
        worksheet.Cells[1, 3].Value = "Email";
        worksheet.Cells[1, 4].Value = "Score";
        worksheet.Cells[1, 5].Value = "Submitted At";
        worksheet.Cells[1, 6].Value = "Feedback";

        worksheet.Row(1).Style.Font.Bold = true;

        int row = 2;
        
        var sortedSubmissions = task.Submissions
            .OrderByDescending(s => s.Score ?? 0)   
            .ThenBy(s =>
                studentProfiles.TryGetValue(s.StudentId, out var profile) ? profile.FullName : "");
        
        foreach (var submission in sortedSubmissions)
        {
            studentProfiles.TryGetValue(submission.StudentId, out var profile);

            worksheet.Cells[row, 1].Value = profile?.FullName ?? "N/A";
            worksheet.Cells[row, 2].Value = profile?.FacultyNumber ?? "-";
            worksheet.Cells[row, 3].Value = profile?.Email ?? "N/A";
            worksheet.Cells[row, 4].Value = submission.Score?.ToString() ?? "Pending";
            worksheet.Cells[row, 5].Value = submission.SubmissionTime.ToString("g");
            worksheet.Cells[row, 6].Value = submission.Feedback ?? "-";

            var scoreCell = worksheet.Cells[row, 4];
            var score = submission.Score;

            if (score == null)
            {
                scoreCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                scoreCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            }
            else if (score >= 85)
            {
                scoreCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                scoreCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGreen);
            }
            else if (score >= 50)
            {
                scoreCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                scoreCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);
            }
            else
            {
                scoreCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                scoreCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightCoral);
            }

            row++;
        }

        worksheet.Cells.AutoFitColumns();
        return (package.GetAsByteArray(), task.Name);
    }
}
