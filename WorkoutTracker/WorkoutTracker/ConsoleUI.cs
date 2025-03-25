namespace WorkoutTracker; 
using Spectre.Console; 


public class ConsoleUI{
    DataManager dataManager;
 
    public ConsoleUI(){
        dataManager  = new DataManager();

    }

    public void Show(){
        Console.WriteLine("Select Mode: Login or Create Account");
        var mode = AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Please select mode").AddChoices(new[]{"Login","Create Account"}));


        if (mode == "Create Account"){
            Console.WriteLine("Enter User Name:");
            Console.WriteLine("Enter password:");
        }
      



    }
}
    
