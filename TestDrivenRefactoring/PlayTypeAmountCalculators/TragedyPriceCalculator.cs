namespace TestDrivenRefactoring.PlayTypeAmountCalculators
{
    public class TragedyPriceCalculator : IPlayTypePriceCalculator
    {
        public int GetCalculatedPrice(int audienceCount)
        {
            var amount = 40000;
            if (audienceCount > 30)
            {
                amount += 1000 * (audienceCount - 30);
            }

            return amount;
        }

        public PlayType PlayType => PlayType.Tragedy;
    }
}