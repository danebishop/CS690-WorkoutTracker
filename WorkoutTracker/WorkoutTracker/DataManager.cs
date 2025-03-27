using System.ComponentModel.DataAnnotations;

namespace WorkoutTracker; 

public class DataManager{

    FileSaver fileSaver;
    public Dictionary <User, UserPassword> UserDictionary {get;}
    public List<Workoutdata> WorkoutStoredData {get;}
    public List<Groups> Groups {get;}
  
 

    


    public DataManager(){

        UserDictionary = new Dictionary<User, UserPassword> ();
        var userFileContent = File.ReadAllLines("users.txt");
        
        foreach (var line in userFileContent){
            string splitLine = line;
            
            string[] splitLineList = splitLine.Split(new char[] { ':' });

            if (splitLineList.Length < 2){ 
                continue; 
            }else{
                UserDictionary.Add(new User(splitLineList[0].Trim()), new UserPassword( splitLineList[1].Trim()));
            }
        }

        fileSaver = new FileSaver("groups.txt");
        Groups = new List<Groups>();
        var groupFileContent = File.ReadAllLines("groups.txt");
        if (groupFileContent.Length < 1)
        {
            Console.WriteLine("No current groups.");
        }else{
              // Process each line in the file
            foreach (var line in groupFileContent)
            {
                var splitted = line.Split(";", StringSplitOptions.RemoveEmptyEntries);

                var groupName = splitted[0];

                var group = new Groups(groupName);

                for (int i = 1; i < splitted.Length; i++)
                {
                    var userName = splitted[i];
                    var user = new User(userName); 
                    group.Users.Add(user); 
                }

                // Add the group to the GroupList
                Groups.Add(group);
            }

            Console.WriteLine("Groups have been loaded successfully.");
        }

        


        fileSaver = new FileSaver("workout-data.txt");
        WorkoutStoredData = new List<Workoutdata>();
        var workoutFileContent = File.ReadAllLines("workout-data.txt");
        if (workoutFileContent.Length < 1)
        {
            Console.WriteLine("No workout data found in the file.");
        }
        else
        {
            foreach (var line in workoutFileContent)
            {
                var splitted = line.Split(";", StringSplitOptions.RemoveEmptyEntries);

                var workoutName = splitted[0];
                var workout = new WorkoutName(workoutName);

                // Workout Duration (TimeSpan or int)
                var workoutDuration = float.Parse(splitted[1]);

                
                // User
                var userName = splitted[2];
                var user = new User(userName);

                // DateTime
                DateTime dateTime;
                if (!DateTime.TryParse(splitted[3], out dateTime))
                {
                    // Handle incorrect date format
                    Console.WriteLine($"Invalid date format: {splitted[3]}");
                    continue; // Skip this line
                }

                // Groups
                var group = splitted[4];
                var groups = new Groups(group);

                // Adding Workout data
                WorkoutStoredData.Add(new Workoutdata(workout, workoutDuration, user, dateTime, groups));
            }
        }

    
    }




    public void AddNewWorkoutData(Workoutdata data)
    {
        this.WorkoutStoredData.Add(data);
        this.fileSaver.AppendWorkoutData(data);
    }


    public bool UniqueUser(User user)
    {
        if (UserDictionary.ContainsKey(user))
        {
            Console.WriteLine("That username is already in use, please try another.");
            return false;
        }
        else
        {
            return true;
        }
    }

    public void SynchronizeUsers()
    {
        File.Delete("users.txt");
        List <string> usersToSave = new List<string>();
        foreach (var user in UserDictionary)
        {
            usersToSave.Add($"{user.Key}:{user.Value}");
        }
        File.WriteAllLines("users.txt", usersToSave);
    }

    public void AddUser(User user, UserPassword userPassword)
    {
        UserDictionary.Add(user, userPassword);
        SynchronizeUsers();
    }
    public List<Workoutdata> GetWorkoutsByName(string workoutName)
    {
        var filteredWorkouts = WorkoutStoredData.Where(workout => workout.WorkoutName.Name == workoutName).ToList();
        return filteredWorkouts;
    }

    public void SaveGroups()
    {
        List<string> lines = new List<string>();

        foreach (var group in Groups)
        {
            var line = group.Name + ";" + string.Join(";", group.Users.Select(u => u.Name));
            lines.Add(line);
        }

        File.WriteAllLines("groups.txt", lines);
        Console.WriteLine("Groups saved successfully.");
    }


    public void AddUserToGroup(string groupName, string userName)
    {
        var group = Groups.FirstOrDefault(g => g.Name == groupName);
        
        if (group != null)
        {
            // Check if the user is already in the group
            if (group.Users.Any(u => u.Name == userName))
            {
                Console.WriteLine($"User {userName} is already in the group {groupName}.");
            }
            else
            {
                // Add the user to the group
                group.Users.Add(new User(userName));
                SaveGroups();  // Save changes to file
                Console.WriteLine($"User {userName} added to group {groupName}.");
            }
        }
        else
        {
            Console.WriteLine($"Group {groupName} not found.");
        }
    }

    public void CreateGroup(string groupName, string userName)
    {
        var newGroup = new Groups(groupName);
        newGroup.Users.Add(new User(userName));
        Groups.Add(newGroup);
        SaveGroups();  // Save changes to file
        Console.WriteLine($"New group {groupName} created and {userName} added.");
    }   

}