using System.Collections.Immutable;
using FluentAssertions;

namespace TestDrivenRefactoring.Tests
{
    public class VideoStoreTests
    {
        private readonly VideoStore _subject; 

        public VideoStoreTests()
        {
            _subject = new VideoStore();
        }

        [Fact]
        public void Statement_WhenValidParametersAreGiven_ThenCorrectOutput()
        {
            // Arrange
            var plays = new Dictionary<string, Play>
            {
                {"hamlet", new Play("Hamlet", PayType.Tragedy)},
                {"as-like", new Play("As You Like It", PayType.Comedy)},
                {"othello", new Play("Othello", PayType.Tragedy)},
            }.ToImmutableDictionary();

            var invoice = new Invoice("BigCo", new List<Performance>
            {
                new("hamlet", 55),
                new("as-like", 35),
                new("othello", 40),
            });

            const string expectedResult = "Statement for BigCo\r\n" +
                                          "  Hamlet: € 650,00 (55 seats)\r\n  As You Like It: € 580,00 (35 seats)\r\n  Othello: € 500,00 (40 seats)\r\n" +
                                          "Amount owed is € 1.730,00\r\nYou earned 47 credits";

            // Act
            var result = _subject.Statement(invoice, plays);

            // Assert
            result.Should().Be(expectedResult);
        }
    }
}