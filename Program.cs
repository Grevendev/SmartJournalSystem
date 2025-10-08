using SmartJournalSystem.Models;
using SmartJournalSystem.Services;

// ======== STEG 1: INITIERA PATIENTSERVICE OCH TESTDATA =========

var patientService = new PatientService();

// Skapa patienter
var patient1 = new Patient { Name = "Alice", Age = 30 };
patientService.RegisterPatient(patient1);

var patient2 = new Patient { Name = "Bob", Age = 45 };
patientService.RegisterPatient(patient2);

// Skapa patient-användare
var patientUser1 = new User { Name = "AliceUser", Role = Role.Patient };
patientUser1.AssignedPatientIds.Add(patient1.Id);

var patientUser2 = new User { Name = "BobUser", Role = Role.Patient };
patientUser2.AssignedPatientIds.Add(patient2.Id);

// Skapa staff och tilldela patienter
var staffUser = new User { Name = "Dr. Smith", Role = Role.Staff };
staffUser.AssignedPatientIds.Add(patient1.Id);
staffUser.AssignedPatientIds.Add(patient2.Id);

// Skapa admin
var adminUser = new User { Name = "AdminUser", Role = Role.Admin };

// Lägg till journalanteckningar
patientService.AddJournalEntry(patient1.Id, "General checkup OK", PermissionLevel.Patient);
patientService.AddJournalEntry(patient1.Id, "Staff-only note", PermissionLevel.StaffOnly);
patientService.AddJournalEntry(patient2.Id, "Routine test completed", PermissionLevel.Patient);

// Lista av alla användare
var users = new List<User> { patientUser1, patientUser2, staffUser, adminUser };

// ======== STEG 2: LOGIN LOOP =========

User? currentUser = null;

while (currentUser == null)
{
  Console.Write("Enter your username: ");
  var input = Console.ReadLine();

  currentUser = users.FirstOrDefault(u => u.Name.Equals(input, StringComparison.OrdinalIgnoreCase));

  if (currentUser == null)
    Console.WriteLine("User not found. Try again.");
}

// ======== STEG 3: HUVUDMENY BASERAT PÅ ROLL =========

while (true)
{
  Console.WriteLine($"\nWelcome, {currentUser.Name}! Role: {currentUser.Role}");

  if (currentUser.Role == Role.Patient)
  {
    Console.WriteLine("1. View my journal entries");
    Console.WriteLine("2. Logout");
    Console.Write("Choice: ");
    var choice = Console.ReadLine();

    if (choice == "1")
    {
      foreach (var pid in currentUser.AssignedPatientIds)
      {
        var entries = patientService.GetJournalEntries(currentUser, pid);
        var patient = patientService.GetPatient(pid);
        Console.WriteLine($"\nJournal for {patient?.Name}:");

        foreach (var entry in entries)
        {
          if (entry.Permission == PermissionLevel.Patient || entry.Permission == PermissionLevel.AllStaff)
          {
            Console.WriteLine($"- [{entry.CreatedAt}] {entry.Content} (Permission: {entry.Permission})");
          }
        }
      }
    }
    else if (choice == "2")
    {
      Console.WriteLine("Logging out...");
      break;
    }
  }
  else if (currentUser.Role == Role.Staff)
  {
    Console.WriteLine("1. View assigned patients");
    Console.WriteLine("2. Add journal entry");
    Console.WriteLine("3. Logout");
    Console.Write("Choice: ");
    var choice = Console.ReadLine();

    if (choice == "1")
    {
      foreach (var pid in currentUser.AssignedPatientIds)
      {
        var patient = patientService.GetPatient(pid);
        if (patient != null)
          Console.WriteLine($"- {patient.Name}, {patient.Age} years old");
      }
    }
    else if (choice == "2")
    {
      Console.Write("Patient Id: ");
      int pid = int.Parse(Console.ReadLine() ?? "0");
      var patient = patientService.GetPatient(pid);

      if (patient == null || !currentUser.AssignedPatientIds.Contains(pid))
      {
        Console.WriteLine("You cannot add journal entry for this patient.");
      }
      else
      {
        Console.Write("Entry content: ");
        string content = Console.ReadLine() ?? "";
        Console.WriteLine("Permission level? (1=StaffOnly, 2=AllStaff, 3=Patient): ");
        int perm = int.Parse(Console.ReadLine() ?? "1");
        PermissionLevel level = perm switch
        {
          2 => PermissionLevel.AllStaff,
          3 => PermissionLevel.Patient,
          _ => PermissionLevel.StaffOnly
        };
        patientService.AddJournalEntry(pid, content, level);
      }
    }
    else if (choice == "3") break;
  }
  else if (currentUser.Role == Role.Admin)
  {
    Console.WriteLine("1. View all users");
    Console.WriteLine("2. Logout");
    Console.Write("Choice: ");
    var choice = Console.ReadLine();

    if (choice == "1")
    {
      foreach (var u in users)
      {
        Console.WriteLine($"- {u.Name} (Role: {u.Role})");
      }
    }
    else if (choice == "2") break;
  }
}
