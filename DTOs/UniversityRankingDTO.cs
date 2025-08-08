namespace UniCodeProject.API.DTOs;

public class UniversityRankingDto
{
    public string UniversityName { get; set; } = null!;
    public int TotalStudents { get; set; }
    public int Place { get; set; } 
    public int TotalPoints { get; set; } 
}
