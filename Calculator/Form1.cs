using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Calculator
{
    public partial class Form1 : Form
    {
        string value = "", valuePrev = "";
        List<string> operators = new List<string> { "+", "-", "*", "/" };

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsNumber(e.KeyChar))
            {
                addNumberToInput(e.KeyChar.ToString());
            }
            else if (e.KeyChar == '.')
            {
                addPeriodToInput();
            }
            else if (operators.Any(op => e.KeyChar.Equals(Convert.ToChar(op))))
            {
                addOperator(e.KeyChar.ToString());
            }
            else if (e.KeyChar == '(')
            {
                addLeftBracket();
            }
            else if (e.KeyChar == ')')
            {
                addRightBracket();
            }
            else if (e.KeyChar == Convert.ToChar(Keys.Enter) || e.KeyChar == '=')
            {
                calculate();
            }
            else if (e.KeyChar == Convert.ToChar(Keys.Back))
            {
                backspace();
            }
        }

        private void txtInput_TextChanged(object sender, EventArgs e)
        {
            txtInput.Select(txtInput.Text.Length, 0);
        }

        private void txtHistory_TextChanged(object sender, EventArgs e)
        {
            txtHistory.Select(txtHistory.Text.Length, 0);
        }

        private void btnNumber_Click(object sender, EventArgs e)
        {
            addNumberToInput(((Button)sender).Text);
            this.ActiveControl = null;
        }

        private void btnPeriod_Click(object sender, EventArgs e)
        {
            addPeriodToInput();
            this.ActiveControl = null;
        }

        private void btnPlusMinus_Click(object sender, EventArgs e)
        {
            if (!value.StartsWith("-"))
            {
                value = "-" + value;
            }
            else
            {
                value = value.Remove(0, 1);
            }

            txtInput.Text = value;
            txtError.Clear();

            this.ActiveControl = null;
        }

        private void btnOperators_Click(object sender, EventArgs e)
        {
            addOperator(((Button)sender).Text);
            this.ActiveControl = null;
        }

        private void btnBracketLeft_Click(object sender, EventArgs e)
        {
            addLeftBracket();
            this.ActiveControl = null;
        }

        private void btnBracketRight_Click(object sender, EventArgs e)
        {
            addRightBracket();
            this.ActiveControl = null;
        }

        private void btnCalc_Click(object sender, EventArgs e)
        {
            calculate();
            this.ActiveControl = null;
        }

        private void btnBackspace_Click(object sender, EventArgs e)
        {
            backspace();
            this.ActiveControl = null;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            if (value != "")
            {
                value = "";
            }
            else if (valuePrev != "")
            {
                valuePrev = "";
            }

            txtInput.Text = value;
            txtHistory.Text = valuePrev;
            txtError.Clear();

            this.ActiveControl = null;
        }

        private void btnClearAll_Click(object sender, EventArgs e)
        {
            value = "";
            valuePrev = "";
            txtInput.Text = value;
            txtHistory.Text = valuePrev;
            txtError.Clear();

            this.ActiveControl = null;
        }

        private void lstHistory_MouseClick(object sender, MouseEventArgs e)
        {
            int selectedIndex = lstHistory.IndexFromPoint(e.Location);

            if (selectedIndex == -1 || lstHistory.Items[selectedIndex].ToString() == "")
            {
                lstHistory.ClearSelected();
            }
        }

        private void lstHistory_DoubleClick(object sender, EventArgs e)
        {
            if (lstHistory.SelectedItem != null)
            {
                string selectedItem = lstHistory.SelectedItem.ToString();

                if (selectedItem.Contains("History"))
                {
                    return;
                }
                else if (selectedItem.StartsWith("="))
                {
                    value = selectedItem.Remove(0, 2);
                }
                else
                {
                    value = selectedItem;
                }

                txtInput.Text = value;
            }
        }

        private void btnClearList_Click(object sender, EventArgs e)
        {
            lstHistory.Items.Clear();
        }

        private void btnExportList_Click(object sender, EventArgs e)
        {
            bool validList = false;

            foreach (var item in lstHistory.Items)
            {
                if (item.ToString().Any(char.IsDigit))
                {
                    validList = true;
                    break;
                }
            }

            if (validList)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.FileName = "CalculatorHistory-" + DateTime.Now.ToString("yyyyMMddhhmmss");
                sfd.DefaultExt = "txt";
                sfd.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string file = sfd.FileName;
                        StreamWriter sw = new StreamWriter(file);

                        foreach (var item in lstHistory.Items)
                        {
                            if (!item.ToString().Contains("History"))
                            {
                                sw.WriteLine(item.ToString());
                            }
                        }

                        sw.Close();
                        lstHistory.Items.Add("History Saved Successfully");
                    }
                    catch (Exception)
                    {
                        lstHistory.Items.Add("Failed to Save History");
                    }
                }
                else
                {
                    lstHistory.Items.Add("Failed to Save History");
                }
            }
            else
            {
                lstHistory.Items.Add("History is Empty");
            }
        }

        private void addNumberToInput(string num)
        {
            if (value == "0")
            {
                value = "";
            }

            if (valuePrev.EndsWith(")"))
            {
                valuePrev += " *";
                txtHistory.Text = valuePrev;
            }

            value += num;

            txtInput.Text = value;
            txtError.Clear();
        }

        private void addPeriodToInput()
        {
            if (value.Contains("."))
            {
                txtError.Text = "Error: Can only contain 1 period";
            }
            else
            {
                value += ".";

                txtInput.Text = value;
                txtError.Clear();
            }
        }

        private void addOperator(string calcOperator)
        {
            addMissingZero();

            if (value != "")
            {
                if (txtHistory.Text == "")
                {
                    valuePrev = value;
                }
                else
                {
                    valuePrev += " " + value;
                }

                valuePrev += " " + calcOperator;

                value = "";

                txtInput.Text = value;
                txtHistory.Text = valuePrev;
                txtError.Clear();
            }
            else if (valuePrev.EndsWith(")"))
            {
                valuePrev += " " + calcOperator;
                txtHistory.Text = valuePrev;
            }
            else
            {
                if (operators.Any(op => valuePrev.EndsWith(op)))
                {
                    valuePrev = valuePrev.Remove(valuePrev.Length - 1, 1) + calcOperator;
                    txtHistory.Text = valuePrev;
                }
            }
        }

        private void addLeftBracket()
        {
            addMissingZero();

            if (value == "" || value == "0")
            {
                if (valuePrev.EndsWith(")"))
                {
                    valuePrev += " * (";
                }
                else
                {
                    valuePrev += " (";
                }
            }
            else if (value != "" && valuePrev == "")
            {
                valuePrev = value + " * (";
                value = "";
            }
            else if (value != "" && valuePrev != "")
            {
                valuePrev += " " + value + " * (";
                value = "";
            }

            txtInput.Text = value;
            txtHistory.Text = valuePrev;
            txtError.Clear();
        }

        private void addRightBracket()
        {
            addMissingZero();

            if (enoughLeftBrackets())
            {
                if (value == "")
                {
                    if (valuePrev.EndsWith("("))
                    {
                        txtError.Text = "Error: No value entered";
                        return;
                    }
                    else
                    {
                        valuePrev += " )";
                        value = "";
                    }
                }
                else if (value != "")
                {
                    valuePrev += " " + value + " )";
                    value = "";
                }

                txtInput.Text = value;
                txtHistory.Text = valuePrev;
                txtError.Clear();
            }
            else
            {
                txtError.Text = "Error: Not enough left brackets";
            }
        }

        private void calculate()
        {
            DataTable dt = new DataTable();
            string calculation;

            addMissingZero();
            addMissingRightBrackets();

            if (valuePrev != "" && value != "")
            {
                calculation = valuePrev + " " + value;
            }
            else if (valuePrev != "" && value == "")
            {
                if (operators.Any(op => valuePrev.EndsWith(op)))
                {
                    valuePrev = valuePrev.Remove(valuePrev.Length - 2, 2);
                }

                calculation = valuePrev;
            }
            else
            {
                if (operators.Any(op => value.Contains(op)))
                {
                    calculation = value;
                }
                else
                {
                    return;
                }
            }

            try
            {
                var result = dt.Compute(calculation, "");
                value = result.ToString();
                txtInput.Text = value;

                lstHistory.Items.Add(calculation);
                lstHistory.Items.Add("= " + value);
                lstHistory.Items.Add("");
                lstHistory.SelectedIndex = lstHistory.Items.Count - 2;


                txtInput.Text = value;
                valuePrev = "";
                txtHistory.Text = valuePrev;

                txtError.Clear();
            }
            catch (Exception ex)
            {
                txtError.Text = ex.Message;
            }
        }

        private void backspace()
        {
            if (value != "")
            {
                value = value.Remove(value.Length - 1);
            }

            txtInput.Text = value;
            txtHistory.Text = valuePrev;
            txtError.Clear();
        }

        private void addMissingZero()
        {
            if (value.EndsWith("."))
            {
                value += "0";
            }
        }

        private int countLeftBrackets()
        {
            int count = 0;

            foreach (char c in valuePrev)
            {
                if (c == '(')
                {
                    count += 1;
                }
            }

            return count;
        }

        private int countRightBrackets()
        {
            int count = 0;

            foreach (char c in valuePrev)
            {
                if (c == ')')
                {
                    count += 1;
                }
            }

            return count;
        }

        private bool enoughLeftBrackets()
        {
            if (countLeftBrackets() > countRightBrackets())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void addMissingRightBrackets()
        {
            int count = countLeftBrackets() - countRightBrackets();

            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    value += " )";
                }
            }
        }

        private void updateTextboxes()
        {
            txtInput.Text = value;
            txtHistory.Text = valuePrev;
            txtError.Clear();
        }
    }
}
