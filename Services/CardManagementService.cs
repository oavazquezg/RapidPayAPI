
using System;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using RapidPayAPI.DTOs;
using RapidPayAPI.Models;

namespace RapidPayAPI.Services
{    
    public class CardManagementService
    {
        private readonly AppDbContext _dbContext;
        private readonly PaymentFeeService _paymentFeeService;
        public CardManagementService(AppDbContext dbContext, PaymentFeeService paymentFeeService)
        {
            _dbContext = dbContext;
            _paymentFeeService = paymentFeeService;
        }        
        
        /// <summary>
        /// Creates a new credit card for the customer
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<CreditCardDto> CreateCard(int customerId)
        {
            Customer customer = await _dbContext.Customers.FirstOrDefaultAsync(c => c.Id == customerId);
            if (customer == null)
            { 
                throw new ArgumentException("Customer does not exist");
            }

            //Create the credit card
            CreditCard card = new CreditCard()
            {
                CustomerId = customerId, 
                CardNumber = GenerateRandomCardNumber(),
                Customer = customer,
                CardHolderName = GenerateCardHolderName(customer),
                ExpirationDate = DateTime.Today.AddYears(3).Date,
                CVV = GenerateRandomCVV()
            };

            //Add new card to the database
            _dbContext.CreditCards.Add(card);
            customer.CreditCards.Add(card);
            await _dbContext.SaveChangesAsync();            
            
            CreditCardDto creditCardDto = new CreditCardDto
            {
                CardNumber = card.CardNumber,
                CardHolderName = card.CardHolderName,
                ExpirationDate = card.ExpirationDate.Date,
                CVV = card.CVV
            };
            return creditCardDto;
        }

        /// <summary>
        /// Validates the credit card details
        /// </summary>
        /// <param name="creditCardNumber"></param>
        /// <param name="expirationDate"></param>
        /// <param name="cvv"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public async Task ValidateCreditCard(string creditCardNumber, DateTime expirationDate, string cvv, decimal amount)
        {            
            CreditCard creditCard = await _dbContext.CreditCards.FirstOrDefaultAsync(c => c.CardNumber == creditCardNumber);
            if (creditCard == null)
            {
                throw new ArgumentException("Invalid credit card.");                
            }

            if (creditCard.ExpirationDate.Date != expirationDate.Date)
            {
                throw new ArgumentException("Invalid expiration date.");                
            }

            if (creditCard.CVV != cvv)
            {
                throw new ArgumentException("Invalid CVV.");                
            }

            var paymentFee = _paymentFeeService.GetCurrentFee();
            var totalAmount = amount + paymentFee;
            var availableBalance = CreditCard.CreditCardLimit - creditCard.Balance;

            if(totalAmount > availableBalance)
            {
                throw new ArgumentException("Amount exceeds credit card balance.");                
            }            
        } 

        /// <summary>
        /// Register payment
        /// </summary>
        /// <param name="payment"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task RegisterPayment(Payment payment)
        {
            CreditCard creditCard = await _dbContext.CreditCards.FirstOrDefaultAsync(c => c.CardNumber == payment.CreditCardNumber);
            if (creditCard == null)
            {
                throw new ArgumentException("Invalid credit card.");
            }

            //update the credit card balance
            decimal TotalAmount = payment.Amount + payment.PaymentFee;
            var newBalance = creditCard.Balance + TotalAmount;
            creditCard.Balance = newBalance;
            
            _dbContext.Payments.Add(payment);
            await _dbContext.SaveChangesAsync();            
        }

        /// <summary>
        /// Get the balance of the credit card
        /// </summary>
        /// <param name="cardNumber"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<decimal> GetCardBalance(string cardNumber)
        {
            CreditCard creditCard = await _dbContext.CreditCards.FirstOrDefaultAsync(c => c.CardNumber == cardNumber);
            if (creditCard == null)
            {
                throw new ArgumentException("Invalid credit card.");
            }

            return creditCard.Balance;
        }

        private string GenerateRandomCardNumber()
        {
            Random random = new Random();
            string cardNumber = "";
            for (int i = 0; i < 15; i++)
            {
                cardNumber += random.Next(0, 10).ToString();
            }
            return cardNumber;
        }

        private string GenerateCardHolderName(Customer customer)
        {
            return customer.FirstName + " " + customer.LastName;
        }

        private string GenerateRandomCVV()
        {
            Random random = new Random();
            string cvv = "";
            for (int i = 0; i < 3; i++)
            {
                cvv += random.Next(0, 10).ToString();
            }
            return cvv;
        }
    }
}