using System.ComponentModel.DataAnnotations;
namespace myLeave.Dtos
{
    public class LeaveDto
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }
        public string? LeaveType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? StartDateFormatted { get; set; }
        public string? EndDateFormatted { get; set; }
        public string? Reason { get; set; }
        public string? Status { get; set; }
    }
}