using System;

namespace SmartJournalSystem.Models
{
  // Behörighetsnivå för journalanteckningar
  public enum PermissionLevel
  {
    StaffOnly, // Endast tilldelad personal
    AllStaff,  // All personal
    Patient    // Patient kan se
  }

  // En journalanteckning kopplad till en patient
  public class JournalEntry
  {
    public int Id { get; set; }
    public int PatientId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public PermissionLevel Permission { get; set; } = PermissionLevel.StaffOnly;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
  }
}
