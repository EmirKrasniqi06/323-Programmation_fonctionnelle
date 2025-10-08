var data = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
data = Enumerable.Range(0, 1500).ToArray();

int sum = 0;

void SumImpure(int number)
{
    sum = sum + number;
}

int SumPure(int number1,int number2)
{
    return number1 + number2;
}

int realSum = data.Sum();
Console.WriteLine($"Référence: {realSum}");
for (int i = 1; i <= 10; i++)
{
    sum = 0;
    Console.WriteLine($"{i} noeuds");
    var jobs = data.Chunk(i).ToList();

    var tasks = new List<Task<int>>();
    jobs.AsParallel().ToList().ForEach(job =>
    {
        Task<int> task = Task.Run(() =>
                //job.ToList().ForEach(number => SumImpure(number))
                job.Aggregate(SumPure)
            );
        tasks.Add(task);
    });

    //Partie synchrone
    tasks.ForEach(task => 
    {
        task.Wait();
        sum += task.Result;
    });

    Console.WriteLine($"Résultat:{sum} [+-{Math.Abs(realSum-sum)}]");
}

/*
Tuple<int,int> gpsCoordinate = Tuple.Create(5,10);
gpsCoordinate.Item1++;
gpsCoordinate = Tuple.Create(gpsCoordinate.Item1 + 1, gpsCoordinate.Item2 + 1);

var gpsCoordinate2 = (50, 100);
gpsCoordinate2.Item1++;
*/