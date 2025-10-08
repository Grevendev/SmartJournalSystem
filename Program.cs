using SmartJournalSystem.Models;
using SmartJournalSystem.Services;

var userService = new UserService();
var patientService = new PatientService();

Console.WriteLine("=== Smart Journal System ===");
Console.Write("Username: ");
string username = Console.ReadLine() ?? "";
Console.Write("Password: ");
string password = Console.ReadLine() ?? "";

var currentUser = userService.Login(username, password);
if (currentUser == null)
  return;

while (true)
{
  Console.WriteLine("\n--- Main Menu ---");
  if (currentUser.Role == Role.Admin)
  {
    Console.WriteLine("1. Register patient");
    Console.WriteLine("2. List patients");
    Console.WriteLine("3. Logout");
    Console.Write("Choice: ");
    var choice = Console.ReadLine();

    if (choice == "1")
    {
      Console.Write("Name: ");
      string name = Console.ReadLine() ?? "";
      Console.Write("Age: ");
      int age = int.Parse(Console.ReadLine() ?? "0");
      patientService.RegisterPatient(new Patient { Name = name, Age = age });
    }
    else if (choice == "2")
    {
      patientService.ListAll();
    }
    else if (choice == "3") break;
  }
  else if (currentUser.Role == Role.Staff)
  {
    Console.WriteLine("1. View assigned patients");
    Console.WriteLine("2. Logout");
    Console.Write("Choice: ");
    var choice = Console.ReadLine();

    if (choice == "1")
    {
      foreach (var pid in currentUser.AssignedPatientIds)
      {
        var patient = patientService.GetPatient(pid);
        if (patient != null)
          Console.WriteLine($" - {patient.Name}, {patient.Age} years old");
      }
    }
    else if (choice == "2") break;
  }
  else if (currentUser.Role == Role.Patient)
  {
    Console.WriteLine("1. View my journal");
    Console.WriteLine("2. Logout");
    var choice = Console.ReadLine();

    if (choice == "1")
    {
      Console.WriteLine($" Your journal is empty for now, {currentUser.Username}.");
    }
    else if (choice == "2") break;
  }
}