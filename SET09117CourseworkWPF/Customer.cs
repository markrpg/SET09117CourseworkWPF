using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SET09117CourseworkWPF
{
    /// <summary>
    /// Customer class
    /// Mark McLaughlin
    /// 40200606
    /// </summary>
    public class Customer : Point2D
    {
        //Local variable to hold customer requirement
        public int c;

        /// <summary>
        /// Holds distance from depot
        /// </summary>
        public double distanceFromDepot { get; set; }

        /// <summary>
        /// Holds state of customer (Visited or not)
        /// </summary>
        public bool Visited { get; set; }

        /// <summary>
        /// Customers default constructor 
        /// </summary>
        /// <param name="x">The X coordinate of the customer.</param>
        /// <param name="y">The Y coordinate of the customer.</param>
        /// <param name="requirement">The requirement of the customer.</param>
        public Customer(int x, int y, int requirement)
        {
            //Assign the local variables
            this.X = x;
            this.Y = y;
            this.c = requirement;
            Visited = false;
        }

    }
}
