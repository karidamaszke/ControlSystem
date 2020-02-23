using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlSystem
{
    public class SecondOrderSystem
    {
        public bool isInitialized = false;

        public double a { get; private set; }
        public double b { get; private set; }
        public double A { get; private set; }
        public double k { get; private set; }

        public static readonly double step = 0.005;
        public static readonly double endTime = 20 * (1 / step);
        
        private static readonly int signalPeriod = 8;
        private InputSignal inputSignal;

        public List<double> Input { get; private set; }
        public List<double> State;
        public List<double> StateDot;
        public List<double> StateDotDot;
        public List<double> Output { get; private set; }
        public List<double> Error { get; private set; }

        public SecondOrderSystem()
        {
            this.a = 0;
            this.b = 0;
            this.A = 0;
            this.k = 0;
        }

        public void SetArguments(string aParam, string bParam, string AParam, string kParam)
        {
            this.isInitialized = false;

            this.a = Double.Parse(aParam);
            this.b = Double.Parse(bParam);
            this.A = Double.Parse(AParam);
            this.k = Double.Parse(kParam);

            if (!CheckStability())
                throw new Exception("System is not stable! Change your parameters!");

            this.isInitialized = true;
        }

        public void SetInputSignal(InputSignal input)
        {
            this.inputSignal = input;
        }

        public bool CheckStability()
        {
            /*  Based on Routh–Hurwitz stability criterion for
             *  second order control system with transfer function:
             *  
             *  G(s) = kA / [(s + a)(s + b)]
             */

            if ((a + b > 0) && (a * b + k * A > 0))
                return true;

            return false;
        }

        public void Calculate()
        {
            ClearVectors();

            for (int time = 0; time <= endTime; time++)
            {
                GetInputSample(time);
                GetOutputSample(time);

                Error.Add(Input[time] - Output[time]);
            }
        }

        private void ClearVectors()
        {
            Input = new List<double>();
            State = new List<double> { 0 };
            StateDot = new List<double> { 0 };
            StateDotDot = new List<double> { 0 };
            Output = new List<double>();
            Error = new List<double>();
        }

        private void GetInputSample(int time)
        {
            double currentInputSample = 0;

            switch (this.inputSignal)
            {
                case InputSignal.Rectangular:
                    if ((int)(time * step / (signalPeriod / 2)) % 2 == 0)
                        currentInputSample = 1.0;
                    else
                        currentInputSample = -1.0;
                    break;

                case InputSignal.Sinus:
                    currentInputSample = Math.Sin(2 * Math.PI * time * step / signalPeriod);
                    break;

                case InputSignal.Triangular:
                    if (Input.Count == 0) currentInputSample = step;
                    else if ((int)(time * step / (signalPeriod / 4)) % 4 == 0) currentInputSample = Input[Input.Count - 1] + step;
                    else if ((int)(time * step / (signalPeriod / 4)) % 4 == 1) currentInputSample = Input[Input.Count - 1] - step;
                    else if ((int)(time * step / (signalPeriod / 4)) % 4 == 2) currentInputSample = Input[Input.Count - 1] - step;
                    else if ((int)(time * step / (signalPeriod / 4)) % 4 == 3) currentInputSample = Input[Input.Count - 1] + step;

                    break;

                default:
                    throw new Exception("Unknown input signal!");
            }

            Input.Add(currentInputSample);
        }

        private void GetOutputSample(int time)
        {
            /* Linear dynamic second order system can be described 
             * by state space model in controllable canonical form:
             *  
             *  |x' |   |   0       1 |   | x |   | 0 |
             *  |   | = |             | * |   | + |   |* u(t)
             *  |x''|   |-ab-ka   -a-b|   | x'|   | 1 |
             *
             *                     | x |
             *  y(t) = | kA  0 | * |   |
             *                     | x'|
             */

            StateDotDot.Add(Input[time] - ((a * b) + (k * A)) * State[State.Count - 1] - (a + b) * StateDot[StateDot.Count - 1]);
            Integrate(StateDotDot, StateDot, time);
            Integrate(StateDot, State, time);
            Output.Add(State[time] * k * A);
        }

        private void Integrate(List<double> derivative, List<double> integral, int time)
        {
            /* 
             * Basic numerical method of integration - rectangle rule.
             */

            var currentintegral = ((derivative[derivative.Count - 1] + derivative[derivative.Count - 2]) * step / 2);
            integral.Add(integral[integral.Count - 1] + currentintegral);
        }
    }
}
