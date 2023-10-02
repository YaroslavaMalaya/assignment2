using LastSeenTask;
namespace LastSeenTestProject1;

[TestFixture]
public class ChangeLanguageTests
{
    private ChangeLanguage _changeLanguage;

    [SetUp]
    public void SetUp()
    {
        _changeLanguage = new ChangeLanguage();
    }

    [Test]
    public void WhenLanguageIsUA_LastSeenFormatterResultsInUA()
    {
        var language = "ua";

        _changeLanguage.ChangeLang(language);

        Assert.That(_changeLanguage.result1, Is.EqualTo("онлайн"));
        Assert.That(_changeLanguage.result2, Is.EqualTo("тільки що"));
        Assert.That(_changeLanguage.result3, Is.EqualTo("менше хвилини тому"));
        Assert.That(_changeLanguage.result4, Is.EqualTo("декілька хвилин тому"));
        Assert.That(_changeLanguage.result5, Is.EqualTo("годину тому"));
        Assert.That(_changeLanguage.result6, Is.EqualTo("сьогодні"));
        Assert.That(_changeLanguage.result7, Is.EqualTo("вчора"));
        Assert.That(_changeLanguage.result8, Is.EqualTo("на цьому тижні"));
        Assert.That(_changeLanguage.result9, Is.EqualTo("давно"));
    }

    [Test]
    public void WhenLanguageIsES_LastSeenFormatterResultsInUES()
    {
        var language = "es";

        _changeLanguage.ChangeLang(language);

        Assert.That(_changeLanguage.result1, Is.EqualTo("en línea"));
        Assert.That(_changeLanguage.result2, Is.EqualTo("en este momento"));
        Assert.That(_changeLanguage.result3, Is.EqualTo("hace menos de un minuto"));
        Assert.That(_changeLanguage.result4, Is.EqualTo("hace un par de minutos"));
        Assert.That(_changeLanguage.result5, Is.EqualTo("hace una hora"));
        Assert.That(_changeLanguage.result6, Is.EqualTo("hoy"));
        Assert.That(_changeLanguage.result7, Is.EqualTo("ayer"));
        Assert.That(_changeLanguage.result8, Is.EqualTo("esta semana"));
        Assert.That(_changeLanguage.result9, Is.EqualTo("hace mucho tiempo"));
    }

    [Test]
    public void WhenLanguageIsAnother_LastSeenFormatterResultsInEN()
    {
        var language = "fr";

        _changeLanguage.ChangeLang(language);

        Assert.That(_changeLanguage.result1, Is.EqualTo("online"));
        Assert.That(_changeLanguage.result2, Is.EqualTo("just now"));
        Assert.That(_changeLanguage.result3, Is.EqualTo("less than a minute ago"));
        Assert.That(_changeLanguage.result4, Is.EqualTo("couple of minutes ago"));
        Assert.That(_changeLanguage.result5, Is.EqualTo("an hour ago"));
        Assert.That(_changeLanguage.result6, Is.EqualTo("today"));
        Assert.That(_changeLanguage.result7, Is.EqualTo("yesterday"));
        Assert.That(_changeLanguage.result8, Is.EqualTo("this week"));
        Assert.That(_changeLanguage.result9, Is.EqualTo("long time ago"));
    }
}