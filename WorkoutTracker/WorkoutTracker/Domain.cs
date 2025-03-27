namespace WorkoutTracker;

public class User{
    public string Name{get;}
    public User(string name){
        this.Name = name;
    }
    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;
        User other = (User)obj;
        return this.Name == other.Name; 
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode();  
    }

    public override string ToString(){
        return this.Name;
    }
}

public class UserPassword{
    public string Password{get;}
    public UserPassword(string password){
        this.Password = password;
    }
    public override string ToString(){
        return this.Password;
    }
}
public class WorkoutName{
    public string Name{get;}
    public WorkoutName(string name){
        this.Name = name;
    }
    public override string ToString(){
        return this.Name;
    }
}


public class Groups
{
    public string Name { get; }
    public List<User> Users { get; }

    public Groups(string name)
    {
        Name = name;
        Users = new List<User>();
    }

    public override string ToString()
    {
        return this.Name;
    }
}


public class Workoutdata
{
    public WorkoutName WorkoutName { get; }
    public float WorkoutDuration{ get; } 
    public User User { get; }
    public DateTime TimeStamp { get; }
    public Groups Groups { get; }

    public Workoutdata(WorkoutName workoutName, float workoutDuration, User user, DateTime timeStamp, Groups groups)
    {
        this.WorkoutName = workoutName;
        this.WorkoutDuration = workoutDuration;
        this.User = user;
        this.TimeStamp = timeStamp;
        this.Groups = groups;
    }


}

public class WorkoutManager
    {
        private List<Workoutdata> WorkoutStoredData;

        public WorkoutManager(List<Workoutdata> workoutStoredData)
        {
            WorkoutStoredData = workoutStoredData;
        }

        // Method to get unique workout names
        public List<string> GetUniqueWorkoutNames()
        {
            // Using a HashSet to ensure uniqueness
            HashSet<string> uniqueWorkoutNames = new HashSet<string>();

            foreach (var workout in WorkoutStoredData)
            {
                uniqueWorkoutNames.Add(workout.WorkoutName.Name);  // Add the workout name to the HashSet
            }

            return uniqueWorkoutNames.ToList();
        }
    }