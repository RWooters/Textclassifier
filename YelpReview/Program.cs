using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using Textclassification; 
using YelpReview.Models;
using static VO.Con;

namespace YelpReview
{
	internal class Program
	{
		static void Main(string[] args)
		{
			int i = 0;
			Textclassification.Classifier cFier = new Textclassification.Classifier();
			YelpDataSource yr = new YelpDataSource();
			foreach (YelpModel review in yr.GetAllYelpReviews() )
			{
				i++;
				if (i < 500)
				{
					QOut(review.TypeReview, review.ReviewText);
					cFier.TeachPhrases(review.TypeReview, review.ReviewText);
					continue;
				}	
				
				if(i >= 500 && i < 750)
				{
					var result = cFier.FindStatCategory(review.ReviewText);
					if(!result.StartsWith(review.TypeReview))
					{
						QOut("Failed !!!!");
						QOut(result, review.TypeReview);
					}
					else
					{
						QOut("Matched ");
					}
					QOut("----------------");
					continue;
				}
				break;
			}
			//QOut("Hello y-all");
			QOut();
			InKey();

		}
	}
}
