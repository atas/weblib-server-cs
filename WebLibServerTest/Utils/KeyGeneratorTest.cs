using WebLibServer.Utils;

namespace WebLibServerTest.Utils
{
    public class KeyGeneratorTest
    {
        [Theory]
        [InlineData(8)]
        [InlineData(16)]
        [InlineData(32)]
        public void GetUniqueKey_ShouldReturnKeyOfSpecifiedLength(int size)
        {
            string key = KeyGenerator.GetUniqueKey(size);
            Assert.Equal(size, key.Length);
        }

        [Fact]
        public void GetUniqueKey_ShouldOnlyContainValidChars()
        {
            string key = KeyGenerator.GetUniqueKey(100); // 100 or any reasonable length
            foreach (char c in key)
            {
                Assert.Contains(c, KeyGenerator.chars); // This assumes 'chars' is public or internal. Otherwise, you'd need to redefine the char set here or adjust accessibility.
            }
        }

        [Fact]
        public void GetUniqueKey_MultipleCalls_ShouldProduceDifferentKeys()
        {
            const int numberOfCalls = 100;
            var keys = new HashSet<string>();

            for (int i = 0; i < numberOfCalls; i++)
            {
                keys.Add(KeyGenerator.GetUniqueKey(10)); // 10 or any other reasonable length
            }

            // Assuming a random generation, we expect almost all results to be unique. 
            // However, there's always a minuscule chance of collision with random generation.
            Assert.True(keys.Count == numberOfCalls);
        }
    }
}