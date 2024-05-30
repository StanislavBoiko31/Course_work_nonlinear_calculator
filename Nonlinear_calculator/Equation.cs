using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nonlinear_calculator
{
    public abstract class Equation
    {
        protected double[] coefficients;
        protected double[,] approximation;
        protected double accuracy;
        protected const double LowLimValue = 1e-12;
        protected const int MaxLimIterationI = 200;
        protected const int MaxLimIterationJ = 50;
        public Equation (double[] coefficients, double[,] approximation, double accuracy)
        {
            this.coefficients = coefficients;
            this.approximation = approximation;
            this.accuracy = accuracy;
        }

        public abstract double[,] F(double[,] approximation); // обчислення значень функцій

        public abstract double[,] J(double[,] approximation); // обчислення якобіана

        public double[,] InverseM(double[,] matrix) // обернення матриці
        {
            int n = matrix.GetLength(0);
            double determinant = matrix[0, 0] * matrix[1, 1] - (matrix[0, 1] * matrix[1, 0]);
            if (determinant == 0)
            {
                throw new Exception("Даний метод не може розв'язати цю систему.");
            }
            double[,] inverseMatrix = new double[n, n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (i == j)
                    {
                        inverseMatrix[i, j] = matrix[i, j] / determinant;
                    }
                    else
                    {
                        inverseMatrix[i, j] = -matrix[i, j] / determinant;
                    }
                }
            }
            return inverseMatrix;
        }

        public double[,] MultiplyMatrices(double[,] matrix1, double[,] matrix2) // множення матриць
        {
            int rows1 = matrix1.GetLength(0);
            double[,] result = new double[rows1, 1];
            result[0, 0] = matrix1[0, 0] * matrix2[0, 0] + matrix1[0, 1] * matrix2[1, 0];
            result[1, 0] = matrix1[1, 0] * matrix2[0, 0] + matrix1[1, 1] * matrix2[1, 0];
            return result;
        }

        public double[,] AddMatrices(double[,] matrix1, double[,] matrix2) // додавання матриць
        {
            int rows = matrix1.GetLength(0);
            int columns = matrix1.GetLength(1);
            double[,] result = new double[rows, columns];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    result[i, j] = matrix1[i, j] + matrix2[i, j];
                }
            }
            return result;
        }
        public double GetVectorLength(double[,] vector) // пошук векторної довжини
        {
            double first_coordinate = vector[0, 0];
            double second_coordinate = vector[1, 0];
            double length = Math.Sqrt(first_coordinate * first_coordinate + second_coordinate * second_coordinate);
            return length;
        }

        public double[,] SecantJ(double[,] matrix1, double[,] matrix2) // обчислення якобіана для методу січних
        {
            int rows1 = matrix1.GetLength(0);
            double[,] numerator = new double[rows1, rows1];
            double denominator = new double();

            for (int i = 0; i < rows1; i++)
            {
                for (int j = 0; j < rows1; j++)
                {
                    numerator[i, j] = matrix1[i, 0] * matrix2[j, 0];
                }
                denominator += matrix1[i, 0] * matrix2[i, 0];
            }
            if (denominator == 0)
            {
                throw new Exception("Даний метод не може розв'язати цю систему.");
            }
            for (int i = 0; i < rows1; i++)
            {
                for (int j = 0; j < rows1; j++)
                {
                    numerator[i, j] /= denominator;
                }
            }
            return numerator;
        }

        public  double[] NewtonMethod( double[,] approximation)
        {
            if (Math.Abs(F(approximation)[0,0]) <= accuracy && Math.Abs(F(approximation)[1,0]) <= accuracy)
            {
                return new double[] { approximation[0, 0], approximation[1, 0], 1, 1};
            }

            double[,] delta_new = new double[2, 1]; // задаю початкове значення для нового Х
            delta_new[0, 0] = approximation[0, 0];
            delta_new[1, 0] = approximation[1, 0];
            int iterationCount = 0;

            for (int i = 0; i < MaxLimIterationI; i++)
            {
                iterationCount++;

                double[,] functions = F(approximation); // знаходжу значення функцій
                double[,] derivatives = J(approximation); // знаходжу якобіан 

                double[,] inverse_j = InverseM(derivatives); // знаходжу обернену

                double[,] delta_app = MultiplyMatrices(inverse_j, functions); // перемножую матриці
                delta_app[0, 0] = -delta_app[0, 0];
                delta_app[1, 0] = -delta_app[1, 0];
                double t = 1;
                double[,] copy_delta_app = new double[2, 1];

                for (int j = 0; j < MaxLimIterationJ; j++) // використовую модифікацію для покращення збіжності
                {
                    copy_delta_app[0, 0] = t * delta_app[0, 0];
                    copy_delta_app[1, 0] = t * delta_app[1, 0];

                    if (Math.Abs(copy_delta_app[0, 0]) < LowLimValue && copy_delta_app[0, 0] != 0 ||
                        Math.Abs(copy_delta_app[1, 0]) < LowLimValue && copy_delta_app[1, 0] != 0)
                    {
                        return null;
                    }

                    double[,] new_app = AddMatrices(copy_delta_app, approximation); 

                    if (GetVectorLength(F(new_app)) <= GetVectorLength(F(approximation)))
                    {
                        break;
                    }
                    t = t / 2;

                    if (j == MaxLimIterationJ - 1)
                    {
                        return null;
                    }
                }

                delta_new = AddMatrices(copy_delta_app, delta_new); // додаю наближення

                if (GetVectorLength(delta_new) == 0)
                {
                    return null;
                }
                double difference =  GetVectorLength(copy_delta_app ); // знаходжу різницю
                if (difference > accuracy) // якщо різниця велика
                {
                    approximation[0, 0] = delta_new[0, 0]; // присвоюю початковому наближення нові значення
                    approximation[1, 0] = delta_new[1, 0];
                }
                else // якщо різниця мала то перевіряю результат
                {
                    double[,] check_res = F(delta_new);
                    if (Math.Abs(check_res[0, 0]) <= accuracy && Math.Abs(check_res[1, 0]) <= accuracy)
                    {
                        double complexity = iterationCount * 4;
                        return new double[] { delta_new[0, 0], delta_new[1, 0], complexity, 1 };
                    }
                    if (Math.Abs(check_res[0, 0]) <= 3 * accuracy && Math.Abs(check_res[1, 0]) <= 3 * accuracy)
                    {
                        double complexity = iterationCount * 4;
                        return new double[] { delta_new[0, 0], delta_new[1, 0], complexity, -1 };
                    } 
                    return null;
                }
            }
            return null;

        }

        public  double[] SecantMethod(double[,] approximation)
        {
            if (Math.Abs(F(approximation)[0, 0]) <= accuracy && Math.Abs(F(approximation)[1, 0]) <= accuracy)
            {
                return new double[] { approximation[0, 0], approximation[1, 0], 1, 1 };
            }

            double[,] delta_new = new double[2, 1]; // задаю початкове значення для нового Х
            delta_new[0, 0] = approximation[0, 0];
            delta_new[1, 0] = approximation[1, 0];
            double[,] derivatives = new double[2, 2];
            double[,] delta_app = new double[2, 1];
            int iterationCount = 0;

            for (int i = 0; i < MaxLimIterationI; i++)
            {
                iterationCount++;
                double[,] functions = F(approximation); // знаходжу значення функцій
                if (i == 0)
                {
                    derivatives = J(approximation); // знаходжу якобіан 
                }
                else
                {
                    double[,] temp_j = SecantJ(functions, delta_app); // знаходжу після першої ітерації 

                    for (int j = 0; j < 2; j++)
                    {
                        for (int k = 0; k < 2; k++)
                        {
                            derivatives[j, k] = derivatives[j, k] + temp_j[j, k];
                        }
                    }
                }

                double[,] inverse_j = InverseM(derivatives); // знаходжу обернену

                delta_app = MultiplyMatrices(inverse_j, functions); // перемножую матриці
                delta_app[0, 0] = -delta_app[0, 0];
                delta_app[1, 0] = -delta_app[1, 0];
                double t = 1;
                double[,] copy_delta_app = new double[2, 1];

                for (int j = 0; j < MaxLimIterationJ; j++) // використовую модифікацію для покращення збіжності
                {

                    copy_delta_app[0, 0] = t * delta_app[0, 0];
                    copy_delta_app[1, 0] = t * delta_app[1, 0];

                    double[,] new_app = AddMatrices(copy_delta_app, approximation);

                    if (Math.Abs(copy_delta_app[0, 0]) < LowLimValue && copy_delta_app[0, 0] != 0 ||
                        Math.Abs(copy_delta_app[1, 0]) < LowLimValue && copy_delta_app[1, 0] != 0)
                    {
                        return null;
                    }

                    if (GetVectorLength(F(new_app)) <= GetVectorLength(F(approximation)))
                    {
                        break;
                    }
                    t = t / 2;

                    if (j == MaxLimIterationJ - 1)
                    {
                        return null;
                    }
                } 

                delta_new = AddMatrices(copy_delta_app, delta_new); // додаю наближення
                if (GetVectorLength(delta_new) == 0)
                {
                    return null;
                }

                double difference = GetVectorLength(copy_delta_app); // знаходжу різницю
                if (difference > accuracy)
                {
                    approximation[0, 0] = delta_new[0, 0]; // присвоюю початковому наближення нові значення
                    approximation[1, 0] = delta_new[1, 0];
                }
                else // якщо різниця мала то показую результат
                {
                    double[,] check_res = F(delta_new);

                    if (Math.Abs(check_res[0, 0]) <= accuracy && Math.Abs(check_res[1, 0]) <= accuracy)
                    {
                        double complexity = iterationCount * 4;
                        return new double[] { delta_new[0, 0], delta_new[1, 0], complexity, 1 };
                    }
                    if (Math.Abs(check_res[0, 0]) <= 3 * accuracy && Math.Abs(check_res[1, 0]) <= 3 * accuracy)
                    {
                        double complexity = iterationCount * 4;
                        return new double[] { delta_new[0, 0], delta_new[1, 0], complexity, -1 };
                    }
                    return null;
                }
            }
            return null;
        }
    }
}
