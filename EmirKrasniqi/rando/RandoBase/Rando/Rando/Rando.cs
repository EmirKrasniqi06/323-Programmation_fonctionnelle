using System.Globalization;
using System.Xml.Linq;

namespace Rando
{
    public partial class Rando : Form
    {
        private List<Trackpoint> trackpoints;
        private List<Point> points;
        Color[] gradient = new Color[]
        {
            Color.FromArgb(255, 144, 238, 144), // Vert clair
            Color.FromArgb(255, 162, 216, 128),
            Color.FromArgb(255, 180, 194, 112),
            Color.FromArgb(255, 198, 172, 96),
            Color.FromArgb(255, 216, 150, 80),
            Color.FromArgb(255, 234, 128, 64),
            Color.FromArgb(255, 244, 106, 48),
            Color.FromArgb(255, 248, 84, 36),
            Color.FromArgb(255, 252, 62, 24),
            Color.FromArgb(255, 254, 48, 18),
            Color.FromArgb(255, 255, 32, 12),
            Color.FromArgb(255, 255, 16, 6),
            Color.FromArgb(255, 255, 0, 0) // Rouge vif
        };

        public Rando()
        {
            Image backgroundImg = new Bitmap("../../../../../../map.png");
            this.BackgroundImage = backgroundImg;

            InitializeComponent();

            trackpoints = ReadGpx("../../../../../../gpx/gemmikandersteg.gpx");
            points = ToPoints(trackpoints, this.ClientSize.Width, this.ClientSize.Height);

            this.Paint += Rando_Form_Paint;

            SaveGpx("output.gpx", trackpoints);
            MessageBox.Show("Trace sauvegardée !");
        }

        static List<Trackpoint> ReadGpx(string filePath)
        {
            XDocument doc = XDocument.Load(filePath);
            XNamespace ns = "http://www.topografix.com/GPX/1/1";

            return doc.Descendants(ns + "trkpt")
                .Select(trkpt => new Trackpoint
                {
                    Latitude = double.Parse(trkpt.Attribute("lat").Value, CultureInfo.InvariantCulture),
                    Longitude = double.Parse(trkpt.Attribute("lon").Value, CultureInfo.InvariantCulture),
                    Elevation = double.Parse(trkpt.Element(ns + "ele").Value, CultureInfo.InvariantCulture)
                })
                .ToList();
        }

        private static void SaveGpx(string filePath, List<Trackpoint> trackpoints)
        {
            XNamespace ns = "http://www.topografix.com/GPX/1/1";

            var gpx = new XElement(ns + "gpx",
                new XAttribute("version", "1.1"),
                new XAttribute("creator", "RandoApp"),
                new XElement(ns + "trk",
                    new XElement(ns + "trkseg",
                        trackpoints.Select(tp =>
                            new XElement(ns + "trkpt",
                                new XAttribute("lat", tp.Latitude.ToString(CultureInfo.InvariantCulture)),
                                new XAttribute("lon", tp.Longitude.ToString(CultureInfo.InvariantCulture)),
                                new XElement(ns + "ele", tp.Elevation.ToString(CultureInfo.InvariantCulture))
                            )
                        )
                    )
                )
            );

            gpx.Save(filePath);
        }


        private static List<Point> ToPoints(List<Trackpoint> trackpoints, int width, int height)
        {
            double minLat = trackpoints.Min(tp => tp.Latitude);
            double maxLat = trackpoints.Max(tp => tp.Latitude);
            double minLon = trackpoints.Min(tp => tp.Longitude);
            double maxLon = trackpoints.Max(tp => tp.Longitude);

            return trackpoints.Select(tp =>
                new Point(
                    (int)((tp.Longitude - minLon) / (maxLon - minLon) * width),
                    height - (int)((tp.Latitude - minLat) / (maxLat - minLat) * height)
                )
            ).ToList();
        }

        private void Rando_Form_Paint(object sender, PaintEventArgs e)
        {
            if (points != null && points.Count > 1)
            {
                double minEle = trackpoints.Min(tp => tp.Elevation);
                double maxEle = trackpoints.Max(tp => tp.Elevation);

                for (int i = 0; i < points.Count - 1; i++)
                {
                    double elevation = trackpoints[i].Elevation;

                    // Normaliser entre 0 et gradient.Length-1
                    int idx = (int)((elevation - minEle) / (maxEle - minEle) * (gradient.Length - 1));
                    if (idx < 0) idx = 0;
                    if (idx >= gradient.Length) idx = gradient.Length - 1;

                    using (Pen pen = new Pen(gradient[idx], 2))
                    {
                        e.Graphics.DrawLine(pen, points[i], points[i + 1]);
                    }
                }
            }
        }

        private static double HaversineDistance(Trackpoint p1, Trackpoint p2)
        {
            double R = 6371000; // rayon de la Terre en mètres
            double dLat = (p2.Latitude - p1.Latitude) * Math.PI / 180.0;
            double dLon = (p2.Longitude - p1.Longitude) * Math.PI / 180.0;
            double lat1 = p1.Latitude * Math.PI / 180.0;
            double lat2 = p2.Latitude * Math.PI / 180.0;

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(lat1) * Math.Cos(lat2) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private static List<Trackpoint> ConcatGpx(List<Trackpoint> t1, List<Trackpoint> t2, double toleranceMeters = 100)
        {
            if (HaversineDistance(t1.Last(), t2.First()) > toleranceMeters &&
                HaversineDistance(t2.Last(), t1.First()) > toleranceMeters)
            {
                throw new Exception("Les deux tracés sont trop éloignés pour être fusionnés !");
            }

            return t1.Concat(t2).ToList();
        }

    }
}
