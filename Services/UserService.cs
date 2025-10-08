using SmartJournalSystem.Models;

namespace SmartJournalSystem.Services;

//Handles user login and authorization
public class UserService
{
  private List<User> users = new();
  public UserService()
  {
    //Predefined example users
    var admin = new User { Username = "admin", Password = "admin123", Role = Role.Admin };
    var staff1 = new User { Username = "dr_alice", Password = "pass123", Role = Role.Staff };
    var staff2 = new User { Username = "dr_bob", Password = "pass123", Role = Role.Staff };
    var patient1 = new User { Username = "john", Password = "john123", Role = Role.Patient };
    var patient2 = new User { Username = "jane", Password = "jane123", Role = Role.Patient };

    //Assign patients to staff
    staff1.AssignedPatientIds.Add(1);
    staff2.AssignedPatientIds.Add(2);

    users.AddRange([admin, staff1, staff2, patient1, patient2]);
  }
  public User? Login(string username, string password)
  {
    var user = users.FirstOrDefault(u => u.Username == username && u.Password == password);
    if (user == null)
    {
      Console.WriteLine("Invalid credentials.");
      return null;
    }
    Console.WriteLine($"Welcome {user.Username} ! You are logged in as {user.Role}.");
    return user;
  }
}