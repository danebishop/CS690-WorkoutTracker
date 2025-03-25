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



}