using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using static System.Console;
using System.Globalization;

namespace CarInventoryManagement
{
    class CarInventoryManagement
    {
        static void Main()
        {
            // Declarations
            const int SENTINEL_VALUE = 4;
            int menuChoice = 0;
            var carsJson = File.ReadAllText(Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName, "cars.json")); // Our local JSON database cars.json is read to the project.
            List<Car> cars = JsonSerializer.Deserialize<List<Car>>(carsJson); // Locally-saved JSON database is deserialized and converted to the console-side list that users can view, add to, and remove from.

            // Stylized ASCII welcome screen
            WriteLine(" ________________________________________________________________ \r\n|  ____________________________________________________________  |\r\n| | Welcome to the Car Dealership Inventory Management System. | |\r\n| |____________________________________________________________| |\r\n|________________________________________________________________|");
            WriteLine(" ");

            // Menu screen, which will loop until the user enters 4 (sentinel value) to quit the program.
            while (menuChoice != SENTINEL_VALUE)
            {
                menuChoice = 0;
                WriteLine("== MENU ==");
                WriteLine("1. View inventory");
                WriteLine("2. Add to inventory");
                WriteLine("3. Remove from inventory");
                WriteLine("4. Quit");
                while (menuChoice < 1 || menuChoice > 4) // While user input does not match 1 through 4 (menu options):
                {
                    Write("Please select a valid option (e.g. 2): ");
                    try
                    {
                        menuChoice = Convert.ToInt32(ReadLine());
                    }
                    catch // If an error occurs trying to convert user input to an integer, set menuChoice to 0 and start loop over
                    {
                        WriteLine("Input must be a valid number!");
                        menuChoice = 0;
                    }
                }
                if (menuChoice == 1) // If user chooses to view inventory:
                {
                    Clear(); // Clears prior screen for a neater UI when displaying inventory.
                    int i = 1; // Although list index starts at 0, here integer i is assigned as 1 so the first car starts at 1 when displayed.
                    foreach (Car car in cars)
                    {
                        WriteLine(i + ": " + car.Year + " " + car.Make + " " + car.Model + " | " + "Color: " + car.Color + " | " + "Miles: " + car.Mileage + " | " + car.Owners + " Owners | Price: " + car.Price.ToString("C2", CultureInfo.GetCultureInfo("en-US")));
                        if (car.Owners == 1 && car.Accidents == 0 && car.HasCleanTitle == true)
                        {
                            WriteLine("   GREAT DEAL: 1-Owner, 0 accidents, clean title!");
                        }
                        else if (car.HasCleanTitle == true)
                        {
                            WriteLine($"   {car.Accidents} accidents, clean title");
                        }
                        else
                        {
                            WriteLine($"   {car.Accidents} accidents, SALVAGED/FLOODED/TOTALED TITLE");
                        }
                        i++; // After a car is displayed, increase i by 1 increment.
                    }
                }
                if (menuChoice == 2) // If user chooses to add to inventory:
                {
                    cars.Add(AddCar()); // The method AddCar() is called, and the returned object is added to the list cars.
                    string jsonString = JsonSerializer.Serialize(cars); // Auto-saving feature: the JSON database is updated immediately following a car being added to the list.
                    File.WriteAllText(Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName, "cars.json"), jsonString); // The updated JSON file is written over.
                }
                if (menuChoice == 3) // If user chooses to remove from inventory:
                {
                    try // Program will attempt to remove the car at the selected index, if it exists.
                    {
                        cars.Remove(cars.ElementAt(RemoveCar())); // Method RemoveCar() is called that returns an integer (index).
                        WriteLine("Car has been successfully removed.");
                        string jsonString = JsonSerializer.Serialize(cars); // Once the car is removed from the list, the updated list is reflected to the JSON database.
                        File.WriteAllText(Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName, "cars.json"), jsonString); // The updated JSON file is written over.
                    }
                    catch // If there does not exist a car at the selected index, nothing will be removed.
                    {
                        WriteLine("Could not find a car to remove at this index!");
                    }
                }

            }

        }
        public static Car AddCar() // This method guides the user in adding a car, and returns the car as an object to be added to list cars.
        {
            Clear(); // Screen is cleared for a neater UI.
            Car newCar = new Car();
            string userYesOrNo = " ";
            Write("Please enter the make of the car (e.g. Daihatsu): ");
            newCar.Make = ReadLine();
            Write("Please enter the model of the car (e.g. Terios): ");
            newCar.Model = ReadLine();
            while (newCar.Year < 1900 || newCar.Year > 2100) // 2100 is allowed as the maximum year for futureproofing reasons.
            {
                Write("Please enter the year of the car (e.g. 2008): ");
                try
                {
                    newCar.Year = Convert.ToInt32(ReadLine());
                }
                catch
                {
                    WriteLine("Input must be a valid number! ");
                }
            }
            newCar.Mileage = -1; // Miles are temporarily set to -1, as a cars miles cannot be -1.
            while (newCar.Mileage == -1)
            {
                Write("Please enter the cars miles (e.g. (126241): ");
                try
                {
                    newCar.Mileage = Convert.ToInt32(ReadLine());
                }
                catch
                {
                    WriteLine("Input must be a valid number! ");
                }
            }
            Write("Please enter the color of the car (e.g. Silver): ");
            newCar.Color = ReadLine();
            newCar.Owners = -1;
            while (newCar.Owners < 0)
            {
                Write("How many owners did the car previously have? (e.g. 3): ");
                try
                {
                    newCar.Owners = Convert.ToInt32(ReadLine());
                }
                catch
                {
                    WriteLine("Input must be a valid number!");
                }
            }
            newCar.Accidents = -1;
            while (newCar.Accidents < 0)
            {
                Write("How many accidents was the car in? If none, enter 0. (e.g. 2): ");
                try
                {
                    newCar.Accidents = Convert.ToInt32(ReadLine());
                }
                catch
                {
                    WriteLine("Input must be a valid number!");
                }
            }
            while (userYesOrNo == " ")
            {
                Write("Has your car ever been salvaged, flooded, or deemed a total loss? Y/N: ");
                userYesOrNo = ReadLine();
                if (userYesOrNo == "Y")
                {
                    newCar.HasCleanTitle = false;
                }
                else if (userYesOrNo == "N")
                {
                    newCar.HasCleanTitle = true;
                }
                else
                {
                    userYesOrNo = " ";
                }
            }
            newCar.Price = -1;
            while (newCar.Price < 0)
            {
                Write("Finally, please enter the price in dollars (e.g. 5799.99): ");
                try
                {
                    newCar.Price = Convert.ToDouble(ReadLine());
                }
                catch
                {
                    WriteLine("Input must be a valid number!");
                }
            }
            WriteLine($"Successfully added {newCar.Year} {newCar.Make} {newCar.Model} to inventory with price of {newCar.Price.ToString("C2", CultureInfo.GetCultureInfo("en-US"))}.");
            return newCar;
        }
        public static int RemoveCar() // This method asks the user for an index, and returns an integer to the list cars to be removed at that element.
        {
            int userIndex;
            Write("Please enter the index of the car you wish to remove (e.g. 3): ");
            try
            {
                userIndex = Convert.ToInt32(ReadLine());
                userIndex = userIndex - 1; // Since C# starts at 0 and the displayed list starts at 1, userIndex is subtracted by 1 to match the inner list.
            }
            catch // If the user does not input a valid integer:
            {
                WriteLine("Invalid input!");
                userIndex = -1; // Index starts at 0, so by setting it to -1 there won't be a car to remove.
            }

            return userIndex;

        }
    }

    class Car // This class acts as a template for adding a car to the database, housing details such as make, model, etc.
    {
        public string Make { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public int Mileage { get; set; }
        public string Color { get; set; }
        public int Owners { get; set; }
        public int Accidents { get; set; }
        public bool HasCleanTitle { get; set; }
        public double Price { get; set; }
    }
}

