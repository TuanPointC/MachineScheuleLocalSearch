using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMS
{
    internal class Instance
    {
        public int NumberOfMachines { get; set; }
        public int NumberOfJobs { get; set; }
        public List<double> Times { get; set; } = new List<double>();

        public Instance(int numberOfMachines, int numberOfJobs, List<double> times)
        {
            NumberOfMachines = numberOfMachines;
            NumberOfJobs = numberOfJobs;
            Times = times;
        }

        public double GetTime(int id)
        {
            return Times[id];
        }
    }
}
