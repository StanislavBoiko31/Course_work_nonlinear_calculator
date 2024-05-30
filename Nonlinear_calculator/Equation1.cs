using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nonlinear_calculator
{
    public class Equation1 : Equation
    {
        public Equation1(double[] coefficients, double[,] approximation, double accuracy) : base(coefficients, approximation, accuracy)
        {

        }
        public override double[,] F(double[,] approximation) // обчислення значень функцій
        {
            double[,] functions = new double[2, 1];
            try
            {
                functions[0, 0] = Math.Pow(approximation[0, 0], coefficients[0]) + coefficients[1] * Math.Sin(Math.PI * approximation[1, 0]) + coefficients[2];
                functions[1, 0] = coefficients[3] * approximation[0, 0] + Math.Pow(approximation[1, 0], coefficients[4]) + coefficients[5];
            if (double.IsNaN(functions[0, 0]) || double.IsNaN(functions[1, 0]) || double.IsInfinity(functions[0,0]) || double.IsInfinity(functions[1, 0]))
            {
                    throw new Exception("Даний метод не може розв'язати цю систему.");
            }
            }
            catch (Exception)
            {
                throw new Exception("Даний метод не може розв'язати цю систему.");
            }
            return functions;
        }
        public override double[,] J(double[,] approximation) // обчислення якобіана
        {
            double[,] derivatives = new double[2, 2];
            try
            {
                derivatives[0, 0] = coefficients[0] * (Math.Pow(approximation[0, 0], coefficients[0] - 1));
                derivatives[0, 1] = coefficients[1] * Math.PI * Math.Cos(Math.PI * approximation[1, 0]);
                derivatives[1, 0] = coefficients[3];
                derivatives[1, 1] = coefficients[4] * Math.Pow(approximation[1, 0], coefficients[4] - 1);
                if (double.IsNaN(derivatives[0, 0]) || double.IsNaN(derivatives[1, 1]) || double.IsInfinity(derivatives[0, 0]) || double.IsInfinity(derivatives[1, 1]))
                {
                    throw new Exception("Даний метод не може розв'язати цю систему.");
                }
            }
            catch (Exception)
            {
                throw new Exception("Даний метод не може розв'язати цю систему.");
            }
            return derivatives;
        }
    }
}






