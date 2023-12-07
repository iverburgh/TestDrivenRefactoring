namespace TestDrivenRefactoring.PlayTypeAmountCalculators
{
    public interface IPlayTypePriceCalculator
    {
        int GetCalculatedPrice(int audienceCount);

        PlayType PlayType { get; }
    }
}