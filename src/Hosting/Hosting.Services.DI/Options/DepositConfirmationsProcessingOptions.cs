namespace Hosting.Services.DI.Options;

public class DepositConfirmationsProcessingOptions
{
    public TimeSpan Interval { get; set; }
    public int MinConfirmations { get; set; }
}