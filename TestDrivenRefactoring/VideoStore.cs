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
                var amount = GetAmount(play, perf);

                volumeCredits += GetVolumeCredits(perf, play);

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

        private static int GetVolumeCredits(Performance perf, Play play)
        {
            var volumeCredits = Math.Max(perf.Audience - 30, 0);
            // add extra credit for every ten comedy attendees
            if (PayType.Comedy == play.PayType) volumeCredits += (int)Math.Floor((decimal)perf.Audience / 5);
            return volumeCredits;
        }

        private static int GetAmount(Play play, Performance performance)
        {
            int amount;

            switch (play.PayType)
            {
                case PayType.Tragedy:
                    amount = 40000;
                    if (performance.Audience > 30)
                    {
                        amount += 1000 * (performance.Audience - 30);
                    }

                    break;

                case PayType.Comedy:
                    amount = 30000;
                    if (performance.Audience > 20)
                    {
                        amount += 10000 + 500 * (performance.Audience - 20);
                    }
                    amount += 300 * performance.Audience;
                    break;

                default:
                    throw new Exception($"unknown type: {play.PayType}");
            }

            return amount;
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