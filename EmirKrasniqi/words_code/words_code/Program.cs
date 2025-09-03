using System.Text.RegularExpressions; // pour utiliser Regex

Console.WriteLine("\n------------------------------------------------------------------------------------------------------");
Console.WriteLine(" Partie 1 : Recherche par critère\n");

// A. Filtrage basique
Console.WriteLine("------------------");
Console.WriteLine(" Filtrage basique");
Console.WriteLine("------------------\n");

string[] words = { "bonjour", "hello", "monde", "vert", "rouge", "bleu", "jaune", "hi", "xis" };

// Mots sans X
Console.WriteLine("Mots sans X");
words.Where(wordNoX => !wordNoX.Contains('x')).Reverse().ToList().ForEach(Console.WriteLine);
// Reverse() --> affiche dans l’ordre inverse de celui naturellement calculé

// Mots avec 4 ou plus caractères
Console.WriteLine("\nMots avec 4 ou plus caractères");
words.Where(word4chars => word4chars.Length >= 4).Order().ToList().ForEach(Console.WriteLine);
// Order() --> tri A-Z

// Mots avec le même nombre de caractères que la moyenne dans la liste
Console.WriteLine("\nMots avec le même nombre de caractères que la moyenne dans la liste");
words.Where(wordAverageLength => wordAverageLength.Count() == Math.Round(words.Average(wordAverageLength => wordAverageLength.Length), 0)).OrderDescending().ToList().ForEach(Console.WriteLine);
// Order() --> tri Z-A


// B. Données parasites 1
Console.WriteLine("\n---------------------");
Console.WriteLine(" Données parasites 1");
Console.WriteLine("---------------------\n");

string[] spuriousWords1 = { "whatThe!!!", "bonjour", "hello", "monde", "vert", "rouge", "bleu", "jaune", "My kingdom for a horse !", "Ooops I did it again" };

spuriousWords1.Skip(1).SkipLast(2).ToList().ForEach(Console.WriteLine);


// C. Données parasites 2
Console.WriteLine("\n---------------------");
Console.WriteLine(" Données parasites 2");
Console.WriteLine("---------------------\n");

string[] spuriousWords2 = { "+++++", "<<<<<", ">>>>>", "bonjour", "hello", "@@@@", "vert", "rouge", "bleu", "jaune", "#####", "%%%%%%%" };

spuriousWords2.Where(word => Regex.IsMatch(word, "^[a-zA-Z]")).ToList().ForEach(Console.WriteLine);


// D. Élitisme
Console.WriteLine("\n----------");
Console.WriteLine(" Élitisme");
Console.WriteLine("----------\n");

string[] wordsElitism = { "i am the winner", "hello", "monde", "vert", "rouge", "bleu", "i am the looser" };

// Console.WriteLine($"The winner is : {wordsElitism.First()}");
wordsElitism.Take(1).ToList().ForEach(word => Console.WriteLine($"The winner is : {word}"));
// Console.WriteLine($"The looser is : {wordsElitism.Last()}");
wordsElitism.TakeLast(1).ToList().ForEach(word => Console.WriteLine($"The winner is : {word}"));



Console.WriteLine("\n\n------------------------------------------------------------------------------------------------------");
Console.WriteLine(" Partie 2: Epsilon\n");

List<string> wordsList = new List<string> { "bonjour", "hello", "supernova", "billyboybeatsme", "rouge", "bleu", "jaune" };

Dictionary<char, double> frequenciesLetters = new Dictionary<char, double>
{
    { 'a', 7.636 },
    { 'b', 0.901 },
    { 'c', 3.260 },
    { 'd', 3.669 },
    { 'e', 14.715 },
    { 'f', 1.066 },
    { 'g', 0.866 },
    { 'h', 0.737 },
    { 'i', 7.529 },
    { 'j', 0.613 },
    { 'k', 0.049 },
    { 'l', 5.456 },
    { 'm', 2.968 },
    { 'n', 7.095 },
    { 'o', 5.796 },
    { 'p', 2.521 },
    { 'q', 1.362 },
    { 'r', 6.553 },
    { 's', 7.948 },
    { 't', 7.244 },
    { 'u', 6.311 },
    { 'v', 1.628 },
    { 'w', 0.114 },
    { 'x', 0.387 },
    { 'y', 0.308 },
    { 'z', 0.136 }
};

//Func<string, double> Epsilon = word => dictionary => ;

double Epsilon (String wordToAnalyse, Dictionary<char, double> frequencies)
{

    return wordToAnalyse.GroupBy(character => character)
                .ToDictionary(group => group.Key, group => group.Count())
        .Sum(c => frequencies[c.Key] / 100.0 / c.Value);
}

wordsList
    .Where(w =>
    {
        double e = Epsilon(w, frequenciesLetters);
        return e >= 0.5 && e <= 0.97;
    })
    .ToList()
    .ForEach(wordToAnalyse => Console.WriteLine(wordToAnalyse));


Console.ReadLine();