
using System;
using System.Collections.Generic;
using System.Linq;

using CrossPlatformLibrary.Bootstrapping;
using CrossPlatformLibrary.IoC;
using CrossPlatformLibrary.Settings.IntegrationTests.Stubs;

using FluentAssertions;

using Xunit;

namespace CrossPlatformLibrary.Settings.IntegrationTests
{
    [Collection("SettingsService")]
    public class SettingsServiceTests
    {
        private static Bootstrapper bootstrapper;

        public SettingsServiceTests()
        {
            if (SimpleIoc.Default.IsRegistered<ISettingsService>() == false)
            {
                bootstrapper = new Bootstrapper();
                bootstrapper.Startup();
            }

            ////var settingsService = SimpleIoc.Default.GetInstance<ISettingsService>();
            ////settingsService.RemoveAll();
        }

        /*
        
                   type == typeof(byte) ||
                   type == typeof(char) ||
                   type == typeof(DateTimeOffset) ||
                   type == typeof(ushort) ||
                   type == typeof(ulong) ||
            */

        [Fact]
        public void ShouldReadWriteBool()
        {
            // Arrange
            bool inputValue = true;

            // Act
            var outputValue = ReadWriteValueOfType(inputValue);

            // Assert
            outputValue.Should().Be(inputValue);
        }

        [Fact]
        public void ShouldReadWriteByte()
        {
            // Arrange
            byte inputValue = byte.MaxValue;

            // Act
            var outputValue = ReadWriteValueOfType(inputValue);

            // Assert
            outputValue.Should().Be(inputValue);
        }

        [Fact]
        public void ShouldReadWriteUri()
        {
            // Arrange
            Uri inputValue = new Uri("http://www.thomasgalliker.ch");

            // Act
            var outputValue = ReadWriteValueOfType(inputValue);

            // Assert
            outputValue.Should().Be(inputValue);
        }

        [Fact]
        public void ShouldReadWriteGuid()
        {
            // Arrange
            Guid inputValue = Guid.NewGuid();

            // Act
            var outputValue = ReadWriteValueOfType(inputValue);

            // Assert
            outputValue.Should().Be(inputValue);
        }

        [Fact]
        public void ShouldReadWriteShort()
        {
            // Arrange
            short inputValue = short.MaxValue;

            // Act
            var outputValue = ReadWriteValueOfType(inputValue);

            // Assert
            outputValue.Should().Be(inputValue);
        }

        [Fact]
        public void ShouldReadWriteInt()
        {
            // Arrange
            int inputValue = 999;

            // Act
            var outputValue = ReadWriteValueOfType(inputValue);

            // Assert
            outputValue.Should().Be(inputValue);
        }

        [Fact]
        public void ShouldReadWriteUInt()
        {
            // Arrange
            uint inputValue = uint.MaxValue;

            // Act
            var outputValue = ReadWriteValueOfType(inputValue);

            // Assert
            outputValue.Should().Be(inputValue);
        }

        [Fact]
        public void ShouldReadWriteLong()
        {
            // Arrange
            long inputValue = long.MaxValue;

            // Act
            var outputValue = ReadWriteValueOfType(inputValue);

            // Assert
            outputValue.Should().Be(inputValue);
        }

        [Fact]
        public void ShouldReadWriteString()
        {
            // Arrange
            string inputValue = "TestString";

            // Act
            var outputValue = ReadWriteValueOfType(inputValue);

            // Assert
            outputValue.Should().Be(inputValue);
        }

        [Fact]
        public void ShouldReadWriteFloat()
        {
            // Arrange
            float inputValue = 999.99f;

            // Act
            var outputValue = ReadWriteValueOfType(inputValue);

            // Assert
            outputValue.Should().Be(inputValue);
        }

        [Fact]
        public void ShouldReadWriteDouble()
        {
            // Arrange
            double inputValue = 999.77d;

            // Act
            var outputValue = ReadWriteValueOfType(inputValue);

            // Assert
            outputValue.Should().Be(inputValue);
        }

        [Fact]
        public void ShouldReadWriteDecimal()
        {
            // Arrange
            decimal inputValue = 999.77m;

            // Act
            var outputValue = ReadWriteValueOfType(inputValue);

            // Assert
            outputValue.Should().Be(inputValue);
        }

        [Fact]
        public void ShouldReadWriteDateTimeAsUniversalTime()
        {
            // Arrange
            var inputValue = DateTime.Now.ToLocalTime();

            // Act
            var outputValue = ReadWriteValueOfType(inputValue);

            // Assert
            outputValue.Ticks.Should().Be(inputValue.Ticks);
        }

        [Fact]
        public void ShouldReadWriteDateTimeAsLocalTime()
        {
            // Arrange
            var inputValue = DateTime.Now.ToUniversalTime();

            // Act
            var outputValue = ReadWriteValueOfType(inputValue);

            // Assert
            outputValue.Ticks.Should().Be(inputValue.Ticks);
        }

        [Fact]
        public void ShouldReadWriteSerializableObject()
        {
            // Arrange
            var person = new Person { Name = "Thomas", Age = 99 };

            // Act
            var outputValue = ReadWriteValueOfType(person);

            // Assert
            outputValue.Should().NotBeNull();
            outputValue.Name.Should().Be(person.Name);
            outputValue.Age.Should().Be(person.Age);
        }

        [Fact]
        public void ShouldReadWriteNullableValue()
        {
            // Arrange
            bool? inputValue = true;

            // Act
            var outputValue = ReadWriteValueOfType(inputValue);

            // Assert
            outputValue.Should().BeTrue();
        }

        [Fact]
        public void ShouldReadWriteList()
        {
            IEnumerable<Person> personList = new List<Person> { new Person { Name = "Person1", Age = 77 }, new Person { Name = "Person2", Age = 88 }, };

            var outputValue = ReadWriteValueOfType(personList);

            // Assert
            outputValue.ElementAt(0).Name.Should().Be(personList.ElementAt(0).Name);
            outputValue.ElementAt(0).Age.Should().Be(personList.ElementAt(0).Age);
            outputValue.ElementAt(1).Name.Should().Be(personList.ElementAt(1).Name);
            outputValue.ElementAt(1).Age.Should().Be(personList.ElementAt(1).Age);
        }

        #region Migration Tests

        [Fact]
        public void ShouldMigrateGuidToString()
        {
            // Arrange
            var originalValue = Guid.NewGuid();

            // Act
            var migrationResult = this.MigrateSetting<Guid, string>(originalValue);

            // Assert
            migrationResult.From.Should().Be(originalValue);
            migrationResult.To.Should().Be(originalValue.ToString());
        }

        [Fact]
        public void ShouldMigrateIntegerToDouble()
        {
            // Arrange
            int originalValue = 999;

            // Act
            var migrationResult = this.MigrateSetting<int, double>(originalValue);

            // Assert
            migrationResult.From.Should().Be(originalValue);
            migrationResult.To.Should().Be(originalValue);
        }

        private MigrationResult<TFrom, TTo> MigrateSetting<TFrom, TTo>(TFrom from)
        {
            // Arrange
            string settingsKeyName = "migrated_" + Guid.NewGuid();

            TFrom originalValue = ReadWriteValueOfType(from, settingsKeyName);

            TTo migratedValue = ReadValueOfType<TTo>(settingsKeyName);

            return new MigrationResult<TFrom, TTo>(originalValue, migratedValue);
        }

        #endregion

        private static T ReadWriteValueOfType<T>(T inputValue, string key = null)
        {
            // Arrange
            string settingsKeyName = "key_" + Guid.NewGuid();
            if (!string.IsNullOrEmpty(key))
            {
                settingsKeyName = key;
            }
            var settingsService = SimpleIoc.Default.GetInstance<ISettingsService>();
            var property = new SettingsProperty<T>(settingsService, settingsKeyName);

            // Act
            property.Value = inputValue;
            var outputValue = property.Value;

            return outputValue;
        }

        private static T ReadValueOfType<T>(string settingsKeyName)
        {
            var settingsService = SimpleIoc.Default.GetInstance<ISettingsService>();
            var property = new SettingsProperty<T>(settingsService, settingsKeyName);

            return property.Value;
        }
    }
}