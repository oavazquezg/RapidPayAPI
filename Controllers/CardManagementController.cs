using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RapidPayAPI.DTOs;
using RapidPayAPI.Models;
using RapidPayAPI.Services;

namespace RapidPayAPI.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class CardManagementController : ControllerBase
{
    private readonly ILogger<CardManagementController> _logger;
    private readonly CardManagementService _cardManagementService;
    private readonly PaymentFeeService _paymentFeeService;
    

    public CardManagementController(ILogger<CardManagementController> logger
    , CardManagementService cardManagementService
    , PaymentFeeService paymentFeeService)
    {
        _logger = logger;
        _cardManagementService = cardManagementService;
        _paymentFeeService = paymentFeeService;
    }   
    
    [HttpPost("CreateCard", Name = "CreateCard")]    
    public async Task<ActionResult<CreditCardDto>> CreateCard(int customerId)    
    {
        try
        {
            if (customerId == 0)
            {
                return BadRequest("Customer Id is required.");
            }
            var newCard = await _cardManagementService.CreateCard(customerId);
            return Created(string.Empty, newCard);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Customer does not exist.");
            return BadRequest("Invalid customer.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating card.");
            return StatusCode(500, "Error creating card.");
        }        
    }

    [HttpPost("Pay", Name = "Pay")]
    public async Task<IActionResult> Pay(string creditCardNumber, DateTime expDate, string cvv, decimal amount)
    {
        try
        {            
            ValidatePaymentInput(creditCardNumber, cvv, amount);            
            await _cardManagementService.ValidateCreditCard(creditCardNumber, expDate, cvv, amount);            

            //Register the payment
            Payment payment = new Payment
            {
                CreditCardNumber = creditCardNumber,
                Amount = amount,
                PaymentDate = DateTime.Now,
                PaymentFee = _paymentFeeService.GetCurrentFee()
            };

            await _cardManagementService.RegisterPayment(payment);                
            return Ok("Payment registered successfully.");            
        }
        catch (ArgumentException ex)
        {            
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering payment.");
            return StatusCode(500, "Error registering payment.");
        }
    }

    [HttpGet("GetCardBalance", Name = "GetCardBalance")]
    public async Task<ActionResult<decimal>> GetCardBalance(string cardNumber)
    {
        try
        {
            if (string.IsNullOrEmpty(cardNumber))
            {
                return BadRequest("Card number is required.");
            }

            decimal balance = await _cardManagementService.GetCardBalance(cardNumber);

            return Ok(balance);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Invalid card number.");
            return BadRequest("Invalid card number.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting card balance.");
            return StatusCode(500, "Error getting card balance.");
        }
        
    }


    private void ValidatePaymentInput(string creditCardNumber, string cvv, decimal amount)
    {
        if (string.IsNullOrEmpty(creditCardNumber))
        {
            throw new ArgumentException("Credit card number is required.");            
        }

        if (string.IsNullOrEmpty(cvv))
        {
            throw new ArgumentException("CVV is required.");            
        }

        if (amount <= 0)
        {
            throw new ArgumentException("Amount must be greater than zero.");            
        }        
    }

}


