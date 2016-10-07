///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	Solution/Project:  CreditCardListProjct/Project2
//	File Name:         MenuOption.cs
//	Description:       Enumeration of the available menu options the user may choose from. 
//	Course:            CSCI 2210 - Data Structures	
//	Author:            Casey Edwards, zcee10@etsu.edu
//	Created:           Wednesday, September 22, 2016
//	Copyright:         Casey Edwards, 2016
//
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

namespace CreditCardListProject
{
    /// <summary>
    /// Enumeration of menu options for the driver class.
    /// </summary>
    enum MenuOption
    {
        /// <summary>
        /// Create empty list
        /// </summary>
        CREATE_EMPTY_LIST = 1,
        /// <summary>
        /// Load list
        /// </summary>
        LOAD_LIST,
        /// <summary>
        /// Add card
        /// </summary>
        ADD_CARD,
        /// <summary>
        /// Remove card
        /// </summary>
        REMOVE_CARD,
        /// <summary>
        /// Search by index
        /// </summary>
        SEARCH_INDEX,
        /// <summary>
        /// Search by card number
        /// </summary>
        SEARCH_CARD_NUM,
        /// <summary>
        /// Display a customer's cards
        /// </summary>
        DISPLAY_CUSTOMER_CARDS,
        /// <summary>
        /// Display valid cards
        /// </summary>
        DISPLAY_VALID_CARDS,
        /// <summary>
        /// Sort cards
        /// </summary>
        SORT_CARDS,
        /// <summary>
        /// Display all cards
        /// </summary>
        DISPLAY_ALL,
        /// <summary>
        /// Exit program
        /// </summary>
        EXIT
    }
}
