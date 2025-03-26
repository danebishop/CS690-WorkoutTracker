namespace WorkoutTracker.Tests;
using WorkoutTracker;


public class DataManagerTests
{
    DataManager dataManager;
  

    public DataManagerTests(){
        File.WriteAllText("users.txt", "sampleUser:samplePassword" + Environment.NewLine + "sampleUser1:samplePassword1"); 
        File.WriteAllText("workout-data.txt","");
        
        dataManager = new DataManager(); 
        
        WorkoutName sampleWorkoutName = new WorkoutName("sampleWorkoutName");
        Groups sampleGroup = new Groups("sampleGroup");
        User sampleUser = new User("sampleUser");
        DateTime timestamp = DateTime.Now;
        int workoutDuration=10;
        Workoutdata sampleData = new Workoutdata(sampleWorkoutName, workoutDuration, sampleUser, timestamp, sampleGroup);
        dataManager.AddNewWorkoutData(sampleData);
    }

    [Fact]
    public void Test_AddUser(){
        Assert.Equal(2, dataManager.UserDictionary.Count);
        dataManager.AddUser(new User("sampleUser2"), new UserPassword("samplePassword2"));
        Assert.Equal(3, dataManager.UserDictionary.Count);
    }

    [Fact]
    public void Test_Unique_User_False(){
        Assert.Equal(false, dataManager.UniqueUser(new User("sampleUser")));
    }

    [Fact]
    public void Test_Unique_User_True(){
        Assert.Equal(true, dataManager.UniqueUser(new User("sampleUser99")));
    }

    [Fact]
    public void Test_Add_New_Workout_Data(){
        Assert.Equal(1, dataManager.WorkoutStoredData.Count);
        WorkoutName sampleWorkoutName = new WorkoutName("sampleWorkoutName");
        Groups sampleGroup = new Groups("sampleGroup");
        User sampleUser = new User("sampleUser");
        DateTime timestamp = DateTime.Now;
        int workoutDuration=10;
        Workoutdata sampleData = new Workoutdata(sampleWorkoutName, workoutDuration, sampleUser, timestamp, sampleGroup);
        dataManager.AddNewWorkoutData(sampleData);
        Assert.Equal(2, dataManager.WorkoutStoredData.Count);

    }

}