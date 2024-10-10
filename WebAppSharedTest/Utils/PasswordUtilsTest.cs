using WebAppShared.Utils;

namespace WebAppSharedTest.Utils
{
    public class PasswordUtilsTest
    {
        [Fact]
        public void GetRandomSalt_DefaultLength_ShouldReturnCorrectLength()
        {
            string salt = PasswordUtils.GetRandomSalt();
            Assert.Equal(16, salt.Length);
        }

        [Theory]
        [InlineData(8)]
        [InlineData(20)]
        [InlineData(32)]
        public void GetRandomSalt_CustomLength_ShouldReturnCorrectLength(int length)
        {
            string salt = PasswordUtils.GetRandomSalt(length);
            Assert.Equal(length, salt.Length);
        }

        [Fact]
        public void GetRandomSalt_MultipleCalls_ShouldProduceDifferentResults()
        {
            const int numberOfCalls = 1000;
            var salts = new HashSet<string>();

            for (int i = 0; i < numberOfCalls; i++)
            {
                var salt = PasswordUtils.GetRandomSalt();
                Assert.False(salts.Contains(salt), $"Salt {salt} was not unique");
                salts.Add(salt);
            }
        }

        [Fact]
        public void GetHash_SameInput_ShouldReturnSameHash()
        {
            string password = "testPassword";
            string salt = PasswordUtils.GetRandomSalt();

            string hash1 = PasswordUtils.GetHash(password, salt);

            for (int i = 0; i < 20; i++)
            {
                var hash2 = PasswordUtils.GetHash(password, salt);
                Assert.Equal(hash1, hash2);
            }
        }

        [Fact]
        public void Sha256Hash_SameInput_ShouldReturnSameHash()
        {
            string data = "testData";

            string hash1 = PasswordUtils.Sha256Hash(data);

            for (int i = 0; i < 20; i++)
            {
                var hash2 = PasswordUtils.Sha256Hash(data);
                Assert.Equal(hash1, hash2);
            }
        }

        // GetHash() variability test
        [Fact]
        public void GetHash_DifferentPasswords_ShouldReturnDifferentHashes()
        {
            string password1 = "password1";
            string password2 = "password2";
            string salt = PasswordUtils.GetRandomSalt();

            string hash1 = PasswordUtils.GetHash(password1, salt);
            string hash2 = PasswordUtils.GetHash(password2, salt);

            Assert.NotEqual(hash1, hash2);
        }

        // GetHash() variability test
        [Fact]
        public void GetHash_DifferentSalts_ShouldReturnDifferentHashes()
        {
            string password = "testPassword";
            string salt1 = PasswordUtils.GetRandomSalt();
            string salt2;
            do
            {
                salt2 = PasswordUtils.GetRandomSalt();
            } while (salt1 == salt2);

            string hash1 = PasswordUtils.GetHash(password, salt1);
            string hash2 = PasswordUtils.GetHash(password, salt2);

            Assert.NotEqual(hash1, hash2);
        }

        // GetHash() error handling test
        [Fact]
        public void GetHash_InvalidBase64Salt_ShouldThrowException()
        {
            string password = "testPassword";
            string invalidSalt = "InvalidBase64@@";

            Assert.Throws<FormatException>(() => PasswordUtils.GetHash(password, invalidSalt));
        }

        [Theory]
        [InlineData("fixedTestData", "fa77fbcf830570c3a8ff2a17d9407fc46004fdeec6353171f8b55fb861ff05e2")]
        [InlineData("someValue", "8814cc8dfaa43fe6398301a09b3c9dd897b8787516e88c2197f64d4bc5b3b955")]
        public void Sha256Hash_FixedInput_ShouldReturnExpectedHash(string input, string expectedHash)
        {
            string actualHash = PasswordUtils.Sha256Hash(input);
            Assert.Equal(expectedHash, actualHash);
        }
    }
}