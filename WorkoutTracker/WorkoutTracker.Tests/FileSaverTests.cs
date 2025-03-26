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
    public void Test_FileSaver_AppendWorkoutData()
    {
        WorkoutName sampleWorkoutName = new WorkoutName("sampleWorkout");
        User sampleUser = new User("sampleUser");
        var sampleDuration = 10;
        DateTime sampleTimeStamp = DateTime.Now;
        var workoutGroup = "sampleGroup";
        Groups sampleGroup = new Groups(workoutGroup);
        Workoutdata sampleData = new Workoutdata(sampleWorkoutName, sampleDuration, sampleUser, sampleTimeStamp, sampleGroup);
        fileSaver.AppendWorkoutData(sampleData);
        var contentFromFile = File.ReadAllText(testFileName);
        Assert.Equal("sampleWorkout;10;sampleUser;"+sampleTimeStamp+";sampleGroup"+Environment.NewLine, contentFromFile);
    }
}
