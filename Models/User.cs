namespace SmartJournalSystem.Models;

public enum Role { Admin, Staff, Patient, }

public class User
{
  public int Id { get; set; }
  public string Name { get; set; } = "";
  public string Username { get; set; } = "";
  public string Password { get; set; } = "";
  public Role Role { get; set; }

  // For staff and Patient: which patient IDs they have access to
  public List<int> AssignedPatientIds { get; set; } = new List<int>();
}