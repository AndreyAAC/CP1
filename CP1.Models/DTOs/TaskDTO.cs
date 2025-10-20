using System.Text.Json.Serialization;

namespace CP1.Models.DTOs;

public class TaskDTO
{
    [JsonPropertyName("taskId")] 
    public int TaskId { get; set; }

    [JsonPropertyName("name")] 
    public string Name { get; set; } = null!;

    [JsonPropertyName("description")] 
    public string? Description { get; set; }

    [JsonPropertyName("createdDate")] 
    public DateTime CreatedDate { get; set; }

    [JsonPropertyName("createdBy")] 
    public string? CreatedBy { get; set; }

    [JsonPropertyName("status")] 
    public bool Status { get; set; }
}