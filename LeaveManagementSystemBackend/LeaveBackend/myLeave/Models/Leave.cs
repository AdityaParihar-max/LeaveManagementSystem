using myLeave.Models;
public class Leave
{
    public int Id { get; set; }
    public string? LeaveType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Reason { get; set; }
    public string? Status { get; set; }

    public string? EmployeeEmail { get; set; }
    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }
}

