using System;
using System.Diagnostics.CodeAnalysis;
using Lexicon.Common;
using NSubstitute;
using NUnit.Framework;

namespace Lexicon.Core.Tests
{
    [TestFixture]
    [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
    public class VocabularyManagerTests
    {
        private VocabularyManager _appManager;
        private IImporter _importer;
        private IVocabularyRepository _vocabularyRepository;

        [SetUp]
        public void SetUp()
        {
            _vocabularyRepository = Substitute.For<IVocabularyRepository>();
            _importer = Substitute.For<IImporter>();

            _appManager = new VocabularyManager(_vocabularyRepository, _importer);
        }

        [Test]
        public void ctor_throws_ArgumentNullException_if_nulls_passed()
        {
            Assert.Throws<ArgumentNullException>(() => new VocabularyManager(Substitute.For<IVocabularyRepository>(), null));
            Assert.Throws<ArgumentNullException>(() => new VocabularyManager(null, Substitute.For<IImporter>()));
            Assert.Throws<ArgumentNullException>(() => new VocabularyManager(Substitute.For<IVocabularyRepository>(), new IImporter[] { null }));
            Assert.Throws<ArgumentNullException>(() => new VocabularyManager(Substitute.For<IVocabularyRepository>(), Substitute.For<IImporter>(), null, Substitute.For<IImporter>()));
        }

        
    }
}
