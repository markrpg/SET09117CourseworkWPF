using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using Svg;

namespace SET09117CourseworkWPF
{
    /// <summary>
    /// VRSolution class
    /// Mark McLaughlin
    /// 40200606
    /// </summary>
    class VRSolution
    {
        //VRProblem object to hold instance passed by constuctor
        public VRProblem prob;
        //Linked List of Linked Lists of customers
        public List<List<Customer>> soln;
        //List to hold all customers sorted from lowest to highest from depot
        public List<Customer> depotToCustSorted;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="problem">The customers route problem being passed.</param>
        public VRSolution(VRProblem problem)
        {
            //Sets the local prob VRProblem object to instance passed by constructor
            this.prob = problem;
        }

        //The dumb solution to route problem (One route per customer)
        public void oneRoutePerCustomerSolution()
        {
            //Sets up the linked list of linked lists of customers
            this.soln = new List<List<Customer>>();

            //Go through each customer and set their route
            foreach (Customer c in prob.customers)
            {
                //Create new list of customer and add route information
                List<Customer> route = new List<Customer>();
                //Add customer to the route list
                route.Add(c);
                //Add the route to the list of routes
                soln.Add(route);
            }
        }


        /// <summary>
        /// Method to work out solution costs
        /// </summary>
        /// <returns>Returns the cost</returns>
        public double solnCost()
        {
            //Local variable to hold cost
            double cost = 0;
            //For each list of routes in soln linked list work out the distance and the overall cost
            foreach (List<Customer> route in soln)
            {
                //Create new Customer object assign depot's values to it
                Customer prev = this.prob.depot;
                //iterate through all routes within each list of routes
                foreach (Customer c in route)
                {
                    //assign the local cost variable by working out the distance from the depot x&y and the first customer x&y
                    cost += prev.distance(c);
                    //Assign the local customer object prev to the last customers values
                    prev = c;
                }
                //Take the cost of the depot to customers values and add customer to depot to it, so the cost reflects going 
                //to the customer and returning to the depot
                cost += prev.distance(this.prob.depot);
            }
            //Return the total cost which is the total distance
            return Math.Round(cost,3);
        }

        /// <summary>
        /// Authors Solution to the route finding problem - 40200606
        /// </summary>
        public void mySolver()
        {
            
            //Sorted list of customers by distance to depot (shortest)
            this.depotToCustSorted = new List<Customer>();
            //Get sorted list
            this.depotToCustSorted = getSortedDistanceFromDepot();
            //Sets up the linked list of linked lists of customers
            this.soln = new List<List<Customer>>();
            //Local variable to store depot capacity
            int capacity = this.prob.depot.c;
            //Keep count
            int count = 0;
            //Original size of list
            int origionalSize = depotToCustSorted.Count;

            //While we havn't visited all customers continue building routes
            while (count < origionalSize)
            {
                //Set for new route
                List<Customer> route = new List<Customer>();
                //Set capacity
                capacity = this.prob.depot.c;
                //Filter out customers already visited
                depotToCustSorted = depotToCustSorted.Where(cust => !cust.Visited).ToList();
                //Order by closest from depot so always going to closest customer in every need
                depotToCustSorted = depotToCustSorted.OrderBy(x => x.distanceFromDepot).ToList();
                //To hold current customer
                Customer currentCust = depotToCustSorted[0];

                //Building route
                while (((capacity -= currentCust.c) >= 0))
                {
                    //Set next cust as visited
                    foreach (Customer c in depotToCustSorted.Where(x => x == currentCust))
                    {
                        c.Visited = true;
                        route.Add(currentCust);
                        count++;
                    }

                    //Filter out customers already visited
                    depotToCustSorted = depotToCustSorted.Where(cust => !cust.Visited).ToList();

                    //While we have more than one left to check for nearest neighbour
                    if (depotToCustSorted.Count > 1)
                    {
                        //Find next closest one to visit
                        depotToCustSorted = depotToCustSorted.OrderBy(x => x.distance(currentCust)).ToList();
                        //Set next one to visit
                        currentCust = depotToCustSorted[0];
                    }
                    else if(depotToCustSorted.Count.Equals(1))
                    {
                        //Set next visit
                        currentCust = depotToCustSorted[0];
                    }
                }


                //Add new route
                soln.Add(route);
            }

        }

        /// <summary>
        /// Set all customers to not visited.
        /// </summary>
        public void resetCustomersVisited()
        {
            foreach (Customer c in this.prob.customers)
            {
                c.Visited = false;
            }

            return;
        }

        /// <summary>
        /// Authors method to get sorted list from depot to customer for all customers - 40200606
        /// </summary>
        /// <returns>Returns sorted list.</returns>
        public List<Customer> getSortedDistanceFromDepot()
        {
            //Temporary list to hold 
            List<Customer> tempList = new List<Customer>();

            //Iterate through each customer and set their depot distance
            foreach (Customer c in prob.customers)
            {
                //Set the distance from depot in each customer
                c.distanceFromDepot = c.distance(this.prob.depot);
            }

            //Get sorted distance from depot list and poplate sorted depotToCustSorted List
            foreach (Customer c in prob.customers.OrderBy(x => x.distanceFromDepot).ToList<Customer>())
            {
                //Add to sorted list
                tempList.Add(c);
            }

            //Return sorted list
            return tempList;
        }


        public void writeSVG(string probFilename, string solnFilename)
        {
            string disk;
            string[] colors = "chocolate, cornflowerblue, crimson, cyan, darkblue, darkcyan, darkgoldenrod".Split(',');
            int colIndex = 0;
            string hdr =
                    "<?xml version='1.0'?>\n" +
                    "<!DOCTYPE svg PUBLIC '-//W3C//DTD SVG 1.1//EN' '../../svg11-flat.dtd'>\n" +
                    "<svg width='8cm' height='8cm' viewBox='0 0 500 500' xmlns='http://www.w3.org/2000/svg' version='1.1'>\n";
            string ftr = "</svg>";
            StringBuilder psb = new StringBuilder();
            StringBuilder ssb = new StringBuilder();
            psb.Append(hdr);
            ssb.Append(hdr);
            foreach (List<Customer> route in this.soln)
            {
                ssb.Append(String.Format("<path d='M{0} {1} ", this.prob.depot.X, this.prob.depot.Y));
                foreach (Customer c in route)
                    ssb.Append(String.Format("L{0} {1}", c.X, c.Y));
                ssb.Append(String.Format("z' stroke='{0}' fill='none' stroke-width='2'/>\n",
                        colors[colIndex++ % colors.Length]));
            }

            foreach (Customer c in this.prob.customers)
            {
                disk = String.Format(
                       "<g transform='translate({0},{1})'>" +
                       "<circle cx='0' cy='0' r='{2}' fill='pink' stroke='black' stroke-width='1'/>" +
                       "<text text-anchor='middle' y='5'>{3}</text>" +
                       "</g>\n",
                       c.X, c.Y, 10, c.c);
                psb.Append(disk);
                ssb.Append(disk);
            }
            disk = String.Format("<g transform='translate({0},{1})'>" +
                    "<circle cx='0' cy='0' r='{2}' fill='pink' stroke='black' stroke-width='1'/>" +
                    "<text text-anchor='middle' y='5'>{3}</text>" +
                    "</g>\n", this.prob.depot.X, this.prob.depot.Y, 20, "D");
            psb.Append(disk);
            ssb.Append(disk);
            psb.Append(ftr);
            ssb.Append(ftr);

            using (StreamWriter sw = new StreamWriter(probFilename))
            {
                sw.Write(psb);
                sw.Close();
            }

            using (StreamWriter sw = new StreamWriter(solnFilename))
            {
                sw.Write(ssb);
                sw.Close();
            }
        }


        public void writeBufferSVG(string probFilename,string bufferimage)
        {
            string disk;
            string[] colors = "chocolate, cornflowerblue, crimson, cyan, darkblue, darkcyan, darkgoldenrod".Split(',');
            int colIndex = 0;
            string hdr =
                    "<?xml version='1.0'?>\n" +
                    "<!DOCTYPE svg PUBLIC '-//W3C//DTD SVG 1.1//EN' '../../svg11-flat.dtd'>\n" +
                    "<svg width='8cm' height='8cm' viewBox='0 0 500 500' xmlns='http://www.w3.org/2000/svg' version='1.1'>\n";
            string ftr = "</svg>";
            StringBuilder psb = new StringBuilder();
            StringBuilder ssb = new StringBuilder();
            psb.Append(hdr);
            ssb.Append(hdr);
            foreach (List<Customer> route in this.soln)
            {
                ssb.Append(String.Format("<path d='M{0} {1} ", this.prob.depot.X, this.prob.depot.Y));
                foreach (Customer c in route)
                    ssb.Append(String.Format("L{0} {1}", c.X, c.Y));
                ssb.Append(String.Format("z' stroke='{0}' fill='none' stroke-width='2'/>\n",
                        colors[colIndex++ % colors.Length]));
            }

            foreach (Customer c in this.prob.customers)
            {
                disk = String.Format(
                       "<g transform='translate({0},{1})'>" +
                       "<circle cx='0' cy='0' r='{2}' fill='pink' stroke='black' stroke-width='1'/>" +
                       "<text text-anchor='middle' y='5'>{3}</text>" +
                       "</g>\n",
                       c.X, c.Y, 10, c.c);
                psb.Append(disk);
                ssb.Append(disk);
            }
            disk = String.Format("<g transform='translate({0},{1})'>" +
                    "<circle cx='0' cy='0' r='{2}' fill='pink' stroke='black' stroke-width='1'/>" +
                    "<text text-anchor='middle' y='5'>{3}</text>" +
                    "</g>\n", this.prob.depot.X, this.prob.depot.Y, 20, "D");
            psb.Append(disk);
            ssb.Append(disk);
            psb.Append(ftr);
            ssb.Append(ftr);

            using (StreamWriter sw = new StreamWriter(probFilename,false))
            {
                sw.Write(ssb);
                sw.Close();
            }

            renderSVG(probFilename,bufferimage);
        }

        public void renderSVG(string probbuffername, string probbufferimage)
        {
            string s;

            using (StreamReader sr = new StreamReader(probbuffername))
            {
                s = sr.ReadToEnd();
                sr.Close();
            }

            var byteArray = Encoding.ASCII.GetBytes(s.ToString());
            using (var stream = new MemoryStream(byteArray))
            {
                var svgDocument = SvgDocument.Open(stream);
                var bitmap = svgDocument.Draw();
                if(File.Exists("svgBuffer.bmp"))
                    File.Delete("svgBuffer.bmp");
                bitmap.Save(probbufferimage, ImageFormat.Bmp);                    

            }
        }

        public void writeOut(string filename)
        {
            using (StreamWriter sw = new StreamWriter(filename))
            {
                foreach (List<Customer> route in this.soln)
                {
                    bool firstOne = true;
                    foreach (Customer c in route)
                    {
                        if (!firstOne)
                            firstOne = false;
                        sw.WriteLine("{0},{1},{2}", c.X, c.Y, c.c);
                    }

                }
            }
        }

    }
}
