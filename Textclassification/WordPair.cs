﻿namespace Textclassification
{
	public class WordPair
	{
		public static string GenKey(string leftWord, string rightWord)
		{
			string key = $"{leftWord.ToLower()}:{rightWord.ToLower()}";
			return key;
		}
		public static string GenFuzyKey(string leftWord, string rightWord)
		{
			string key;
			if (leftWord.StartsWith("$"))
			{
				key = $"{leftWord}:{SoundEx.Miracode.GenerateSoundEx(rightWord)}";
			}
			else if (rightWord.StartsWith("$"))
			{
				key = $"{SoundEx.Miracode.GenerateSoundEx(leftWord)}:{rightWord}";
			}
			else
			{
				key = $"{SoundEx.Miracode.GenerateSoundEx(leftWord)}:{SoundEx.Miracode.GenerateSoundEx(rightWord)}";
			}
			//string key = $"{SoundEx.Miracode.GenerateSoundEx(leftWord)}:{SoundEx.Miracode.GenerateSoundEx(rightWord)}";
			return key;
		}
		public string LeftWord { get; set; }

		public string RightWord { get; set; }
		public int Count { get; set; }

		public WordPair(string leftWord, string rightWord)
		{
			LeftWord = leftWord;
			RightWord = rightWord;
			Count = 0;
		}
		public WordPair()
		{
			Count = 0;
		}

		public override bool Equals(object obj)
		{
			return obj.ToString() == ToString();
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override string ToString()
		{
			return LeftWord.ToLower() + ":" + RightWord.ToLower();
		}
	}
}
