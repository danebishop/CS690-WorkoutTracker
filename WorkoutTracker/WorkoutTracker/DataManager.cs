using System.ComponentModel.DataAnnotations;

namespace WorkoutTracker; 

public class DataManager{

    FileSaver fileSaver;
    public Dictionary <User, UserPassword> UserDictionary {get;}
    public List<Workoutdata> WorkoutStoredData {get;}
    public List<Groups> Group {get;}
  
 

    


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

        fileSaver = new FileSaver("workout-data.txt");
        WorkoutStoredData = new List<Workoutdata>();
        var workoutFileContent = File.ReadAllLines("workout-data.txt");
        foreach (var line in workoutFileContent){
            var splitted = line.Split(";", StringSplitOptions.RemoveEmptyEntries);

            var workoutName = splitted[0];
            var workout = new WorkoutName(workoutName);

            // Workout Duration (TimeSpan or int)
            var workoutTime = splitted[1];

            // Check if the workout duration is time-based or rep-based
            object workoutDuration = null;
            if (TimeSpan.TryParse(workoutTime, out var duration))
            {
                workoutDuration = duration;
            }
            else if (int.TryParse(workoutTime, out var reps))
            {
                workoutDuration = reps;
            }

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




    public void AddNewWorkoutData(Workoutdata data){
        this.WorkoutStoredData.Add(data);
        this.fileSaver.AppendWorkoutData(data);

    }


    public bool UniqueUser(User user){
        if (UserDictionary.ContainsKey(user)){
            Console.WriteLine("That username is already in use, please try another.");
            return false;
        }else{
            return true;
        }
    }

    public void SynchronizeUsers(){
        File.Delete("users.txt");
        List <string> usersToSave = new List<string>();
        foreach (var user in UserDictionary){
            usersToSave.Add($"{user.Key}:{user.Value}");
        }
        File.WriteAllLines("users.txt", usersToSave);
    }

    public void AddUser(User user, UserPassword userPassword){
        UserDictionary.Add(user, userPassword);
        SynchronizeUsers();
    }
    public List<Workoutdata> GetWorkoutsByName(string workoutName)
    {
        var filteredWorkouts = WorkoutStoredData.Where(workout => workout.WorkoutName.Name == workoutName).ToList();
        return filteredWorkouts;
    }


    

}