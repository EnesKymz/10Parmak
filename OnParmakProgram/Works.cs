using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace OnParmakProgram
{
    public partial class Works : Form
    {
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

        private const int WM_VSCROLL = 0x115;
        private const int SB_LINEDOWN = 1;
        public Works()
        {
            InitializeComponent();


        }

        private void Works_Load(object sender, EventArgs e)
        {
            comboBox2.SelectedIndex = 0;
            comboBox1.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;

            richTextBox1.SelectionAlignment = HorizontalAlignment.Left;
            if (this.WindowState == FormWindowState.Maximized)
            {
                panel1.Location = new Point(((this.Width / 2) - (panel1.Width / 2)), panel1.Location.Y);
                panel3.Location = new Point(((this.Width / 2) - (panel3.Width / 2)), panel3.Location.Y);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.Text !="")
            {
               
                    string selectedYear = comboBox1.Text.Split(' ').Last();
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load("./katiplikmetinleri" + selectedYear + ".xml");
                    XmlNodeList metinList = xmlDoc.SelectNodes("/metinler/zabitkatipligimetinleri/metinler[@yil='2023']/metin");
                    comboBox3.Items.Clear();
                    foreach (XmlNode metin in metinList)
                    {
                        string metinnID = metin.Attributes["metinn"].Value;
                        comboBox3.Items.Add(metinnID);
                    }
                if (comboBox3.Items.Count > 0)
                {
                    comboBox3.SelectedIndex = 0;
                }
            }
        }
        private void UpdateTimeText()
        {

            string hour = numericUpDown1.Value.ToString("00");
            string minute = numericUpDown2.Value.ToString("00");


            time.Text = $"{hour} : {minute}";
        }
        private int remainingTime;
        bool timeraktif = false;
        private void button1_Click(object sender, EventArgs e)
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
                button1.Text = "Başlat";

                string textBox2Content = textBox2.Text;

                int totalCharacters = textBox2Content.Replace(" ", "").Length;
                int elapsedMinutes = (Convert.ToInt16(numericUpDown1.Value * numericUpDown2.Value) - remainingTime) / 60;

                int spaceCount = textBox2Content.Count(c => c == ' ');
                int totalWords = textBox2Content.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;

                int minutes = (int)numericUpDown1.Value;
                int seconds = (int)numericUpDown2.Value;
                int totalTime = (minutes * 60) + seconds;
                double wordsPerMinute = (totalWords * 60) / totalTime;
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
        private const int maxScrollSteps = 1;

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

                if (currentLine > lastVisibleLine - 4)
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
                    if (typedWords[i].Equals(correctWords[i], StringComparison.CurrentCulture))
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
                richTextBox1.Text = metin ?? "Metin bulunamadı.";
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

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedItem.ToString() == "Zabıt Katipliği")
            {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load("./katiplikmetinleri2023.xml");

                    XmlNodeList zabitKatipligiList = xmlDoc.SelectNodes("/metinler/zabitkatipligicombobox/sayi");

                    comboBox1.Items.Clear();
                    foreach (XmlNode zabitKatip in zabitKatipligiList)
                    {
                        comboBox1.Items.Add(zabitKatip.InnerText);
                    }
            }

        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            if (textBox1.Text == "İsim Giriniz.")
            {
                textBox1.Text = "";
                textBox1.ForeColor = Color.Black;
            }
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                textBox1.Text = "İsim Giriniz.";
                textBox1.ForeColor = Color.Gray;
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (comboBox3.SelectedItem != null)
            {
                string selectedMetinnID = comboBox3.SelectedItem.ToString();
                string selectedYear = comboBox1.Text.Split(' ').Last();

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load("./katiplikmetinleri"+selectedYear+".xml");

                XmlNode metinNode = xmlDoc.SelectSingleNode($"/metinler/zabitkatipligimetinleri/metinler[@yil='2023']/metin[@metinn='{selectedMetinnID}']");

                if (metinNode != null)
                {
                    richTextBox1.Text = metinNode.InnerText;
                }
                else
                {
                    MessageBox.Show("Metin bulunamadı.");
                }
            }
        }
        bool serbestmod = false;
        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBox7.Checked)
            {
                serbestmod = false;
            }
            else
            {
                serbestmod = true;
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

        private void Works_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void Works_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}
