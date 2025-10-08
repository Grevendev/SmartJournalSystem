namespace SmartJournalSystem.Models;


public enum PermissionLevel
{
  StaffOnly, // Authorized personnel only
  AllStaff, // All staff member
  Patient, // Patient can view
}
public class JournalEntry
{
  public int Id { get; set; }
  public int PatientId { get; set; }
  public string Content { get; set; } = "";
  public PermissionLevel Permission { get; set; } = PermissionLevel.StaffOnly;
  public DateTime CreatedAt { get; set; } = DateTime.Now;
}