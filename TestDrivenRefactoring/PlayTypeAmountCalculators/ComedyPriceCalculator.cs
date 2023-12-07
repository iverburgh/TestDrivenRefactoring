namespace TestDrivenRefactoring.PlayTypeAmountCalculators
{
    public class ComedyPriceCalculator : IPlayTypePriceCalculator
    {
        public int GetCalculatedPrice(int audienceCount)
        {
            var amount = 30000;
            if (audienceCount > 20)
            {
                amount += 10000 + 500 * (audienceCount - 20);
            }
            amount += 300 * audienceCount;
            return amount;
        }

        public PlayType PlayType => PlayType.Comedy;
    }
}