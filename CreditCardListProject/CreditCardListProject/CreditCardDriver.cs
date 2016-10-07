///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	Solution/Project:  CreditCardListProjct/Project2
//	File Name:         CreditCardDriver.cs
//	Description:       Create a console UI that maintains a list of Credit Cards entered by the
//                      user. The user can manipulate the list in several ways, as well as save
//                      or load the data to/from text files. 
//	Course:            CSCI 2210 - Data Structures	
//	Author:            Casey Edwards, zcee10@etsu.edu
//	Created:           Wednesday, September 7, 2016
//	Copyright:         Casey Edwards, 2016
//
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Windows.Forms;

namespace CreditCardListProject
{        
    /// <summary>
    /// Implements a UI interface that manages a CreditCardList of CreditCards
    /// entered by the user. User may sort, search, save, load, add, and remove cards.
    /// </summary>
    class CreditCardDriver
    {
        #region Static Members and Main
        /// <summary>
        /// The credit card list object.
        /// </summary>
        private static CreditCardList creditCardList = null;

        /// <summary>
        /// Sets the console colors, creates a Menu for user interaction, and 
        /// allows the user to manipulate the card list until they are finished.
        /// </summary>
        [STAThread]
        static void Main()
        {
            MenuOption menuChoice;                  // User's choice from the menu.
            UtilityNamespace.Menu mainMenu = new UtilityNamespace.Menu("Credit Card List Manager");  // Options menu.

            // Set background and text colors.
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Blue;

            // Build the main menu.
            BuildMenuOptions(mainMenu);

            // Main program loop
            while (true)
            {
                // Display the menu and obtain the user's choice.
                menuChoice = (MenuOption)mainMenu.GetChoice();

                // Perform the user's selection.
                PerformMenuSelection(menuChoice);
            }
        }
        #endregion

        #region Menu Helper Methods
        /// <summary>
        /// Adds the menu options to the passed menu.
        /// </summary>
        /// <param name="menu">The menu.</param>
        private static void BuildMenuOptions(UtilityNamespace.Menu menu)
        {
            // Add the options to the menu.
            menu += "Create an empty list.";
            menu += "Load an existing list.";
            menu += "Add a new card.";
            menu += "Remove a card.";
            menu += "Display a card by index.";
            menu += "Display a card by card number.";
            menu += "Display all of a customer's cards.";
            menu += "Display all non-expired valid cards.";
            menu += "Sort the cards by card number.";
            menu += "Display all cards.";
            menu += "Exit.";
        }

        /// <summary>
        /// Performs the operation requested by the user.
        /// </summary>
        /// <param name="selection">The selection.</param>
        private static void PerformMenuSelection(MenuOption selection)
        {
            // Perform the corresponding operation based on the menu selection.
            switch (selection)
            {
                #region Create Empty List
                case MenuOption.CREATE_EMPTY_LIST:
                    // Check for save, then create a new list.
                    CheckForSave();
                    creditCardList = new CreditCardList();
                    MessageBox.Show("Empty list created!", "Success");
                    break;
                #endregion
                #region Load List From File
                case MenuOption.LOAD_LIST:
                    // Check for save, then perform the load operation.
                    CheckForSave();
                    try
                    {
                        LoadData();
                        MessageBox.Show("Data has been loaded.", "Success");
                    }
                    catch (System.IO.IOException ex)
                    {
                        // IO error while loading file.
                        MessageBox.Show($"An error occurred while loading file:\n{ex.Message}", "Error");
                    }
                    break;
                #endregion
                #region Add Card
                case MenuOption.ADD_CARD:
                    // If credit card list hasn't been initialized, show an error message.
                    if (creditCardList == null)
                    {
                        MessageBox.Show("No credit card list exists.\nPlease create a new list first.", "Error");
                        break;
                    }
                    // Attempt to gather card information and add it to the card list.
                    try
                    {
                        creditCardList += EnterCardInfo();
                    }
                    catch (ArgumentException ex)
                    {
                        // An invalid argument was passed to the CreditCard constructor.
                        MessageBox.Show($"An error was encountered:\n{ex.Message}");
                    }
                    break;
                #endregion
                #region Remove Card
                case MenuOption.REMOVE_CARD:
                    // If card list is null or empty, show an error message.
                    if (creditCardList == null || creditCardList.Count == 0)
                    {
                        MessageBox.Show("No credit cards exist.", "Error");
                        break;
                    }
                    // Attempt to request the card information and remove the card from the list.
                    Console.Write("\n\tEnter the index of the card to be removed:\n\n");
                    try
                    {
                        creditCardList -= creditCardList[Int32.Parse(Console.ReadLine())];
                    }
                    catch (IndexOutOfRangeException ex)
                    {
                        // An invalid index was entered.
                        MessageBox.Show($"An error was encountered:\n{ex.Message}");
                    }
                    catch (FormatException ex)
                    {
                        // Non-digit entered.
                        MessageBox.Show("Please enter a valid number.", $"{ex.GetType()}");
                    }
                    break;
                #endregion
                #region Search By Index
                case MenuOption.SEARCH_INDEX:
                    // If card list is empty or null, show an error message.
                    if (creditCardList == null || creditCardList.Count == 0)
                    {
                        MessageBox.Show("No credit cards exist.", "Error");
                        break;
                    }
                    // Ask for the card index and attempt to retrieve and display it.
                    Console.Write("\nEnter the index which you wish to retrieve: ");
                    try
                    {
                        DisplayCardInfo(creditCardList[Int32.Parse(Console.ReadLine())]);
                    }
                    catch (IndexOutOfRangeException ex)
                    {
                        // Index was out of range
                        MessageBox.Show($"An error was encountered:\n{ex.Message}", "Error");
                    }
                    catch (FormatException ex)
                    {
                        // Non-digit entered.
                        MessageBox.Show($"Please enter a valid index number.", $"{ex.GetType()}");
                    }
                    break;
                #endregion
                #region Search By Card Number
                case MenuOption.SEARCH_CARD_NUM:
                    // If card list is empty or null, show an error message. 
                    if (creditCardList == null || creditCardList.Count == 0)
                    {
                        MessageBox.Show("No credit cards exist.", "Error");
                        break;
                    }
                    // Attempt to search for a card by number and display the result.
                    try
                    {
                        DisplayCardInfo(SearchByCardNumber());
                    }
                    catch (System.Collections.Generic.KeyNotFoundException ex)
                    {
                        // No matching card was found.
                        MessageBox.Show($"An error was encountered:\n{ex.Message}", "Error");
                    }
                    catch (ArgumentException)
                    {
                        // Invalid card number entered.
                        MessageBox.Show("Invalid card number entered.", $"Error");
                    }
                    break;
                #endregion
                #region Display Customer's Cards
                case MenuOption.DISPLAY_CUSTOMER_CARDS:
                    // If card list is empty or null, show an error message.
                    if (creditCardList == null || creditCardList.Count == 0)
                    {
                        MessageBox.Show("No credit cards exist.", "Error");
                        break;
                    }
                    // Display the list of customer's cards.
                    DisplayCustomerCards();
                    break;
                #endregion
                #region Display Valid Cards
                case MenuOption.DISPLAY_VALID_CARDS:
                    // If card list is empty or null, show an error message.
                    if (creditCardList == null || creditCardList.Count == 0)
                    {
                        MessageBox.Show("No credit cards exist.", "Error");
                        break;
                    }
                    // Attempt to retrieve and display all valid, non-expired cards.
                    try
                    {
                        DisplayAllValidCards();
                    }
                    catch (System.Collections.Generic.KeyNotFoundException)
                    {
                        // No valid cards found.
                        MessageBox.Show("No valid cards were found.", "Error");
                    }
                    break;
                #endregion
                #region Sort Cards
                case MenuOption.SORT_CARDS:
                    // If card list is empty or null, show an error message.
                    if (creditCardList == null || creditCardList.Count == 0)
                    {
                        MessageBox.Show("No credit cards exist.", "Error");
                        break;
                    }
                    // Sort the cards and inform the user of success.
                    creditCardList.SortCards();
                    MessageBox.Show("Cards have been sorted!", "Success");
                    break;
                #endregion
                #region Display All Cards
                case MenuOption.DISPLAY_ALL:
                    // If card list is empty or null, show an error message.
                    // Otherwise, iterate through the card list and display each card.
                    if (creditCardList == null || creditCardList.Count == 0)
                        MessageBox.Show("No cards on file.", "Error");
                    else
                        for (int i = 0; i < creditCardList.Count; i++)
                        {
                            // Display the index of each card before printing the card information.
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"\n\n\n\tIndex: {i}");
                            Console.ForegroundColor = ConsoleColor.Blue;
                            DisplayCardInfo(creditCardList[i]);
                        }
                    break;
                #endregion
                #region Exit Program
                case MenuOption.EXIT:
                    // Check for save, then exit the program.
                    CheckForSave();
                    Environment.Exit(0);
                    break;
                #endregion
            }
        }
        #endregion

        #region Search-related Methods
        /// <summary>
        /// Searches the CreditCardList by card number.
        /// </summary>
        /// <returns>A matching credit card.</returns>
        private static CreditCard SearchByCardNumber()
        {
            // Prompt user for the card number.
            Console.Write("Enter the number for the card you wish to retrieve: ");
            
            // Return the result of searching for the card number entered by user.
            return creditCardList[Console.ReadLine()];
        }

        /// <summary>
        /// Displays all valid, non-expired cards.
        /// </summary>
        /// <exception cref="KeyNotFoundException">No valid, non-expired cards found.</exception>
        private static void DisplayAllValidCards()
        {
            // Build a list of valid cards, and display each card found.
            foreach (CreditCard card in creditCardList.FindValidCards())
                DisplayCardInfo(card);
        }

        /// <summary>
        /// Displays all cards held by a customer by name.
        /// </summary>
        private static void DisplayCustomerCards()
        {
            string customerName = String.Empty; // Customer's name.

            // Prompt user for the customer's name.
            Console.Write("Enter the customer's name exactly as it appears on the card: ");
            customerName = Console.ReadLine();

            // Build a list of the customer cards found and display each card.
            foreach (CreditCard card in creditCardList.FindCardsByName(customerName))
                DisplayCardInfo(card);
        }
        #endregion

        #region Displaying and Entering Card Info
        /// <summary>
        /// Displays a credit card's information.
        /// </summary>
        /// <param name="card">The credit card to display.</param>
        private static void DisplayCardInfo(CreditCard card)
        {
            Console.WriteLine($"\n\n\n{card}");
            Console.WriteLine("\n\n\n\tPress Any Key To Continue");
            Console.ReadKey();
            Console.Clear();
        }
        
        /// <summary>
        /// Gathers the user's credit card information and creates
        /// a CreditCard object.
        /// </summary>
        /// <returns>Credit Card entered by user.</returns>
        private static CreditCard EnterCardInfo()
        {
            string name, phone, email, cardNum, date; // CreditCard constructor fields.

            // Request each field
            Console.Clear();
            Console.Write("Enter the cardholder's name: ");
            name = Console.ReadLine();
            Console.Write("Enter the cardholder's phone number: ");
            phone = Console.ReadLine();
            Console.Write("Enter the cardholder's email address: ");
            email = Console.ReadLine();
            Console.Write("Enter the card number: ");
            cardNum = Console.ReadLine();
            Console.Write("Enter the expiration in MM/YYYY or MM/YY format: ");
            date = Console.ReadLine();

            // Return a newly constructed card.
            return new CreditCard(name, phone, email, cardNum, date);
        }
        #endregion

        #region Save and Load Related Methods
        /// <summary>
        /// Checks the CreditCardList to see if a save operation is needed to prevent data loss.
        /// If so, asks the user if they would like to save the data before continuing.
        /// </summary>
        private static void CheckForSave()
        {
            DialogResult result; // User's Save/Don't Save selection.

            if (creditCardList != null && creditCardList.SaveNeeded)
            {
                // A save is required. Prompt user suggesting they perform a save operation.
                result = MessageBox.Show("Your current data requires saving.\nWould you like to save?",
                                         "Warning", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                    SaveData();
                else
                    MessageBox.Show("Save operation aborted. Data may be lost.", "Warning");
            }
        }

        /// <summary>
        /// Requests the user to specify a file to save current credit card data.
        /// </summary>
        private static void SaveData()
        {
            SaveFileDialog saveDlg = new SaveFileDialog(); // Save File Dialog

            // Set the InitialDirectory, Filter, and Title options.
            saveDlg.InitialDirectory = Application.StartupPath + @"\..\..\SaveFiles";
            saveDlg.Filter = "text files (*.txt)|*.txt|text files (*.text)|*.text|all files|*.*";
            saveDlg.Title = "Save Your Data";

            // Attempt to save the file to the user-specified location.
            try
            {
                if (DialogResult.Cancel != saveDlg.ShowDialog())
                    creditCardList.SaveToFile(saveDlg.FileName);
                else
                    MessageBox.Show("Save operation aborted. Data may be lost.", "Warning");
            }
            catch (System.IO.IOException ex)
            {
                // IO error occurred.
                MessageBox.Show($"An error occurred while saving:\n{ex.Message}");
            }
        }

        /// <summary>
        /// Requests the user to select a file from which to load credit card data.
        /// </summary>
        private static void LoadData()
        {
            OpenFileDialog openDlg = new OpenFileDialog(); // Open File dialog box.

            // Set the InitialDirectory, Filter, and Title options.
            openDlg.InitialDirectory = Application.StartupPath + @"\..\..\SaveFiles";
            openDlg.Filter = "text files (*.txt)|*.txt|text files (*.text)|*.text|all files|*.*";
            openDlg.Title = "Load Data";

            // Attempt to load the file data or cancel.
            if (DialogResult.OK == openDlg.ShowDialog())
                creditCardList = new CreditCardList(openDlg.FileName);
            else
                MessageBox.Show("Load operation aborted. Returning to menu.");
        }
        #endregion
    }
}
