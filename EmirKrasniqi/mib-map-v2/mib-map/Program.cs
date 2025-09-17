using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

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

    // Classe typée pour comparer avec objet anonyme
    class ProductDto
    {
        public string SellerNickname { get; set; }
        public string ProductEn { get; set; }
        public string Revenue { get; set; }
        public string StockCategory { get; set; }
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
            };

            var dictFrToEn = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                {"Pommes","Apples"}, {"Poires","Pears"}, {"Pastèques","Watermelons"}, {"Melons","Melons"},
                {"Noix","Nuts"}, {"Raisin","Grapes"}, {"Pruneaux","Prunes"}, {"Myrtilles","Blueberries"},
                {"Groseilles","Currants"}, {"Pêches","Peaches"}, {"Haricots","Beans"},
                {"Courges","Squashes"}, {"Tomates","Tomatoes"}
            };

            // ------------------ Affichage CSV actuel ------------------
            var products1 = new List<List<string>>();
            products1.Add(new List<string> { "Seller", "Product", "CA", "Stock" });
            products1.AddRange(products.Select(product => new List<string>
            {
                product.Producer.First() + (product.Producer.Length-2).ToString() + product.Producer.Last(),
                dictFrToEn.ContainsKey(product.ProductName) ? dictFrToEn[product.ProductName] : product.ProductName,
                (product.PricePerUnit * product.Quantity).ToString("F2", CultureInfo.InvariantCulture),
                product.Quantity switch
                {
                    < 10 => "Stock faible",
                    >= 10 and <= 15 => "Stock normal",
                    _ => "Stock élevé"
                }
            }));

            products1.ForEach(line => Console.WriteLine("{0,-10} {1,-15} {2,-10} {3,-15}", line[0], line[1], line[2], line[3]));

            // ------------------ Mesure des performances ------------------
            var results = new List<(string Name, long Time, long Memory)>();
            var perfSimple = MeasurePerf(() => SelectSimple(products, dictFrToEn));
            results.Add(("SelectSimple", perfSimple.Time, perfSimple.Memory));

            var perfMethod = MeasurePerf(() => SelectWithMethod(products, dictFrToEn));
            results.Add(("SelectWithMethod", perfMethod.Time, perfMethod.Memory));

            var perfTyped = MeasurePerf(() => SelectTyped(products, dictFrToEn));
            results.Add(("SelectTyped", perfTyped.Time, perfTyped.Memory));

            var perfAnonymous = MeasurePerf(() => SelectAnonymous(products, dictFrToEn));
            results.Add(("SelectAnonymous", perfAnonymous.Time, perfAnonymous.Memory));


            // ------------------ Générer rapport Markdown ------------------
            string mdFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "PerfReport.md");
            using (var writer = new StreamWriter(mdFile))
            {
                writer.WriteLine("# Rapport de performances");
                writer.WriteLine("| Méthode | Temps (ms) | Mémoire (bytes) |");
                writer.WriteLine("|---------|------------|----------------|");

                foreach (var r in results)
                    writer.WriteLine($"| {r.Name} | {r.Time} | {r.Memory} |");

                writer.WriteLine("\n**Recommandations :**");
                writer.WriteLine("- Privilégier les méthodes simples inline si possible.");
                writer.WriteLine("- Les objets typés consomment un peu plus de mémoire que les objets anonymes.");
                writer.WriteLine("- Les méthodes externes améliorent la lisibilité mais ajoutent un léger overhead.");
            }
            Console.WriteLine($"Rapport créé : {mdFile}");

            // ------------------ Export CSV ------------------
            string downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            string filePath = Path.Combine(downloadsPath, "products.csv");
            using (var writer = new StreamWriter(filePath))
            {
                foreach (var line in products1)
                    writer.WriteLine(string.Join(";", line));
            }
            Console.WriteLine($"Fichier CSV créé dans : {filePath}");
        }

        // ------------------ Fonction de mesure ------------------
        static (long Time, long Memory) MeasurePerf(Action action, int iterations = 1000)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            long memBefore = GC.GetTotalMemory(false);
            Stopwatch sw = Stopwatch.StartNew();

            for (int i = 0; i < iterations; i++)
                action();

            sw.Stop();
            long memAfter = GC.GetTotalMemory(false);
            return (sw.ElapsedMilliseconds, memAfter - memBefore);
        }

        // ------------------ Variantes de Select ------------------
        static List<List<string>> SelectSimple(List<Product> products, Dictionary<string, string> dict)
        {
            return products.Select(p => new List<string>
            {
                p.Producer.First() + (p.Producer.Length - 2).ToString() + p.Producer.Last(),
                dict.ContainsKey(p.ProductName) ? dict[p.ProductName] : p.ProductName,
                (p.PricePerUnit * p.Quantity).ToString("F2", CultureInfo.InvariantCulture),
                p.Quantity switch
                {
                    < 10 => "Stock faible",
                    >= 10 and <= 15 => "Stock normal",
                    _ => "Stock élevé"
                }
            }).ToList();
        }

        static List<List<string>> SelectWithMethod(List<Product> products, Dictionary<string, string> dict)
        {
            return products.Select(p => TransformProduct(p, dict)).ToList();
        }

        static List<string> TransformProduct(Product p, Dictionary<string, string> dict)
        {
            return new List<string>
            {
                p.Producer.First() + (p.Producer.Length - 2).ToString() + p.Producer.Last(),
                dict.ContainsKey(p.ProductName) ? dict[p.ProductName] : p.ProductName,
                (p.PricePerUnit * p.Quantity).ToString("F2", CultureInfo.InvariantCulture),
                p.Quantity switch
                {
                    < 10 => "Stock faible",
                    >= 10 and <= 15 => "Stock normal",
                    _ => "Stock élevé"
                }
            };
        }

        static List<ProductDto> SelectTyped(List<Product> products, Dictionary<string, string> dict)
        {
            return products.Select(p => new ProductDto
            {
                SellerNickname = p.Producer.First() + (p.Producer.Length - 2).ToString() + p.Producer.Last(),
                ProductEn = dict.ContainsKey(p.ProductName) ? dict[p.ProductName] : p.ProductName,
                Revenue = (p.PricePerUnit * p.Quantity).ToString("F2", CultureInfo.InvariantCulture),
                StockCategory = p.Quantity switch
                {
                    < 10 => "Stock faible",
                    >= 10 and <= 15 => "Stock normal",
                    _ => "Stock élevé"
                }
            }).ToList();
        }

        static List<object> SelectAnonymous(List<Product> products, Dictionary<string, string> dict)
        {
            return products.Select(p => new
            {
                SellerNickname = p.Producer.First() + (p.Producer.Length - 2).ToString() + p.Producer.Last(),
                ProductEn = dict.ContainsKey(p.ProductName) ? dict[p.ProductName] : p.ProductName,
                Revenue = (p.PricePerUnit * p.Quantity).ToString("F2", CultureInfo.InvariantCulture),
                StockCategory = p.Quantity switch
                {
                    < 10 => "Stock faible",
                    >= 10 and <= 15 => "Stock normal",
                    _ => "Stock élevé"
                }
            }).ToList<object>();
        }
    }
}
