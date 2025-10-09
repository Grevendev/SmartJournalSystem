using System.Collections.Generic;

namespace SmartJournalSystem.Models
{
  // Representerar en patient
  public class Patient
  {
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }

    // Viktigt: lista med journalanteckningar för patienten
    public List<JournalEntry> JournalEntries { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.Now;
  }
}
