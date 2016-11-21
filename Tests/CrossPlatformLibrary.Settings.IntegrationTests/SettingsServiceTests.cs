using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using CrossPlatformLibrary.Bootstrapping;
using CrossPlatformLibrary.Extensions;
using CrossPlatformLibrary.IoC;
using CrossPlatformLibrary.Settings.IntegrationTests.Stubs;

using FluentAssertions;

using Tracing;

using Xunit;
using Xunit.Abstractions;

namespace CrossPlatformLibrary.Settings.IntegrationTests
{
    [Collection("SettingsService")]
    public class SettingsServiceTests
    {
        private static Bootstrapper bootstrapper;
        private readonly ITestOutputHelper testOutputHelper;

        public SettingsServiceTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
            Tracer.SetDefaultFactory(new ActionTracerFactory((s, entry) => testOutputHelper.WriteLine(s)));

            if (bootstrapper == null)
            {
                bootstrapper = new Bootstrapper();
                bootstrapper.Startup();
            }

            ////var settingsService = SimpleIoc.Default.GetInstance<ISettingsService>();
            ////settingsService.RemoveAll();
        }

        // TODO: Theory does not work properly on xUnit.Devices
        [Theory]
        [ClassData(typeof(PropertyInputData))]
        public void ShouldReadWriteNativeTypes(ITestValue testValue)
        {
            // Arrange
            var inputValue = testValue.Value;

            this.testOutputHelper.WriteLine($"TestValue.Value={testValue.Value}");
            this.testOutputHelper.WriteLine($"TestValue.Type={testValue.Value.GetType().GetFormattedName()}");

            // Act
            var outputValue = ReadWriteValueOfType(inputValue, inputValue.GetType());

            // Assert
            outputValue.Should().Be(inputValue);
        }

        public class PropertyInputData : IEnumerable<object[]>
        {
            private readonly List<object[]> data = new List<object[]>
            {
                new object[] { new TestValue<bool>(false) },
                new object[] { new TestValue<bool>(true) },
                new object[] { new TestValue<short>(short.MaxValue) },
                new object[] { new TestValue<ushort>(ushort.MaxValue) },
                new object[] { new TestValue<int>(int.MaxValue) },
                new object[] { new TestValue<uint>(uint.MaxValue) },
                new object[] { new TestValue<float>(float.MinValue) },
                new object[] { new TestValue<float?>(123.999f) },
                new object[] { new TestValue<float>(float.MaxValue) },
                new object[] { new TestValue<double>(double.MaxValue) },
                new object[] { new TestValue<double>(double.MinValue) },
                new object[] { new TestValue<long?>(123L) },
                new object[] { new TestValue<long>(long.MaxValue) },
                new object[] { new TestValue<long>(long.MinValue) },
                new object[] { new TestValue<ulong>(ulong.MaxValue) },
                new object[] { new TestValue<decimal>(decimal.MaxValue) },
                new object[] { new TestValue<string>(new string('*', 10)) },
                new object[] { new TestValue<byte>(byte.MaxValue) },
                //new object[] { new TestValue<Guid>(Guid.NewGuid()) },
                //new object[] { new TestValue<DateTime>(DateTime.Now.ToLocalTime()) },
                //new object[] { new TestValue<DateTime>(DateTime.Now.ToUniversalTime()) },
            };

            public IEnumerator<object[]> GetEnumerator()
            {
                return this.data.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }

        [Fact]
        public void ShouldReadWriteBool()
        {
            // Arrange
            const bool InputValue = true;

            // Act
            var outputValue = ReadWriteValueOfType(InputValue);

            // Assert
            outputValue.Should().Be(InputValue);
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
            float inputValue = float.MaxValue;

            // Act
            var outputValue = ReadWriteValueOfType(inputValue);

            // Assert
            outputValue.Should().Be(inputValue);
        }

        [Fact]
        public void ShouldReadWriteDouble()
        {
            // Arrange
            double inputValue = double.MaxValue;

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
            var inputValue = DateTime.Now.ToUniversalTime();

            // Act
            var outputValue = ReadWriteValueOfType(inputValue);

            // Assert
            outputValue.ToUniversalTime().Ticks.Should().Be(inputValue.Ticks);
        }

        [Fact]
        public void ShouldReadWriteDateTimeAsLocalTime()
        {
            // Arrange
            var inputValue = DateTime.Now.ToLocalTime();

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

            var outputValue = ReadWriteValueOfType(personList).ToList();

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
            migrationResult.To.Should().Be(originalValue.ToString("B"));
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

            Type genericSettingsProperty = typeof(SettingsProperty<>).MakeGenericType(typeof(T));
            ConstructorInfo constr = genericSettingsProperty.GetTypeInfo().DeclaredConstructors.Single(c => c.GetParameters()[0].ParameterType == typeof(ISettingsService) && c.GetParameters()[1].ParameterType == typeof(string));

            var property = (SettingsProperty<T>)constr.Invoke(new object[] { settingsService, settingsKeyName, null});

            // Act
            property.Value = inputValue;
            var outputValue = property.Value;

            return outputValue;
        }

        private static object ReadWriteValueOfType(object inputValue, Type type, string key = null)
        {
            // Arrange
            string settingsKeyName = "key_" + Guid.NewGuid();
            if (!string.IsNullOrEmpty(key))
            {
                settingsKeyName = key;
            }
            var settingsService = SimpleIoc.Default.GetInstance<ISettingsService>();

            Type genericSettingsProperty = typeof(SettingsProperty<>).MakeGenericType(type);
            ConstructorInfo constr = genericSettingsProperty.GetTypeInfo().DeclaredConstructors.Single(c => c.GetParameters()[0].ParameterType == typeof(ISettingsService) && c.GetParameters()[1].ParameterType == typeof(string));

            var property = (ISettingsProperty)constr.Invoke(new object[] { settingsService, settingsKeyName, null });

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

        public interface ITestValue
        {
            object Value { get; }
        }

        private class TestValue<T> : ITestValue, IXunitSerializable
        {
            public TestValue(T value)
            {
                this.Value = value;
                this.Type = typeof(T);
            }
            
            public object Value { get; set; }

            public Type Type { get; set; }

            public void Deserialize(IXunitSerializationInfo info)
            {
                this.Value = info.GetValue<T>("Value");
                this.Type = typeof(T);
            }

            public void Serialize(IXunitSerializationInfo info)
            {
                info.AddValue("Value", this.Value, typeof(T));
            }
        }
    }
}