using System;
using FluentAssertions;
using Xunit;

namespace Masterly.Extensions.Core.UnitTests.Extensions
{
    interface ISomeInterfaceA { }
    interface ISomeInterfaceB : ISomeInterfaceA { }
    abstract class SomeClassA { }
    class SomeClassB : SomeClassA, ISomeInterfaceB { }
    class SomeGenericClassC<T> { }
    class SomeAssignableToGenericClassC : SomeGenericClassC<SomeClassB> { }

    public class TypeExtensionsTests
    {
        [Fact]
        public void GetInterfacesAndAbstractClasses_ShouldReturn_3()
        {
            // Arrange
            Type type = typeof(SomeClassB);

            // Act
            Type[] result = type.GetInterfacesAndAbstractClasses();

            // Assert
            result.Length.Should().Be(3);
        }

        [Fact]
        public void IsAssignableToGenericType_ShouldReturn_True()
        {
            // Arrange
            Type givenType = typeof(SomeAssignableToGenericClassC);
            Type genericType = typeof(SomeGenericClassC<>);

            // Act
            bool result = givenType.IsAssignableToGenericType(genericType);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void GetFullNameWithAssemblyName_ShouldReturn_FullTypeNameWithAssembly()
        {
            // Arrange
            Type type = typeof(SomeClassA);

            // Act
            string result = type.GetFullNameWithAssemblyName();
            string assemblyName = typeof(TypeExtensionsTests).Assembly.ManifestModule.Name.Replace(".dll", string.Empty);
            
            // Assert
            result.Should().Be($"{typeof(SomeClassA).FullName}, {assemblyName}");
        }

        [Theory]
        [InlineData(typeof(SomeClassA))]
        [InlineData(typeof(SomeClassB))]
        [InlineData(typeof(ISomeInterfaceA))]
        public void IsComplex_ShouldReturn_True(Type type)
        {
            // Act
            bool result = type.IsComplex();

            // Assert
            result.Should().BeTrue();
        }

        [Theory]
        [InlineData(typeof(int))]
        [InlineData(typeof(double))]
        [InlineData(typeof(DateTime))]
        public void IsComplex_ShouldReturn_False(Type type)
        {
            // Act
            bool result = type.IsComplex();

            // Assert
            result.Should().BeFalse();
        }
    }
}
