using SmartJournalSystem.Models;
using SmartJournalSystem.Services;
using System;
using System.Linq;

var patientService = new PatientService();
var userService = new UserService();

// ===== Ladda data från JSON =====
patientService.LoadData();
userService.LoadData();

// ===== Skapa testdata om tomt =====
if (!userService.GetAllUsers().Any())
{
  Console.WriteLine("🔧 Creating initial test data...");

  var p1 = new Patient { Name = "Alice", Age = 30 };
  var p2 = new Patient { Name = "Bob", Age = 45 };
  patientService.RegisterPatient(p1);
  patientService.RegisterPatient(p2);

  var admin = new User { Name = "AdminUser", Role = Role.Admin, CreatedAt = DateTime.Now };
  var staff = new User { Name = "Dr.Smith", Role = Role.Staff, CreatedAt = DateTime.Now };
  var patientUser1 = new User { Name = "AliceUser", Role = Role.Patient, CreatedAt = DateTime.Now };

  // Tilldela patienter
  patientUser1.AssignedPatientIds.Add(p1.Id);
  staff.AssignedPatientIds.Add(p1.Id);
  staff.AssignedPatientIds.Add(p2.Id);

  userService.AddUser(admin);
  userService.AddUser(staff);
  userService.AddUser(patientUser1);

  // Journaler
  patientService.AddJournalEntry(p1.Id, "General checkup OK", PermissionLevel.Patient, "Dr.Smith");
  patientService.AddJournalEntry(p1.Id, "Staff-only note", PermissionLevel.StaffOnly, "Dr.Smith");
  patientService.AddJournalEntry(p2.Id, "Routine test completed", PermissionLevel.Patient, "Dr.Smith");

  userService.SaveData();
  patientService.SaveData();
}

// ===== LOGIN =====
User? currentUser = null;
while (currentUser == null)
{
  Console.Write("Enter your username: ");
  var input = Console.ReadLine() ?? "";
  currentUser = userService.FindByName(input);
  if (currentUser == null)
    Console.WriteLine("User not found, try again.");
  else
  {
    Console.WriteLine($"\n✅ Welcome {currentUser.Name} ({currentUser.Role})!");
    Console.WriteLine($"Your account was created: {currentUser.CreatedAt:yyyy-MM-dd HH:mm}\n");
  }
}

// ===== HUVUDLOOP =====
bool running = true;
while (running)
{
  Console.WriteLine("\n=== Main Menu ===");
  Console.WriteLine("1) View my assigned patients (Staff) / My record (Patient)");
  Console.WriteLine("2) Add journal entry (Staff)");
  Console.WriteLine("3) Admin: create patient/user/assign");
  Console.WriteLine("4) Save data");
  Console.WriteLine("0) Exit");
  Console.Write("Choice: ");
  var choice = Console.ReadLine() ?? "";

  switch (choice)
  {
    case "1":
      if (currentUser.Role == Role.Staff)
      {
        Console.WriteLine("\n-- Assigned patients --");
        foreach (var pid in currentUser.AssignedPatientIds)
        {
          var p = patientService.GetPatient(pid);
          if (p != null)
            Console.WriteLine($"{p.Id}: {p.Name} ({p.Age} y) - Created: {p.CreatedAt:yyyy-MM-dd HH:mm}");
        }
      }
      else if (currentUser.Role == Role.Patient)
      {
        Console.WriteLine("\n-- My journal entries --");
        foreach (var pid in currentUser.AssignedPatientIds)
        {
          var p = patientService.GetPatient(pid);
          var entries = patientService.GetJournalEntries(currentUser, pid);
          if (p != null)
          {
            Console.WriteLine($"\n📘 Journal for {p.Name} (Created: {p.CreatedAt:yyyy-MM-dd HH:mm})");
            foreach (var e in entries)
              Console.WriteLine($" - [{e.CreatedAt:yyyy-MM-dd HH:mm}] {e.Author}: {e.Content} ({e.Permission})");
          }
        }
      }
      else
      {
        Console.WriteLine("This option is for Staff or Patient roles.");
      }
      break;

    case "2":
      if (currentUser.Role != Role.Staff)
      {
        Console.WriteLine("Only staff may add journal entries.");
        break;
      }

      Console.Write("Enter patient id: ");
      if (!int.TryParse(Console.ReadLine(), out int pidAdd))
      {
        Console.WriteLine("Invalid id.");
        break;
      }
      if (!currentUser.AssignedPatientIds.Contains(pidAdd))
      {
        Console.WriteLine("You are not assigned to that patient.");
        break;
      }
      Console.Write("Entry content: ");
      var content = Console.ReadLine() ?? "";
      Console.Write("Permission (1=StaffOnly,2=AllStaff,3=Patient): ");
      var permInput = Console.ReadLine() ?? "1";
      var perm = permInput switch
      {
        "2" => PermissionLevel.AllStaff,
        "3" => PermissionLevel.Patient,
        _ => PermissionLevel.StaffOnly
      };
      patientService.AddJournalEntry(pidAdd, content, perm, currentUser.Name);
      break;

    case "3":
      if (currentUser.Role != Role.Admin)
      {
        Console.WriteLine("Admin only.");
        break;
      }

      Console.WriteLine("\n--- Admin Menu ---");
      Console.WriteLine("a) Create patient");
      Console.WriteLine("b) Create staff user");
      Console.WriteLine("c) Create patient user");
      Console.WriteLine("d) Assign patient to staff");
      Console.Write("Choice: ");
      var a = Console.ReadLine() ?? "";

      if (a == "a")
      {
        Console.Write("Patient name: ");
        var pn = Console.ReadLine() ?? "";
        Console.Write("Age: ");
        if (!int.TryParse(Console.ReadLine(), out int pa)) { Console.WriteLine("Invalid age."); break; }
        var np = new Patient { Name = pn, Age = pa };
        patientService.RegisterPatient(np);
        userService.SaveData();
        patientService.SaveData();
      }
      else if (a == "b")
      {
        Console.Write("Staff name: ");
        var sn = Console.ReadLine() ?? "";
        var ns = new User { Name = sn, Role = Role.Staff };
        userService.AddUser(ns);
        userService.SaveData();
      }
      else if (a == "c")
      {
        Console.Write("Patient user name: ");
        var un = Console.ReadLine() ?? "";
        Console.Write("Which patient id to link: ");
        if (!int.TryParse(Console.ReadLine(), out int linkId)) { Console.WriteLine("Invalid id."); break; }
        var up = new User { Name = un, Role = Role.Patient };
        up.AssignedPatientIds.Add(linkId);
        userService.AddUser(up);
        userService.SaveData();
      }
      else if (a == "d")
      {
        Console.Write("Staff name to assign to: ");
        var sname = Console.ReadLine() ?? "";
        var staffUser = userService.FindByName(sname);
        if (staffUser == null || staffUser.Role != Role.Staff) { Console.WriteLine("Staff not found."); break; }

        Console.Write("Patient id: ");
        if (!int.TryParse(Console.ReadLine(), out int patId)) { Console.WriteLine("Invalid id."); break; }
        var p = patientService.GetPatient(patId);
        if (p == null) { Console.WriteLine("Patient not found."); break; }

        if (!staffUser.AssignedPatientIds.Contains(patId))
          staffUser.AssignedPatientIds.Add(patId);

        userService.SaveData();
        Console.WriteLine($"Assigned patient {p.Name} to {staffUser.Name}");
      }
      break;

    case "4":
      userService.SaveData();
      patientService.SaveData();
      Console.WriteLine("Saved.");
      break;

    case "0":
      userService.SaveData();
      patientService.SaveData();
      running = false;
      break;

    default:
      Console.WriteLine("Invalid option.");
      break;
  }
}

Console.WriteLine("Goodbye!");
