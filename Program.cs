char[,] userBoard = new char[10, 10]; // Users own board
char[,] computerBoard = new char[10, 10]; // Computers own board
char[,] userGuessBoard = new char[10, 10]; // Where the user attacks
char[,] computerGuessBoard = new char[10, 10]; // Where the computer attacks

List<int> shipSizes = new List<int> { 5, 4, 3, 3, 2 }; // Different ship lengths
int userShips = 0;
int computerShips = 0;
int theDelay = 2000;


ShowIntro(); // Shows instructions to user
await Task.Delay(theDelay*2); // Pauses code for 5 seconds

SetupBoards(); // Fills all boards with hidden positions
PlaceUserShips(); // Let's user place his ships
PlaceComputerShips(); // PLace computer ships randomly

while (userShips > 0 && computerShips > 0) // Main game loop
{
    Console.Clear();
    Console.WriteLine("Your Board:");
    ShowBoard(userBoard, true); // Prints users board
    Console.WriteLine("\nYour Guesses:");
    ShowBoard(userGuessBoard, false); // Prints user's attack board

    Console.WriteLine($"\nYour ships left: {userShips}");
    Console.WriteLine($"Computer ships left: {computerShips}");

    UserTurn();
    if (computerShips == 0) break; // End loop when computer has no ships left
    await Task.Delay(theDelay); // Pauses code for 2 seconds

    ComputerTurn();
    if (userShips == 0) break; // End loop when user had no ships left
    await Task.Delay(theDelay); // Pauses code for 2 seconds

}

ShowWinner(); // Game end, announces winner

// Methods
void ShowIntro() // User instructions
{
    Console.ForegroundColor = ConsoleColor.DarkRed;
    Console.WriteLine("=== Sink the Ship ===");
    Console.ResetColor();
    Console.WriteLine("You and the computer each have 5 ships.");
    Console.WriteLine("Take turns attacking. First to sink all enemy ships wins.");
    Console.WriteLine("Use numbers 0–9 to pick coordinates.");
    Console.WriteLine("Your ship size will start at 5 charachters long and go down in increments of 1.\n");
}

void SetupBoards() // Prints the boards
{
    for (int row = 0; row < 10; row++) // Loops through rows
    {
        for (int col = 0; col < 10; col++) // Loops through cols
        {
            userBoard[row, col] = ' '; // Sets up board using '~'
            computerBoard[row, col] = ' '; // Sets up board using '~'
            userGuessBoard[row, col] = ' '; // Sets up board using '~'
            computerGuessBoard[row, col] = ' '; // Sets up board using '~'
        }
    }
}

void ShowBoard(char[,] board, bool showShips) // Shows updated board
{
    Console.Write("  ");
    for (int i = 0; i < board.GetLength(1); i++) Console.Write(i + " "); // Prints column numbers
    Console.WriteLine();

    for (int row = 0; row < 10; row++) // Loops through each row
    {
        Console.Write(row + " "); // Prints row number 
        for (int col = 0; col < 10; col++) // Loops through each col in that row
        {
            char value = board[row, col]; // Gets current value from the board
            if (!showShips && value == 'S') value = ' '; // Hides ship if showShips == false
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.Write(value + " "); // Print the value 
            Console.ResetColor();
        }
        Console.WriteLine();
    }
}

void PlaceUserShips()
{
    for (int i = 0; i < shipSizes.Count; i++)
    {
        int size = shipSizes[i];
        bool placed = false;

        while (!placed)
        {
            Console.Clear();
            Console.WriteLine($"Place your ship #{i + 1} (size {size})");
            ShowBoard(userBoard, true);

            int row = AskForNumber("Start Row (0–9): ");
            int col = AskForNumber("Start Column (0–9): ");

            Console.Write("Direction (H for horizontal, V for vertical): "); // User chooses direction to place ship
            string direction = Console.ReadLine().ToUpper();
            bool horizontal = direction == "H";

            if (CanPlaceShip(userBoard, row, col, size, horizontal)) // "CanPlaceShip" is a method that checks so the ship doesnt overlap with another ship or is out of bounds.
            {
                PlaceShip(userBoard, row, col, size, horizontal); // Place ship
                userShips += size; // Increase number of userships
                placed = true;
            }
            else
            {
                Console.WriteLine("Invalid or overlapping. Press any key to try again.");
                Console.ReadKey();
            }
        }
    }
}

void PlaceComputerShips()
{
    Random rnd = new Random();

    for (int i = 0; i < shipSizes.Count; i++)
    {
        int size = shipSizes[i];
        bool placed = false;

        while (!placed)
        {
            int row = rnd.Next(0, 10);
            int col = rnd.Next(0, 10);
            bool horizontal = rnd.Next(2) == 0;

            if (CanPlaceShip(computerBoard, row, col, size, horizontal)) // Checks that ship is not out of bounds or overlapping with another ship
            {
                PlaceShip(computerBoard, row, col, size, horizontal); // Place ship
                computerShips += size; // Increases number of computerShips
                placed = true;
            }
        }
    }
}

bool CanPlaceShip(char[,] board, int row, int col, int size, bool horizontal) // Returns true if ship can be placed, else false
{
    if (horizontal)
    {
        if (col + size > 10) return false; // If column size is bigger than available columns, return false
        for (int i = 0; i < size; i++)
        {
            if (board[row, col + i] != ' ') return false; // if character space isn't available, return false
        }
    }
    else
    {
        if (row + size > 10) return false; // if row size is bigger than available rows, return false
        for (int i = 0; i < size; i++)
        {
            if (board[row + i, col] != ' ') return false; // if charachter space isn't available, return false
        }
    }
    return true;
}

void PlaceShip(char[,] board, int row, int col, int size, bool horizontal) // Places ship
{
    for (int i = 0; i < size; i++)
    {
        if (horizontal)
            board[row, col + i] = 'S';
        else
            board[row + i, col] = 'S';
    }
}

void UserTurn() // Users turn in game
{
    Console.WriteLine("\nYour turn to attack!");

    int row = AskForNumber("Attack row: "); // User chooses row to attack
    int col = AskForNumber("Attack column: "); // User chooses col to attack

    if (userGuessBoard[row, col] == 'X' || userGuessBoard[row, col] == 'O') // If position is already hit
    {
        Console.WriteLine("! Already attacked there. Try again."); // Ask user to re-attack
        UserTurn(); // Repeat
        return; // Ends this call of the method, and returns to the caller
    }

    if (computerBoard[row, col] == 'S') // If attacked position is a ship
    {
        Console.WriteLine("HIT!");
        userGuessBoard[row, col] = 'X'; // Update user's attack board
        computerBoard[row, col] = 'X'; // Update computer's board
        computerShips--; // Amount of computer ships is decreased by one
    }
    else // If attacked position is not a ship
    {
        Console.WriteLine("Miss.");
        userGuessBoard[row, col] = 'O'; // Update user's attack board
        computerBoard[row, col] = 'O'; // Update computer's board
    }
}

void ComputerTurn() // Computer's turn
{
    Console.WriteLine("\nComputer is thinking...");
    Thread.Sleep(theDelay);
    Random rnd = new Random(); 
    int row, col;

    do // Runs code at least once before checking the while loop condition
    {
        row = rnd.Next(0, 10); // Chooses random row to attack
        col = rnd.Next(0, 10); // Chooses random col to attack
    }while (computerGuessBoard[row, col] == 'X' || computerGuessBoard[row, col] == 'O'); // Repeats the attack if position is already taken
    
    if (userBoard[row, col] == 'S') // If attacked position is an user's ship
    {
        Console.WriteLine($"Computer hit your ship at ({row}, {col})!"); // Writes coordinates of the attacked ship
        Thread.Sleep(1000);
            computerGuessBoard[row, col] = 'X'; // Updates computer's attack board
        userBoard[row, col] = 'X'; // Updates user's board
        userShips--; // Decreases amount of user ships by one
    }
    else // If attacked position is not an user's ship
    {
        Console.WriteLine($"Computer missed at ({row}, {col})."); // Write coordinates of the missed positon
        Thread.Sleep(theDelay/2);
        computerGuessBoard[row, col] = 'O'; // Update computer's attack board
        if (userBoard[row, col] == '~') // if user's board position is not updated
            userBoard[row, col] = 'O'; // Marks a miss on the user's board
    }
}

int AskForNumber(string prompt) // Holds message to show to user
{
    int number;
    Console.Write(prompt); // Shows message to user
    while (!int.TryParse(Console.ReadLine(), out number) || number < 0 || number > 9) // If user input is invalid
    {
        Console.Write("Invalid input. " + prompt); // Ask user to choose again
    }
    return number; // Gives result and return it to the variable
}

void ShowWinner() // When game ends
{
    Console.Clear();
    Console.WriteLine("=== GAME OVER ===");

    if (userShips == 0) // If user has no ships left

        Console.WriteLine("You lost! All your ships were sunk. You suck!");
    else // If computer has no ships left
        Console.WriteLine("You won! You destroyed the computer's fleet. Lucky bastard!");
}
