using SmartJournalSystem.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace SmartJournalSystem.Services
{
  public class UserService
  {
    private List<User> users = new();

    public void AddUser(User user)
    {
      int nextId = users.Any() ? users.Max(u => u.Id) + 1 : 1;
      user.Id = nextId;
      users.Add(user);
    }

    public List<User> GetAllUsers()
    {
      return users.ToList();
    }

    public User? FindByName(string name)
    {
      if (string.IsNullOrWhiteSpace(name)) return null;
      return users.FirstOrDefault(u => u.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    // ===== JSON persistence =====
    public void SaveData()
    {
      var dataDir = "data";
      if (!Directory.Exists(dataDir))
        Directory.CreateDirectory(dataDir);

      var options = new JsonSerializerOptions { WriteIndented = true };
      var json = JsonSerializer.Serialize(users, options);
      File.WriteAllText(Path.Combine(dataDir, "users.json"), json);
      Console.WriteLine("💾 User data saved to 'data/users.json'");
    }

    public void LoadData()
    {
      var dataDir = "data";
      if (!Directory.Exists(dataDir))
        return;

      var file = Path.Combine(dataDir, "users.json");
      if (!File.Exists(file))
        return;

      var json = File.ReadAllText(file);
      var loaded = JsonSerializer.Deserialize<List<User>>(json);
      if (loaded != null)
      {
        users = loaded;
        Console.WriteLine($"📂 Loaded {users.Count} users from 'data/users.json'");
      }
    }
  }
}
