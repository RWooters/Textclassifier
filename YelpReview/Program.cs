using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YelpReview.Models;
using static VO.Con;

namespace YelpReview
{
	internal class Program
	{
		static void Main(string[] args)
		{
			int i = 0;
			YelpDataSource yr = new YelpDataSource();
			foreach (YelpModel review in yr.GetAllYelpReviews() )
			{
				QOut(review.TypeReview, review.ReviewText);
				i++;
				if (i > 20)
					break;

			}
			//QOut("Hello y-all");
			QOut();
			InKey();

		}
	}
}
