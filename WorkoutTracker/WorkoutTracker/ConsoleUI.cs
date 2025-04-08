namespace WorkoutTracker;

using System.ComponentModel;
using Spectre.Console; 


public class ConsoleUI
{
    DataManager dataManager;
 
    public ConsoleUI()
    {
        dataManager  = new DataManager();
    }

    public void Show()
    {
        AnsiConsole.Write(new FigletText("Workout Tracker").Color(Color.Cyan1));
        var mode = AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Please select mode").AddChoices(new[]{"Login","Create Account"}));
        if (mode == "Create Account")
        {
            AnsiConsole.Write(new Markup("You selected to [green]Create an Account[/]\n"));

            bool isUnique;
            string newUserName, newPassword, newPassword2;
            newPassword = "";
            newPassword2= "_";
            do
            {
                newUserName = AnsiConsole.Prompt(new TextPrompt<string>("Enter new username(or 'exit' to quit):"));
                isUnique = dataManager.UniqueUser(new User(newUserName));
                if (newUserName == "exit")
                {
                    continue;
                }
                
            }while (isUnique == false);

            if (newUserName != "exit")
            {
                do
                {
                    newPassword = AnsiConsole.Prompt(new TextPrompt<string>("Enter password:"));
                    newPassword2 = AnsiConsole.Prompt(new TextPrompt<string>("Re-enter password:"));
                    if (newPassword != newPassword2)
                    {
                        Console.WriteLine("Passwords don't match, please try again");
                    }
                    else
                    {
                        continue;
                    }
                }while(newPassword != newPassword2);

            }
            if (newUserName != "exit")
            {
                dataManager.AddUser(new User(newUserName), new UserPassword(newPassword));
            }            
        } 

        if (mode == "Login")
        {
            AnsiConsole.Write(new FigletText("Login").Color(Color.Yellow2));
            string userName, enteredPassword;
            User currentUser = null;
            UserPassword userStoredPassword = null;
            do
            {
                userName = AnsiConsole.Prompt(new TextPrompt<string>("Enter your username(or 'exit' to quit):"));
                if (userName == "exit")
                {
                    break;
                }
                User userToCheck = new User(userName);
                bool userExists = dataManager.UserDictionary.ContainsKey(userToCheck);
                if(userExists)
                {
                    currentUser = userToCheck;
                    userStoredPassword = dataManager.UserDictionary[userToCheck];

                    bool passwordCorrect = false;
                    do
                    {
                        enteredPassword = AnsiConsole.Prompt(new TextPrompt<string>("Enter your password(or 'exit' to quit):"));
                        if (enteredPassword == "exit")
                        {
                            break;
                        }

                        if (enteredPassword == userStoredPassword.ToString())
                        {
                            Console.WriteLine("Login Successful");
                            //This is where all the other functionality goes
                            //
                            bool continueNextSteps = true;

                            while(continueNextSteps)
                            {
                                var selectionMode = AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Please select mode").AddChoices(new[]{"Workout Tracking","Workout Reports","Join Group","Logout"}));
                                if (selectionMode == "Join Group")
                                {
                                    AnsiConsole.Write(new FigletText("Join a Group").Color(Color.Blue3));
                                    
                                    // Get unique group names using a HashSet
                                    HashSet<string> uniqueGroupNames = new HashSet<string>();
                                    foreach (var group in dataManager.Groups)
                                    {
                                        uniqueGroupNames.Add(group.Name);  // Add the workout name to the HashSet
                                    }

                                    List<string> uniqueGroupNamesList = uniqueGroupNames.ToList();
                                    var userGroupChoice = AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Select the Group to join or form a new group:").AddChoices(uniqueGroupNamesList.Concat(new[] { "New Group" }).ToArray()));
                                    
                                    if (userGroupChoice == "New Group")
                                    {
                                        var newGroupName = AnsiConsole.Prompt(new TextPrompt<string>("Enter name of new group:"));                                        
                                        
                                        if (uniqueGroupNamesList.Contains(newGroupName))
                                        {
                                            var joinChoice = AnsiConsole.Prompt(new SelectionPrompt<string>().Title("This group already exists, would you like to join it instead?").AddChoices(new[] { "Yes", "No" }));
                                            
                                            if (joinChoice == "Yes")
                                            {
                                                // Add the user to the existing group
                                                dataManager.AddUserToGroup(newGroupName, currentUser.ToString());
                                                Console.WriteLine($"You have successfully joined the group: {newGroupName}");                                        
                                            }
                                            else
                                            {
                                                Console.WriteLine("Please enter a different group name.");
                                            }
                                        }                                    
                                        else
                                        {
                                            // Create the new group and add the user to it
                                            dataManager.CreateGroup(newGroupName, currentUser.ToString());
                                            Console.WriteLine($"You have successfully created the group: {newGroupName} and joined it.");                                    
                                        }
                                    }
                                    else
                                    {
                                        dataManager.AddUserToGroup(userGroupChoice, currentUser.ToString());
                                        Console.WriteLine($"You have successfully joined the group: {userGroupChoice}");
                                    
                                    }
                                }
                                else if (selectionMode == "Workout Tracking")
                                {
                                    AnsiConsole.Write(new FigletText("Workout Tracking").Color(Color.Purple3));
                                    
                                    // Get unique workout names using a HashSet
                                    HashSet<string> uniqueWorkoutNames = new HashSet<string>();
                                    foreach (var workout in dataManager.WorkoutStoredData)
                                    {
                                        uniqueWorkoutNames.Add(workout.WorkoutName.Name);  // Add the workout name to the HashSet
                                    }

                                    List<string> uniqueWorkoutNamesList = uniqueWorkoutNames.ToList();
                                    var workoutType = AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Workout Type").AddChoices(uniqueWorkoutNamesList.Concat(new[] { "New Workout" }).ToArray()));
                                    
                                    if (workoutType == "New Workout")
                                    {
                                        var newWorkoutName = AnsiConsole.Prompt(new TextPrompt<string>("Enter new workout name:"));
                                        var workoutDuration = float.Parse(AnsiConsole.Prompt(new TextPrompt<string>("Enter repetitions or time (min,sec):")));
                                        var workoutGroup = "none";
                                        WorkoutName workoutName = new WorkoutName(newWorkoutName);
                                        Groups group = new Groups(workoutGroup);
                                        DateTime timestamp = DateTime.Now;
                                        Workoutdata data = new Workoutdata(workoutName, workoutDuration, currentUser, timestamp);
                                        dataManager.AddNewWorkoutData(data);
                                    }
                                    else
                                    {
                                        AnsiConsole.Write(new Markup($"You selected [green]{workoutType}[/]\n"));
                                        WorkoutName workoutName = new WorkoutName(workoutType);
                                        var workoutDuration = float.Parse(AnsiConsole.Prompt(new TextPrompt<string>("Enter repetitions or time (min,sec):")));
                                        var workoutGroup = "none";
                                        Groups group = new Groups(workoutGroup);
                                        DateTime timestamp = DateTime.Now;
                                        Workoutdata data = new Workoutdata(workoutName, workoutDuration, currentUser, timestamp);
                                        dataManager.AddNewWorkoutData(data);
                                    }
                                }
                                else if(selectionMode == "Workout Reports")
                                {
                                    AnsiConsole.Write(new FigletText("Workout Reports").Color(Color.Orange3));
                                                                  
                                    
                                    var reportType = AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Choose [green]one[/] please:").AddChoices(new[]{"Report Data","Analyze Reports","Group Data","Exit"}));
                                    
                                    AnsiConsole.Write(new Markup($"You selected [green]{reportType}[/]\n"));
                                    if (reportType == "Report Data")
                                    {
                                        var reportDataType = AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Choose [green]one[/] please:").AddChoices(new[]{"Report All Data","Report Your Data Only","Exit"}));
                                        AnsiConsole.Write(new Markup($"You selected [green]{reportDataType}[/]\n"));

                                        if (reportDataType == "Report All Data")
                                        {
                                            WorkoutManager workoutManager = new WorkoutManager(dataManager.WorkoutStoredData);
                                            List<string> uniqueWorkoutNames = workoutManager.GetUniqueWorkoutNames();
                                            string workoutTypeReport = AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Workout Type").AddChoices(uniqueWorkoutNames.ToArray()));
                                            AnsiConsole.Write(new Markup($"You selected [green]{workoutTypeReport}[/]\n"));
                                                                                        
                                            List<Workoutdata> chosenWorkout = dataManager.GetWorkoutsByName(workoutTypeReport);

                                            // Create a report table
                                            var reportTable = new Spectre.Console.Table();
                                            reportTable.AddColumn("Workout").Centered();
                                            reportTable.AddColumn("Reps/Duration").Centered();
                                            reportTable.AddColumn("Timestamp").Centered();
                                        
                                            // Add some rows
                                            foreach (var workout in chosenWorkout)
                                            {
                                                reportTable.AddRow(workout.WorkoutName.ToString(), workout.WorkoutDuration.ToString(), workout.TimeStamp.ToString());
                                            }

                                            // Render the table to the console
                                            AnsiConsole.MarkupLine($"[bold green]Report for workout:[/] [white]{workoutTypeReport}[/]");
                                            AnsiConsole.Write(reportTable);  

                                        }
                                        else if (reportDataType =="Report Your Data Only")
                                        {
                                            WorkoutManager workoutManager = new WorkoutManager(dataManager.WorkoutStoredData);
                                            List<string> uniqueWorkoutNames = workoutManager.GetUniqueWorkoutNames();
                                            string workoutTypeReport = AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Workout Type").AddChoices(uniqueWorkoutNames.ToArray()));
                                            AnsiConsole.Write(new Markup($"You selected [green]{workoutTypeReport}[/]\n"));
                                            
                                            
                                            List<Workoutdata> allWorkouts = dataManager.GetWorkoutsByName(workoutTypeReport);
                                            List<Workoutdata> chosenWorkout = allWorkouts.Where(workout => workout.User.Name == currentUser.Name).ToList();
                                            
                                            // Create a report table
                                            var reportTable = new Spectre.Console.Table();
                                            reportTable.AddColumn("Workout").Centered();
                                            reportTable.AddColumn("Reps/Duration").Centered();
                                            reportTable.AddColumn("Timestamp").Centered();

                                            // Add rows to the table
                                            foreach (var workout in chosenWorkout)
                                            {
                                                reportTable.AddRow(workout.WorkoutName.ToString(), workout.WorkoutDuration.ToString(), workout.TimeStamp.ToString());
                                            }

                                            // Render the table to the console
                                            AnsiConsole.MarkupLine($"[bold green]Report for workout:[/] [white]{workoutTypeReport}[/]");
                                            AnsiConsole.Write(reportTable);

                                        }
                                        else
                                        {
                                            AnsiConsole.Write(new Markup("You selected[red] Exit [/]\n"));
                                            continue;
                                        }

                                    }
                                    else if(reportType == "Analyze Reports")
                                    {
                                        var analyzeDataType = AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Choose [green]one[/] please:").AddChoices(new[]{"Analyze All Data","Analyze Your Data Only","Exit"}));
                                        
                                        if (analyzeDataType == "Analyze All Data")
                                        {
                                            AnsiConsole.Write(new Markup("You selected to[green] Analyze All Data [/].\n"));
                                            WorkoutManager workoutManager = new WorkoutManager(dataManager.WorkoutStoredData);
                                            List<string> uniqueWorkoutNames = workoutManager.GetUniqueWorkoutNames();
                                            var workoutTypeAnalyze = AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Workout Type").AddChoices(uniqueWorkoutNames.ToArray()));
                                            AnsiConsole.Write(new Markup($"You selected [green] {workoutTypeAnalyze} [/]\n"));

                                            // Get all workouts by the selected workout name
                                            List<Workoutdata> allWorkouts = dataManager.GetWorkoutsByName(workoutTypeAnalyze);

                                            // Extract workout durations
                                            List<float> durations = allWorkouts.Select(workout => workout.WorkoutDuration).ToList();

                                            // Calculate Mean
                                            float mean = durations.Average();

                                            // Calculate Median
                                            float median;
                                            var sortedDurations = durations.OrderBy(d => d).ToList();
                                            int count = sortedDurations.Count;
                                            if (count % 2 == 0)
                                            {
                                                // Average of the two middle numbers if even
                                                median = (sortedDurations[count / 2 - 1] + sortedDurations[count / 2]) / 2;
                                            }
                                            else
                                            {
                                                // Middle number if odd
                                                median = sortedDurations[count / 2];
                                            }

                                            // Calculate Max, min & number of workouts
                                            float maxDuration = durations.Max();
                                            float minDuration = durations.Min();
                                            int timesLogged = allWorkouts.Count;

                                            // Create a report table for the statistics
                                            var statsTable = new Spectre.Console.Table();
                                            // Add columns and label them
                                            statsTable.AddColumn("Statistic").Centered();
                                            statsTable.AddColumn("Value").Centered();

                                            // Add rows with statistics
                                            statsTable.AddRow("Mean Duration", $"{mean} minutes");
                                            statsTable.AddRow("Median Duration", $"{median} minutes");
                                            statsTable.AddRow("Maximum Duration", $"{maxDuration} minutes");
                                            statsTable.AddRow("Minimum Duration", $"{minDuration} minutes");
                                            statsTable.AddRow("Times Logged", $"{timesLogged}");

                                            // Render the statistics table to the console
                                            AnsiConsole.MarkupLine($"[bold green]Analysis for workout:[/] [white]{workoutTypeAnalyze}[/]");
                                            AnsiConsole.Write(statsTable);
                                            continue;

                                        }
                                        else if (analyzeDataType =="Analyze Your Data Only")
                                        {
                                            AnsiConsole.Write(new Markup("You selected to[green] Analyze Your Data [/]only.\n"));                                        
                                            WorkoutManager workoutManager = new WorkoutManager(dataManager.WorkoutStoredData);
                                            List<string> uniqueWorkoutNames = workoutManager.GetUniqueWorkoutNames();
                                            var workoutTypeAnalyze = AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Workout Type").AddChoices(uniqueWorkoutNames.ToArray()));
                                            AnsiConsole.Write(new Markup($"You selected [green] {workoutTypeAnalyze} [/]\n"));
                                            
                                           
                                            // Get all workouts by the selected workout name
                                            List<Workoutdata> allWorkouts = dataManager.GetWorkoutsByName(workoutTypeAnalyze);

                                            // Filter workouts for the specific currentUser based on UserId
                                            List<Workoutdata> chosenWorkout = allWorkouts.Where(workout => workout.User.Name == currentUser.Name).ToList();

                                            // Extract workout durations
                                            List<float> durations = chosenWorkout.Select(workout => workout.WorkoutDuration).ToList();

                                            // Calculate Mean
                                            float mean = durations.Average();

                                            // Calculate Median
                                            float median;
                                            var sortedDurations = durations.OrderBy(d => d).ToList();
                                            int count = sortedDurations.Count;
                                            if (count % 2 == 0)
                                            {
                                                // Average of the two middle numbers if even
                                                median = (sortedDurations[count / 2 - 1] + sortedDurations[count / 2]) / 2;
                                            }
                                            else
                                            {
                                                // Middle number if odd
                                                median = sortedDurations[count / 2];
                                            }

                                            // Calculate Max, min & number of workouts
                                            float maxDuration = durations.Max();
                                            float minDuration = durations.Min();
                                            int timesLogged = chosenWorkout.Count;

                                            // Create a report table for the statistics
                                            var statsTable = new Spectre.Console.Table();
                                            // Add columns and label them
                                            statsTable.AddColumn("Statistic").Centered();
                                            statsTable.AddColumn("Value").Centered();

                                            // Add rows with statistics
                                            statsTable.AddRow("Mean Duration", $"{mean} minutes");
                                            statsTable.AddRow("Median Duration", $"{median} minutes");
                                            statsTable.AddRow("Maximum Duration", $"{maxDuration} minutes");
                                            statsTable.AddRow("Minimum Duration", $"{minDuration} minutes");
                                            statsTable.AddRow("Times Logged", $"{timesLogged}");

                                            // Render the statistics table to the console
                                            AnsiConsole.MarkupLine($"[bold green]Analysis for workout:[/] [white]{workoutTypeAnalyze}[/]");
                                            AnsiConsole.Write(statsTable);
                                            

                                        }

                                        else
                                        {
                                            AnsiConsole.Write(new Markup("You selected to[red] Exit [/].\n"));
                                            continue;
                                        }

                                    }
                                    else if(reportType == "Group Data")
                                    {    
                                        AnsiConsole.Write(new Markup("You selected to[green] Examine Group Data [/]\n"));

                                        List<string> userGroups = dataManager.GetUserGroups(currentUser);

                                        if (userGroups.Count == 0)
                                        {
                                            Console.WriteLine("You are not part of any group.");
                                            continue;
                                        }

                                        var selectedGroup = AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Select the group you want to report or analyze:").AddChoices(userGroups));

                                        var groupOption = AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Select an option:").AddChoices(new[] { "Report Group Workouts", "Analyze Group Workouts", "Exit" }));

                                        if (groupOption == "Report Group Workouts")
                                        {
                                            AnsiConsole.Write(new Markup("You selected to[green] Report Group Workouts [/]\n"));
                                            var groupWorkouts = dataManager.GetGroupWorkouts(selectedGroup);
                                            if (groupWorkouts.Count == 0)
                                                {
                                                    AnsiConsole.MarkupLine("[red]No workouts found for this group.[/]");
                                                    continue;
                                                }

                                            //Get workouts that have at least 2 data entries    
                                            var workoutCounts = groupWorkouts.GroupBy(w => w.WorkoutName.Name).Where(g => g.Count() >= 2).ToDictionary(g => g.Key, g => g.Count());
                                            if (workoutCounts.Count == 0)
                                            {
                                                AnsiConsole.MarkupLine("[yellow]No workouts found with at least two entries in this group.[/]");
                                                continue;
                                            }

                                            var groupWorkoutSelection = AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Choose a workout to report on:").AddChoices(workoutCounts.Keys));
                                            
                                            var chosenWorkout = groupWorkouts.Where(w => w.WorkoutName.Name == groupWorkoutSelection).ToList();
                                                // Build report table
                                            var reportTable = new Table();
                                            reportTable.AddColumn("[yellow]User[/]");
                                            reportTable.AddColumn("[yellow]Duration (min)[/]");
                                            reportTable.AddColumn("[yellow]Date[/]");

                                            foreach (var workout in chosenWorkout)
                                            {
                                                reportTable.AddRow(
                                                    workout.User.ToString(),
                                                    workout.WorkoutDuration.ToString("0.##"),
                                                    workout.TimeStamp.ToShortDateString()
                                                );
                                            }

                                            AnsiConsole.MarkupLine($"[bold green]Report for workout:[/] [white]{groupWorkoutSelection}[/]");
                                            AnsiConsole.Write(reportTable);
                                            
                                        }
                                        else if (groupOption == "Analyze Group Workouts")
                                        {
                                            AnsiConsole.Write(new Markup("You selected to[green] Analyze Group Workouts [/]\n"));
                                            var groupWorkouts = dataManager.GetGroupWorkouts(selectedGroup);
                                            if (groupWorkouts.Count == 0)
                                                {
                                                    AnsiConsole.MarkupLine("[red]No workouts found for this group.[/]");
                                                    continue;
                                                }

                                            //Get workouts that have at least 2 data entries    
                                            var workoutCounts = groupWorkouts.GroupBy(w => w.WorkoutName.Name).Where(g => g.Count() >= 2).ToDictionary(g => g.Key, g => g.Count());
                                            if (workoutCounts.Count == 0)
                                            {
                                                AnsiConsole.MarkupLine("[yellow]No workouts found with at least two entries in this group.[/]");
                                                continue;
                                            }

                                            var groupWorkoutSelection = AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Choose a workout to report on:").AddChoices(workoutCounts.Keys));
                                            
                                            var chosenWorkout = groupWorkouts.Where(w => w.WorkoutName.Name == groupWorkoutSelection).ToList();
                                                // Extract workout durations for the selected group workout
                                            List<float> durations = chosenWorkout.Select(workout => workout.WorkoutDuration).ToList();

                                            // Calculate Mean
                                            float mean = durations.Average();

                                            // Calculate Median
                                            float median;
                                            var sortedDurations = durations.OrderBy(d => d).ToList();
                                            int count = sortedDurations.Count;
                                            if (count % 2 == 0)
                                            {
                                                // Average of the two middle numbers if even
                                                median = (sortedDurations[count / 2 - 1] + sortedDurations[count / 2]) / 2;
                                            }
                                            else
                                            {
                                                // Middle number if odd
                                                median = sortedDurations[count / 2];
                                            }

                                            // Calculate Max, min & number of workouts
                                            float maxDuration = durations.Max();
                                            float minDuration = durations.Min();
                                            int timesLogged = chosenWorkout.Count;

                                            // Create a report table for the group workout statistics
                                            var statsTable = new Spectre.Console.Table();
                                            statsTable.AddColumn("Statistic").Centered();
                                            statsTable.AddColumn("Value").Centered();

                                            // Add rows with statistics
                                            statsTable.AddRow("Mean Duration", $"{mean} minutes");
                                            statsTable.AddRow("Median Duration", $"{median} minutes");
                                            statsTable.AddRow("Maximum Duration", $"{maxDuration} minutes");
                                            statsTable.AddRow("Minimum Duration", $"{minDuration} minutes");
                                            statsTable.AddRow("Times Logged", $"{timesLogged}");

                                            // Render the statistics table to the console
                                            AnsiConsole.MarkupLine($"[bold green]Analysis for group workout:[/] [white]{groupWorkoutSelection}[/]");
                                            AnsiConsole.Write(statsTable);

                                            // Build a detailed report table with individual workout data
                                            var reportTable = new Table();
                                            reportTable.AddColumn("[yellow]User[/]");
                                            reportTable.AddColumn("[yellow]Duration (min)[/]");
                                            reportTable.AddColumn("[yellow]Date[/]");

                                            foreach (var workout in chosenWorkout)
                                            {
                                                reportTable.AddRow(
                                                    workout.User.ToString(),
                                                    workout.WorkoutDuration.ToString("0.##"),
                                                    workout.TimeStamp.ToShortDateString()
                                                );
                                            }

                                            AnsiConsole.Write(reportTable);
                                            


                                        }
                                        else
                                        {
                                            Console.WriteLine("Exiting group data options.");
                                        }
                                                                            
                                    }
                                    else
                                    {
                                        AnsiConsole.Write(new Markup("You selected to[red] Exit [/].\n"));
                                        continue;
                                    }

                                }else{

                                    Console.WriteLine("Logging out...");
                                    continueNextSteps = false;
                                }
                                
                            }
                            break;//Break out of password loop since login was successful

                        } 
                        
                        else
                        {
                            Console.WriteLine("Incorrect password, please try again");
                        }
                    }while(!passwordCorrect);
                    
                    if (enteredPassword == "exit")
                    {
                        break;
                    }
                }
                
                else
                {
                    Console.WriteLine("Username not found, try again");
                }
            }while(true);
    
        }   
    }
    

}
    
