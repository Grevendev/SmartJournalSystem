namespace SmartJournalSystem.Models;

public enum Role { Admin, Staff, Patient }

public class User
{
  public string Username { get; set; } = "";
  public string Password { get; set; } = "";
  public Role Role { get; set; }
}