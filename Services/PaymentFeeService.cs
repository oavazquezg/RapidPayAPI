

public class PaymentFeeService
{
    private Timer _timer;
    private decimal _currentFee;

    public PaymentFeeService()
    {        
    }

    public void Initialize()
    {
        _currentFee = GeneratePaymentFee();
        _timer = new Timer(UpdatePaymentFee, null, TimeSpan.Zero, TimeSpan.FromHours(1));
    }

    public decimal GetCurrentFee()
    {
        return _currentFee;
    }

    private void UpdatePaymentFee(object state)
    {
        _currentFee = GeneratePaymentFee();
    }

    private decimal GeneratePaymentFee()
    {        
        Random random = new Random();
        var feeFactor = (decimal)(random.NextDouble() * 2);

        //setting initial fee in case is 0
        if(_currentFee == 0)
        {
            _currentFee = feeFactor;
            return _currentFee;
        }

        return _currentFee * feeFactor;
    }    
}