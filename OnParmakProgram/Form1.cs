using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Runtime.InteropServices;

namespace OnParmakProgram
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

        private const int WM_VSCROLL = 0x115;
        private const int SB_LINEDOWN = 1;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox2.Focus();
            comboBox2.SelectedIndex = 0;
            comboBox1.SelectedIndex = 0;
            richTextBox1.SelectionAlignment = HorizontalAlignment.Left;
            serbestmod = true;
            if (this.WindowState == FormWindowState.Maximized)
            {
                panel1.Location = new Point(((this.Width / 2) - (panel1.Width / 2)), panel1.Location.Y);
                panel3.Location = new Point(((this.Width / 2) - (panel3.Width / 2)), panel3.Location.Y);
            }

        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedItem == "1. K�s�m")
            {


                XDocument xmlDoc = XDocument.Load("./Dersdatabase.xml");


                string[] selectedDers = comboBox1.SelectedItem.ToString().Split(':');


                var metin = xmlDoc.Descendants("Ders")
                                  .Where(d => d.Attribute("adi").Value == selectedDers[0])
                                  .Select(d => d.Element("Metin").Value)
                                  .FirstOrDefault();
                richTextBox1.Text = metin ?? "Metin bulunamad�.";
                if (metin != null)
                {
                    richTextBox1.Text = metin.Replace("\n", Environment.NewLine).Trim();
                }
            }
        }
        private void UpdateTimeText()
        {

            string hour = numericUpDown1.Value.ToString("00");
            string minute = numericUpDown2.Value.ToString("00");


            time.Text = $"{hour} : {minute}";
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {

        }
        private int remainingTime;
        bool timeraktif = false;
        private void button1_Click(object sender, EventArgs e)
        {
            if (checkBox7.Checked == false)
            {
                if (timeraktif == false)
                {
                    textBox2.Clear();
                    textBox2.Focus();
                    UpdateTimeText();
                    int minutes = (int)numericUpDown1.Value;
                    int seconds = (int)numericUpDown2.Value;
                    remainingTime = (minutes * 60) + (seconds);
                    timer1.Start();
                    button1.Text = "Tekrar Dene";
                    timeraktif = true;
                    serbestmod = false;

                }
            }
            if (timer1.Enabled == true)
            {
                timer1.Stop();
                timeraktif = false;

            }
        }
        private int previousWordCount = 0;
        private int totalWordCount = 0;

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (remainingTime > 0)
            {
                remainingTime--;

                TimeSpan time1 = TimeSpan.FromSeconds(remainingTime);
                time.Text = $"{time1.Minutes:D2} : {time1.Seconds:D2}";
            }
            else
            {
                timer1.Stop();
                MessageBox.Show("Zaman doldu!");
                time.Text = "00 : 00";
                button1.Text = "Ba�lat";

                string textBox2Content = textBox2.Text;

                int totalCharacters = textBox2Content.Replace(" ", "").Length;
                int elapsedMinutes = (Convert.ToInt16(numericUpDown1.Value * numericUpDown2.Value) - remainingTime) / 60;

                int spaceCount = textBox2Content.Count(c => c == ' ');
                int totalWords = textBox2Content.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
                double wordsPerMinute = elapsedMinutes > 0 ? (double)totalWords / elapsedMinutes : totalWords;
                int totalLetterCount = textBox2.Text.Count(char.IsLetter);


                int yanlissayi = Convert.ToInt32(label4.Text);
                float hataorani = ((float)yanlissayi / totalWords) * 100;

                string validElementName = textBox1.Text.Replace(" ", "_");


                Statics istatistik = new Statics();
                istatistik.isim.Text = textBox1.Text;
                istatistik.dakikabasina.Text = wordsPerMinute.ToString();
                istatistik.dogru.Text = label3.Text;
                istatistik.yanlis.Text = label4.Text;
                istatistik.toplam.Text = totalLetterCount.ToString();

                istatistik.hataorani.Text = "%" + " " + hataorani.ToString();
                istatistik.tusvuruslari.Text = totalCharacters + spaceCount.ToString();
                istatistik.Show();
            }
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox5.Checked)
            {
                textBox2.ForeColor = Color.White;
            }
            else
            {
                textBox2.ForeColor = Color.Black;
            }
        }

        private void textBox2_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (checkBox2.Checked)
            {
                if (e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete)
                {
                    e.SuppressKeyPress = true;
                }
            }
        }
        private int scrollStepCount = 0;
        private const int maxScrollSteps = 4;
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (!checkBox7.Checked)
            {
                if (serbestmod == false)
                {


                    textBox2.Focus();
                    UpdateTimeText();
                    int minutes = (int)numericUpDown1.Value;
                    int seconds = (int)numericUpDown2.Value;
                    remainingTime = (minutes * 60) + (seconds);
                    timer1.Start();
                    button1.Text = "Tekrar Dene";
                    serbestmod = true;
                    timeraktif = false;


                }
            }
            string[] kelimeler = richTextBox1.Text.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            string[] girilenKelimeler = textBox2.Text.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            int dogruSayisi = 0;
            int yanlisSayisi = 0;

            for (int i = 0; i < girilenKelimeler.Length; i++)
            {
                if (i < kelimeler.Length && kelimeler[i].Equals(girilenKelimeler[i], StringComparison.OrdinalIgnoreCase))
                {
                    dogruSayisi++;
                }
                else
                {
                    yanlisSayisi++;
                }
            }

            label3.Text = dogruSayisi.ToString();
            label4.Text = yanlisSayisi.ToString();
            label5.Text = (dogruSayisi - yanlisSayisi).ToString();
            if (dogruSayisi - yanlisSayisi <= 0)
            {
                label5.Text = "0";
            }
            if (checkBox6.Checked)
            {
                KelimeTakibi();
            }
            int visibleLines = richTextBox1.ClientSize.Height / richTextBox1.Font.Height;

            int totalLines = richTextBox1.GetLineFromCharIndex(richTextBox1.Text.Length - 1) + 1;

            int firstVisibleLine = richTextBox1.GetLineFromCharIndex(richTextBox1.GetCharIndexFromPosition(new Point(0, 0)));

            int lastVisibleLine = firstVisibleLine + visibleLines - 1;

            string inputText = textBox2.Text;

            if (inputText.Length > 0)
            {
                int charIndex = textBox2.GetCharIndexFromPosition(new Point(0, textBox2.ClientSize.Height));
                int currentLine = richTextBox1.GetLineFromCharIndex(charIndex);

                if ((currentLine > lastVisibleLine) && (lastVisibleLine != totalLines))
                {
                    StartSmoothScroll();
                }
            }


        }
        private void StartSmoothScroll()
        {
            if (!timer2.Enabled)
            {
                scrollStepCount = 0;
                timer2.Start();
            }
        }

        private void KelimeTakibi()
        {
            string correctText = richTextBox1.Text;
            string typedText = textBox2.Text;

            string[] correctWords = correctText.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            string[] typedWords = typedText.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            int currentIndex = 0;

            for (int i = 0; i < correctWords.Length; i++)
            {
                int wordLength = correctWords[i].Length;
                int spaceLength = 1;

                if (i < typedWords.Length)
                {
                    if (typedWords[i].Equals(correctWords[i], StringComparison.OrdinalIgnoreCase))
                    {
                        richTextBox1.Select(currentIndex, wordLength);
                        richTextBox1.SelectionColor = checkBox6.Checked ? Color.Blue : Color.Black;
                    }
                    else
                    {
                        richTextBox1.Select(currentIndex, wordLength);
                        richTextBox1.SelectionColor = checkBox6.Checked ? Color.Red : Color.Black;
                    }
                }
                else
                {
                    richTextBox1.Select(currentIndex, wordLength);
                    richTextBox1.SelectionColor = Color.Black;
                }

                currentIndex += wordLength + spaceLength;
            }

            if (typedWords.Length > correctWords.Length)
            {
                for (int j = correctWords.Length; j < typedWords.Length; j++)
                {
                    string extraWord = typedWords[j];
                    richTextBox1.AppendText(extraWord + " ");
                    richTextBox1.Select(currentIndex, extraWord.Length);
                    richTextBox1.SelectionColor = Color.Red;
                    currentIndex += extraWord.Length + 1;
                }
            }

            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.SelectionColor = Color.Black;
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBox6.Checked)
            {
                richTextBox1.ForeColor = Color.Black;

                richTextBox1.SelectAll();
                richTextBox1.SelectionColor = Color.Black;
                richTextBox1.DeselectAll();
                richTextBox1.Clear();

                XDocument xmlDoc = XDocument.Load("./Dersdatabase.xml");


                string[] selectedDers = comboBox1.SelectedItem.ToString().Split(':');


                var metin = xmlDoc.Descendants("Ders")
                                  .Where(d => d.Attribute("adi").Value == selectedDers[0])
                                  .Select(d => d.Element("Metin").Value)
                                  .FirstOrDefault();
                richTextBox1.Text = metin ?? "Metin bulunamad�.";
                if (metin != null)
                {
                    richTextBox1.Text = metin.Replace("\n", Environment.NewLine).Trim();
                }
            }
            else
            {

            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                time.ForeColor = Color.Transparent;
            }
            else
            {
                time.ForeColor = Color.Red;
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (comboBox2.SelectedItem.ToString() == "1. K�s�m")
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load("./dersler-combobox.xml");

                XmlNodeList zabitKatipligiList = xmlDoc.SelectNodes("/ders-items/dersler/sayi");

                comboBox1.Items.Clear();
                foreach (XmlNode zabitKatip in zabitKatipligiList)
                {
                    comboBox1.Items.Add(zabitKatip.InnerText);
                }
                comboBox1.SelectedIndex = 0;
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            if (textBox1.Text == "�sim Giriniz.")
            {
                textBox1.Text = "";
                textBox1.ForeColor = Color.Black;
            }
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                textBox1.Text = "�sim Giriniz.";
                textBox1.ForeColor = Color.Gray;
            }
        }
        bool serbestmod = false;
        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBox7.Checked)
            {
                serbestmod = false;
                textBox2.Clear();
            }
            else
            {
                serbestmod = true;
                if (timer1.Enabled == true)
                {
                    timer1.Stop();
                    timeraktif = false;

                }
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            SendMessage(richTextBox1.Handle, WM_VSCROLL, (IntPtr)SB_LINEDOWN, IntPtr.Zero);

            scrollStepCount++;
            if (scrollStepCount >= maxScrollSteps)
            {
                timer2.Stop();
                scrollStepCount = 0;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MainForm main = new MainForm();
            main.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox2.Clear();
            richTextBox1.SelectionStart = 0;
            richTextBox1.ScrollToCaret();
        }
    }
}
