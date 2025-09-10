using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace mibmap
{
    public class Product
    {
        public int Location { get; set; }
        public string Producer { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public string Unit { get; set; }
        public double PricePerUnit { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            List<Product> products = new List<Product>
            {
                new Product { Location = 1, Producer = "Bornand", ProductName = "Pommes", Quantity = 20, Unit = "kg", PricePerUnit = 5.50 },
                new Product { Location = 1, Producer = "Bornand", ProductName = "Poires", Quantity = 16, Unit = "kg", PricePerUnit = 5.50 },
                new Product { Location = 1, Producer = "Bornand", ProductName = "Pastèques", Quantity = 14, Unit = "pièce", PricePerUnit = 5.50 },
                new Product { Location = 1, Producer = "Bornand", ProductName = "Melons", Quantity = 5, Unit = "kg", PricePerUnit = 5.50 },
                new Product { Location = 2, Producer = "Dumont", ProductName = "Noix", Quantity = 20, Unit = "sac", PricePerUnit = 5.50 },
                new Product { Location = 2, Producer = "Dumont", ProductName = "Raisin", Quantity = 6, Unit = "kg", PricePerUnit = 5.50 },
                new Product { Location = 2, Producer = "Dumont", ProductName = "Pruneaux", Quantity = 13, Unit = "kg", PricePerUnit = 5.50 },
                new Product { Location = 2, Producer = "Dumont", ProductName = "Myrtilles", Quantity = 12, Unit = "kg", PricePerUnit = 5.50 }
                // … ajoute les autres produits ici
            };

            // Dictionnaire FR → EN
            var dictFrToEn = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                {"Pommes","Apples"},
                {"Poires","Pears"},
                {"Pastèques","Watermelons"},
                {"Melons","Melons"},
                {"Noix","Nuts"},
                {"Raisin","Grapes"},
                {"Pruneaux","Prunes"},
                {"Myrtilles","Blueberries"},
                {"Groseilles","Currants"},
                {"Pêches","Peaches"},
                {"Haricots","Beans"},
                {"Courges","Squashes"},
                {"Tomates","Tomatoes"}
            };


            //List<List<string>> products1 = new List<List<string>>
            //{
            //    new List<string> { "Seller", "Product", "CA" }
            //};

            //products1.AddRange(products.Select(product => new List<string>
            //{
            //    // Nickname of Seller
            //    product.Producer.Substring(0, 3) + "..." + product.Producer.Last(),
            //    // Translate product name
            //    dictFrToEn.ContainsKey(product.ProductName) ? dictFrToEn[product.ProductName] : product.ProductName,
            //    // CA (Chiffre d'affaire)
            //    (product.PricePerUnit * product.Quantity).ToString("F2", CultureInfo.InvariantCulture)

            //}));

            List<List<string>> products1 =
            [
                new List<string> { "Seller", "Product", "CA" },
                .. products.Select(product => new List<string>
                {
                    // Nickname of Seller
                    product.Producer.Substring(0, 3) + "..." + product.Producer.Last(),
                    // Translate product name
                    dictFrToEn.ContainsKey(product.ProductName) ? dictFrToEn[product.ProductName] : product.ProductName,
                    // CA (Chiffre d'affaire)
                    (product.PricePerUnit * product.Quantity).ToString("F2", CultureInfo.InvariantCulture)

                }),
            ];

            products1.ForEach(line => Console.WriteLine("{0,-10} {1,-15} {2,5}", line[0], line[1], line[2]));
            // {0, -10} --> 0 = la prémier colonne, - = alignement par la gauche sinon c'est par la droite, 10 = caractère de la colonne (largeur en caractères)


// ------------------------- Exporter le résultat dans un fichier CSV ---------------------------------------------------------------------------------------------------------------


            // Obtenir le chemin du dossier Téléchargements
            string downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

            // Nom du fichier
            string fileName = "products.csv";

            // Chemin complet 
            string filePath = Path.Combine(downloadsPath, fileName);

            // Exporter le CSV
            using (var writer = new StreamWriter(filePath))
            {
                foreach (var line in products1)
                {
                    writer.WriteLine(string.Join(";", line));
                }
            }

            Console.WriteLine($"Fichier CSV créé dans : {filePath}"); 
        }
    }
}
