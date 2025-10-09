using SmartJournalSystem.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace SmartJournalSystem.Services
{
  // Enkel in-memory service för patienter och journaler + JSON-persistens
  public class PatientService
  {
    private List<Patient> patients = new();

    // Registrera patient (sätter Id automatiskt)
    public void RegisterPatient(Patient patient)
    {
      int nextId = patients.Any() ? patients.Max(p => p.Id) + 1 : 1;
      patient.Id = nextId;
      patients.Add(patient);
      Console.WriteLine($"✅ Patient '{patient.Name}' registered with Id {patient.Id}.");
    }

    // Hämta alla patienter
    public List<Patient> GetAllPatients()
    {
      return patients.ToList();
    }

    // Hämta patient via id
    public Patient? GetPatient(int id)
    {
      return patients.FirstOrDefault(p => p.Id == id);
    }

    // Hitta patient via namn (case-insensitive)
    public Patient? FindByName(string name)
    {
      if (string.IsNullOrWhiteSpace(name)) return null;
      return patients.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    // Lägg till journalanteckning för en patient (sätter entry.Id automatiskt)
    public void AddJournalEntry(int patientId, string content, PermissionLevel permission, string author = "")
    {
      var patient = GetPatient(patientId);
      if (patient == null)
      {
        Console.WriteLine("❌ Patient not found.");
        return;
      }

      int nextEntryId = patients.SelectMany(p => p.JournalEntries).Any()
          ? patients.SelectMany(p => p.JournalEntries).Max(e => e.Id) + 1
          : 1;

      var entry = new JournalEntry
      {
        Id = nextEntryId,
        PatientId = patientId,
        Content = content,
        Author = author,
        Permission = permission,
        CreatedAt = DateTime.Now
      };

      patient.JournalEntries.Add(entry);
      Console.WriteLine($"✅ Journal entry added to patient {patient.Name} (Entry Id {entry.Id}).");
    }

    // Hämta journalanteckningar beroende på användarens rättigheter
    public List<JournalEntry> GetJournalEntries(User user, int patientId)
    {
      var patient = GetPatient(patientId);
      if (patient == null) return new List<JournalEntry>();

      // Admin ser allt
      if (user.Role == Role.Admin)
        return patient.JournalEntries.ToList();

      // Staff måste vara tilldelad patienten
      if (user.Role == Role.Staff)
      {
        if (!user.AssignedPatientIds.Contains(patientId))
          return new List<JournalEntry>();

        // Tilldelad staff ser alla anteckningar (även StaffOnly)
        return patient.JournalEntries.ToList();
      }

      // Patient kan bara se anteckningar som är markerade för patient eller AllStaff
      if (user.Role == Role.Patient)
      {
        if (!user.AssignedPatientIds.Contains(patientId))
          return new List<JournalEntry>();

        return patient.JournalEntries
            .Where(e => e.Permission == PermissionLevel.Patient || e.Permission == PermissionLevel.AllStaff)
            .ToList();
      }

      return new List<JournalEntry>();
    }

    // ===== JSON persistence (patients inkluderar JournalEntries) =====

    public void SaveData()
    {
      var dataDir = "data";
      if (!Directory.Exists(dataDir))
        Directory.CreateDirectory(dataDir);

      var options = new JsonSerializerOptions { WriteIndented = true };
      var patientsJson = JsonSerializer.Serialize(patients, options);
      File.WriteAllText(Path.Combine(dataDir, "patients.json"), patientsJson);
      Console.WriteLine("💾 Patient data saved to 'data/patients.json'");
    }

    public void LoadData()
    {
      var dataDir = "data";
      if (!Directory.Exists(dataDir))
        return;

      var file = Path.Combine(dataDir, "patients.json");
      if (!File.Exists(file))
        return;

      var json = File.ReadAllText(file);
      var loaded = JsonSerializer.Deserialize<List<Patient>>(json);
      if (loaded != null)
      {
        patients = loaded;
        Console.WriteLine($"📂 Loaded {patients.Count} patients from 'data/patients.json'");
      }
    }
  }
}
