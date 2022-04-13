using PMS;

List<double> ProcessTine = new List<double>() { 11.4, 12.6, 19.8, 12.5, 14.4, 9.9, 9.9, 
    7.3, 14.0, 14.4, 17.4, 14.5, 19.0, 7.1, 10.4, 8.8, 17.8, 9.2, 8.4, 9.9, 10.4, 8.2, 
    12.1, 19.4, 9.6, 19.7, 15.6, 11.0, 16.1, 10.0, 19.9, 13.3, 8.1, 12.2, 13.8, 15.9, 
    15.4, 9.7, 19.2, 7.7, 7.9, 15.0, 11.7, 19.1, 8.5, 10.5, 16.4, 12.1, 7.9, 9.7 }; 
int NumberOfMachines = 5;
int NumberOfJobs = ProcessTine.Count;
Instance instance = new Instance(NumberOfMachines,NumberOfJobs,ProcessTine);
Solver solver = new Solver(instance);
solver.Run();
solver.Display();