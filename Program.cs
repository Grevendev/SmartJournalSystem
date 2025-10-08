using SmartJournalSystem.Models;
using SmartJournalSystem.Services;

var patientService = new PatientService();

while (true)
{
  Console.WriteLine("\n=== Smart Journal System ===");
  Console.WriteLine("1. Register a patient");
  Console.WriteLine("2. List all patients");
  Console.WriteLine("3. Exit");
  Console.Write("Choice: ");

  var choice = Console.ReadLine();

  switch (choice)
  {
    case "1":
      Console.Write("Name: ");
      string name = Console.ReadLine() ?? "";
      Console.Write("Age: ");
      int age = int.Parse(Console.ReadLine() ?? "0");

      patientService.RegisterPatient(new Patient { Name = name, Age = age });
      break;

    case "2":
      patientService.ListAll();
      break;

    case "3":
      return;
  }
}
