namespace CP1.Data.Models;

public partial class TaskItem
{
    public int TaskId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime CreatedDate { get; set; }
    public string? CreatedBy { get; set; }
    public bool Status { get; set; }
}