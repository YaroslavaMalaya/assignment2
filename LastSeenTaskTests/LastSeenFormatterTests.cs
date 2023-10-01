using LastSeenTask;

namespace LastSeenTestProject1
{
    [TestFixture]
    public class LastSeenFormatterTests
    {
        private DateTimeOffset now;
        private string lang = "en";

        [SetUp]
        public void Initialize()
        {
            now = DateTimeOffset.Now;
        }

        [Test]
        public void When_UserIsOnline_ShouldReturn_Online()
        {
            var formatter = new LastSeenFormatter();
            Assert.That(formatter.Format(now, now.Subtract(TimeSpan.Zero), lang), Is.EqualTo("online"));
        }

        [Test]
        public void When_UserWasOnline15SecondsAgo_ShouldReturn_JustNow()
        {
            var formatter = new LastSeenFormatter();
            Assert.That(formatter.Format(now, now.Subtract(TimeSpan.FromSeconds(15)), lang), Is.EqualTo("just now"));
        }

        [Test]
        public void When_UserWasOnline40SecondsAgo_ShouldReturn_LessThanAMinuteAgo()
        {
            var formatter = new LastSeenFormatter();
            Assert.That(formatter.Format(now, now.Subtract(TimeSpan.FromSeconds(40)), lang), Is.EqualTo("less than a minute ago"));
        }

        [Test]
        public void When_UserWasOnline20MinutesAgo_ShouldReturn_CoupleOfMinutesAgo()
        {
            var formatter = new LastSeenFormatter();
            Assert.That(formatter.Format(now, now.Subtract(TimeSpan.FromMinutes(20)), lang), Is.EqualTo("couple of minutes ago"));
        }

        [Test]
        public void When_UserWasOnline70MinutesAgo_ShouldReturn_HourAgo()
        {
            var formatter = new LastSeenFormatter();
            Assert.That(formatter.Format(now, now.Subtract(TimeSpan.FromMinutes(70)), lang), Is.EqualTo("an hour ago"));
        }

        [Test]
        public void When_UserWasOnline3HoursAgo_ShouldReturn_Today()
        {
            var formatter = new LastSeenFormatter();
            Assert.That(formatter.Format(now, now.Subtract(TimeSpan.FromHours(3)), lang), Is.EqualTo("today"));
        }

        [Test]
        public void When_UserWasOnline48HoursAgo_ShouldReturn_Yesterday()
        {
            var formatter = new LastSeenFormatter();
            Assert.That(formatter.Format(now, now.Subtract(TimeSpan.FromHours(47)), lang), Is.EqualTo("yesterday"));
        }

        [Test]
        public void When_UserWasOnline6DaysAgo_ShouldReturn_ThisWeek()
        {
            var formatter = new LastSeenFormatter();
            Assert.That(formatter.Format(now, now.Subtract(TimeSpan.FromDays(6)), lang), Is.EqualTo("this week"));
        }

        [Test]
        public void When_UserWasOnline10DaysAgo_ShouldReturn_LongTimeAgo()
        {
            var formatter = new LastSeenFormatter();
            Assert.That(formatter.Format(now, now.Subtract(TimeSpan.FromDays(10)), lang), Is.EqualTo("long time ago"));
        }
    }
}