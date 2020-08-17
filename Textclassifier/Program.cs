using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Textclassification;

namespace Textclassifier
{
	class Program
	{
		static void Main(string[] args)
		{
			Test_Category();
		}

		static void Test_Category()
		{
			Category cat;
			cat = new Category("Test");
			cat.TeachPhrases("What time is it");
			cat.TeachPhrases("What is the time");
			cat.TeachPhrases("time");

			Console.WriteLine(cat.Test("the time"));
			Console.WriteLine(cat.Test("the date"));
			Console.ReadLine();
			return;
		}
	}
}
