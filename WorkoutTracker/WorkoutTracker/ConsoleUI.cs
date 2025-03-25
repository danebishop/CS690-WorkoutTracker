namespace WorkoutTracker; 
using Spectre.Console; 


public class ConsoleUI{
    DataManager dataManager;
 
    public ConsoleUI(){
        dataManager  = new DataManager();

    }

    public void Show(){
    
        var mode = AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Please select mode").AddChoices(new[]{"Login","Create Account"}));

        if (mode == "Create Account"){
            bool isUnique;
            string newUserName, newPassword, newPassword2;
            newPassword = "";
            newPassword2= "_";
            do{
                newUserName = AnsiConsole.Prompt(new TextPrompt<string>("Enter new username(or 'exit' to quit):"));
                isUnique = dataManager.UniqueUser(new User(newUserName));
                if (newUserName == "exit"){
                    continue;
                }
                
            }while (isUnique == false);
            if (newUserName != "exit"){
                do{
                    newPassword = AnsiConsole.Prompt(new TextPrompt<string>("Enter password:"));
                    newPassword2 = AnsiConsole.Prompt(new TextPrompt<string>("Re-enter password:"));
                    if (newPassword != newPassword2){
                        Console.WriteLine("Passwords don't match, please try again");
                    }else{
                        continue;
                    }
                }while(newPassword != newPassword2);
            }
            if (newUserName != "exit"){
                dataManager.AddUser(new User(newUserName), new UserPassword(newPassword));
            }

        }
      



    }
}
    
