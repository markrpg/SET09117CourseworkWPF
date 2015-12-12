using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SET09117CourseworkWPF
{
    /// <summary>
    /// VRProblem class 
    /// Mark McLaughlin
    /// 40200606
    /// </summary>
    class VRProblem
    {
        //Holds copy of the filename
        public string id { get; set; }
        //Holds depot information
        public Customer depot;
        //Holds a collection of customers
        public List<Customer> customers;


        /// <summary>
        /// Default constructor for VRProblem
        /// </summary>
        /// <param name="filename">The filename of the route problem.</param>
        public VRProblem(string filename)
        {
            //Set local string id to the filename thats being passed to the constructor
            this.id = filename;
            //Open new streamreader to open the file 
            using (StreamReader sr = new StreamReader(filename))
            {
                //Reads line from file
                string s = sr.ReadLine();
                //Splits the line from the file using delimiter "," and put it into string array
                string[] dpt = s.Split(',');
                //Create new customer with values from readline
                depot = new Customer(int.Parse(dpt[0]), 
                                     int.Parse(dpt[1]),     
                                     int.Parse(dpt[2]));

                //Initialize the customers list
                customers = new List<Customer>();

                //Go through every customer in the file, 
                while((s = sr.ReadLine()) != null)
                {
                    //For each line in the file, take the values and add them to the customer linked list
                    string[] wrd = s.Split(',');
                    customers.Add(new Customer(int.Parse(wrd[0]),   
                                               int.Parse(wrd[1]), 
                                               int.Parse(wrd[2])));
                }
                //Close the stream
                sr.Close();
            }

        }

        /// <summary>
        /// Method to return the size of the customers linked list
        /// </summary>
        /// <returns>Returns count of customers</returns>
        public int size()
        {
            //Return value
            return this.customers.Count();
        }
        
    }
}
