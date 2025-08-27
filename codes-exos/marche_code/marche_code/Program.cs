using IronXL;
using System;
using System.Linq;

namespace marche_code
{
    public class Product
    {
        public int Stand { get; set; }     // Colonne A
        public string Seller { get; set; }   // Colonne B
        public string Name { get; set; }          // Colonne C
        public int Quantity { get; set; }        // Colonne D
    }
    class Program
    {
        public static void Main(string[] args)
        {
            // Charger le fichier Excel
            WorkBook workBook = WorkBook.Load(
                @"C:\Users\ph76act\Desktop\git\323-Programmation_fonctionnelle\exos\marché\Place du marché.xlsx"
            );

            WorkSheet workSheet = workBook.GetWorkSheet("Produits");

            // Lire tous les produits dans une liste
            List<Product> produits = ReadProducts(workSheet, 2, 75); // lignes 2 à 75

            // Compter les vendeurs de pêches
            int peachSellers = produits.Count(p => p.Name == "Pêches");
            Console.WriteLine("Il y a " + peachSellers + " vendeurs de pêches");

            // Trouver le producteur avec le plus de pastèques
            var maxPasteques = produits
                .Where(p => p.Name == "Pastèques")
                .OrderByDescending(p => p.Quantity)
                .FirstOrDefault();

            if (maxPasteques != null)
            {
                Console.WriteLine($"C'est {maxPasteques.Seller} qui a le plus de pastèques (stand {maxPasteques.Stand}, {maxPasteques.Quantity} pièces)");
            }
        }

        // Méthode pour lire les produits depuis Excel et les convertir en liste d'objets Product
        public static List<Product> ReadProducts(WorkSheet sheet, int startRow, int endRow)
        {
            List<Product> produits = new List<Product>();

            for (int row = startRow; row <= endRow; row++)
            {
                Product p = new Product
                {
                    Stand = sheet[$"A{row}"].IntValue,
                    Seller = sheet[$"B{row}"].StringValue,
                    Name = sheet[$"C{row}"].StringValue,
                    Quantity = sheet[$"D{row}"].IntValue
                };

                produits.Add(p);
            }

            return produits;
        }

    }
}
