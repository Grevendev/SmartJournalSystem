namespace SmartJournalSystem.Models;


public enum PermissionLevel
{
  StaffOnly, // Authorized personnel only
  AllStaff, // All personnel can see
  Patient, // Patient kan see
}
public class JournalEntry
{
  public int Id { get; set; }
  public int PatientId { get; set; }
  public string Content { get; set; } = "";
  public PermissionLevel Permission { get; set; } = PermissionLevel.StaffOnly;
  public DateTime CreateAt { get; set; } = DateTime.Now;
}