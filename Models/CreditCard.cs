namespace RapidPayAPI.Models
{
    public class CreditCard
    {   
        public string CardNumber { get; set; }
        public int CustomerId { get; set; }
        public string CardHolderName { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string CVV { get; set; }
        public decimal Balance { get; set; }
        public const decimal CreditCardLimit = 10000;
        public Customer Customer { get; set; }
        
    }
}