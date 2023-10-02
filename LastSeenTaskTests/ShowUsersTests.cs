using Moq;
using LastSeenTask;
namespace LastSeenTestProject1;

[TestFixture]
public class ShowUsersTests
{
    [Test]
    public void CheckUsersNicknameAndLastSeenDate_InUsersData()
    {
        var usersLoaderMock = new Mock<IUserDataLoader>();
        //var formatterMock = new Mock<ILastSeenFormatter>();
        //var loadUsers = new LoadUsers(usersLoaderMock.Object, formatterMock.Object);
        var userData1 = new UserData { data = new[] 
        { new User { Nickname = "User1", LastSeenDate = DateTime.Today.AddMinutes(-5)}, 
            new User { Nickname = "User2", LastSeenDate = DateTime.Today.AddSeconds(-10)} }};

        usersLoaderMock.Setup(loader => loader.LoadUsers(It.IsAny<int>())).Returns(userData1);
        //loadUsers.usersShow("en");
            
        DateTimeOffset check1 = DateTime.Today.AddMinutes(-5);
        DateTimeOffset check2 = DateTime.Today.AddSeconds(-10);
            
        //usersLoaderMock.Verify(loader => loader.LoadUsers(It.IsAny<int>()), Times.AtLeastOnce);
        Assert.That(userData1.data[0].Nickname, Is.EqualTo("User1"));
        Assert.That(userData1.data[0].LastSeenDate, Is.EqualTo(check1));
        Assert.That(userData1.data[1].Nickname, Is.EqualTo("User2"));
        Assert.That(userData1.data[1].LastSeenDate, Is.EqualTo(check2));
    }
}
