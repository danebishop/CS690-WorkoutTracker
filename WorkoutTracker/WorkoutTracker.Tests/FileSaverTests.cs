namespace WorkoutTracker.Tests;
using WorkoutTracker;


public class FileSaverTests
{
    FileSaver fileSaver;
    string testFileName;

    
    public FileSaverTests(){
        testFileName = "test-doc.txt";
        File.Delete(testFileName);
        fileSaver = new FileSaver(testFileName);
        
    }

    [Fact]
    public void Test_FileSaver_AppendLine()
    {
        fileSaver.AppendLine("Hello World");
        var contentFromFile = File.ReadAllText(testFileName);
        Assert.Equal("Hello World"+Environment.NewLine, contentFromFile);
        
    }

    [Fact]
    public void Test_FileSaver_AppendTimeData()
    {
        //WorkoutName sampleWorkout = new WorkoutName("sampleWorkout");
        //User sampleUser = new User("sampleUser");


    }
}/*
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
*/