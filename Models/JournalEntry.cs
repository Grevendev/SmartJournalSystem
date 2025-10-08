namespace SmartJournalSystem.Models;

public class JournalEntry
{
  public DateTime Date { get; set; } = DateTime.Now;
  public string Text { get; set; } = "";
  public string CreatedBy { get; set; } = "";
}