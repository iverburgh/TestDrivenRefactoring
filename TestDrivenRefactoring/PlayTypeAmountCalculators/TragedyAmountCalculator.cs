namespace TestDrivenRefactoring.PlayTypeAmountCalculators
{
    public class TragedyAmountCalculator : IPlayTypeAmountCalculator
    {
        public int CalculateAmount(int audienceCount)
        {
            var amount = 40000;
            if (audienceCount > 30)
            {
                amount += 1000 * (audienceCount - 30);
            }

            return amount;
        }
    }
}