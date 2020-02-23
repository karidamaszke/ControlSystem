using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ControlSystem
{
    public partial class RootLocusWindow : Window
    {
        private static SolidColorBrush color = Brushes.Red;

        public RootLocusWindow()
        {
            InitializeComponent();
        }

        public void CreateRootLocus(double firstPole, double secondPole)
        {
            if (firstPole == secondPole)
                DrawRootLocusDoublePole(firstPole);
            else
                DrawRootLocus(Math.Max(firstPole, secondPole),
                    Math.Min(firstPole, secondPole));
        }

        private void DrawRootLocusDoublePole(double pole)
        {
            Line yLine = new Line
            {
                X1 = 150,
                X2 = 150,
                Y1 = 20,
                Y2 = 480,

                Stroke = color,
                StrokeThickness = 2
            };

            Label firstRoot = new Label
            {
                Content = -pole,
                Height = 30,
                Width = 30,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(150, 240, 0, 0)
            };

            lines.Children.Add(yLine);
            lines.Children.Add(firstRoot);
        }

        private void DrawRootLocus(double fasterPole, double slowerPole)
        {
            Line xLine = new Line
            {
                X1 = 25,
                X2 = (295 - (270 * (slowerPole / fasterPole))),
                Y1 = 240,
                Y2 = 240,

                Stroke = color,
                StrokeThickness = 2
            };

            Line yLine = new Line
            {
                X1 = ((295 - (270 * (slowerPole / fasterPole))) + 25) / 2,
                X2 = ((295 - (270 * (slowerPole / fasterPole))) + 25) / 2,
                Y1 = 20,
                Y2 = 480,

                Stroke = color,
                StrokeThickness = 2
            };

            Ellipse firstPoint = new Ellipse
            {
                Width = 6,
                Height = 6,
                Margin = new Thickness(22, 237, 0, 0),
                Fill = new SolidColorBrush(Colors.Red)
            };

            Ellipse secondPoint = new Ellipse
            {
                Width = 6,
                Height = 6,
                Margin = new Thickness((292 - (270 * (slowerPole / fasterPole))), 237, 0, 0),
                Fill = new SolidColorBrush(Colors.Red)
            };

            Label firstRootLabel = new Label
            {
                Content = -fasterPole,
                Height = 30,
                Width = 30,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(15, 240, 0, 0)
            };

            Label secondRootLabel = new Label
            {
                Content = -slowerPole,
                Height = 30,
                Width = 30,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness((285 - (270 * (slowerPole / fasterPole))), 240, 0, 0)
            };

            lines.Children.Add(xLine);
            lines.Children.Add(yLine);
            lines.Children.Add(firstPoint);
            lines.Children.Add(secondPoint);
            lines.Children.Add(firstRootLabel);
            lines.Children.Add(secondRootLabel);
        }
    }
}
