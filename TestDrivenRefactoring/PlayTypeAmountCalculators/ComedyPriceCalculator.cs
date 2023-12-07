namespace TestDrivenRefactoring.PlayTypeAmountCalculators
{
    public class ComedyPriceCalculator : IPlayTypeAmountCalculator
    {
        public int CalculateAmount(int audienceCount)
        {
            var amount = 30000;
            if (audienceCount > 20)
            {
                amount += 10000 + 500 * (audienceCount - 20);
            }
            amount += 300 * audienceCount;
            return amount;
        }
    }
}