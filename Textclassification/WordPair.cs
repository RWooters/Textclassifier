using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Textclassification
{
	public class WordPair
	{
		
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
			//return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override string ToString()
		{
			return LeftWord.ToLower() + ":" + RightWord.ToLower();
			//return base.ToString();
		}
	}
}
