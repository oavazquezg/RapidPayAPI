namespace RapidPayAPI.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public string CreditCardNumber { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal PaymentFee { get; set; }

        public CreditCard CreditCard { get; set; }
    }
}