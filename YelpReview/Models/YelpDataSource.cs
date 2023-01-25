using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YelpReview.Models
{
	public class YelpDataSource
	{
		string filePath;

		public YelpDataSource()
		{
			string strExeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
			//This will strip just the working path name:
			//C:\Program Files\MyApplication
			filePath = Path.Combine( System.IO.Path.GetDirectoryName(strExeFilePath), "yelp_labelled.txt");
		}

		public IEnumerable<YelpModel> GetAllYelpReviews()
		{
			foreach(var line in File.ReadLines(filePath))
			{
				var model = new YelpModel();
				var yLine = line.Split('\t');
				model.ReviewText = yLine[0];
				model.TypeReview = yLine[1];
				yield return model;

			}
		}
	}
}
