///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	Solution/Project:  CreditCardListProject/Project2
//	File Name:         CreditCardType.cs
//	Description:       Enum of the different issuer types a credit card can represent. 
//	Course:            CSCI 2210 - Data Structures	
//	Author:            Casey Edwards, zcee10@etsu.edu
//	Created:           Thursday, September 8, 2016
//	Copyright:         Casey Edwards, 2016
//
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


namespace CreditCardListProject
{
    /// <summary>
    /// Enumerated listing of the credit card types.
    /// </summary>
    enum CreditCardType
    {
        /// <summary>
        /// Invalid card (doesn't pass Luhn's Algorithm).
        /// </summary>
        INVALID = 0,
        /// <summary>
        /// Mastercard card.
        /// </summary>
        MASTERCARD,
        /// <summary>
        /// Visa card.
        /// </summary>
        VISA,
        /// <summary>
        /// Discover card.
        /// </summary>
        DISCOVER,
        /// <summary>
        /// American Express card.
        /// </summary>
        AMERICAN_EXPRESS,
        /// <summary>
        /// Card is valid but does not match any previous types.
        /// </summary>
        OTHER
    }
}
