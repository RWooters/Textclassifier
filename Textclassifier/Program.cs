using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Textclassification;
using static VO.Con;

namespace Textclassifier
{
	class Program
	{
		static void Main(string[] args)
		{
            //Test_Category();
            Test_Classifier();
        }

		static void Test_Category()
		{
			Category cat;
			cat = new Category("Test");
			cat.TeachPhrases("What time is it");
			cat.TeachPhrases("What is the time");
			cat.TeachPhrases("time");

			Console.WriteLine( cat.Test("the time"));
			Console.WriteLine(cat.Test("the date"));
			Console.WriteLine(cat.Test("Time"));
			Console.WriteLine(cat.Test("What will the Time be in 10 minits"));
			Console.WriteLine(cat.Test("What is the time"));
			Console.WriteLine(cat.Test("What is the Date"));
			Console.ReadLine();
			return;
		}
		static void Test_Classifier()
        {
			Classifier cFier;
			cFier = new Classifier();

			cFier.TeachPhrases("time", "what time is it");
			cFier.TeachPhrases("time", "time");
			cFier.TeachPhrases("time", "What will the Time be in 10 minits");
			cFier.TeachPhrases("time", "What is the time");

			cFier.TeachPhrases("date", "what date is it");
			cFier.TeachPhrases("date", "date");
			cFier.TeachPhrases("date", "What will the date be in 10 days");
			cFier.TeachPhrases("date", "What is the date");

			string f;
			f = "whats is the time";
			QOut(cFier.FindStatCategory(f), "=", f);

			f = "date";
			QOut(cFier.FindStatCategory(f), "=", f);

			f = "tell me the date";
			QOut(cFier.FindStatCategory(f), "=", f);

			f = "tell me tim";
			QOut(cFier.FindStatCategory(f), "=", f);

			f = "tell me the time";
			QOut(cFier.FindStatCategory(f), "=", f);
			
			f = "what is the color of the sky";
			QOut(cFier.FindStatCategory(f), "=", f);

			QOut();
			InKey();

			return;
        }
	}
}
