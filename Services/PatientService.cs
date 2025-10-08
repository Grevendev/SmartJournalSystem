using SmartJournalSystem.Models;

namespace SmartJournalSystem.Services;

public class PatientService
{
  private List<Patient> patients = new();
  private List<JournalEntry> journalEntries = new();
  public void RegisterPatient(Patient patient)
  {
    patient.Id = patients.Count + 1;
    patients.Add(patient);
    Console.WriteLine($"Patient '{patient.Name}' registered seccessfully.");
  }
  public void ListAll()
  {
    foreach (var p in patients)
    {
      Console.WriteLine($"- [{p.Id}] {p.Name}, {p.Age} years old");
    }
  }
  public Patient? GetPatient(int id) => patients.FirstOrDefault(p => p.Id == id);
  //Add journalentry
  public void AddJournalEntry(int patientId, string content, PermissionLevel permission)
  {
    journalEntries.Add(new JournalEntry { Id = journalEntries.Count + 1, PatientId = patientId, Content = content, Permission = permission });
    Console.WriteLine("Journal entry added successfully.");
  }
  // Retrive records depending on the user
  public List<JournalEntry> GetJournalEntries(User user, int patientId)
  {
    if (user.Role == Role.Patient && user.AssignedPatientIds.Contains(patientId))
    {
      return journalEntries.Where(j => j.PatientId == patientId).ToList();
    }
    else if (user.Role == Role.Staff && user.AssignedPatientIds.Contains(patientId))
    {
      return journalEntries.Where(j => j.PatientId == patientId).ToList();
    }
    else if (user.Role == Role.Admin)
    {
      return journalEntries.Where(j => j.PatientId == patientId).ToList();
    }
    return new List<JournalEntry>();
  }
}