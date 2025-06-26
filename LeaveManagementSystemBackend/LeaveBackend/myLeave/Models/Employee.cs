
using System.Text.Json.Serialization;

namespace myLeave.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }

        public string? Role { get; set; }

        [JsonIgnore]
        public List<Leave>? Leaves { get; set; }
    }

}
