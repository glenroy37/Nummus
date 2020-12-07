using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Moq;
using Nummus.Service;
using NUnit.Framework;

namespace NummusUnitTests {
    public class FormatServiceTest {
        private FormatService _formatService;

        [Test]
        public void TestFormattingAustria() {
            SetupLocale("de-AT");
            Assert.AreEqual("€ 123,45", _formatService.FormatToLocalCurrency(123.45m));
        }

        [Test]
        public void TestFormattingUnitedKingdom() {
            SetupLocale("en-GB");
            Assert.AreEqual("£123.45", _formatService.FormatToLocalCurrency(123.45m));
        }

        private void SetupLocale(string locale) {
            _formatService = new FormatService(
                new ConfigurationBuilder()
                    .AddInMemoryCollection(new Dictionary<string, string> {
                        ["Locale"] = locale
                    }).Build()
                );
        }
    }
}