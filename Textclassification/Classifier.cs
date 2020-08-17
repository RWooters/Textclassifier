using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Textclassification
{

	public class Classifier
	{
		SortedDictionary<string, Category> _Categories;

		public Classifier()
		{
			_Categories = new SortedDictionary<string, Category>();
		}

		Category GetOrCreateCategory(string cat)
		{
			Category c;
			if (!_Categories.TryGetValue(cat, out c))
			{
				c = new Category(cat);
				_Categories.Add(cat, c);
			}
			return c;
		}

		public void TeachPhrases(string cat, string[] phrases)
		{
			GetOrCreateCategory(cat).TeachPhrases(phrases);
		}
		public void TeachPhrases(string cat, string phrases)
		{
			GetOrCreateCategory(cat).TeachPhrases(phrases);
		}

		public string FindFirstCategory(string phrases)
		{
			string firstCat = "";
			string[] wordList;

			wordList = phrases.Split(' ');

			foreach (KeyValuePair<string, Category> cat in _Categories)
			{
				int best = 0;
				int test = 0;
				test = cat.Value.Test(wordList);
				if ( test > best)
				{
					best = test;
					firstCat = cat.Value.Name;
				}
			}
			return firstCat;
		}

	}

}
