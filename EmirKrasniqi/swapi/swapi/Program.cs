using swapi;
using System.Net.Http.Json;

class Program
{
    static async Task Main(string[] args)
    {
        using var http = new HttpClient { BaseAddress = new Uri("https://swapi.dev/api/") };

        // -----------------------
        //       PLANÈTE 1
        // -----------------------

        // 1. Film au titre le plus long
        var films = await http.GetFromJsonAsync<FilmResult>("films/");
        var longestTitle = films.results.OrderByDescending(f => f.title.Length).First();
        Console.WriteLine($"1. Film au titre le plus long : {longestTitle.title}");

        // 2. Personnage présent dans le plus de films
        var characters = await http.GetFromJsonAsync<CharacterResult>("people/");
        var mostPresent = characters.results.OrderByDescending(c => c.films.Count).First();
        Console.WriteLine($"2. Personnage le plus présent : {mostPresent.name} ({mostPresent.films.Count} films)");

        // 3. Planète la plus peuplée
        var planets = await http.GetFromJsonAsync<PlanetResult>("planets/");
        var mostPopulated = planets.results
            .Where(p => long.TryParse(p.population, out _))
            .OrderByDescending(p => long.Parse(p.population))
            .First();
        Console.WriteLine($"3. Planète la plus peuplée : {mostPopulated.name} ({mostPopulated.population} habitants)");

        // 4. Combien de X-Wings pour un Star Destroyer ?
        var starships = await http.GetFromJsonAsync<StarshipResult>("starships/");
        var xwing = starships.results.FirstOrDefault(s => s.name == "X-wing");
        var starDestroyer = starships.results.FirstOrDefault(s => s.name == "Star Destroyer");

        if (xwing != null && starDestroyer != null &&
            long.TryParse(xwing.cost_in_credits, out long xwingCost) &&
            long.TryParse(starDestroyer.cost_in_credits, out long sdCost))
        {
            var howMany = sdCost / xwingCost;
            Console.WriteLine($"4. Avec un Star Destroyer ({sdCost} crédits), tu peux acheter {howMany} X-Wings ({xwingCost} crédits chacun).");
        }
        else
        {
            Console.WriteLine("Impossible de calculer : prix non disponible.");
        }

        // 5. Obi-Wan Kenobi peut-il piloter le Millennium Falcon ?
        var allPeople = await http.GetFromJsonAsync<CharacterResult>("people/");
        var obiwan = allPeople.results.FirstOrDefault(c => c.name == "Obi-Wan Kenobi");

        if (obiwan != null && obiwan.starships.Any())
        {
            bool canPilotFalcon = false;
            foreach (var starshipUrl in obiwan.starships)
            {
                var ship = await http.GetFromJsonAsync<Starship>(starshipUrl);
                if (ship.name == "Millennium Falcon") canPilotFalcon = true;
            }
            Console.WriteLine($"5. Obi-Wan Kenobi peut-il piloter le Millennium Falcon ? {(canPilotFalcon ? "Oui" : "Non")}");
        }
        else
        {
            Console.WriteLine("Obi-Wan Kenobi n’a pas de vaisseaux référencés.");
        }

        // 6. Vaisseau le plus rapide en vitesse lumière
        var allStarships = await http.GetFromJsonAsync<StarshipResult>("starships/");
        var fastest = allStarships.results
            .Select(s =>
            {
                bool validSpeed = int.TryParse(s.max_atmosphering_speed, out int speed);
                bool validHyper = double.TryParse(s.hyperdrive_rating, out double hyper);

                if (validSpeed && validHyper && hyper > 0)
                {
                    double vmax = speed * (1 / hyper);
                    return new { s.name, vmax };
                }
                return null;
            })
            .Where(x => x != null)
            .OrderByDescending(x => x.vmax)
            .FirstOrDefault();

        Console.WriteLine(fastest != null
            ? $"6. Le vaisseau le plus rapide est : {fastest.name} avec vmax={fastest.vmax:F2}"
            : "Impossible de déterminer le vaisseau le plus rapide.");

        // 7. Nombre de vaisseaux plus rapides que la moyenne
        var starshipsForSpeed = allStarships.results
            .Where(s => int.TryParse(s.max_atmosphering_speed, out _))
            .Select(s => new { s.name, Speed = int.Parse(s.max_atmosphering_speed) })
            .ToList();

        if (starshipsForSpeed.Any())
        {
            double avgSpeed = starshipsForSpeed.Average(s => s.Speed);
            var fasterShips = starshipsForSpeed.Where(s => s.Speed > avgSpeed).ToList();
            Console.WriteLine($"7. Moyenne des vitesses : {avgSpeed:F2}");
            Console.WriteLine($" - Nombre de vaisseaux plus rapides que la moyenne : {fasterShips.Count}");
            fasterShips.Select(s => $"{s.name} ({s.Speed})").Write();
        }

        // 8. Budget nécessaire en CHF
        const double conversionRate = 0.778;
        var fleetCost = allStarships.results
            .Where(s => long.TryParse(s.cost_in_credits, out _))
            .Sum(s => long.Parse(s.cost_in_credits));

        double fleetCostCHF = fleetCost * conversionRate;
        Console.WriteLine($"8. Coût total de la flotte : {fleetCost} crédits");
        Console.WriteLine($" - En francs suisses : {fleetCostCHF:N0} CHF");

        // 9. Génération CSV
        string csvPath = "vaisseau.txt";
        using (var sw = new StreamWriter(csvPath))
        {
            sw.WriteLine("Name,Price,Length,Films,Planets");
            foreach (var ship in allStarships.results)
            {
                string name = ship.name;
                string price = ship.cost_in_credits;
                string length = ship.length;
                var filmTitles = new List<string>();
                foreach (var filmUrl in ship.films ?? new List<string>())
                {
                    var film = await http.GetFromJsonAsync<Film>(filmUrl);
                    filmTitles.Add(film.title.ToLower().Replace(" ", "-"));
                }
                string filmsList = string.Join("-", filmTitles);
                string planetsList = "n/a";
                sw.WriteLine($"{name},{price},{length},{filmsList},{planetsList}");
            }
        }
        Console.WriteLine($"9. Fichier CSV généré : {csvPath}");


        // -----------------------
        //       PLANÈTE 2
        // -----------------------

        Console.WriteLine("\n--- Planète 2 ---");
        Console.Write("Entre un titre de film Star Wars : ");
        string inputTitle = Console.ReadLine();

        // Récupérer la liste des films
        var allFilms = await http.GetFromJsonAsync<FilmResult>("films/");

        // Calculer la distance de Levenshtein pour chaque titre
        int DistanceLevenshtein(string a, string b)
        {
            int[,] d = new int[a.Length + 1, b.Length + 1];

            for (int i = 0; i <= a.Length; i++) d[i, 0] = i;
            for (int j = 0; j <= b.Length; j++) d[0, j] = j;

            for (int i = 1; i <= a.Length; i++)
            {
                for (int j = 1; j <= b.Length; j++)
                {
                    int cost = (a[i - 1] == b[j - 1]) ? 0 : 1;
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost
                    );
                }
            }
            return d[a.Length, b.Length];
        }

        // Trouver le film le plus proche
        var bestMatch = allFilms.results
            .Select(f => new { film = f, dist = DistanceLevenshtein(inputTitle.ToLower(), f.title.ToLower()) })
            .OrderBy(x => x.dist)
            .First();

        if (bestMatch.dist <= 5) // tolérance max
        {
            var film = bestMatch.film;

            Console.WriteLine($"\nFilm trouvé : {film.title}");
            Console.WriteLine($"Sortie : {film.release_date}");
            Console.WriteLine($"Réalisateur : {film.director}");
            Console.WriteLine($"Synopsis : {film.opening_crawl}\n");

            Console.WriteLine("Acteurs principaux :");
            foreach (var url in film.characters.Take(5))
            {
                var character = await http.GetFromJsonAsync<Character>(url);
                Console.WriteLine($" - {character.name}");
            }
        }
        else
        {
            Console.WriteLine("Aucun film ne correspond suffisamment à ta saisie.");
        }



        // -----------------------
        //       PLANÈTE 3
        // -----------------------

        var firstFilm = films.results.First();
        var actors = new List<string>();
        foreach (var url in firstFilm.characters.Take(5))
        {
            var character = await http.GetFromJsonAsync<Character>(url);
            actors.Add($"<li>{character.name}</li>");
        }

        string templatePath = "../../../../ressources/billboard.html";
        string template = File.ReadAllText(templatePath);

        string html = template
            .Replace("{{title}}", firstFilm.title)
            .Replace("{{release_date}}", firstFilm.release_date)
            .Replace("{{director}}", firstFilm.director)
            .Replace("{{opening_crawl}}", firstFilm.opening_crawl)
            .Replace("{{actors}}", string.Join("\n", actors));

        string outputPath = "result.html";
        File.WriteAllText(outputPath, html);

        Console.WriteLine($"HTML généré : {outputPath}");

        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = outputPath,
            UseShellExecute = true
        });
    }
}
