namespace SmartJournalSystem.Models;

public class Patient
{
  public int Id { get; set; }
  public string Name { get; set; } = "";
  public int Age { get; set; }
  public List<JournalEntry> JournalsEnries { get; set; } = new();
}