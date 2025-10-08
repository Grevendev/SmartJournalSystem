using System;
using System.Collections.Generic;
using System.Linq;
using SmartJournalSystem.Models;

namespace SmartJournalSystem.Services
{
  // En enkel service för att hantera patienter och journalanteckningar i minnet.
  public class PatientService
  {
    // Interna listor (hålls i minnet i detta exempel)
    private readonly List<Patient> patients = new();
    private readonly List<JournalEntry> journalEntries = new();

    // Registrera ny patient
    public void RegisterPatient(Patient patient)
    {
      patient.Id = patients.Count + 1;
      patients.Add(patient);
      Console.WriteLine($"✅ Patient '{patient.Name}' registered with Id {patient.Id}.");
    }

    // Hämta hela listan med patienter (används i admin-menyn)
    public List<Patient> GetAllPatients()
    {
      // Returnera en ny lista så att den interna listan inte kan modifieras av avsikten
      return patients.ToList();
    }

    // Hämta enskild patient efter id
    public Patient? GetPatient(int id)
    {
      return patients.FirstOrDefault(p => p.Id == id);
    }

    // Lägg till journalanteckning för patient
    public void AddJournalEntry(int patientId, string content, PermissionLevel permission)
    {
      // validera att patient existerar
      var patient = GetPatient(patientId);
      if (patient == null)
      {
        Console.WriteLine("❌ Patient not found.");
        return;
      }

      journalEntries.Add(new JournalEntry
      {
        Id = journalEntries.Count + 1,
        PatientId = patientId,
        Content = content,
        Permission = permission,
        CreatedAt = DateTime.Now
      });

      Console.WriteLine("✅ Journal entry added successfully.");
    }

    // Hämta journalanteckningar för en patient med behörighetskontroll utifrån user
    public List<JournalEntry> GetJournalEntries(User user, int patientId)
    {
      // Hämta alla anteckningar för patienten
      var entriesForPatient = journalEntries.Where(j => j.PatientId == patientId).ToList();

      // Admin ser allt
      if (user.Role == Role.Admin)
      {
        return entriesForPatient;
      }

      // Personal måste vara tilldelad patienten för att se något
      if (user.Role == Role.Staff)
      {
        if (!user.AssignedPatientIds.Contains(patientId))
          return new List<JournalEntry>();

        // Staff som är tilldelad får se alla anteckningar för patienten
        return entriesForPatient;
      }

      // Patient ser endast anteckningar markerade som Patient eller AllStaff, och endast för sin egen patient-id
      if (user.Role == Role.Patient)
      {
        if (!user.AssignedPatientIds.Contains(patientId))
          return new List<JournalEntry>();

        return entriesForPatient
            .Where(j => j.Permission == PermissionLevel.Patient || j.Permission == PermissionLevel.AllStaff)
            .ToList();
      }

      // Default: ingen åtkomst
      return new List<JournalEntry>();
    }
  }
}
