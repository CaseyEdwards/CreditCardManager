//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	Solution/Project:	CreditCardListProject/Project 2
//	File Name:		    CreditCardList.cs
//	Description:	    Implementation of a class that manages a List of CreditCard objects, and presents
//                          methods for manipulating, searching, and sorting the List.
//	Course:			    CSCI 2210-201 - Data Structures
//	Author:			    Casey Edwards, zcee10@etsu.edu
//	Created:	        Wednesday, September 21, 2016
//	Copyright:		    Casey Edwards, 2016
//
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using System.IO;
using System;

namespace CreditCardListProject
{
    class CreditCardList
    {
        #region Properties
        private List<CreditCard> CardList;   // Credit Card list.
        public int Count    // Number of cards in the CardList.
        {
            get
            {
                return CardList.Count;
            }
        }
        public bool SaveNeeded { get; private set; }  // Flags true if changes are made since last save.
        #endregion

        #region Constructors        
        /// <summary>
        /// Default constructor.
        /// Initializes a new instance of the <see cref="CreditCardList"/> class
        /// with an empty list.
        /// </summary>
        public CreditCardList()
        {
            // Create an empty list and set SaveNeeded to False.
            CardList = new List<CreditCard>();
            SaveNeeded = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreditCardList"/> class.
        /// Takes the file path of a card data file and populates the card list with
        /// the file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        public CreditCardList(string filePath)
        {
            // Create a new CardList and load data from the filepath.
            CardList = new List<CreditCard>();
            LoadFromFile(filePath);
        }
        #endregion

        #region Indexers        
        /// <summary>
        /// Gets or sets the <see cref="CreditCard"/> at the specified index of the CardList.
        /// </summary>
        /// <value>
        /// The <see cref="CreditCard"/> to be added.
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns>The CreditCard object at the specified index.</returns>
        /// <exception cref="System.IndexOutOfRangeException">The index is not within the range of the list.</exception>
        public CreditCard this[int index]
        {
            get
            {
                // If the index is out of range, throw an exception.
                if (index < 0 || index > CardList.Count - 1)
                    throw new IndexOutOfRangeException($"The index must be within the range 0 to {Count}.");
                else
                    return CardList[index];
            }
            set { CardList[index] = value; }
        }
        
        /// <summary>
        /// Gets the <see cref="CreditCard"/> with the specified card number.
        /// </summary>
        /// <value>
        /// The <see cref="CreditCard"/>.
        /// </value>
        /// <param name="cardNumber">The card number.</param>
        /// <returns>The matching CreditCard</returns>
        /// <exception cref="KeyNotFoundException">No card matches the card number.</exception>
        public CreditCard this[string cardNumber]
        {
            get
            {
                // List must be sorted for correct operation of BinarySearch.
                SortCards();
                // Create a dummy card with the passed cardNumber and use it for searching.
                CreditCard fakeCard = new CreditCard(cardNum: cardNumber);
                int index = CardList.BinarySearch(fakeCard);

                // If a match was found, return the card. Otherwise, throw an exception.
                if (index != -1)
                    return CardList[index];
                else
                    throw new KeyNotFoundException("No card matches the card number.");
            }
        }
        #endregion

        #region Operator Overloading        
        /// <summary>
        /// Implements the operator +.
        /// Takes a CreditCard object as the second parameter and adds the object
        /// to the CreditCardList.
        /// </summary>
        /// <param name="cardListObject">The CreditCardList object.</param>
        /// <param name="card">The credit card.</param>
        /// <returns>
        /// The updated CreditCardList.
        /// </returns>
        public static CreditCardList operator + (CreditCardList cardListObject, CreditCard card)
        {
            cardListObject.CardList.Add(card);
            cardListObject.SaveNeeded = true;
            return cardListObject;
        }

        /// <summary>
        /// Implements the operator -.
        /// Removes the CreditCard passed in the second parameter from the CreditCardList.
        /// </summary>
        /// <param name="cardListObject">The card list object.</param>
        /// <param name="card">The card.</param>
        /// <returns>
        /// The updated CreditCardList.
        /// </returns>
        public static CreditCardList operator - (CreditCardList cardListObject, CreditCard card)
        {
            cardListObject.CardList.Remove(card);
            cardListObject.SaveNeeded = true;
            return cardListObject;
        }
        #endregion

        #region Sorting and Searching        
        /// <summary>
        /// Sorts the credit cards lexigraphically based on the card numbers.
        /// Sets the SaveNeeded flag to true.
        /// </summary>
        public void SortCards()
        {
            // Call the List<T>.Sort method and set SaveNeeded to true.
            CardList.Sort();
            SaveNeeded = true;
        }

        /// <summary>
        /// Searches the list of credit cards for all those held by the customer
        /// whose name is passed in the argument.
        /// </summary>
        /// <param name="name">The customer's name.</param>
        /// <returns>List of the cards held by the customer.</returns>
        /// <exception cref="KeyNotFoundException">No cards matching the name were found.</exception>
        public List<CreditCard> FindCardsByName(string name)
        {
            List<CreditCard> CustomerCardList = new List<CreditCard>(); // List of customer's cards.

            // Iterate over the list and search for cards that match the passed name,
            // and such cards to the return list.
            foreach (CreditCard card in CardList)
                if (card.NameMatches(name))
                    CustomerCardList.Add(card);

            // If no cards were found, throw an exception.
            if (CustomerCardList.Count == 0)
                throw new KeyNotFoundException("No cards matching the name were found.");

            return CustomerCardList;
        }

        /// <summary>
        /// Searches the list of credit cards for valid, non-expired cards, and
        /// returns the list to the caller.
        /// </summary>
        /// <returns>List of valid credit cards.</returns>
        /// <exception cref="KeyNotFoundException">No valid cards found.</exception>
        public List<CreditCard> FindValidCards()
        {
            List<CreditCard> ValidCards = new List<CreditCard>();   // List of valid credit cards.

            // Iterate over the CardList, checking each card for validity and expiredness.
            // Add each valid card to the return list.
            foreach (CreditCard card in CardList)
                if (card.Valid && !card.Expired)
                    ValidCards.Add(card);

            // Throw an exception if no valid cards are found.
            if (ValidCards.Count == 0)
                throw new KeyNotFoundException("No valid cards found.");

            return ValidCards;
        }
        #endregion

        #region Saving and Loading         
        /// <summary>
        /// Saves the current list of CreditCard information to a pipe-delimited text file.
        /// Exceptions propogate up to the calling method.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        public void SaveToFile(string filePath)
        {
            StreamWriter writer = null;   // Writes data to the file.

            try
            {
                // Open the specified file with write access and add each card's data to the file.
                writer = new StreamWriter(new FileStream(filePath, FileMode.Create, FileAccess.Write));
                foreach (CreditCard card in CardList)
                {
                    writer.WriteLine(card.ToFileFormatString());
                }
                // Success; set SaveNeeded to false.
                SaveNeeded = false;
            }
            finally
            {
                // Close the writer, if opened.
                if (writer != null)
                    writer.Close();
            }
        }
               
        /// <summary>
        /// Loads credit card information from a file.
        /// Errors propogate up to the calling method.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        public void LoadFromFile(string filePath)
        {
            StreamReader reader = null;  // Reader for the data file.
            string[] fields;             // Container for the parsed card information.

            try
            {
                // Set the SaveNeeded flag to false.
                SaveNeeded = false;

                // Attempt to open the file path and loop over the data.
                reader = new StreamReader(filePath);
                while (reader.Peek() != -1)
                {
                    // Split the line on the pipe char into separate fields.
                    fields = reader.ReadLine().Split('|');
                    // Attempt to build a card from the lines. If invalid data or missing
                    // fields are encountered, skip to the next line.
                    try
                    {
                        BuildAndAddCard(fields);
                    }
                    catch (Exception ex) when (ex is ArgumentException || ex is IndexOutOfRangeException)
                    {
                        // Data field is invalid or a field is missing. Continue to the next line
                        // but reset the SaveNeeded flag to True.
                        SaveNeeded = true;
                        continue;
                    }
                }
            }
            finally
            {
                // Make sure the reader is closed.
                if (reader != null)
                    reader.Close();
            }
        }

        /// <summary>
        /// Helper method that builds CreditCard objects from data parsed 
        /// out of text files. Exceptions propogate up to the calling method.
        /// </summary>
        /// <param name="cardFields">The card fields.</param>
        private void BuildAndAddCard(string[] cardFields)
        {
            // Separate out the fields
            string name = cardFields[0];
            string phone = cardFields[1];
            string email = cardFields[2];
            string number = cardFields[3];
            string date = cardFields[4];

            // Build a new card and add it to the CardList.
            CardList.Add(new CreditCard(name, phone, email, number, date));
        }
        #endregion
    }
}
