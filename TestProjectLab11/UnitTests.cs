using PairProgram;

namespace TestProjectLab11
{
    public class UnitTests
    {
        [Fact]
        public void Positive_Register_ValidCredentials_ReturnsTrue()
        {
            var service = new UserService();
            bool result = service.Register("ivan_shurpatov", "Pass123");

            Assert.True(result);
            Assert.Single(service.Users);
        }

        [Fact]
        public void Positive_Login_CorrectPassword_ReturnsTrue()
        {
            var service = new UserService();
            service.Register("anna_helkevich", "Pass123");

            bool result = service.Login("anna_helkevich", "Pass123");
            Assert.True(result);
        }

        [Fact]
        public void Positive_HashPassword_SameInputProducesSameHash()
        {
            string hash1 = UserService.HashPassword("Pass123");
            string hash2 = UserService.HashPassword("Pass123");

            Assert.Equal(hash1, hash2);
            Assert.False(string.IsNullOrEmpty(hash1));
        }

        [Fact]
        public void Positive_Register_MultipleUniqueUsers_AllAdded()
        {
            var service = new UserService();

            bool user1 = service.Register("ivan_shurpatov", "Pass123");
            bool user2 = service.Register("timur_leonov", "Pass123");

            Assert.True(user1);
            Assert.True(user2);
            Assert.Equal(2, service.Users.Count);
        }

        [Fact]
        public void Positive_ImportFromCsv_ValidData_AddsUsers()
        {
            var service = new UserService();
            var lines = new List<string>
            {
                "Username;PasswordHash",
                "ChickenGunPro;Pass123",
                "BrawlStarsPro;Pass123"
            };

            int imported = service.ImportFromCsvLines(lines);

            Assert.Equal(2, imported);
            Assert.Equal(2, service.Users.Count);
        }

        [Fact]
        public void Negative_Register_EmptyUsername_ReturnsFalse()
        {
            var service = new UserService();

            bool result = service.Register("", "Pass123");

            Assert.False(result);
            Assert.Empty(service.Users);
        }

        [Fact]
        public void Negative_Register_WhitespacePassword_ReturnsFalse()
        {
            var service = new UserService();

            bool result = service.Register("ChickenGunPro", "   ");

            Assert.False(result);
            Assert.Empty(service.Users);
        }

        [Fact]
        public void Negative_Register_DuplicateUsernameCaseInsensitive_ReturnsFalse()
        {
            var service = new UserService();
            service.Register("admin", "Pass123");

            bool result = service.Register("ADMIN", "Pass123");

            Assert.False(result);
            Assert.Single(service.Users);
        }

        [Fact]
        public void Negative_Login_WrongPassword_ReturnsFalse()
        {
            var service = new UserService();
            service.Register("admin", "Pass123");

            bool result = service.Login("admin", "WrongPass123");

            Assert.False(result);
        }

        [Fact]
        public void Negative_ImportFromCsv_MalformedLines_AddsNothing()
        {
            var service = new UserService();
            var malformedLines = new List<string>
            {
                "Username;PasswordHash",
                "fadsfasdfasdfsad",
                "asdasdasdasdasd"
            };

            int imported = service.ImportFromCsvLines(malformedLines);
            Assert.Equal(0, imported);
            Assert.Empty(service.Users);
        }
    }
}
