//Auteur : JMY
//Date   : 25.9.2025
//Lieu   : ETML
//Descr. : Entraînement au test 323
//         Cet outil permet de comparer 2 fichiers (avec le même nombre de lignes) ligne par ligne et indiquer les différences... Il permet aussi de faire du chiffrement

using System.Data.Common;
using System.Security.Cryptography.X509Certificates;

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
// TODO: 01 Charger le contenu texte du fichier A (indice: File.ReadAllLines...)
string[] linesA = File.ReadAllLines(pathA);

// TODO: 02 Charger le contenu texte du fichier B (indice: File.ReadAllLines...)
string[] linesB = File.ReadAllLines(pathB);

// TODO: 03 Vérifier que les fichier ont le même nombre de lignes
if (linesA.Count() != linesB.Count())
{
    Console.WriteLine("Erreur: les fichiers n'ont pas le même nombre de ligne");
    Environment.Exit(-2);
}

Console.WriteLine(">Fichiers chargés avec succés");

// TODO: 04 Définir les fonctions de nettoyage
// Une fonction de nettoyage reçoit un texte (une ligne de fichier) et renvoie cette même ligne adaptée
// Il existe la fonction Replace sur les string...
// Le caractère tabulation s’écrit \t
Func<string, string> cleanSpaces = text => text.Replace(" ", "");
Func<string, string> cleanTabs = text => text.Replace("\t", "");
Func<string, string> enforceCase = text => text.ToLower();

/// OPTIONS DE NETTOYAGE
Console.WriteLine("Choisir les options:");

Console.Write("-Ignorer les espaces [o/n]: ");
bool ignoreSpaces = Console.ReadLine() == "o";

Console.Write("-Ignorer les tabulations [o/n]: ");
bool ignoreTabs = Console.ReadLine() == "o";

Console.Write("-Ignorer la casse [o/n]: ");
bool ignoreCase = Console.ReadLine() == "o";

// TODO:  05 Appliquer le nettoyage selon la demande utilisateur
if (ignoreSpaces)
{
    linesA.Select(cleanSpaces);
    linesB.Select(cleanSpaces);
}
if (ignoreTabs)
{
    linesA.Select(cleanTabs);
    linesB.Select(cleanTabs);
}
if (ignoreCase)
{
    linesA.Select(enforceCase);
    linesB.Select(enforceCase);
}

// TODO: 06 Créer et remplir une liste de LinesComparison à partir de linesA et linesB
List<LinesComparison> comparisons = new();
//for (int i=0; i < linesA.Count(); i++)
//{
//    comparisons.Add(new LinesComparison() { Number = i, ContentA = linesA[i], ContentB = linesB[i] });
//}

//int i=0;
comparisons = linesA.Select((line,index) => new LinesComparison() { Number = index/*i++*/, ContentA = line, ContentB = linesB[index/*i*/] }).ToList();

// TODO: 07 Sélectionner les lignes qui ont des différences
var diffLines = comparisons.Where(comparison => comparison.ContentA != comparison.ContentB).ToList();

// TODO: 08 Afficher le nombre de lignes identiques et différentes entre les 2 fichiers
Console.WriteLine("Les lignes différentes sont " + diffLines.Count());
Console.WriteLine("Les lignes identiques sont " + (comparisons.Count() - diffLines.Count()));

// TODO: 09 Définir une fonction qui compte les différences (caractères différents) entre deux textes (sera utilisé pour les 2 lignes de A et B...)
// Pour info/rappel, la fonction Zip (comme une fermeture éclair) permet d’associer deux listes.
// Et pour info/rappel, un string est une liste de char...
// Ainsi "12345".Zip("ABCDE", (a, b) => $"{a}{b}").ToList().ForEach(Console.Write);//1A2B3C4D5E
// ATTENTION: zip ne prend que le nombre d’éléments minimum commun entre 2 listes...
// Ceci implique une correction: en plus du nombre de différences, il faut ajouter la différence du nombre de caractères entre les deux...
Func<LinesComparison, int> countVariations = _ => _.ContentA.Zip(_.ContentB, (a, b) => a == b).Count(c => c == false) + Math.Abs(_.ContentA.Length - _.ContentB.Length);

// TODO: 10 Afficher pour chaque ligne différente, le nombre de variations
diffLines.Select(dl => new { LineNumber = dl.NumberHuman, Variations = countVariations(dl) }).ToList()
    .ForEach(v => Console.WriteLine($"Dans la ligne {v.LineNumber} il y a : {v.Variations} variations"));

/// Diff coloré
// TODO: 11 Colorier les différences
// Pour chaque ligne où il y a des différences:
// On affiche ainsi:
// Les lettres similaires sont en vert
// Les lettres différentes sont en rouge (options entre[a/b])
// On n’indique rien sur les caractères en plus ou en moins

diffLines.ForEach(_ =>
{
    Console.Write($"Ligne {_.NumberHuman}: ");

    _.ContentA.Zip(_.ContentB, (a, b) =>
    {
        if (a == b)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(a);
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write($"[{a}/{b}]");
        }
        Console.ResetColor();
        return 0; 
    }).ToList();

    Console.WriteLine();
});

/// Chiffrement
// TODO: 11 Créer une fonction qui chiffre le 1er fichier en décalant les caratères d’un nombre
//saisi par l’utilisateur (clé)
// Le contenu chiffré est enregistré sur le disque dans le fichier "cipheredA.txt"
// Le pendant de ReadAllLines est WriteAllLines
Console.Write("\n\nSPECIAL FEATURE: Clé de chiffrement [1-25]: ");
byte key = Convert.ToByte(Console.ReadLine());

Func<string, int, string> cipher = (line, k) =>
    new string(line.Select(ch =>
        char.IsLetter(ch)
            ? (char)((ch >= 'A' && ch <= 'Z')
                ? 'A' + ((ch - 'A' + k) % 26)
                : 'a' + ((ch - 'a' + k) % 26))
            : ch
    ).ToArray());

linesA
    .Select(line => cipher(line, key))
    .ToList()
    .ForEach(line => File.AppendAllText("cipheredA.txt", line + Environment.NewLine));

Console.WriteLine("Fichier chiffré enregistré : cipheredA.txt");


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
