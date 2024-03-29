﻿using System.Collections.Generic;

namespace Textclassification
{
	public class Category
	{
		protected const string SOS = "$$";
		protected const string EOS = "$.";

		protected SortedDictionary<string, WordPair> _wordPair;

		public string Name { get; set; }

		public Category(string name)
		{
			Name = name;
			_wordPair = new SortedDictionary<string, WordPair>();
		}

		public Category()
		{
			_wordPair = new SortedDictionary<string, WordPair>();

		}

		public void TeachCategory()
		{

		}

		public void TeachPhrases(string words)
		{
			string[] wordList;
			wordList = words.Split(' ');
			//wordList = words.Split(new char[] { ' ' });
			TeachPhrases(wordList);
		}
		public void TeachPhrases(string[] words)
		{
			string prev;
			string next;

			prev = SOS;
			foreach (string word in words)
			{
				next = word;
				TeachWordPair(prev, next);
				prev = next;
			}
			next = EOS;
			TeachWordPair(prev, next);

		}

		public void TeachWordPair(string leftWord, string rightWord)
		{
			//string key = $"{leftWord.ToLower()}:{rightWord.ToLower()}";
			string key = WordPair.GenFuzyKey(leftWord, rightWord);
			WordPair wp;
			if (!_wordPair.TryGetValue(key, out wp))
			{
				wp = new WordPair() { LeftWord = leftWord, RightWord = rightWord };
				_wordPair.Add(key, wp);
			}
			wp.Count++;

		}

		public int TestWordPair(string leftWord, string rightWord)
		{
			//string key = $"{leftWord.ToLower()}:{rightWord.ToLower()}";
			string key = WordPair.GenFuzyKey(leftWord, rightWord);
			WordPair wp;
			if (_wordPair.TryGetValue(key, out wp))
			{
				return wp.Count;
			}
			return 0;

		}
		public int Test(string words)
		{
			string[] wordList;
			wordList = words.Split(' ');
			return Test(wordList);
		}
		public int Test(string[] words)
		{
			int acum;
			int match, test;
			string prev;
			string next;

			acum = 0;
			match = 0;
			test = 0;
			prev = SOS;
			foreach (string word in words)
			{
				next = word;
				test = TestWordPair(prev, next);
				if (test > 0)
				{
					match += 1;
					acum += test;
				}
				//acum += TestWordPair(prev, next);
				prev = next;
			}
			next = EOS;
			test = TestWordPair(prev, next);
			if (test > 0)
			{
				match += 1;
				acum += test;
			}
			//acum += TestWordPair(prev, next);
			return match * acum;
		}
		public override bool Equals(object obj)
		{
			return obj is Category category &&
					 Name == category.Name;
		}

		public override int GetHashCode()
		{
			return -1125283371 + EqualityComparer<string>.Default.GetHashCode(Name);
		}
	}
}
