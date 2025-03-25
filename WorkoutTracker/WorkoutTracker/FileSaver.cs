namespace WorkoutTracker; 
using System.IO; 

public class FileSaver{
    string fileName; 

    public FileSaver(string fileName){
        this.fileName = fileName;
        if (!File.Exists(this.fileName)){
            File.Create(this.fileName).Close();
        }
    }

    //making a new funciton to just append a line to the file 
    public void AppendLine(string line){
        File.AppendAllText(this.fileName, line + Environment.NewLine);
    }

    public void AppendDataTime(WorkoutDataTime data){
        File.AppendAllText(this.fileName, data.WorkoutName + ":" + data.workTime + ":" + data.User + ":" + data.TimeStamp + ":" + data.Groups + Environment.NewLine);
    }

    public void AppendDataReps(WorkoutDataReps data){
        File.AppendAllText(this.fileName, data.WorkoutName + ":" + data.workReps + ":" + data.User + ":" + data.TimeStamp + ":" + data.Groups + Environment.NewLine);
    }

}


