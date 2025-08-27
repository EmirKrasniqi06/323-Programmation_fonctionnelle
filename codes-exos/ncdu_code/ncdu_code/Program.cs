using System;
using System.IO;

namespace ncdu_code
{
    class Program
    {
        static void Main(string[] args)
        {
            string rootPath = @"C:\Users\ph76act\Desktop\git\323-Programmation_fonctionnelle";

            Console.WriteLine($"\n-------------- {rootPath} -----------------------------------------\n");

            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(rootPath);

                // Afficher les dossiers
                DirectoryInfo[] subDirs = dirInfo.GetDirectories();
                foreach (DirectoryInfo subDir in subDirs)
                {
                    long folderSize = 0;
                    FileInfo[] filesInFolder = subDir.GetFiles(); // fichiers uniquement dans ce dossier
                    foreach (FileInfo f in filesInFolder)
                    {
                        folderSize += f.Length;
                    }

                    string relativeDir = Path.GetRelativePath(rootPath, subDir.FullName);
                    Console.WriteLine($"{FormatSize(folderSize)} [          ] {relativeDir}");
                }

                // Afficher les fichiers
                FileInfo[] files = dirInfo.GetFiles();
                foreach (FileInfo file in files)
                {
                    string relativeFile = Path.GetRelativePath(rootPath, file.FullName);
                    Console.WriteLine($"{FormatSize(file.Length)} [          ] {relativeFile}");
                }
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Accès refusé au répertoire.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
            }
        }

        // Méthode pour formater les tailles en KB, MB, GB
        static string FormatSize(long bytes)
        {
            double size = bytes;
            string unit = "KiB";

            if (size >= 1024)
            {
                size /= 1024;
                unit = "KiB";
            }
            if (size >= 1024)
            {
                size /= 1024;
                unit = "MiB";
            }
            if (size >= 1024)
            {
                size /= 1024;
                unit = "GiB";
            }

            return $"{size:0.##} {unit}";
        }
    }
}
