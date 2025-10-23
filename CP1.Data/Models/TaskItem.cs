namespace CP1.Data.Models;

public partial class TaskItem
{
    public int Id { get; set; }              
    public string Name { get; set; } = null!;   
    public string? Description { get; set; }     
    public string Status { get; set; } = "Pending"; 
    public DateTime DueDate { get; set; }       
    public DateTime? CreatedAt { get; set; }     
    public bool? Approved { get; set; }          
}
