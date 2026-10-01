using PairProgram;

namespace TestProject
{
    public class UserServiceUnitTests
    {
        [Fact]
        public void Register_NewUser_ReturnsTrueAndAddsToList()
        {
            var service = new UserService();

            bool result = service.Register("alex", "12345");

            Assert.True(result);
            Assert.Single(service.Users);
            Assert.Equal("alex", service.Users[0].Username);
        }

        [Fact]
        public void Register_DuplicateUsername_ReturnsFalse()
        {
            var service = new UserService();
            service.Register("alex", "12345");

            bool result = service.Register("ALEX", "other_password");

            Assert.False(result);
            Assert.Single(service.Users);
        }

        [Fact]
        public void Login_ValidAndInvalidCredentials_ReturnsExpected()
        {
            var service = new UserService();
            service.Register("user1", "correct_pass");

            bool validLogin = service.Login("user1", "correct_pass");
            bool wrongPass = service.Login("user1", "wrong_pass");
            bool notFound = service.Login("unknown", "correct_pass");

            Assert.True(validLogin);
            Assert.False(wrongPass);
            Assert.False(notFound);
        }

        [Fact]
        public void ImportFromCsv_IgnoresHeaderAndDuplicates()
        {
            var service = new UserService();
            service.Register("existing_user", "pass");

            var csvLines = new List<string>
            {
                "Username;PasswordHash",
                "existing_user;HASH123",
                "new_user;HASH456"
            };

            int imported = service.ImportFromCsvLines(csvLines);

            Assert.Equal(1, imported);
            Assert.Equal(2, service.Users.Count);
        }
    }
}
