using System.Collections.Immutable;
using System.Globalization;
using System.Text;
using System.Text.Json.Serialization;
using TestDrivenRefactoring.PlayTypeAmountCalculators;

namespace TestDrivenRefactoring
{
    public class VideoStore
    {
        private List<IPlayTypePriceCalculator> _playTypePriceCalculatorList = new()
        {
            new ComedyPriceCalculator(),
            new TragedyPriceCalculator(),
        };

        public string Statement(Invoice invoice, IReadOnlyDictionary<string, Play> plays)
        {
            int totalAmount = 0;
            int volumeCredits = 0;

            IFormatProvider format = new CultureInfo("en-US");

            var result = new StringBuilder().AppendLine($"Statement for {invoice.Customer}");

            foreach (var perf in invoice.Performances)
            {
                var play = plays[perf.PlayId];
                var amount = GetAmount(play, perf);

                volumeCredits += GetVolumeCredits(perf.Audience, play.PlayType);

                // print line for this order
                result.AppendFormat(format, $"  {play.Name}: {amount / 100:C}");
                result.AppendLine($" ({perf.Audience} seats)");
                totalAmount += amount;
            }

            result.AppendFormat(format, $"Amount owed is {totalAmount / 100:C}");
            result.AppendLine("");
            result.Append($"You earned {volumeCredits} credits");
            return result.ToString();
        }

        public static int GetVolumeCredits(int audienceCount, PlayType playType)
        {
            var volumeCredits = Math.Max(audienceCount - 30, 0);
            if (PlayType.Comedy == playType)
            {
                // add extra credit for every five comedy attendees
                volumeCredits += audienceCount / 5;
            }
            return volumeCredits;
        }

        private int GetAmount(Play play, Performance performance)
        {
            var playTypePriceCalculator = _playTypePriceCalculatorList
                .FirstOrDefault(ptpc => ptpc.PlayType == play.PlayType);
            return playTypePriceCalculator?.GetCalculatedPrice(performance.Audience)
                   ?? throw new Exception($"unknown type: {play.PlayType}");
        }
    }

    public enum PlayType : byte
    {
        Tragedy,
        Comedy
    }

    public class Play
    {
        public string Name { get; }
        public PlayType PlayType { get; }

        public Play(string name, PlayType payType)
        {
            Name = name;
            PlayType = payType;
        }
    }

    public class Performance
    {
        public int Audience { get; }
        public string PlayId { get; }

        public Performance(string playId, int audience)
        {
            PlayId = playId;
            Audience = audience;
        }
    }

    public class Invoice
    {
        public string Customer { get; }
        public ImmutableList<Performance> Performances { get; }

        [JsonConstructor] // JsonSerializer.Deserialize will use this ctor 
        public Invoice(string customer, ImmutableList<Performance> performances)
        {
            Customer = customer;
            Performances = performances;
        }

        public Invoice(string customer, IEnumerable<Performance> performances)
            : this(customer, performances.ToImmutableList())
        {
        }
    }
}