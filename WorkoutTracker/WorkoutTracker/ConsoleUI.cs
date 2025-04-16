namespace WorkoutTracker;

using System.ComponentModel;
using Spectre.Console;

public class ConsoleUI
{
    private readonly DataManager dataManager;

    public ConsoleUI()
    {
        dataManager = new DataManager();
    }

    public void Show()
    {
        AnsiConsole.Write(new FigletText("Workout Tracker").Color(Color.Cyan1));
        var mode = AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Please select mode").AddChoices(new[]{"Login","Create Account"}));

        if (mode == "Create Account")
            HandleAccountCreation();
        else if (mode == "Login")
            HandleLogin();
    }

    private void HandleAccountCreation()
    {
        AnsiConsole.Write(new Markup("You selected to [green]Create an Account[/]\n"));
        string newUserName;
        do
        {
            newUserName = AnsiConsole.Prompt(new TextPrompt<string>("Enter new username(or 'exit' to quit):"));
            if (newUserName == "exit") return;
        } while (!dataManager.UniqueUser(new User(newUserName)));

        string password, confirmPassword;
        do
        {
            password = AnsiConsole.Prompt(new TextPrompt<string>("Enter password:"));
            confirmPassword = AnsiConsole.Prompt(new TextPrompt<string>("Re-enter password:"));
            if (password != confirmPassword)
                Console.WriteLine("Passwords don't match, please try again");
        } while (password != confirmPassword);

        dataManager.AddUser(new User(newUserName), new UserPassword(password));
    }

    private void HandleLogin()
    {
        AnsiConsole.Write(new FigletText("Login").Color(Color.Yellow2));
        while (true)
        {
            var userName = AnsiConsole.Prompt(new TextPrompt<string>("Enter your username(or 'exit' to quit):"));
            if (userName == "exit") return;

            var userToCheck = new User(userName);
            if (!dataManager.UserDictionary.TryGetValue(userToCheck, out var storedPassword))
            {
                Console.WriteLine("Username not found, try again");
                continue;
            }

            while (true)
            {
                var enteredPassword = AnsiConsole.Prompt(new TextPrompt<string>("Enter your password(or 'exit' to quit):"));
                if (enteredPassword == "exit") return;

                if (enteredPassword == storedPassword.ToString())
                {
                    Console.WriteLine("Login Successful");
                    MainMenu(userToCheck);
                    return;
                }

                Console.WriteLine("Incorrect password, please try again");
            }
        }
    }

    private void MainMenu(User currentUser)
    {
        bool continueNextSteps = true;
        while (continueNextSteps)
        {
            var selection = AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Please select mode").AddChoices(new[]{ "Workout Tracking","Workout Reports","Join Group","Logout" }));
            switch (selection)
            {
                case "Workout Tracking":
                    HandleWorkoutTracking(currentUser);
                    break;
                case "Workout Reports":
                    HandleWorkoutReports(currentUser);
                    break;
                case "Join Group":
                    HandleGroupManagement(currentUser);
                    break;
                case "Logout":
                    Console.WriteLine("Logging out...");
                    continueNextSteps = false;
                    break;
            }
        }
    }

    private void HandleWorkoutTracking(User currentUser)
    {
        AnsiConsole.Write(new FigletText("Workout Tracking").Color(Color.Purple3));
        var workoutNames = dataManager.WorkoutStoredData.Select(w => w.WorkoutName.Name).Distinct().ToList();
        var workoutType = PromptSelection("Workout Type", workoutNames.Concat(new[] { "New Workout" }));

        var name = workoutType == "New Workout"
            ? AnsiConsole.Prompt(new TextPrompt<string>("Enter new workout name:"))
            : workoutType;

        var durationInput = AnsiConsole.Prompt(new TextPrompt<string>("Enter repetitions or time (min,sec):"));
        if (!float.TryParse(durationInput, out float duration))
        {
            Console.WriteLine("Invalid input. Please enter a numeric value.");
            return;
        }

        var workout = new Workoutdata(new WorkoutName(name), duration, currentUser, DateTime.Now);
        dataManager.AddNewWorkoutData(workout);

        Console.WriteLine($"Workout '{name}' logged successfully.");
    }

    private void HandleWorkoutReports(User currentUser)
    {
        AnsiConsole.Write(new FigletText("Workout Reports").Color(Color.Green));
        var topLevelOption = PromptSelection("Choose report type", new[] { "Report Data", "Analyze Reports", "Group Data", "Exit" });
        AnsiConsole.Write(new Markup($"You selected [green]{topLevelOption}[/]\n"));

        if (topLevelOption == "Exit") return;

        if (topLevelOption == "Report Data")
        {
            var reportType = PromptSelection("Choose detail level", new[] { "Report All Data", "Report Your Data Only", "Exit" });
            AnsiConsole.Write(new Markup($"You selected [green]{reportType}[/]\n"));
            if (reportType == "Exit") return;

            var manager = new WorkoutManager(dataManager.WorkoutStoredData);
            var workoutNames = manager.GetUniqueWorkoutNames();
            var selected = PromptSelection("Select Workout Type", workoutNames);
            AnsiConsole.Write(new Markup($"You selected [green]{selected}[/] workout\n"));

            var data = reportType == "Report All Data"
                ? dataManager.GetWorkoutsByName(selected)
                : dataManager.GetWorkoutsByName(selected).Where(w => w.User.Name == currentUser.Name).ToList();

            var table = new Table();
            table.AddColumn("[green]Workout[/]").Centered();
            table.AddColumn("[green]Reps/Duration[/]").Centered();
            table.AddColumn("[green]Timestamp[/]").Centered();

            foreach (var w in data)
                table.AddRow(w.WorkoutName.ToString(), w.WorkoutDuration.ToString(), w.TimeStamp.ToString());

            AnsiConsole.Write(table);
        }
        else if (topLevelOption == "Analyze Reports")
        {
            var type = PromptSelection("Choose analysis type", new[] { "Analyze All Data", "Analyze Your Data Only", "Exit" });
            AnsiConsole.Write(new Markup($"You selected [green]{type}[/]\n"));
            if (type == "Exit") return;

            var manager = new WorkoutManager(dataManager.WorkoutStoredData);
            var workoutNames = manager.GetUniqueWorkoutNames();
            var selected = PromptSelection("Select Workout Type", workoutNames);
            AnsiConsole.Write(new Markup($"You selected [green]{selected}[/] workout\n"));

            var data = dataManager.GetWorkoutsByName(selected);
            if (type == "Analyze Your Data Only")
                data = data.Where(w => w.User.Name == currentUser.Name).ToList();

            var durations = data.Select(w => w.WorkoutDuration).OrderBy(d => d).ToList();
            if (!durations.Any())
            {
                AnsiConsole.MarkupLine("[red]No data to analyze.[/]");
                return;
            }

            var mean = durations.Average();
            var median = durations.Count % 2 == 0
                ? (durations[durations.Count / 2 - 1] + durations[durations.Count / 2]) / 2
                : durations[durations.Count / 2];

            var max = durations.Max();
            var min = durations.Min();
            var total = durations.Count;

            var table = new Table();
            table.AddColumn("[blue]Statistic[/]").Centered();
            table.AddColumn("[blue]Value[/]").Centered();
            table.AddRow("Mean", mean.ToString("0.##"));
            table.AddRow("Median", median.ToString("0.##"));
            table.AddRow("Max", max.ToString("0.##"));
            table.AddRow("Min", min.ToString("0.##"));
            table.AddRow("Total Entries", total.ToString());

            AnsiConsole.Write(table);
        }
        else if (topLevelOption == "Group Data")
        {
            var userGroups = dataManager.GetUserGroups(currentUser);
            if (!userGroups.Any())
            {
                AnsiConsole.MarkupLine("[red]You are not part of any group.[/]");
                return;
            }

            var group = PromptSelection("Select your group", userGroups);
            var groupUsers = dataManager.GetGroupUsers(group);
            var groupWorkouts = dataManager.WorkoutStoredData.Where(w => groupUsers.Contains(w.User)).ToList();

            var manager = new WorkoutManager(groupWorkouts);
            var common = manager.GetPopularWorkouts();

            if (!common.Any())
            {
                AnsiConsole.MarkupLine("[red]No group workouts with enough data.[/]");
                return;
            }

            var action = PromptSelection("Choose an option", new[] { "Report Group Workouts", "Analyze Group Workouts", "Exit" });
            if (action == "Exit") return;
            AnsiConsole.Write(new Markup($"You selected [green]{action}[/]\n"));

            var workout = PromptSelection("Select Workout", common);
            AnsiConsole.Write(new Markup($"You selected [green]{workout}[/]\n"));
            var selectedWorkouts = groupWorkouts.Where(w => w.WorkoutName.Name == workout).ToList();

            if (action == "Report Group Workouts")
            {
                
                var table = new Table();
                table.AddColumn("User").Centered();
                table.AddColumn("Workout").Centered();
                table.AddColumn("Duration").Centered();
                table.AddColumn("Timestamp").Centered();

                foreach (var w in selectedWorkouts)
                    table.AddRow(w.User.Name, w.WorkoutName.ToString(), w.WorkoutDuration.ToString("0.##"), w.TimeStamp.ToString("g"));
                
                AnsiConsole.Write(table);
            }
            else if (action == "Analyze Group Workouts")
            {
                var durations = selectedWorkouts.Select(w => w.WorkoutDuration).OrderBy(d => d).ToList();
                var mean = durations.Average();
                var median = durations.Count % 2 == 0
                    ? (durations[durations.Count / 2 - 1] + durations[durations.Count / 2]) / 2
                    : durations[durations.Count / 2];

                var lowest = selectedWorkouts.OrderBy(w => w.WorkoutDuration).First();
                var highest = selectedWorkouts.OrderByDescending(w => w.WorkoutDuration).First();

                var extremes = new Table();
                extremes.AddColumn("Type").Centered();
                extremes.AddColumn("User").Centered();
                extremes.AddColumn("Value").Centered();
                extremes.AddColumn("Date").Centered();
                extremes.AddRow("Lowest", lowest.User.Name, lowest.WorkoutDuration.ToString("0.##"), lowest.TimeStamp.ToString("g"));
                extremes.AddRow("Highest", highest.User.Name, highest.WorkoutDuration.ToString("0.##"), highest.TimeStamp.ToString("g"));

                var stats = new Table();
                stats.AddColumn("Statistic").Centered();
                stats.AddColumn("Value").Centered();
                stats.AddRow("Mean", mean.ToString("0.##"));
                stats.AddRow("Median", median.ToString("0.##"));

                AnsiConsole.Write(extremes);
                AnsiConsole.Write(stats);
            }
        }
    }

    private void HandleGroupManagement(User currentUser)
    {
        AnsiConsole.Write(new FigletText("Join a Group").Color(Color.Blue3));
        var groupNames = dataManager.Groups.Select(g => g.Name).Distinct().ToList();
        var selection = PromptSelection("Select a group or create new", groupNames.Concat(new[] { "New Group" }));

        if (selection == "New Group")
        {
            var newName = AnsiConsole.Prompt(new TextPrompt<string>("Enter name for new group:"));
            if (groupNames.Contains(newName))
            {
                if (PromptSelection("Group exists. Join?", new[] { "Yes", "No" }) == "Yes")
                {
                    dataManager.AddUserToGroup(newName, currentUser.Name);
                }
            }
            else
            {
                dataManager.CreateGroup(newName, currentUser.Name);
            }
        }
        else
        {
            dataManager.AddUserToGroup(selection, currentUser.Name);
        }
    }

    private string PromptSelection(string title, IEnumerable<string> choices)
    {
        return AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title(title)
                .AddChoices(choices)
        );
    }
}
