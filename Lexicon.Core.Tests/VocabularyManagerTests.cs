using System;
using System.Collections.Generic;
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
        private IWordComparisonStrategy _wordComparisonStrategy;

        [SetUp]
        public void SetUp()
        {
            _vocabularyRepository = Substitute.For<IVocabularyRepository>();
            _wordComparisonStrategy = new DefaultWordComparisonStrategy();
            _importer = Substitute.For<IImporter>();

            _vocabularyManager = new VocabularyManager(_vocabularyRepository, _wordComparisonStrategy, _importer);
        }

        [Test]
        public void ctor_throws_ArgumentNullException_if_nulls_passed()
        {
            Assert.Throws<ArgumentNullException>(() => new VocabularyManager(Substitute.For<IVocabularyRepository>(), Substitute.For<IWordComparisonStrategy>(), null));
            Assert.Throws<ArgumentNullException>(() => new VocabularyManager(null, Substitute.For<IWordComparisonStrategy>(), Substitute.For<IImporter>()));
            Assert.Throws<ArgumentNullException>(() => new VocabularyManager(Substitute.For<IVocabularyRepository>(), null, Substitute.For<IImporter>()));
            Assert.Throws<ArgumentNullException>(() => new VocabularyManager(Substitute.For<IVocabularyRepository>(), Substitute.For<IWordComparisonStrategy>(), new IImporter[] { null }));
            Assert.Throws<ArgumentNullException>(() => new VocabularyManager(Substitute.For<IVocabularyRepository>(), Substitute.For<IWordComparisonStrategy>(), Substitute.For<IImporter>(), null, Substitute.For<IImporter>()));
        }

        [Test]
        public void When_one_WordDefinition_with_signle_foreign_CreateWord_adds_single_native_single_pair_and_single_foreign()
        {
            var wordDef = new WordDefinition("тест");
            wordDef.Translations.Add("test");

            _vocabularyManager.CreateWord(wordDef);

            //assert
            var pairs = _vocabularyManager.WordPairs.ForeignsFor("тест");
            CollectionAssertEx.ContainsOnly(pairs, "test");

            var native = _vocabularyManager.NativeWords.AllValues();
            CollectionAssertEx.ContainsOnly(native, "тест");

            var foreign = _vocabularyManager.ForeignWords.AllValues();
            CollectionAssertEx.ContainsOnly(foreign, "test");
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
            var pairs = _vocabularyManager.WordPairs.ForeignsFor("задача");
            CollectionAssertEx.ContainsOnly(pairs, tran);

            var natives = _vocabularyManager.NativeWords.AllValues();
            CollectionAssertEx.ContainsOnly(natives, "задача");

            var foreigns = _vocabularyManager.ForeignWords.AllValues();
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
            var pairs = _vocabularyManager.WordPairs.ForeignsFor("задача");
            CollectionAssertEx.ContainsOnly(pairs, "problem", "objective", "task");

            var natives = _vocabularyManager.NativeWords.AllValues();
            CollectionAssertEx.ContainsOnly(natives, "задача");

            var foreigns = _vocabularyManager.ForeignWords.AllValues();
            CollectionAssertEx.ContainsOnly(foreigns, "problem", "objective", "task");
        }

        [Test]
        public void When_two_WordDefinitions_for_same_natives_with_one_same_foreignCrateWord_adds_single_native_single_foreign_and_single_pair()
        {
            var wordDef1 = new WordDefinition("задача");
            wordDef1.Translations.Add("problem");

            _vocabularyManager.CreateWord(wordDef1);

            var wordDef2 = new WordDefinition("задача");
            wordDef2.Translations.Add("problem");

            _vocabularyManager.CreateWord(wordDef2);

            //assert
            var pairs = _vocabularyManager.WordPairs.ForeignsFor("задача");
            CollectionAssertEx.ContainsOnly(pairs, "problem");

            var natives = _vocabularyManager.NativeWords.AllValues();
            CollectionAssertEx.ContainsOnly(natives, "задача");

            var foreigns = _vocabularyManager.ForeignWords.AllValues();
            CollectionAssertEx.ContainsOnly(foreigns, "problem");
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
            var pairs1 = _vocabularyManager.WordPairs.ForeignsFor("тест");
            CollectionAssertEx.ContainsOnly(pairs1, "test");

            var pairs2 = _vocabularyManager.WordPairs.ForeignsFor("задача");
            CollectionAssertEx.ContainsOnly(pairs2, "objective", "task");

            var natives1 = _vocabularyManager.NativeWords.AllValues();
            CollectionAssertEx.ContainsOnly(natives1, "тест", "задача");

            var foreigns = _vocabularyManager.ForeignWords.AllValues();
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
            var pairs1 = _vocabularyManager.WordPairs.ForeignsFor("проблема");
            CollectionAssertEx.ContainsOnly(pairs1, "problem");

            var pairs2 = _vocabularyManager.WordPairs.ForeignsFor("задача");
            CollectionAssertEx.ContainsOnly(pairs2, "problem", "task");

            var natives1 = _vocabularyManager.NativeWords.AllValues();
            CollectionAssertEx.ContainsOnly(natives1, "проблема", "задача");

            var foreigns = _vocabularyManager.ForeignWords.AllValues();
            CollectionAssertEx.ContainsOnly(foreigns, "problem", "task");
        }

        [Test]
        public void When_three_WordDefinitions_for_three_different_natives_with_one_foreign_CreateWord_adds_three_natives_three_pairs_and_one_foreign()
        {
            var wordDef1 = new WordDefinition("проблема");
            wordDef1.Translations.Add("problem");

            _vocabularyManager.CreateWord(wordDef1);

            var wordDef2 = new WordDefinition("задача");
            wordDef2.Translations.Add("problem");

            _vocabularyManager.CreateWord(wordDef2);

            var wordDef3 = new WordDefinition("трудный");
            wordDef3.Translations.Add("problem");

            _vocabularyManager.CreateWord(wordDef3);

            //assert
            var pairs1 = _vocabularyManager.WordPairs.ForeignsFor("проблема");
            CollectionAssertEx.ContainsOnly(pairs1, "problem");

            var pairs2 = _vocabularyManager.WordPairs.ForeignsFor("задача");
            CollectionAssertEx.ContainsOnly(pairs2, "problem");

            var pairs3 = _vocabularyManager.WordPairs.ForeignsFor("трудный");
            CollectionAssertEx.ContainsOnly(pairs3, "problem");

            var natives = _vocabularyManager.NativeWords.AllValues();
            CollectionAssertEx.ContainsOnly(natives, "проблема", "задача", "трудный");

            var foreigns = _vocabularyManager.ForeignWords.AllValues();
            CollectionAssertEx.ContainsOnly(foreigns, "problem");
        }

        [Test]
        public void When_a_native_with_foreigns_is_defined_GetWordPairs_returns_collections_of_word_pairs_defined_for_that_native()
        {
            string native = "проблема";
            string[] foreigns = {"problem", "task"};
            _vocabularyManager.CreateWord(createWordDefinition(native, foreigns));

            IList<WordPair> pairs = _vocabularyManager.GetWordPairs(native);
            var actual = pairs.Select(x => x.ForeignWord.Value);

            CollectionAssertEx.ContainsOnly(actual, foreigns);
        }

        [Test]
        public void When_multiple_natives_with_their_foreigns_defined_GetWordPairs_returns_collections_of_word_pairs_defined_for_specified_native()
        {
            string native1 = "проблема";
            string[] foreigns1 = { "problem" };
            _vocabularyManager.CreateWord(createWordDefinition(native1, foreigns1));

            string native2 = "задача";
            string[] foreigns2 = { "problem", "objective", "task", "goal" };
            _vocabularyManager.CreateWord(createWordDefinition(native2, foreigns2));

            IList<WordPair> pairs = _vocabularyManager.GetWordPairs(native2);
            var actual = pairs.Select(x => x.ForeignWord.Value);

            CollectionAssertEx.ContainsOnly(actual, foreigns2);
        }

        [Test]
        public void When_no_word_is_found_GetWordPairs_returns_empty_collection()
        {
            IList<WordPair> pairs = _vocabularyManager.GetWordPairs("test");

            Assert.IsNotNull(pairs);
            CollectionAssert.IsEmpty(pairs);
        }

        private WordDefinition createWordDefinition(string native, params string[] foreigns)
        {
            var wordDef = new WordDefinition(native);
            foreach (var f in foreigns)
                wordDef.Translations.Add(f);
            return wordDef;
        }
    }
}
