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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ControlSystem
{
    public enum InputSignal
    {
        Rectangular,
        Sinus,
        Triangular
    }

    public partial class ControlSystemUI : Window
    {
        private readonly SecondOrderSystem controlSystem;

        public ControlSystemUI()
        {
            InitializeComponent();
            controlSystem = new SecondOrderSystem();
        }

        private void IsEnterPressed(Object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                SaveArguments(sender, e);
        }

        private void SaveArguments(object sender, RoutedEventArgs e)
        {
            try
            {
                this.controlSystem.SetArguments(
                    aParam: aParam.Text,
                    bParam: bParam.Text,
                    AParam: AParam.Text,
                    kParam: kParam.Text);

                MessageBox.Show("System is stable", "Succes");
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Exception");
            }
        }

        private void ShowSystemSchema(object sender, RoutedEventArgs e)
        {
            try
            {
                SystemSchemaWindow schema = new SystemSchemaWindow();
                schema.Show();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Show System Schema - Exception");
            }
        }

        private void DrawRootLocus(object sender, RoutedEventArgs e)
        {
            if (this.controlSystem.CheckStability())
            {
                try
                {
                    RootLocusWindow rootLocus = new RootLocusWindow();
                    rootLocus.Show();
                    rootLocus.CreateRootLocus(this.controlSystem.a, this.controlSystem.b);
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message, "Draw Root Locus - Exception");
                }
            }
            else
                MessageBox.Show("System is unstable!! Change parameters.", "ERROR");
        }

        private InputSignal GetInputSignal()
        {
            if (inputSinus.IsChecked == true)
                return InputSignal.Sinus;
            else if (inputRectangle.IsChecked == true)
                return InputSignal.Rectangular;
            else if (inputTriangle.IsChecked == true)
                return InputSignal.Triangular;
            else
                throw new Exception("Choose one of possible input signal!");
        }

        private void CalculateResponse(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!controlSystem.isInitialized)
                    throw new Exception("First input apropriate parameters!");

                this.controlSystem.SetInputSignal(GetInputSignal());
                this.controlSystem.Calculate();

                DrawGraphs();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Calculating response - Exception");
            }
        }

        private void DrawGraphs()
        {
            CanvasY.Children.Clear();
            CanvasE.Children.Clear();

            Label[] yResponseCoeffs =
                {
                     responseCoeff_y11, responseCoeff_y10, responseCoeff_y9, responseCoeff_y8, responseCoeff_y7, responseCoeff_y6, responseCoeff_y5, responseCoeff_y4, responseCoeff_y3,
                     responseCoeff_y2, responseCoeff_y1, responseCoeff_y0, responseCoeff_ym1, responseCoeff_ym2, responseCoeff_ym3, responseCoeff_ym4, responseCoeff_ym5, responseCoeff_ym6,
                     responseCoeff_ym7, responseCoeff_ym8, responseCoeff_ym9, responseCoeff_ym10, responseCoeff_ym11
                };

            Label[] xResponseCoeffs =
               {
                     responseCoeff_x1, responseCoeff_x2, responseCoeff_x3, responseCoeff_x4, responseCoeff_x5, responseCoeff_x6, responseCoeff_x7, responseCoeff_x8, responseCoeff_x9,
                     responseCoeff_x10, responseCoeff_x11, responseCoeff_x12, responseCoeff_x13, responseCoeff_x14, responseCoeff_x15, responseCoeff_x16, responseCoeff_x17, responseCoeff_x18,
                     responseCoeff_x19, responseCoeff_x20
                };

            Label[] yErrorCoeffs =
                {
                     errorCoeff_y11, errorCoeff_y10, errorCoeff_y9, errorCoeff_y8, errorCoeff_y7, errorCoeff_y6, errorCoeff_y5, errorCoeff_y4, errorCoeff_y3,
                     errorCoeff_y2, errorCoeff_y1, errorCoeff_y0, errorCoeff_ym1, errorCoeff_ym2, errorCoeff_ym3, errorCoeff_ym4, errorCoeff_ym5, errorCoeff_ym6,
                     errorCoeff_ym7, errorCoeff_ym8, errorCoeff_ym9, errorCoeff_ym10, errorCoeff_ym11
                };

            Label[] xErrorCoeffs =
               {
                     errorCoeff_x1, errorCoeff_x2, errorCoeff_x3, errorCoeff_x4, errorCoeff_x5, errorCoeff_x6, errorCoeff_x7, errorCoeff_x8, errorCoeff_x9,
                     errorCoeff_x10, errorCoeff_x11, errorCoeff_x12, errorCoeff_x13, errorCoeff_x14, errorCoeff_x15, errorCoeff_x16, errorCoeff_x17, errorCoeff_x18,
                     errorCoeff_x19, errorCoeff_x20
                };

            var scales = GetScaleFactor();

            Plot(this.controlSystem.Output, Brushes.Red, CanvasY, xResponseCoeffs, yResponseCoeffs, 
                SecondOrderSystem.step * SecondOrderSystem.endTime, scales["inOut"]);
            Plot(this.controlSystem.Input, Brushes.Blue, CanvasY, xResponseCoeffs, yResponseCoeffs, 
                SecondOrderSystem.step * SecondOrderSystem.endTime, scales["inOut"]);
            Plot(this.controlSystem.Error, Brushes.Green, CanvasE, xErrorCoeffs, yErrorCoeffs, 
                SecondOrderSystem.step * SecondOrderSystem.endTime, scales["error"]);
        }

        private void Plot(List<double> values, SolidColorBrush color, Canvas canvas, Label[] xCoeff, Label[] yCoeff, double endTime, double scale)
        {
            int samplesTime = values.Count;

            double xStart = indicatorLine.X1;
            double yStart = indicatorLine.Y1;

            double xRange = xTopRangeLine.X2 - indicatorLine.X1;
            double yRange = indicatorLine.Y1 - yTopRangeLine.Y1;

            double X1 = xStart;
            double Y1 = yStart;
            double X2, Y2;

            for (int i = 0; i < samplesTime; i++)
            {
                X2 = xStart + (i + 1) * xRange / samplesTime;
                Y2 = yStart - values[i] * yRange / scale;

                Line line = new Line
                {
                    X1 = X1,
                    X2 = X2,
                    Y1 = Y1,
                    Y2 = Y2,

                    Stroke = color,
                    StrokeThickness = 1
                };

                canvas.Children.Add(line);

                X1 = X2;
                Y1 = Y2;
            }

            double yUnit = scale / 11;
            double xUnit = endTime / 20;

            for (int i = 0; i < yCoeff.Length; i++) yCoeff[i].Content = ((11 - i) * yUnit).ToString("F");
            for (int i = 0; i < xCoeff.Length; i++) xCoeff[i].Content = (xUnit * (i + 1)).ToString("F");
        }

        private Dictionary<string, double> GetScaleFactor()
        {
            Dictionary<string, double> scales = new Dictionary<string, double>();

            double outScale = Math.Abs(this.controlSystem.Output.Max()) >= Math.Abs(this.controlSystem.Output.Min())
                ? Math.Abs(this.controlSystem.Output.Max())
                : Math.Abs(this.controlSystem.Output.Min());
            double inScale = Math.Abs(this.controlSystem.Input.Max()) >= Math.Abs(this.controlSystem.Input.Min())
                ? Math.Abs(this.controlSystem.Input.Max())
                : Math.Abs(this.controlSystem.Input.Min());
            scales.Add("inOut", outScale >= inScale ? outScale : inScale);

            double errorScale = Math.Abs(this.controlSystem.Error.Max()) >= Math.Abs(this.controlSystem.Error.Min()) ? Math.Abs(this.controlSystem.Error.Max()) : Math.Abs(this.controlSystem.Error.Min());
            scales.Add("error", errorScale);

            return scales;
        }
    }
}
