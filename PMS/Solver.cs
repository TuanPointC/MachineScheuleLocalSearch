using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMS
{
    internal class Solver
    {
        private Instance? _instance { get; set; }
        private Solution? _solution { get; set; }

        public Solver(Instance instance)
        {
            _instance = instance;
            _solution = new Solution(instance.NumberOfMachines, instance.NumberOfJobs);
        }

        public void InsertJob(int indexJob, int indexMachine)
        {
            if (_solution != null && _instance != null)
            {
                _solution.Machines[indexMachine].Add(indexJob);
                _solution.TotalTime[indexMachine] += _instance.GetTime(indexMachine);
                _solution.MaxTime = Math.Max(_solution.MaxTime, _solution.TotalTime[indexMachine]);
                _solution.Address[indexJob] = indexMachine;
            }
        }

        public void Mutate()
        {
            if (_solution != null && _instance != null)
            {
                var Random = new Random();
                var step = Random.Next(5, 25);
                for (int i = 0; i < step; i++)
                {
                    var machine1 = Random.Next(0, _solution.Machines.Count - 1);
                    var machine2 = Random.Next(0, _solution.Machines.Count - 1);
                    while (machine1 == machine2)
                    {
                        machine2 = Random.Next(0, _instance.NumberOfMachines - 1);
                    }

                    var job1 = Random.Next(0, _solution.Machines[machine1].Count - 1);
                    var job2 = Random.Next(0, _solution.Machines[machine2].Count - 1);
                    while (job1 == job2)
                    {
                        job2 = Random.Next(0, _solution.Machines[machine2].Count - 1);
                    }

                    //Swap job between 2 machines
                    var tmp1 = _solution.Machines[machine1].ElementAt(job1);
                    var tmp2 = _solution.Machines[machine2].ElementAt(job2);
                    _solution.Machines[machine1].Remove(tmp1);
                    _solution.Machines[machine1].Add(tmp2);
                    _solution.Machines[machine2].Remove(tmp2);
                    _solution.Machines[machine2].Add(tmp1);

                    // Re-Set total
                    _solution.TotalTime[machine1] += (tmp2 - tmp1);
                    _solution.TotalTime[machine2] += (tmp1 - tmp2);

                    // Re-set address
                    _solution.Address[tmp1] = machine1;
                    _solution.Address[tmp2] = machine2;

                    // Update max time
                    _solution.UpdateMaxTime();


                }
            }
        }

        public static HashSet<int> Replace(HashSet<int> set, int first, int second)
        {
            var firstA = new List<int>();
            var lastA = new List<int>();
            foreach (var item in set)
            {
                if (item == first)
                {
                    break;
                }
                firstA.Add(item);
            }

            for (var i = firstA.Count + 1; i < set.Count; i++)
            {
                lastA.Add(set.ElementAt(i));
            }
            var newSet = new HashSet<int>();
            newSet.UnionWith(firstA);
            newSet.Add(second);
            newSet.UnionWith(lastA);
            return newSet;
        }
        public bool Swap()
        {
            if (_solution != null && _instance != null)
            {
                _solution.UpdateMaxTime();
                var idMaxMachine = _solution.MaxMachine;

                HashSet<int> JobOfMaxMachine = new();
                foreach (var job in _solution.Machines[idMaxMachine])
                {
                    JobOfMaxMachine.Add(job);
                }
                JobOfMaxMachine = JobOfMaxMachine.OrderBy(s => s).ToHashSet();

                for (var iterJobMax = 0; iterJobMax < JobOfMaxMachine.Count; iterJobMax++)
                {
                    for (int j = 0; j < _instance.NumberOfMachines; j++)
                    {
                        if (idMaxMachine == j)
                        {
                            continue;
                        }
                        for (var iterJobOtherMachine = 0; iterJobOtherMachine < _solution.Machines[j].Count; iterJobOtherMachine++)
                        {
                            var timeMaxMachine = _solution.MaxTime - _instance.GetTime(iterJobMax) + _instance.GetTime(_solution.Machines[j].ElementAt(iterJobOtherMachine));
                            var timeOtherMachine = _solution.TotalTime[j] - _instance.GetTime(_solution.Machines[j].ElementAt(iterJobOtherMachine)) + _instance.GetTime(iterJobMax);
                            if (timeMaxMachine < _solution.MaxTime && timeOtherMachine < _solution.MaxTime)
                            {
                                var tmp = JobOfMaxMachine.ElementAt(iterJobMax);
                                var tmp2 = _solution.Machines[j].ElementAt(iterJobOtherMachine);
                                JobOfMaxMachine = Replace(JobOfMaxMachine, tmp, tmp2);
                                _solution.Machines[j] = Replace(_solution.Machines[j], tmp2, tmp);
                                _solution.Address[tmp] = j;
                                _solution.Address[tmp2] = idMaxMachine;
                                _solution.Machines[idMaxMachine] = JobOfMaxMachine;
                                _solution.TotalTime[j] = timeOtherMachine;
                                _solution.TotalTime[idMaxMachine] = timeMaxMachine;
                                _solution.UpdateMaxTime();
                                Console.WriteLine($"MaxTime: {_solution.MaxTime} Score: {_solution.GetScore()}");
                                return true;
                            }
                        }
                    }
                }
            }
            return true;
        }

        public void LocalSearch()
        {
            var IsImprove = true;
            while (IsImprove)
            {
                IsImprove = !Swap();
            }
        }

        public void Run()
        {
            Construction();
            for (int iter = 0; iter < 10000; iter++)
            {
                //Mutate();
                LocalSearch();
            }
        }

        public void Construction()
        {
            List<Tuple<double, int>> jobWithTime = new();
            for (int i = 0; i < _instance?.NumberOfJobs; i++)
            {
                jobWithTime.Add(new Tuple<double, int>(_instance.GetTime(i), i));
            }

            jobWithTime.Sort();

            foreach (var job in jobWithTime)
            {
                if (_solution != null)
                {
                    var indexMachine = _solution.GetShortestMachine();
                    InsertJob(job.Item2, indexMachine);
                }
            }
        }

        public void Display()
        {
            _solution?.Display();
        }
    }
}
