using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO; 
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Nonlinear_calculator
{
    public partial class ResultForm : Form
    {
        protected double[] result;
        protected string type_system;
        protected double[] coefficients;
        protected int accuracy;

        public ResultForm(double[] result, double[] coefficients, string type_string, int accuracy)
        {
            InitializeComponent();

            this.result = result;
            this.coefficients = coefficients;
            this.type_system = type_string;
            this.accuracy = accuracy;
        }
        private void Form3_Load(object sender, EventArgs e)
        {
            label4.Hide();
            string formatString = "F" + accuracy;
            // Виводжу розв'язки
            label1.Text = $"x = {result[0].ToString(formatString)}";
            label2.Text = $"y = {result[1].ToString(formatString)}";
            // Виводжу практичну складність
            int complexity = (int)Math.Round(result[2]);
            label3.Text = $"Практична складність = {complexity}";
            if (result[3] == -1)
            {
                label4.Text = "Даний метод знайшов розв'язки цієї системи, але з гіршою точністю.";
                label4.Show();
            }
        }
        private void button1_Click_1(object sender, EventArgs e) // оброблюю збереження файла
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            saveFileDialog.Title = "Save Text File";
            saveFileDialog.FileName = "result_nonlinear_calculator.txt";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = saveFileDialog.FileName;
                string text1;
                string text2;
                string text3 = label1.Text;
                string text4 = label2.Text;

                if (type_system == "first")
                {
                    text1 = $"x^{coefficients[0]} + {coefficients[1]}*sin(PI * y) + {coefficients[2]} = 0";
                    text2 = $"{coefficients[3]}x + y^{coefficients[4]} + {coefficients[5]} = 0\n";
                }
                else
                {
                    text1 = $"{coefficients[0]}*e^x + {coefficients[1]}*y + {coefficients[2]} = 0";
                    text2 = $"x^{coefficients[3]} + y^{coefficients[4]} + {coefficients[5]} = 0 \n";
                }
                try
                {
                    using (StreamWriter writer = new StreamWriter(filePath))
                    {
                        writer.WriteLine(text1);
                        writer.WriteLine(text2);
                        writer.WriteLine(text3);
                        writer.WriteLine(text4);
                    }
                    MessageBox.Show("Файл успішно збережено!", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception)
                {
                    MessageBox.Show($"Помилка при зберіганні файлу", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}

