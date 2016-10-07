///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	Solution/Project:  CreditCardListProject/Project2
//	File Name:         CreditCard.cs
//	Description:       Represents a person's credit card information such as name, number, expiry, etc. 
//	Course:            CSCI 2210 - Data Structures	
//	Author:            Casey Edwards, zcee10@etsu.edu
//	Created:           Wednesday, September 7, 2016
//	Copyright:         Casey Edwards, 2016
//
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CreditCardListProject
{
    /// <summary>
    /// Represents a credit card's information.
    /// Allows storage of a cardholder's name, phone, and email address.
    /// Also stores a credit card number, expiration date, and type.
    /// </summary>
    class CreditCard : IComparable<CreditCard>, IEquatable<CreditCard>
    {
        #region Properties
        private string Name;               // Cardholder's name.
        private string ValidEmail;         // Cardholder's email address.
        private string ValidPhone;         // Cardholder's phone number.
        private string CardNum;            // Card number.
        private DateTime Expiry;           // Expiration date.
        private CreditCardType CardType;   // Card type (Discover, Visa, etc.)
        public bool Expired                // Expiration status of the card.
        {
            get
            {
                return DateTime.Today >= Expiry;
            }
        }
        public bool Valid                  // Validity status of the card.
        {
            get
            {
                return CardType != CreditCardType.INVALID;
            }
        }
        #endregion

        #region Constructors 
        /// <summary>
        /// Initializes a new instance of the CreditCard class.
        /// Performs validation of information.
        /// </summary>
        /// <param name="name">The cardholder's name.</param>
        /// <param name="email">The cardholder's email.</param>
        /// <param name="phone">The cardholder's phone.</param>
        /// <param name="cardNum">The card number.</param>
        /// <param name="expiry">The expiry.</param>
        public CreditCard(string name="John Doe", string phone="555-5555", string email="XXXXX@XXXX.XXX",
                            string cardNum="012345678901234", string expiry="01/1990")
        {
            // Where applicable, use the setter methods to perform validation.
            // Ensure name is not null or empty.
            if (String.IsNullOrEmpty(name))
                throw new ArgumentException("Name cannot be empty!");
            Name = name;
            Email = email;
            Phone = phone;
            CardNumber = cardNum;
            Expiration = expiry;

            // Perform validation on the card.
            Validate();
        }

        #endregion

        #region Getters And Setters

        /// <summary>
        /// Gets or sets the card number.
        /// Strips the number of any non-digit characters, and
        /// ensures the number is 12-19 digits long.
        /// </summary>
        /// <value>
        /// The card number.
        /// </value>
        private string CardNumber
        {
            get { return CardNum; }
            set
            {
                // Strips non-digit characters
                Regex extractDigitsPattern = new Regex(@"[^0-9]+");
                string cardNumberDigits = extractDigitsPattern.Replace(value, @"");
                // Ensures number has 12-19 digits
                Regex cardNumberPattern = new Regex(@"^\d{12,19}$");
                Match match = cardNumberPattern.Match(cardNumberDigits);
                // Throw an exception if the value doesn't match the pattern.
                if (match.Value == String.Empty)
                    throw new ArgumentException("Card number has an illegal number of digits.");
                else
                    CardNum = match.Value;
            }
        }

        /// <summary>
        /// Gets or sets the phone number.
        /// Ensures the number has 10 or 7 digits (with or without area code).
        /// Allows for optional parentheses around the area code, and allows the 
        /// number blocks to be delimited by spaces, hyphens, or periods.
        /// </summary>
        /// <value>
        /// The phone number.
        /// </value>
        private string Phone
        {
            get { return ValidPhone; }
            set
            {
                // Ensure phone number entered in acceptable format.
                Regex phonePattern = new Regex(@"^(\(?\d{3}[)\- \.]?)? ?\d{3}[\-\. ]?\d{4}$");
                Match match = phonePattern.Match(value);
                // Throw an exception if the number doesn't match the pattern.
                if (match.Value == String.Empty)
                    throw new ArgumentException("Phone number is in an invalid format.");
                else
                    ValidPhone = match.Value;
            }
        }

        /// <summary>
        /// Gets or sets the email.
        /// Ensures the email follows a xxxx@yyyy.zzz pattern, with varying lengths
        /// of valid characters in each block. Obtained from www.regular-expressions.info
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        private string Email
        {
            get { return ValidEmail; }
            set
            {
                // Ensure email is entered in acceptable format.
                Regex emailPattern = new Regex(@"[A-Z0-9._%+-]+@(?:[A-Z0-9-]+\.)+[A-Z]{2,}", 
                                                RegexOptions.IgnoreCase);
                Match match = emailPattern.Match(value);
                // Throw an exception if the email does not match the pattern.
                if (match.Value == String.Empty)
                    throw new ArgumentException("Invalid E-mail address.");
                else
                    ValidEmail = match.Value;
            }
        }

        /// <summary>
        /// Parses and sets the passed expiration date.
        /// </summary>
        /// <value>
        /// The expiration date.
        /// </value>
        /// <exception cref="ArgumentException">Invalid Date Format</exception>
        private string Expiration
        {
            set
            {
                // Ensure string matches MM/YYYY or MM/YY format.
                Regex expiryPattern = new Regex(@"\b[0-1]?\d/(\d\d|19\d\d|20\d\d)\b");
                Match match = expiryPattern.Match(value);

                // Throw an exception if the date is not given in correct format.
                if (match.Value == String.Empty)
                    throw new ArgumentException("Invalid Date Format");
                else
                {
                    // Parse the value into a DateTime object. If the value is in MM/YY format,
                    // assume the year is "20YY" and parse accordingly.
                    if (match.Value.Length == 5)
                        Expiry = new DateTime(Convert.ToInt32("20" + match.Value.Substring(3)),
                                        Convert.ToInt32(match.Value.Substring(0, 2)), 1);
                    else // Parse the string as it is.
                        Expiry = new DateTime(Convert.ToInt32(match.Value.Substring(3)),
                                        Convert.ToInt32(match.Value.Substring(0, 2)), 1);
                }
            }
        }
        #endregion

        #region Validation Methods
        /// <summary>
        /// Ensures card number and expiration date validity.
        /// Sets the CardType field based on the card's IIN.
        /// </summary>
        public void Validate()
        {
            int cardIIN = Convert.ToInt32(CardNum.Substring(0, 6));  // The Issuer Identification Number

            if (PassLuhnsAlgo())
            {
                // Card passes the checksum algorithm; determine card type.
                if (cardIIN / 10000 == 34 || cardIIN / 10000 == 37)
                {
                    // Card IIN starts with 34 or 37: American Express
                    CardType = CreditCardType.AMERICAN_EXPRESS;
                }
                else if (cardIIN / 100000 == 4)
                {
                    // Card IIN starts with 4: Visa
                    CardType = CreditCardType.VISA;
                }
                else if (cardIIN / 10000 >= 51 && cardIIN / 10000 <= 55)
                {
                    // Card IIN between 51xxxx and 55xxxx inclusive: Mastercard.
                    CardType = CreditCardType.MASTERCARD;
                }
                else if (cardIIN / 100 == 6011 || cardIIN / 1000 == 644 || cardIIN / 10000 == 65)
                {
                    // Card IIN starts with 6011, 644, or 65: Discover
                    CardType = CreditCardType.DISCOVER;
                }
                else
                {
                    // Card doesn't match any previous criteria: Other
                    CardType = CreditCardType.OTHER;
                }
            }
            else
            {
                // Card failed Luhn's Algorithm.
                CardType = CreditCardType.INVALID;
            }
        }

        /// <summary>
        /// Determines if the card number passes the Luhn's Algorithm for validity.
        /// Doubles the value of every other number starting at the next to last index,
        /// summing the digits of the result if the result is greater than 9, then
        /// takes the sum of these numbers with the unaltered places and applies
        /// a mod-10 operation to check for validity.
        /// </summary>
        /// <returns>True for a pass, else False.</returns>
        private bool PassLuhnsAlgo()
        {
            bool pass = false;  // Pass the algorithm test.
            List<Decimal> digitList = new List<Decimal>(CardNum.Length);  // Credit card digits.

            for (int i = 0; i < CardNum.Length; i++)
                digitList.Add(Decimal.Parse(CardNum.Substring(i, 1)));
            // Start at the next to last digit, and skip left by two places each run.
            for (int i = digitList.Count - 2; i >= 0; i -= 2)
            {
                digitList[i] *= 2;
                if (digitList[i] > 9)
                    digitList[i] -= 9; // Sums the digits of a doubled number.
            }
            if (digitList.Sum() % 10 == 0)
                pass = true;  // Card number is validated.

            return pass;
        }
       
        /// <summary>
        /// Determines if the name on the card matches the name in the parameter
        /// without exposing the private Name field.
        /// </summary>
        /// <param name="name">The name to match.</param>
        /// <returns>True if name matches, otherwise false.</returns>
        public bool NameMatches(string name)
        {
            return Name == name;
        }
        #endregion

        #region Overridden Methods and Interface Implementations        
        /// <summary>
        /// Compares a CreditCard instance to another instance.
        /// Determines comparison value via lexigraphical comparison of card numbers.
        /// </summary>
        /// <param name="otherCard">The other CreditCard.</param>
        /// <returns>Comparison Value</returns>
        public int CompareTo(CreditCard otherCard)
        {
            // Return the result of the CardNum's CompareTo method,
            // passing in the value of the otherCard's CardNum.
            return CardNum.CompareTo(otherCard.CardNum);
        }

        /// <summary>
        /// Indicates whether the current CreditCard is equal to another CreditCard by card number.
        /// </summary>
        /// <param name="other">The CreditCard object to compare with.</param>
        /// <returns>
        /// true if the current CreditCard is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        bool IEquatable<CreditCard>.Equals(CreditCard other)
        {
            // Return the string equality result of the CardNum field.
            return CardNum == other.CardNum;
        }

        /// <summary>
        /// Determines whether the specified object is equal to this instance.
        /// If null is passed in, calls the Object class's Equals(null) method.
        /// Otherwise, if a non-CreditCard is passed in, throws an exception.
        /// Otherwise, calls the implementation of IEquatable.Equals of this class.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentException">Cannot compare a CreditCard object to a {obj.GetType()}</exception>
        public override bool Equals(object obj)
        {
            // If object is null, return the Object.Equals(null) result.
            if (obj == null)
                return base.Equals(obj);

            // If object is not a credit card, throw an exception.
            if (!(obj is CreditCard))
                throw new ArgumentException($"Cannot compare a CreditCard object to a {obj.GetType()} object.");

            // Call the IEquatable implementation, passing the object casted as a CreditCard.
            return Equals(obj as CreditCard);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            // Use the hash code of the CardNum field.
            return CardNum.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this credit card.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this credit card.
        /// </returns>
        public override string ToString()
        {
            string cardCensor = String.Empty;   // The string of X's used to censor a card number.

            // Create the necessary number of X's for the censor
            for (int i = 0; i < CardNum.Length - 4; i++)
                cardCensor += "X";

            // Return the string representation with the censor applied to the card number.
            return $"Name       : {Name}\nEmail      : {ValidEmail}\nPhone      : {ValidPhone}\n"
                    + $"Card Number: {cardCensor + CardNum.Substring(CardNum.Length - 4)}\n"
                    + $"Expiry     : {Expiry:MM/yyyy}\nType       : {CardType}";
        }
        #endregion

        #region Other        
        /// <summary>
        /// Creates a string representing the card information in pipe-delimited format.
        /// Used for saving data to a file.
        /// </summary>
        /// <returns>Pipe-delimited string representation of the card.</returns>
        internal string ToFileFormatString()
        {
            return $"{Name}|{ValidPhone}|{ValidEmail}|{CardNum}|{Expiry:MM/yyyy}";
        }
        #endregion
    }
}