﻿using System;
using Xunit;
using Xunit.Sdk;

namespace FluentAssertions.Equivalency.Specs;

public partial class SelectionRulesSpecs
{
    public class Accessibility
    {
        [Fact]
        public void When_a_property_is_write_only_it_should_be_ignored()
        {
            // Arrange
            var subject = new ClassWithWriteOnlyProperty
            {
                WriteOnlyProperty = 123,
                SomeOtherProperty = "whatever"
            };

            var expected = new
            {
                SomeOtherProperty = "whatever"
            };

            // Act
            Action action = () => subject.Should().BeEquivalentTo(expected);

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void When_a_property_is_private_it_should_be_ignored()
        {
            // Arrange
            var subject = new Customer("MyPassword")
            {
                Age = 36,
                Birthdate = new DateTime(1973, 9, 20),
                Name = "John"
            };

            var other = new Customer("SomeOtherPassword")
            {
                Age = 36,
                Birthdate = new DateTime(1973, 9, 20),
                Name = "John"
            };

            // Act
            Action act = () => subject.Should().BeEquivalentTo(other);

            // Assert
            act.Should().NotThrow();
        }

        [Fact]
        public void When_a_field_is_private_it_should_be_ignored()
        {
            // Arrange
            var subject = new ClassWithAPrivateField(1234)
            {
                Value = 1
            };

            var other = new ClassWithAPrivateField(54321)
            {
                Value = 1
            };

            // Act
            Action act = () => subject.Should().BeEquivalentTo(other);

            // Assert
            act.Should().NotThrow();
        }

        [Fact]
        public void When_a_property_is_protected_it_should_be_ignored()
        {
            // Arrange
            var subject = new Customer
            {
                Age = 36,
                Birthdate = new DateTime(1973, 9, 20),
                Name = "John"
            };

            subject.SetProtected("ActualValue");

            var expected = new Customer
            {
                Age = 36,
                Birthdate = new DateTime(1973, 9, 20),
                Name = "John"
            };

            expected.SetProtected("ExpectedValue");

            // Act
            Action act = () => subject.Should().BeEquivalentTo(expected);

            // Assert
            act.Should().NotThrow();
        }

        [Fact]
        public void When_a_property_is_internal_and_it_should_be_included_it_should_fail_the_assertion()
        {
            // Arrange
            var actual = new ClassWithInternalProperty
            {
                PublicProperty = "public",
                InternalProperty = "internal",
                ProtectedInternalProperty = "internal"
            };

            var expected = new ClassWithInternalProperty
            {
                PublicProperty = "public",
                InternalProperty = "also internal",
                ProtectedInternalProperty = "also internal"
            };

            // Act
            Action act = () => actual.Should().BeEquivalentTo(expected, options => options.IncludingInternalProperties());

            // Assert
            act.Should().Throw<XunitException>()
                .WithMessage("*InternalProperty*internal*also internal*ProtectedInternalProperty*");
        }

        private class ClassWithInternalProperty
        {
            public string PublicProperty { get; set; }

            internal string InternalProperty { get; set; }

            protected internal string ProtectedInternalProperty { get; set; }
        }

        [Fact]
        public void When_a_field_is_internal_it_should_be_excluded_from_the_comparison()
        {
            // Arrange
            var actual = new ClassWithInternalField
            {
                PublicField = "public",
                InternalField = "internal",
                ProtectedInternalField = "internal"
            };

            var expected = new ClassWithInternalField
            {
                PublicField = "public",
                InternalField = "also internal",
                ProtectedInternalField = "also internal"
            };

            // Act / Assert
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void When_a_field_is_internal_and_it_should_be_included_it_should_fail_the_assertion()
        {
            // Arrange
            var actual = new ClassWithInternalField
            {
                PublicField = "public",
                InternalField = "internal",
                ProtectedInternalField = "internal"
            };

            var expected = new ClassWithInternalField
            {
                PublicField = "public",
                InternalField = "also internal",
                ProtectedInternalField = "also internal"
            };

            // Act
            Action act = () => actual.Should().BeEquivalentTo(expected, options => options.IncludingInternalFields());

            // Assert
            act.Should().Throw<XunitException>().WithMessage("*InternalField*internal*also internal*ProtectedInternalField*");
        }

        private class ClassWithInternalField
        {
            public string PublicField;

            internal string InternalField;

            protected internal string ProtectedInternalField;
        }

        [Fact]
        public void When_a_property_is_internal_it_should_be_excluded_from_the_comparison()
        {
            // Arrange
            var actual = new ClassWithInternalProperty
            {
                PublicProperty = "public",
                InternalProperty = "internal",
                ProtectedInternalProperty = "internal"
            };

            var expected = new ClassWithInternalProperty
            {
                PublicProperty = "public",
                InternalProperty = "also internal",
                ProtectedInternalProperty = "also internal"
            };

            // Act / Assert
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void Private_protected_properties_are_ignored()
        {
            // Arrange
            var subject = new ClassWithPrivateProtectedProperty("Name", 13);
            var other = new ClassWithPrivateProtectedProperty("Name", 37);

            // Act/Assert
            subject.Should().BeEquivalentTo(other);
        }

        private class ClassWithPrivateProtectedProperty
        {
            public ClassWithPrivateProtectedProperty(string name, int value)
            {
                Name = name;
                Value = value;
            }

            public string Name { get; }

            private protected int Value { get; }
        }

        [Fact]
        public void Private_protected_fields_are_ignored()
        {
            // Arrange
            var subject = new ClassWithPrivateProtectedField("Name", 13);
            var other = new ClassWithPrivateProtectedField("Name", 37);

            // Act/Assert
            subject.Should().BeEquivalentTo(other);
        }

        private class ClassWithPrivateProtectedField
        {
            public ClassWithPrivateProtectedField(string name, int value)
            {
                Name = name;
                this.value = value;
            }

            public string Name;

            private protected int value;
        }
    }
}
