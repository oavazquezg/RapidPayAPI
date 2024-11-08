namespace RapidPayAPI.DTOs
{
    public class CreditCardDto
    {        
        public string CardNumber { get; set; }             
        public string CardHolderName { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string CVV { get; set; }        
    }
}