//Auteur : JMY
//Date   : 25.9.2025
//Lieu   : ETML
//Descr. : Entraînement au test 323
//         Cet outil permet de comparer 2 fichiers (avec le même nombre de lignes) ligne par ligne et indiquer les différences... Il permet aussi de faire du chiffrement

///MENU
Console.WriteLine("+--------------------------------+");
Console.WriteLine("|DIFFIT : A very limited DIFFTOOL|");
Console.WriteLine("+--------------------------------+");

Console.Write("Fichier A: ");
string? pathA = Console.ReadLine();

Console.Write("Fichier B: ");
string? pathB = Console.ReadLine();

// Vérification des entrées utilisateur
var paths = new string?[] { pathA, pathB };
bool filesAreValid = paths.Aggregate(true, (a, b) => a && b != null && File.Exists(b));
if (!filesAreValid)
{
    Console.WriteLine("Erreur: les fichiers doivent être existants et accessibles !");
    Environment.Exit(-2);
}

/// CHARGEMENT DES DONNÉES

string[] linesA = { "..." };


string[] linesB = { "..." };


if (true)
{
    Console.WriteLine("Erreur: les fichiers n'ont pas le même nombre de ligne");
    Environment.Exit(-2);
}

Console.WriteLine(">Fichiers chargés avec succés");

/// Fonctions de nettoyage
Func<string, string> cleanSpaces = text => text;
Func<string, string> cleanTabs = text => text;
Func<string, string> enforceCase = text => text;

/// OPTIONS DE NETTOYAGE
Console.WriteLine("Choisir les options:");

Console.Write("-Ignorer les espaces [o/n]: ");
bool ignoreSpaces = Console.ReadLine() == "o";

Console.Write("-Ignorer les tabulations [o/n]: ");
bool ignoreTabs = Console.ReadLine() == "o";

Console.Write("-Ignorer la casse [o/n]: ");
bool ignoreCase = Console.ReadLine() == "o";


/// Calcul des comparaisons
List<LinesComparison> comparisons = new();

var diffLines = new Liste<LinesComparison>();

Func<LinesComparison, int> countVariations = _ => -1;



/// Diff coloré


/// Chiffrement
Console.Write("\n\nSPECIAL FEATURE: Clé de chiffrement [1-25]: ");
byte key = Convert.ToByte(Console.ReadLine());


/// <summary>
/// Classe pour porter une information de comparaison
/// </summary>
public class LinesComparison
{
    public int Number { get; set; }
    public string ContentA { get; set; } = "";
    public string ContentB { get; set; } = "";

    /// <summary>
    /// Ajuste le numéro de ligne...
    /// </summary>
    public int NumberHuman
    {
        get => Number + 1;
    }

    public int LengthVariation { get => Math.Abs(ContentA.Length - ContentB.Length); }
}
