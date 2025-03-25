namespace WorkoutTracker; 

public class DataManager{

    FileSaver fileSaver;

    public Dictionary <User, UserPassword> UserDictionary {get;}
    public List<WorkoutName> WorkoutNames {get;}
    public List<Groups> Group {get;}
    public List<WorkoutDataReps> WorkoutDataReps{get;}
    public List<WorkoutDataTime> WorkoutDataTime{get;}
 

    


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



  





}