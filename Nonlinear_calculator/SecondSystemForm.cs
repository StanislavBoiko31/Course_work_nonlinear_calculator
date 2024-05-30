using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nonlinear_calculator
{
    public partial class SecondSystemForm : Form
    {
        private FirstSystemForm form1;
        public SecondSystemForm()
        {
            InitializeComponent();

            // додаємо вибір у перший випадаючий список
            comboBox1.Items.Add("1");
            comboBox1.Items.Add("2");
            // додаємо вибір у випадаючий список з методами
            comboBox2.Items.Add("Метод січних");
            comboBox2.Items.Add("Метод Ньютона");

            comboBox1.SelectedItem = "2";
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Відкриваємо першу форму, якщо користувач змінив індекс у випадаючому списку
            string selectedValue = comboBox1.SelectedItem?.ToString();
            if (selectedValue == "1")
            {
                this.Hide();

                form1 = new FirstSystemForm();
                form1.ShowDialog();
                this.Close();

                form1.Dispose();
            }
        }
        private void button1_Click_1(object sender, EventArgs e)
        {
            double[] my_coefficients = new double[6];
            double[,] approximation = new double[2, 1];
            int accuracy = 0;
            string selectedMethod = "";

            // Передаємо посилання на змінні my_coefficients, approximation та accuracy для заповнення функцією read_input_data
            bool isInputValid = read_input_data(ref my_coefficients, ref approximation, ref accuracy, ref selectedMethod);
            double new_accuracy = Math.Pow(10, -accuracy);
            if (!isInputValid)
            {
                return;
            }
            try
            {
                // Створюємо другу систему рівнянь
                Equation2 my_equation = new Equation2(my_coefficients, approximation, new_accuracy);

                double[] result = new double[3]; // Змінна для зберігання результату
                switch (selectedMethod)
                {
                    case "Метод січних":
                        result = my_equation.SecantMethod(approximation); // Використовуємо метод січних
                        break;

                    case "Метод Ньютона":
                        result = my_equation.NewtonMethod(approximation); // Використовуємо метод Ньютона
                        break;

                    default:
                        MessageBox.Show("Невідомий метод обрано.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                }
                if (result == null)
                {
                    MessageBox.Show("Цей метод не може розв'язати дану систему");
                }
                else
                {
                    ResultForm form3 = new ResultForm(result, my_coefficients, "second", accuracy); // Виводимо результат у новій формі
                    form3.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($" {ex.Message}"); // Повертаємо помилку
            }
        }
        private bool read_input_data(ref double[] my_coefficients, ref double[,] approximation, ref int accuracy, ref string selectedMethod)
        {
            TextBox[] all_TextBoxes = { textBox1, textBox2, textBox3, textBox4, textBox5, textBox6, textBox11, textBox12, textBox13 };

            bool hasEmptyFields = false;
            // Перевірка заповненості всіх текстових полів
            foreach (var textBox in all_TextBoxes)
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.BackColor = Color.Red; 
                    hasEmptyFields = true;
                }
                else
                {
                    textBox.BackColor = SystemColors.Window; 
                }
            }
            if (hasEmptyFields)
            {
                MessageBox.Show("Будь ласка, заповніть всі текстові поля.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (comboBox2.SelectedIndex == -1) // Перевіряємо чи користувач вибрав метод розв1язання
            {
                MessageBox.Show("Будь ласка, виберіть метод розв'язання.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            try
            {
                for (int i = 0; i < 6; i++)
                {
                    my_coefficients[i] = ValidateInput.ValidateCoefficient(all_TextBoxes[i]); // зчитуємо коефіцієнти
                }
                // Зчитуємо значення для approximation з двох текстових полів
                approximation[0, 0] = ValidateInput.ValidateCoefficient(textBox12);
                approximation[1, 0] = ValidateInput.ValidateCoefficient(textBox13);

                accuracy = ValidateInput.ValidateAccuracy(textBox11); // зчитуємо точність

                selectedMethod = comboBox2.SelectedItem?.ToString(); // зчитуємо метод розв'язання

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($" {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error); // Повертаємо помилку
                return false;
            }
        }
        //кнопка для очищення всіх текстових полів
        private void button2_Click(object sender, EventArgs e)
        {
            TextBox[] all_TextBoxes = { textBox1, textBox2, textBox3, textBox4, textBox5, textBox6, textBox11, textBox12, textBox13 };

            foreach (var textBox in all_TextBoxes)
            {
                textBox.Clear();
            }
        }
    }
}
