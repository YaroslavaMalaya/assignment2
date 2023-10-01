using LastSeenTask;

namespace LastSeenTestProject1
{
    [TestFixture]
    public class UsersLoaderTests
    {
        private HttpClient _client;
        private UsersLoader _usersLoader;

        [SetUp]
        public void Setup()
        {
            _client = new HttpClient();
            _usersLoader = new UsersLoader(_client);
        }

        [TearDown]
        public void TearDown()
        {
            _client.Dispose();
        }

        [Test]
        public void LoadUsers_SuccessfulResponse_ReturnsUserData()
        {
            var userData = _usersLoader.LoadUsers(0);

            Assert.IsNotNull(userData);
            Assert.IsNotNull(userData.data);
        }

        [Test]
        public void CheckUsersLoaderData_LastSeenDateNicknameIsOnline()
        {
            var userData = _usersLoader.LoadUsers(0);
            var outputCapture = new ConsoleCapture();
            outputCapture.StartCapture();

            foreach (var user in userData.data)
            {
                Console.WriteLine($"{user.Nickname} was online {user.LastSeenDate}.");
            }

            outputCapture.StopCapture();
            var capturedOutput = outputCapture.GetCapturedText();
            Assert.IsTrue(capturedOutput.Contains("was online"));
        }
    }

    public class ConsoleCapture : IDisposable
    {
        private readonly StringWriter _stringWriter;
        private readonly TextWriter _originalOutput;

        public ConsoleCapture()
        {
            _stringWriter = new StringWriter();
            _originalOutput = Console.Out;
            Console.SetOut(_stringWriter);
        }

        public void StartCapture()
        {
            _stringWriter.GetStringBuilder().Clear();
        }

        public void StopCapture()
        {
            _stringWriter.Flush();
        }

        public string GetCapturedText()
        {
            return _stringWriter.ToString();
        }

        public void Dispose()
        {
            Console.SetOut(_originalOutput);
            _stringWriter.Dispose();
        }
    }
}
