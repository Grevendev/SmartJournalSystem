using System.Collections.Generic;

namespace SmartJournalSystem.Models
{
  // Roller i systemet
  public enum Role
  {
    Admin,
    Staff,
    Patient
  }

  // Representerar en användare (admin, personal eller patient)
  public class User
  {
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;     // Visningsnamn / login-namn i detta demo
    public string Username { get; set; } = string.Empty; // (ej nödvändig i demo, men bra att ha)
    public string Password { get; set; } = string.Empty; // TODO: hash i verkligt system
    public Role Role { get; set; }

    // Vilka patient-IDs användaren är tilldelad (för staff) eller vilken patient en patient-användare hör till
    public List<int> AssignedPatientIds { get; set; } = new();
  }
}
