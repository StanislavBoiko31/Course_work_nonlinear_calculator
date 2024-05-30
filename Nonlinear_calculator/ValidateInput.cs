using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nonlinear_calculator
{
    public static class ValidateInput

    {
        // Перевірка кількості десяткових знаків
        private static bool IsValidNumAfter(string input)
        {
            int IndexComma = input.IndexOf(',');
            if (IndexComma != -1 && input.Length - IndexComma - 1 > 3)
            {
                return false;
            }
            return true;
        }
        public static double ValidateCoefficient(TextBox currentTextBox)
        {
            double parsedValue;
            if (double.TryParse(currentTextBox.Text, out parsedValue))
            {
                
                if (!IsValidNumAfter(currentTextBox.Text))
                {
                    currentTextBox.BackColor = Color.Red;
                    throw new Exception("Будь ласка, введіть число яке містить не більше трьох знаків після коми.");
                }
                // Перевірка на діапазон
                if (parsedValue > 1000 || parsedValue < -1000)
                {
                    currentTextBox.BackColor = Color.Red;
                    throw new Exception("Будь ласка, введіть число від -1000 до 1000.");
                }
                // Якщо всі перевірки пройдені, повертаємо коректне значення
                currentTextBox.BackColor = Color.White;
                return parsedValue;
            }
            // Якщо не вдалося отримати значення, позначаємо текстове поле червоним і повертаємо помилку
            currentTextBox.BackColor = Color.Red;
            throw new Exception("Невірний формат числа. Введіть число від -1000 до 1000 та яке містить не більше трьох знаків після коми.");
        }
        // валідуємо точність
        public static int ValidateAccuracy(TextBox currentTextBox)
        {
            int parsedValue;
            if (int.TryParse(currentTextBox.Text, out parsedValue))
            {   // Перевірка на діапазон
                if (parsedValue > 10 || parsedValue < 1)
                {
                    throw new Exception("Точність повинна бути в межах від 1 до 10.");
                }
                return parsedValue;
            }
            throw new Exception("Точність повинна бути цілим числом від 1 до 10.");
        }
    }
}
