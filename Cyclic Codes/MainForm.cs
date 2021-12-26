using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CyclicCodes
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        int n = 0;
        int k = 0;
        bool Validated_G = false;
        bool Validated_kn = false;
        PolyNomial gX;
        PolyNomial U;

        private void MainForm_Load(object sender, EventArgs e)
        {
        }

        #region "Validations"

        private string Validate_kn()
        {
            try
            {
                Validated_kn = false;

                if (k_TextBox.Text == "")
                {
                    return "Error - Invalid value of 'k'";
                }
                else if (n_TextBox.Text == "")
                {
                    return "Error - Invalid value of 'n'";
                }
                else
                {
                    k = Convert.ToInt32(k_TextBox.Text);
                    n = Convert.ToInt32(n_TextBox.Text);
                    if (k >= n)
                    {
                        return "Error - 'k' must be less than 'n'";
                    }
                }

                Validated_kn = true;
                return "No Error";
            }
            catch (Exception ex)
            {
                return "Error - " + ex.Message;
            }
        }

        private string Validate_G()
        {
            try
            {
                gX = null;
                U = null;
                Validated_G = false;
                G_Input_TextBox.Text = ApplyPolyFormat(G_Input_TextBox.Text);

                var s = G_Input_TextBox.Text.Replace(" ", "");
                s = s.Replace("X+", "+").Replace("+X", "");

                if (s.Contains("X"))
                {
                    return "Error - Invalid characters found.";
                }

                for (var i = 0; i < 10; i++)
                {
                    s = s.Replace(i.ToString(), "");
                }

                s = s.Replace("+", "");

                if (s.Length != 0)
                {
                    return "Error - Invalid characters found.";
                }
                
                Validated_G = true;
                
                return "No Error";
            }
            catch (Exception ex)
            {
                return "Error - " + ex.Message;
            }
        }

        #endregion

        #region "Inputs <n, k, G>

        private void kn_TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue > 57 || e.KeyValue == 32)
            {
                e.SuppressKeyPress = true;
            }
        }

        private void kn_TextBox_Leave(object sender, EventArgs e)
        {
            try
            {
                var err = Validate_kn();
                if (err.StartsWith("Error - "))
                {
                    if (err.Contains("'k' must be less than 'n'"))
                    {
                        mainErrorProvider.SetError(k_TextBox, "k must be less than n");
                        mainErrorProvider.SetError(n_TextBox, "n must be greator than k");
                    }
                }
                else
                {
                    mainErrorProvider.Clear();
                    G_Input_TextBox_Leave(sender, e);
                }
            }
            catch { }
        }

        private void G_Input_TextBox_Leave(object sender, EventArgs e)
        {
            var err = Validate_G();
            if (err.StartsWith("Error - "))
            {
                mainErrorProvider.SetError(G_Input_TextBox, err);
            }
            else
            {
                mainErrorProvider.Clear();
                if (!Validated_kn)
                {
                    kn_TextBox_Leave(sender, e);
                }
            }
        }

        #endregion

        #region "Error Detection"

        private void detectErrorButton_Click(object sender, EventArgs e)
        {
            if (!r_ED_TextBox.Text.Contains("X"))
            {
                r_ED_TextBox.Text = r_ED_TextBox.Text.Replace(" ", "");
                if (r_ED_TextBox.Text.Length > n)
                {
                    mainErrorProvider.SetError(r_ED_TextBox, "Length of the received Vector <r> is greator than the length of the codeword.");
                    return;
                }
                else if (r_ED_TextBox.Text.Length < n)
                {
                    mainErrorProvider.SetError(r_ED_TextBox, "Length of the received Vector <r> is less than the length of the codeword.");
                    return;
                }
                else
                {
                    mainErrorProvider.Clear();
                }
            }

            var r = new PolyNomial(r_ED_TextBox.Text);
            var SX = r.Remainder(gX);
            if (r_ED_TextBox.Text.Contains("X"))
            {
                S_ED_TextBox.Text = SX.ToPolynomialString();
            }
            else
            {
                S_ED_TextBox.Text = SX.ToBinaryString().PadRight(n - k, '0');
            }
            
            if (SX.IsAllZero)
            {
                S_ED_TextBox.Text = "0";
                S_ED_Label.Text = "No Error. The received vector <r> is a valid codeword.";
                S_ED_Label.ForeColor = Color.Green;
            }
            else
            {
                S_ED_Label.Text = "Error. The received vector <r> is not a valid codeword.";
                S_ED_Label.ForeColor = Color.Red;
            }
        }

        private void ror_ED_Button_Click(object sender, EventArgs e)
        {
            try
            {
                if (r_ED_TextBox.Text.Contains("X"))
                {
                    var r = new PolyNomial(r_ED_TextBox.Text);
                    var rS = r.ToBinaryString().PadRight(n,'0');
                    rS = rS.Last() + rS.Substring(0, rS.Length - 1);
                    r_ED_TextBox.Text = ApplyPolyFormat((new PolyNomial(rS).ToPolynomialString()));
                }
                else
                {
                    r_ED_TextBox.Text = r_ED_TextBox.Text.Replace(" ", "");
                    r_ED_TextBox.Text = r_ED_TextBox.Text.Last() + r_ED_TextBox.Text.Substring(0, r_ED_TextBox.Text.Length - 1);
                }
                detectErrorButton.PerformClick();
            }
            catch { }
        }

        private void rol_ED_Button_Click(object sender, EventArgs e)
        {
            try
            {
                if (r_ED_TextBox.Text.Contains("X"))
                {
                    var r = new PolyNomial(r_ED_TextBox.Text);
                    var rS = r.ToBinaryString().PadRight(n, '0');
                    rS = rS.Substring(1, rS.Length - 1) + rS.First();
                    r_ED_TextBox.Text = ApplyPolyFormat(new PolyNomial(rS).ToPolynomialString());
                }
                else
                {
                    r_ED_TextBox.Text = r_ED_TextBox.Text.Replace(" ", "");
                    r_ED_TextBox.Text = r_ED_TextBox.Text.Substring(1, r_ED_TextBox.Text.Length - 1) + r_ED_TextBox.Text.First();
                }
                detectErrorButton.PerformClick();
            }
            catch { }
        }

        private void r_ED_TextBox_Leave(object sender, EventArgs e)
        {
            if (r_ED_TextBox.Text.Contains("X"))
            {
                r_ED_TextBox.Text = ApplyPolyFormat(r_ED_TextBox.Text);
            }
            detectErrorButton.PerformClick();
        }

        private void r_ED_TextBox_TextChanged(object sender, EventArgs e)
        {
            S_ED_TextBox.Clear();
            S_ED_Label.Text = "";
        }

        #endregion

        #region "Error Correction"

        private void correctErrorButton_Click(object sender, EventArgs e)
        {
            if (!r_EC_TextBox.Text.Contains("X"))
            {
                r_EC_TextBox.Text = r_EC_TextBox.Text.Replace(" ", "");
                if (r_EC_TextBox.Text.Length > n)
                {
                    mainErrorProvider.SetError(r_EC_TextBox, "Length of the received Vector <r> is greater than the length of the codeword.");
                    return;
                }
                else if (r_EC_TextBox.Text.Length < n)
                {
                    mainErrorProvider.SetError(r_EC_TextBox, "Length of the received Vector <r> is less than the length of the codeword.");
                    return;
                }
                else
                {
                    mainErrorProvider.Clear();
                }
            }

            var r = new PolyNomial(r_EC_TextBox.Text);
            var SX = r.Remainder(gX);
            
            if (r_EC_TextBox.Text.Contains("X"))
            {
                S_EC_TextBox.Text = SX.ToPolynomialString();
            }
            else
            {
                S_EC_TextBox.Text = SX.ToBinaryString().PadRight(n - k, '0');
            } 
            
            if (SX.IsAllZero)
            {
                S_EC_TextBox.Text = "0".PadRight(n - k, '0');
                U_EC_TextBox.Text = r_EC_TextBox.Text;
            }
            else
            {
                SX = SX.LeftShift(n - SX.Length);
                var eUX = r.AddWithoutCarry(SX);

                if (r_EC_TextBox.Text.Contains("X"))
                {
                    U_EC_TextBox.Text = eUX.LeftShift(n - eUX.Length).ToPolynomialString();
                }
                else
                {
                    U_EC_TextBox.Text = eUX.LeftShift(n - eUX.Length).ToBinaryString();
                }

                if (U_EC_TextBox.Text == "")
                {
                    U_EC_TextBox.Text = "0".PadRight(n, '0');
                }
            }
        }

        private void r_EC_TextBox_Leave(object sender, EventArgs e)
        {
            correctErrorButton.PerformClick();
        }

        private void r_EC_TextBox_TextChanged(object sender, EventArgs e)
        {
            S_EC_TextBox.Clear();
            U_EC_TextBox.Clear();
        }

        #endregion

        private void mainTabControl_Selecting(object sender, TabControlCancelEventArgs e)
        {
            #region "Codewords"

            if (e.TabPageIndex == 1)
            {
                if (Validated_G && Validated_kn)
                {
                    if (gX == null)
                    {
                        gX = new PolyNomial(G_Input_TextBox.Text);
                    }
                    if (U == null)
                    {
                        var totalCodewords = Convert.ToInt32(Math.Pow(2, k));
                        U = new PolyNomial(totalCodewords);

                        var sb = new StringBuilder();

                        for (var m = 0; m < totalCodewords; m++)
                        {
                            var mBinary = Convert.ToString(m, 2).PadLeft(k, '0');
                            sb.Append(Environment.NewLine + mBinary + "—————— ");

                            var mX = new PolyNomial(mBinary).RightShift(n - k);
                            var rem = (mX.Remainder(gX));
                            sb.Append(rem.ToBinaryString().PadRight(n-k,'0') + mBinary);
                        }

                        U_TextBox.Text = ApplyBinaryFormat(sb.ToString());
                        U_TextBox.SelectAll();
                        U_TextBox.SelectionAlignment = HorizontalAlignment.Center;
                        U_TextBox.DeselectAll();
                    }
                }
                else
                {
                    mainTabControl.SelectedIndex = 0;
                    MessageBox.Show("Enter valid inputs for 'k', 'n' and 'G' first.", "Error!!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            #endregion 

            #region "Error Detection & Correction"

            else if (e.TabPageIndex == 2 || e.TabPageIndex == 3)
            {
                if (Validated_G && Validated_kn)
                {
                    if (gX == null)
                    {
                        gX = new PolyNomial(G_Input_TextBox.Text);
                    }
                }
                else
                {
                    mainTabControl.SelectedIndex = 0;
                    MessageBox.Show("Enter valid inputs for 'k', 'n' and 'G' first.", "Error!!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        
            #endregion
        }

        public string ApplyBinaryFormat(string s)
        {
            s = s.Replace(" ", "");
            s = s.Trim(new char[] { '\n', '\r' });
            s = s.Replace("1", "1  ");
            s = s.Replace("0", "0  ");
            s = s.Replace("—", "—  ");
            return s;
        }

        public string ApplyPolyFormat(string s)
        {
            s = s.Replace(" ", "");
            s = s.Trim(new char[] { '\n', '\r' });
            s = s.Replace("+", " + ");
            s = s.Trim();
            return s;
        }

    }
}