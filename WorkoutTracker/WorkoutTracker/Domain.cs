namespace WorkoutTracker;

public class User{
    public string Name{get;}
    public User(string name){
        this.Name = name;
    }
    public override string ToString(){
        return this.Name;
    }
}

public class UserPassword{
    public string Name{get;}
    public UserPassword(string password){
        this.Name = password;
    }
    public override string ToString(){
        return this.Name;
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

public class Groups{
    public string Name {get;}
    public List<User> Users {get;}
    public Groups(string name){
        this.Name = name;
        this.Users = new List<User>();
    }
    public override string ToString(){
        return this.Name;
    }
}



public class WorkoutDataReps{
    public WorkoutName WorkoutName{get;}
    public int workReps{get;}
    public User User {get;}
    public DateTime TimeStamp {get;}
    public Groups Groups{get;}

    public WorkoutDataReps(WorkoutName workoutname, int workReps, User user, DateTime timeStamp, Groups groups){
        this.WorkoutName = workoutname; 
        this.workReps = workReps;
        this.User = user;
        this.TimeStamp = timeStamp;
        this.Groups = groups;
    }

}

public class WorkoutDataTime{
    public WorkoutName WorkoutName{get;}
    public TimeSpan workTime{get;}
    public User User {get;}
    public DateTime TimeStamp {get;}
    public Groups Groups{get;}

    public WorkoutDataTime(WorkoutName workoutname, TimeSpan workTime, User user, DateTime timeStamp, Groups groups){
        this.WorkoutName = workoutname; 
        this.workTime = workTime;
        this.User = user;
        this.TimeStamp = timeStamp;
        this.Groups = groups;
    }

}

