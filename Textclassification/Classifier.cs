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

		Category GetCategory(string cat)
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
			GetCategory(cat).TeachPhrases(phrases);
		}
		public void TeachPhrases(string cat, string phrases)
		{
			GetCategory(cat).TeachPhrases(phrases);
		}

		public string FindFirstCategory(string phrases)
		{
			string firstCat = "";
			string[] wordList;

			wordList = phrases.Split(' ');

			int best = 0;
			foreach (KeyValuePair<string, Category> cat in _Categories)
			{
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

		public string FindStatCategory(string phreaes)
		{
			string firstCat = "";
			string[] wordList;
			SortedDictionary<int, string> statList = new SortedDictionary<int, string>();

			wordList = phreaes.Split(' ');
			
			int acum = 0;
			int best = 0;
			int test;

			foreach (KeyValuePair<string, Category> cat in _Categories)
			{
				test = cat.Value.Test(wordList);
				acum += test;
				 
				if ( !statList.TryGetValue(test, out string name)) 
					statList.Add(test, cat.Value.Name);

			}

			foreach(KeyValuePair<int, string> stat in statList)
			{
				//check for divide by zero
				test = stat.Key > 0 ? stat.Key * 100 / acum : 0;
				if (test > best)
				{
					best = test;
					firstCat = stat.Value;
				}

			}
			if (best < 80)
				firstCat = "dont know";
					
			return firstCat;
		}
	}

}
