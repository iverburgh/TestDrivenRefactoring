using System.Collections.Immutable;
using System.Globalization;
using System.Text;
using System.Text.Json.Serialization;

namespace TestDrivenRefactoring
{
    public class VideoStore
    {
        public string Statement(Invoice invoice, IReadOnlyDictionary<string, Play> plays)
        {
            int totalAmount = 0;
            int volumeCredits = 0;

            IFormatProvider format = new CultureInfo("en-US");

            var result = new StringBuilder().AppendLine($"Statement for {invoice.Customer}");

            foreach (var perf in invoice.Performances)
            {
                var play = plays[perf.PlayId];
                int thisAmount;

                switch (play.PayType)
                {
                    case PayType.Tragedy:
                        thisAmount = 40000;
                        if (perf.Audience > 30) thisAmount += 1000 * (perf.Audience - 30);

                        break;

                    case PayType.Comedy:
                        thisAmount = 30000;
                        if (perf.Audience > 20) thisAmount += 10000 + 500 * (perf.Audience - 20);
                        thisAmount += 300 * perf.Audience;
                        break;

                    default:
                        throw new Exception($"unknown type: {play.PayType}");
                }

                // add volume credits
                volumeCredits += Math.Max(perf.Audience - 30, 0);
                // add extra credit for every ten comedy attendees
                if (PayType.Comedy == play.PayType) volumeCredits += (int)Math.Floor((decimal)perf.Audience / 5);

                // print line for this order
                result.AppendFormat(format, $"  {play.Name}: {thisAmount / 100:C}");
                result.AppendLine($" ({perf.Audience} seats)");
                totalAmount += thisAmount;
            }

            result.AppendFormat(format, $"Amount owed is {totalAmount / 100:C}");
            result.AppendLine("");
            result.Append($"You earned {volumeCredits} credits");
            return result.ToString();
        }
    }

    public enum PayType : byte
    {
        Tragedy,
        Comedy
    }


    public class Play
    {
        public string Name { get; }
        public PayType PayType { get; }

        public Play(string name, PayType payType)
        {
            Name = name;
            PayType = payType;
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