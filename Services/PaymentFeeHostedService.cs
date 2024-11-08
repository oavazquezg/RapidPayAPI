

public class PaymentFeeHostedService : IHostedService
{
    private readonly PaymentFeeService _paymentFeeService;

    public PaymentFeeHostedService(PaymentFeeService paymentFeeService)
    {
        _paymentFeeService = paymentFeeService;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {        
        _paymentFeeService.Initialize();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {        
        return Task.CompletedTask;
    }
}