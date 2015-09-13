using System;
using System.Linq;


using Moq;

using Xunit;

namespace CrossPlatformLibrary.Settings.Tests
{
    public class SettingsPropertyTests
    {
        [Fact]
        public void ShouldThrowArgumentNullExceptionIfSettingsServiceIsNull()
        {
            // Arrange
            string settingsKey = "keyName";

            // Act
            Action action = () => new SettingsProperty<string>(null, settingsKey);

            // Assert
            Assert.Throws<ArgumentNullException>(action);
        }

        [Fact]
        public void ShouldThrowArgumentNullExceptionIfKeyIsEmpty()
        {
            // Arrange
            string settingsKey = string.Empty;

            // Act
            Action action = () => new SettingsProperty<string>(null, settingsKey);

            // Assert
            Assert.Throws<ArgumentNullException>(action);
        }

        [Fact]
        public void ShouldNotAcceptKeyLengthLongerThan255Characters()
        {
            // Arrange
            var settingsServiceMock = new Mock<ISettingsService>();
            string settingsKey = GenerateRandomString(256);

            // Act
            Action action = () => new SettingsProperty<string>(settingsServiceMock.Object, settingsKey);

            // Assert
            Assert.Throws<ArgumentException>(action);
        }

        private static string GenerateRandomString(int length)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}