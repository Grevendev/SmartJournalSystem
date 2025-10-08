using SmartJournalSystem.Models;

namespace SmartJournalSystem.Services;

public class PatientService
{
  private List<Patient> patients = new();
  public void RegisterPatient(Patient patient)
  {
    patient.Id = patients.Count + 1;
    patients.Add(patient);
    Console.WriteLine($"Patient '{patient.Name}' registered seccessfully.");
  }
  public Patient? GetPatient(int id)
  {
    return patients.FirstOrDefault(p => p.Id == id);
  }
  public void ListAll()
  {
    Console.WriteLine("Registered patients: ");
    foreach (var p in patients)
      Console.WriteLine($"- [{p.Id}] {p.Name}, {p.Age} years old");
  }
}