using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Sockets;
using Lexicon.Common;
using NSubstitute;
using NUnit.Framework;

namespace Lexicon.Core.Tests
{
    [TestFixture]
    [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
    public class VocabularyManagerTests
    {
        private VocabularyManager _vocabularyManager;
        private IImporter _importer;
        private IVocabularyRepository _vocabularyRepository;

        [SetUp]
        public void SetUp()
        {
            _vocabularyRepository = Substitute.For<IVocabularyRepository>();
            _importer = Substitute.For<IImporter>();

            _vocabularyManager = new VocabularyManager(_vocabularyRepository, _importer);
        }

        [Test]
        public void ctor_throws_ArgumentNullException_if_nulls_passed()
        {
            Assert.Throws<ArgumentNullException>(() => new VocabularyManager(Substitute.For<IVocabularyRepository>(), null));
            Assert.Throws<ArgumentNullException>(() => new VocabularyManager(null, Substitute.For<IImporter>()));
            Assert.Throws<ArgumentNullException>(() => new VocabularyManager(Substitute.For<IVocabularyRepository>(), new IImporter[] { null }));
            Assert.Throws<ArgumentNullException>(() => new VocabularyManager(Substitute.For<IVocabularyRepository>(), Substitute.For<IImporter>(), null, Substitute.For<IImporter>()));
        }

        [Test]
        public void When_one_WordDefinition_with_signle_foreign_CreateWord_adds_single_native_single_pair_and_single_foreign()
        {
            var wordDef = new WordDefinition("тест");
            wordDef.Translations.Add("test");

            _vocabularyManager.CreateWord(wordDef);

            //assert
            var pairs = _vocabularyManager.WordPairs.Where(x => x.NativeWord.Value.Equals("тест")).Select(x => x.ForeignWord.Value);
            CollectionAssertEx.ContainsOnly(pairs, "test");

            var native = _vocabularyManager.NativeWords.SingleOrDefault(x => x.Value.Equals("тест"));
            Assert.IsNotNull(native);

            var foreign = _vocabularyManager.ForeignWords.SingleOrDefault(x => x.Value.Equals("test"));
            Assert.IsNotNull(foreign);
        }

        [Test]
        public void When_one_WordDefinition_with_four_different_foreigns_CreateWord_adds_single_native_four_pairs_and_four_foreigns()
        {
            var tran = new[] {"problem", "objective", "task", "goal"};
            var wordDef = new WordDefinition("задача");
            foreach(var t in tran)
                wordDef.Translations.Add(t);

            _vocabularyManager.CreateWord(wordDef);

            //assert
            var pairs = _vocabularyManager.WordPairs.Where(x => x.NativeWord.Value.Equals("задача")).Select(x => x.ForeignWord.Value);
            CollectionAssertEx.ContainsOnly(pairs, tran);

            var natives = _vocabularyManager.NativeWords.Where(x => x.Value.Equals("задача")).Select(x => x.Value);
            CollectionAssertEx.ContainsOnly(natives, "задача");

            var foreigns = _vocabularyManager.ForeignWords.Select(x => x.Value);
            CollectionAssertEx.ContainsOnly(foreigns, tran);
        }

        [Test]
        public void When_two_WordDefinitions_for_single_native_with_three_different_foreigns_CreateWord_adds_single_native_three_pairs_and_three_foreigns()
        {
            var wordDef1 = new WordDefinition("задача");
            wordDef1.Translations.Add("problem");

            _vocabularyManager.CreateWord(wordDef1);

            var wordDef2 = new WordDefinition("задача");
            wordDef2.Translations.Add("objective");
            wordDef2.Translations.Add("task");

            _vocabularyManager.CreateWord(wordDef2);

            //assert
            var pairs = _vocabularyManager.WordPairs.Where(x => x.NativeWord.Value.Equals("задача")).Select(x => x.ForeignWord.Value);
            CollectionAssertEx.ContainsOnly(pairs, "problem", "objective", "task");

            var natives = _vocabularyManager.NativeWords.Where(x => x.Value.Equals("задача")).Select(x => x.Value);
            CollectionAssertEx.ContainsOnly(natives, "задача");

            var foreigns = _vocabularyManager.ForeignWords.Select(x => x.Value);
            CollectionAssertEx.ContainsOnly(foreigns, "problem", "objective", "task");
        }

        [Test]
        public void When_two_WordDefinitions_for_two_different_natives_with_three_different_foreigns_CreateWord_adds_two_natives_three_pairs_and_three_foreigns()
        {
            var wordDef1 = new WordDefinition("тест");
            wordDef1.Translations.Add("test");

            _vocabularyManager.CreateWord(wordDef1);

            var wordDef2 = new WordDefinition("задача");
            wordDef2.Translations.Add("objective");
            wordDef2.Translations.Add("task");

            _vocabularyManager.CreateWord(wordDef2);

            //assert
            var pairs1 = _vocabularyManager.WordPairs.Where(x => x.NativeWord.Value.Equals("тест")).Select(x => x.ForeignWord.Value);
            CollectionAssertEx.ContainsOnly(pairs1, "test");

            var pairs2 = _vocabularyManager.WordPairs.Where(x => x.NativeWord.Value.Equals("задача")).Select(x => x.ForeignWord.Value);
            CollectionAssertEx.ContainsOnly(pairs2, "objective", "task");

            var natives1 = _vocabularyManager.NativeWords.Where(x => x.Value.Equals("тест")).Select(x => x.Value);
            CollectionAssertEx.ContainsOnly(natives1, "тест");

            var natives2 = _vocabularyManager.NativeWords.Where(x => x.Value.Equals("задача")).Select(x => x.Value);
            CollectionAssertEx.ContainsOnly(natives2, "задача");

            var foreigns = _vocabularyManager.ForeignWords.Select(x => x.Value);
            CollectionAssertEx.ContainsOnly(foreigns, "test", "objective", "task");
        }

        [Test]
        public void When_two_WordDefinitions_for_two_different_natives_with_three_intersected_foreigns_CreateWord_adds_two_natives_three_pairs_and_two_foreigns()
        {
            var wordDef1 = new WordDefinition("проблема");
            wordDef1.Translations.Add("problem");

            _vocabularyManager.CreateWord(wordDef1);

            var wordDef2 = new WordDefinition("задача");
            wordDef2.Translations.Add("problem");
            wordDef2.Translations.Add("task");

            _vocabularyManager.CreateWord(wordDef2);

            //assert
            var pairs1 = _vocabularyManager.WordPairs.Where(x => x.NativeWord.Value.Equals("проблема")).Select(x => x.ForeignWord.Value);
            CollectionAssertEx.ContainsOnly(pairs1, "problem");

            var pairs2 = _vocabularyManager.WordPairs.Where(x => x.NativeWord.Value.Equals("задача")).Select(x => x.ForeignWord.Value);
            CollectionAssertEx.ContainsOnly(pairs2, "problem", "task");

            var natives1 = _vocabularyManager.NativeWords.Where(x => x.Value.Equals("проблема")).Select(x => x.Value);
            CollectionAssertEx.ContainsOnly(natives1, "проблема");

            var natives2 = _vocabularyManager.NativeWords.Where(x => x.Value.Equals("задача")).Select(x => x.Value);
            CollectionAssertEx.ContainsOnly(natives2, "задача");

            var foreigns = _vocabularyManager.ForeignWords.Select(x => x.Value);
            CollectionAssertEx.ContainsOnly(foreigns, "problem", "task");
        }
    }
}
