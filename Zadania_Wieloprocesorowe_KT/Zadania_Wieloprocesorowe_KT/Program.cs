using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
class Program
{
    static Random rnd = new Random();

    static void Main(string[] args)
    {
        long proby = 0;
        long zle_proby = 0;
        //tworzymy zmienne dla mnożnika czasu wg mocy obliczeniowej danych procesorów 
        double[] pmultipliers = { 1, 1.25, 1.5, 1.75 };

        //tworzymy dwuwymiarową tablicę 2x100 elementów która dla zadań będzie przypisywać losowy czas wykonania 
        int[,] processes = new int[2, 100];
        for (int i = 0; i < processes.GetLength(1); i++)
        {
            processes[0, i] = rnd.Next(10, 91);
        }
        //tworzymy zmienne przechowujące najbardziej efektywny rezultat
        int[,] suboptimal_solution = new int[2, 100];
        double suboptimal_time = 3000;

        // Odliczanie czasu działania pętli
        Stopwatch time = new Stopwatch();
        time.Start(); // rozpoczyna odliczanie
        TimeSpan time_limit = TimeSpan.FromMinutes(5);

    GenerateParent:
        //w 2 rzędzie przypisujemy który procesor ma wykonać dane zadanie 
        int[,] parent = GenerateParent(processes);

    MutateChild:

        //sprawdzamy czy nie upłynął czas obliczeniowy    
        if (time.Elapsed > time_limit)
        {
            Console.WriteLine("Czas upłynął. Najlepsze znaleznione rozwiązanie to:");
            ShowProcesses(1, pmultipliers[0], suboptimal_solution);
            ShowProcesses(2, pmultipliers[1], suboptimal_solution);
            ShowProcesses(3, pmultipliers[2], suboptimal_solution);
            ShowProcesses(4, pmultipliers[3], suboptimal_solution);
            Console.WriteLine($"Czas wykonywania procesu wynosi: {suboptimal_time}.");
            return;
        }
        proby++;

        Console.WriteLine($"Próba: {proby}");

        double tk_parent = CountActualProcessTime(parent, pmultipliers);
        Console.WriteLine($"Aktualny czas wykonywania procesów: {tk_parent}");

        //aktualizujemy najlepsze dotychczas rozwiązanie 
        if (tk_parent < suboptimal_time)
        {
            suboptimal_time = tk_parent;
            suboptimal_solution = CloneArray(parent);
        }
        //tworzymy i mutujemy potomka 
        int[,] child = CloneArray(parent);
        child = Mutate(child);

        //sprawdzamy wolną przestrzeń potomka 
        double tk_child = CountActualProcessTime(child, pmultipliers);

        //aktualizujemy najlepsze dotychczas rozwiązanie 
        if (tk_child < suboptimal_time)
        {
            suboptimal_time = tk_child;
            suboptimal_solution = CloneArray(child);
        }

        //ustalamy warunki 
        if (tk_parent < tk_child)
        {
            zle_proby++;
            if (zle_proby % 2000 == 0) // Co 2000 prób
            {
                Console.WriteLine("Generowanie nowego rodzica.");
                goto GenerateParent;
            }
            goto MutateChild;
        }
        else
        {
            parent = CloneArray(child);
            goto MutateChild;
        }
    }

    static void ShowProcesses(int processor_number, double multiplier, int[,] current_processes)
    {
        Console.WriteLine($"Procesor {processor_number}. Aktualne zadania: ");
        Console.WriteLine("----------------------------------------------------------------------------------------");
        for (int i = 0; i < current_processes.GetLength(1); i++)
        {
            if (current_processes[1, i] == processor_number)
            {
                Console.WriteLine($"Zadanie nr {i + 1}. Czas wykonania = {current_processes[0, i] * multiplier}");
            }
        }
        Console.WriteLine("----------------------------------------------------------------------------------------");
        Console.WriteLine($"Łączny czas zadań dla procesora: {CountProcessesTime(processor_number, multiplier, current_processes)} ");
        Console.WriteLine("----------------------------------------------------------------------------------------");
    }
    static double CountProcessesTime(int processor_number, double multiplier, int[,] current_processes)
    {
        double time = 0;
        for (int i = 0; i < current_processes.GetLength(1); i++)
        {
            if (current_processes[1, i] == processor_number)
            {
                double process_time = current_processes[0, i] * multiplier;
                time += process_time;
            }
        }
        return time;
    }
    static double CountActualProcessTime(int[,] current_process, double[] multipliers)
    {
        double[] times = new double[4];
        times[0] = CountProcessesTime(1, multipliers[0], current_process);
        times[1] = CountProcessesTime(2, multipliers[1], current_process);
        times[2] = CountProcessesTime(3, multipliers[2], current_process);
        times[3] = CountProcessesTime(4, multipliers[3], current_process);
        double tk = 0;
        foreach (double t in times)
        {
            if (tk < t)
            {
                tk = t;
            }
        }
        return tk;
    }
    static int[,] GenerateParent(int[,] processes)
    {
        for (int i = 0; i < processes.GetLength(1); i++)
        {
            processes[1, i] = rnd.Next(1, 5);
        }
        return processes;
    }

    static int[,] Mutate(int[,] processes)
    {
        int random_process = rnd.Next(100);
        int random_processor = rnd.Next(1, 5);

        processes[1, random_process] = random_processor;
        return processes;
    }
    static int[,] CloneArray(int[,] base_array)
    {
        int[,] newArray = new int[2, 100];
        for (int i = 0; i < base_array.GetLength(0); i++)
        {
            for (int j = 0; j < base_array.GetLength(1); j++)
            {
                newArray[i, j] = base_array[i, j];
            }
        }
        return newArray;
    }

}
