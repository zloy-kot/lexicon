using System;
using NSubstitute;
using NUnit.Framework;

namespace Lexicon.SimpleTextStorage.Tests
{
    [TestFixture]
    public class SerializerRegistryTests
    {
        private SerializerRegistry _target;

        [SetUp]
        public void SetUp()
        {
            _target = new SerializerRegistry();
        }

        [Test]
        public void When_a_serializer_for_a_type_is_registered_and_trying_to_register_a_serializer_for_the_same_type_Register_throws_ArgumentException()
        {
            ISimpleSerializer<FakeType> s1 = Substitute.For<ISimpleSerializer<FakeType>>();
            ISimpleSerializer<FakeType> s2 = Substitute.For<ISimpleSerializer<FakeType>>();
            _target.Register(s1);

            Assert.Throws<ArgumentException>(() => _target.Register(s2));
        }

        [Test]
        public void When_a_serializer_for_a_type_is_registered_GetSerializer_returns_that_serializer_by_the_type()
        {
            ISimpleSerializer<FakeType> s = Substitute.For<ISimpleSerializer<FakeType>>();

            _target.Register(s);

            var actual = _target.GetSerializer<FakeType>();

            Assert.AreSame(s, actual);
        }

        [Test]
        public void When_a_serializer_for_a_type_is_NOT_registered_GetSerializer_throws_ArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _target.GetSerializer<FakeType>());
        }

        internal class FakeType
        {
            public long Id { get; set; }

            public string Name { get; set; }
        }
    }
}