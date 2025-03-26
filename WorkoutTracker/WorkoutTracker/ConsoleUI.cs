namespace WorkoutTracker;

using System.ComponentModel;
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
        if (mode == "Login"){
            string userName, enteredPassword;
            User currentUser = null;
            UserPassword userStoredPassword = null;
            do{
                userName = AnsiConsole.Prompt(new TextPrompt<string>("Enter your username(or 'exit' to quit):"));
                if (userName == "exit"){
                    break;
                }
                User userToCheck = new User(userName);
                bool userExists = dataManager.UserDictionary.ContainsKey(userToCheck);
                if(userExists){
                    currentUser = userToCheck;
                    userStoredPassword = dataManager.UserDictionary[userToCheck];

                    bool passwordCorrect = false;
                    do{
                        enteredPassword = AnsiConsole.Prompt(new TextPrompt<string>("Enter your password(or 'exit' to quit):"));
                        if (enteredPassword == "exit"){
                            break;
                        }
                        if (enteredPassword == userStoredPassword.ToString()){
                            Console.WriteLine("Login Successful");
                            //This is where all the other functionality goes
                            //
                            bool continueNextSteps = true;
                            while(continueNextSteps){
                                var selectionMode = AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Please select mode").AddChoices(new[]{"Workout Tracking","Workout Reports","Logout"}));
                                if (selectionMode == "Workout Tracking"){
                                    // Get unique workout names using a HashSet
                                    HashSet<string> uniqueWorkoutNames = new HashSet<string>();
                                    foreach (var workout in dataManager.WorkoutStoredData){
                                        uniqueWorkoutNames.Add(workout.WorkoutName.Name);  // Add the workout name to the HashSet
                                    }
                                    List<string> uniqueWorkoutNamesList = uniqueWorkoutNames.ToList();

                                    var workoutType = AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Workout Type").AddChoices(uniqueWorkoutNamesList.Concat(new[] { "New Workout" }).ToArray()));
                                    
                                    if (workoutType == "New Workout"){
                                        var newWorkoutName = AnsiConsole.Prompt(new TextPrompt<string>("Enter new workout name:"));
                                        var workoutDuration = AnsiConsole.Prompt(new TextPrompt<string>("Enter repetitions or time (min,sec):"));
                                        var workoutGroup = "none";
                                        WorkoutName workoutName = new WorkoutName(newWorkoutName);
                                        Groups group = new Groups(workoutGroup);
                                        DateTime timestamp = DateTime.Now;
                                        Workoutdata data = new Workoutdata(workoutName, workoutDuration, currentUser, timestamp, group);
                                        dataManager.AddNewWorkoutData(data);
                                    }else{
                                        Console.WriteLine("You selected "+ workoutType);
                                        WorkoutName workoutName = new WorkoutName(workoutType);
                                        var workoutDuration = AnsiConsole.Prompt(new TextPrompt<string>("Enter repetitions or time (min,sec):"));
                                        var workoutGroup = "none";
                                        Groups group = new Groups(workoutGroup);
                                        DateTime timestamp = DateTime.Now;
                                        Workoutdata data = new Workoutdata(workoutName, workoutDuration, currentUser, timestamp, group);
                                        dataManager.AddNewWorkoutData(data);

                                    }

                                }else if(selectionMode == "Workout Reports"){
                                    Console.WriteLine("WorkoutReporting");
                                    var reportType = AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Choose [green]one[/] please:").AddChoices(new[]{"Report Data","Analyze Reports","Group Data","Exit"}));
                                    if (reportType == "Report Data"){
                                        Console.WriteLine("Report Data CHOSEN");
                                        var reportDataType = AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Choose [green]one[/] please:").AddChoices(new[]{"Report All Data","Report Your Data Only","Exit"}));
                                        if (reportDataType == "Report All Data"){
                                            Console.WriteLine("Report All Data CHOSEN");
                                            WorkoutManager workoutManager = new WorkoutManager(dataManager.WorkoutStoredData);
                                            List<string> uniqueWorkoutNames = workoutManager.GetUniqueWorkoutNames();
                                            string workoutTypeReport = AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Workout Type").AddChoices(uniqueWorkoutNames.Concat(new[] { "New Workout" }).ToArray()));
                                            Console.WriteLine(workoutTypeReport+"CHOSEN");
                                            
                                            List<Workoutdata> chosenWorkout = dataManager.GetWorkoutsByName(workoutTypeReport);

                                            var table = new Table();
                                            // Add some columns&Label them
                                            table.AddColumn(new TableColumn(workoutTypeReport).Centered());
                                            table.AddColumn(new TableColumn("Reps/Duration").Centered());
                                            table.AddColumn(new TableColumn("Timestamp").Centered());
                                            // Add some rows
                                            foreach (var workout in chosenWorkout)
                                            {
                                                table.AddRow($"Workout Name: {workout.WorkoutName.Name}, Duration: {workout.GetWorkoutDuration()}, Date: {workout.TimeStamp}");
                                            }
                                
                                            // Render the table to the console
                                            AnsiConsole.Write(table);
                                            


                                        }else if (reportDataType =="Report Your Data Only"){
                                            Console.WriteLine("Report Your Data Only CHOSEN");
                                            WorkoutManager workoutManager = new WorkoutManager(dataManager.WorkoutStoredData);
                                            List<string> uniqueWorkoutNames = workoutManager.GetUniqueWorkoutNames();
                                            string workoutTypeReport = AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Workout Type").AddChoices(uniqueWorkoutNames.Concat(new[] { "New Workout" }).ToArray()));
                                            Console.WriteLine(workoutTypeReport+"CHOSEN");

                                        }else{
                                            Console.WriteLine("Exit CHOSEN");
                                            continue;
                                        }

                                    }else if(reportType == "Analyze Reports"){
                                        Console.WriteLine("Analyze Reports CHOSEN");
                                        var analyzeDataType = AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Choose [green]one[/] please:").AddChoices(new[]{"Analyze All Data","Analyze Your Data Only","Exit"}));
                                        if (analyzeDataType == "Analyze All Data"){
                                            Console.WriteLine("Analyze All Data CHOSEN");                            
                                            WorkoutManager workoutManager = new WorkoutManager(dataManager.WorkoutStoredData);
                                            List<string> uniqueWorkoutNames = workoutManager.GetUniqueWorkoutNames();
                                            var workoutTypeAnalyze = AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Workout Type").AddChoices(uniqueWorkoutNames.Concat(new[] { "New Workout" }).ToArray()));
                                            Console.WriteLine(workoutTypeAnalyze+"CHOSEN");

                                        }else if (analyzeDataType =="Analyze Your Data Only"){
                                            Console.WriteLine("Analyze Your Data Only CHOSEN");                                            
                                            WorkoutManager workoutManager = new WorkoutManager(dataManager.WorkoutStoredData);
                                            List<string> uniqueWorkoutNames = workoutManager.GetUniqueWorkoutNames();
                                            var workoutTypeAnalyze = AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Workout Type").AddChoices(uniqueWorkoutNames.Concat(new[] { "New Workout" }).ToArray()));
                                            Console.WriteLine(workoutTypeAnalyze+"CHOSEN");

                                        }else{
                                            Console.WriteLine("Exit CHOSEN");
                                            continue;
                                        }

                                    }else if(reportType == "Group Data"){
                                        Console.WriteLine("Examine Group Data CHOSEN");

                                    }else{
                                        Console.WriteLine("Exit CHOSEN");
                                        continue;
                                    }


                                }else{
                                    Console.WriteLine("Logging out...");
                                    continueNextSteps = false;
                                }
                                
                            }
                            break;//Break out of password loop since login was successful

                        }else{
                            Console.WriteLine("Incorrect password, please try again");
                        }
                    }while(!passwordCorrect);
                    if (enteredPassword == "exit"){
                        break;
                    }
                }else{
                    Console.WriteLine("Username not found, try again");
                }
            }while(true);
    

        }
      


    }
}
    
