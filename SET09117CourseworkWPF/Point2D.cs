using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SET09117CourseworkWPF
{
    /// <summary>
    /// Point2D class
    /// Mark McLaughlin
    /// 40200606
    /// </summary>
    public class Point2D
    {
        /// <summary>
        /// Protected getter / setters X & Y
        /// </summary>
        public double X { get; set; }
        public double Y { get; set; }

        /// <summary>
        /// Method to return the distance between 
        /// </summary>
        /// <param name="c">The customer object passed to calculate distance</param>
        /// <returns>Returns distance</returns>
        public double distance(Customer c)
        {
            //Calculate the distance and return it
            double a = X - c.X;
            double b = Y - c.Y;
            return Math.Sqrt(a * a + b * b);
        }

    }

}
