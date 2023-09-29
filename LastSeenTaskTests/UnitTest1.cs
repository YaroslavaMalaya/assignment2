using LastSeenTask;

namespace LastSeenTestProject1
{
    [TestFixture]
    public class LastSeenFormatterTests
    {
        private DateTimeOffset now;

        [SetUp]
        public void Initialize()
        {
            now = DateTimeOffset.Now;
        }

        [Test]
        public void When_UserIsOnline_ShouldReturn_Online()
        {
            var formatter = new LastSeenFormatter();
            Assert.That(formatter.Format(now, now.Subtract(TimeSpan.Zero)), Is.EqualTo("online"));
        }

        [Test]
        public void When_UserWasOnline15SecondsAgo_ShouldReturn_JustNow()
        {
            var formatter = new LastSeenFormatter();
            Assert.That(formatter.Format(now, now.Subtract(TimeSpan.FromSeconds(15))), Is.EqualTo("just now"));
        }

        [Test]
        public void When_UserWasOnline40SecondsAgo_ShouldReturn_LessThanAMinuteAgo()
        {
            var formatter = new LastSeenFormatter();
            Assert.That(formatter.Format(now, now.Subtract(TimeSpan.FromSeconds(40))), Is.EqualTo("less than a minute ago"));
        }

        [Test]
        public void When_UserWasOnline20MinutesAgo_ShouldReturn_CoupleOfMinutesAgo()
        {
            var formatter = new LastSeenFormatter();
            Assert.That(formatter.Format(now, now.Subtract(TimeSpan.FromMinutes(20))), Is.EqualTo("couple of minutes ago"));
        }

        [Test]
        public void When_UserWasOnline70MinutesAgo_ShouldReturn_HourAgo()
        {
            var formatter = new LastSeenFormatter();
            Assert.That(formatter.Format(now, now.Subtract(TimeSpan.FromMinutes(70))), Is.EqualTo("an hour ago"));
        }

        [Test]
        public void When_UserWasOnline3HoursAgo_ShouldReturn_Today()
        {
            var formatter = new LastSeenFormatter();
            Assert.That(formatter.Format(now, now.Subtract(TimeSpan.FromHours(3))), Is.EqualTo("today"));
        }

        [Test]
        public void When_UserWasOnline48HoursAgo_ShouldReturn_Yesterday()
        {
            var formatter = new LastSeenFormatter();
            Assert.That(formatter.Format(now, now.Subtract(TimeSpan.FromHours(47))), Is.EqualTo("yesterday"));
        }

        [Test]
        public void When_UserWasOnline6DaysAgo_ShouldReturn_ThisWeek()
        {
            var formatter = new LastSeenFormatter();
            Assert.That(formatter.Format(now, now.Subtract(TimeSpan.FromDays(6))), Is.EqualTo("this week"));
        }

        [Test]
        public void When_UserWasOnline10DaysAgo_ShouldReturn_LongTimeAgo()
        {
            var formatter = new LastSeenFormatter();
            Assert.That(formatter.Format(now, now.Subtract(TimeSpan.FromDays(10))), Is.EqualTo("long time ago"));
        }
    }
}