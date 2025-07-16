using System.Text.Json.Serialization;

namespace UniCodeProject.API.DataModels;

public class TaskTestCaseModel
{
    public int Id { get; set; }

    public string InputData { get; set; } = null!;
    public string ExpectedOutput { get; set; } = null!;

    public int TaskModelId { get; set; }
    
    [JsonIgnore]
    public TaskModel? Task { get; set; }
}
