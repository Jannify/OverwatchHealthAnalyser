using Emgu.CV;
using Emgu.CV.OCR;
using Emgu.CV.Structure;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Overwatch_Health_Analyser
{
    public partial class Form1 : Form
    {
        private int[] last_value;
        private Color last_color;
        private int fails;
        private bool running;
        private Bitmap actImage;
        private Rectangle orginalRect = new Rectangle(335, 1365, 110, 60);
        Random rnd;

        public Form1()
        {
            InitializeComponent();
            last_value = new int[1];
            running = false;
            fails = 0;
            rnd = new Random();
            PythonInstance.ShowColor(0, 0, 0);

            button2.Click += (senders, args) => {
                if (running)
                {
                    running = false;
                    pictureBox4.Image = Properties.Resources.Off;
                    //AppUpdate().Dispose();
                    pictureBox1.Image = null;
                    pictureBox2.BackColor = Color.FromKnownColor(KnownColor.White);
                    PythonInstance.ShowColor(0, 0, 0);
                    textBox1.Text = "0/0";
                }
            };

            button1.Click += (senders, args) => {
                running = true;
                pictureBox4.Image = Properties.Resources.On;
                AppUpdate();
            };
        }

        private async Task AppUpdate()
        {
            while (true)
            {
                EmguVC();
                await Task.Delay(7);
            }
        }

        private void EmguVC()
        {

            Tesseract tesseract = new Tesseract(@"D:\Programme\Emgu\emgucv-windesktop 3.4.1.2976\Emgu.CV.OCR\tessdata", "owz", OcrEngineMode.Default);
            //var bmpScreenshot = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, PixelFormat.Format32bppArgb);
            //var gfxScreenshot = Graphics.FromImage(bmpScreenshot);
            //gfxScreenshot.CopyFromScreen(Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y, 0, 0, Screen.PrimaryScreen.Bounds.Size, CopyPixelOperation.SourceCopy);
            //Image<Bgr, byte> image = LiftImage(bmpScreenshot.Clone(new Rectangle(345, 1365, 140, 55), System.Drawing.Imaging.PixelFormat.Format4bppIndexed));


            Rectangle rect = new Rectangle(orginalRect.Location, orginalRect.Size);
            rect.X += rnd.Next(7) - 3;
            rect.Y += rnd.Next(7) - 3;
            rect.Width += rnd.Next(7) - 3;
            rect.Height += rnd.Next(7) - 3;

            Bitmap tmpBitmap = new Bitmap(rect.Width, rect.Height, PixelFormat.Format24bppRgb);
            var gfxScreenshot = Graphics.FromImage(tmpBitmap);
            gfxScreenshot.CopyFromScreen(rect.X, rect.Y, 0, 0, new Size(rect.Width, rect.Height), CopyPixelOperation.SourceCopy);

            Image<Bgr, byte> image = LiftImage(tmpBitmap.Clone(new Rectangle(0, 0, rect.Width, rect.Height), PixelFormat.Format4bppIndexed));
            actImage = image.ToBitmap();



            //tesseract.SetVariable("editor_image_text_color", "2");
            tesseract.SetImage(image);
            int[] health = ValidatHealth(tesseract.GetUTF8Text());
            if (health != null && health.Length == 2)
            {
                textBox1.Text = HealthToString(health);
                DisplayHealthColor(health);
                pictureBox1.Image = image.ToBitmap();
            }
        }

        private Image<Bgr, byte> LiftImage(Bitmap map)
        {
            var image = new Image<Bgr, byte>(map);
            image._EqualizeHist();
            image._GammaCorrect(14.0d);
            image = image.Not();
            image = image.Mul(image);
            image = image.Mul(image);

            return image;
        }

        private int[] ValidatHealth(string raw)
        {
            int[] raw_health = null;
            raw = raw.Replace("\r\n", "");
            raw = Regex.Replace(raw, @"\s+", "");
            if (raw.Contains("/"))
            {
                string[] rawSearchSeperate = raw.Split('/');
                if (rawSearchSeperate.Length == 2 && rawSearchSeperate[0].ToString().Length <= 3 && rawSearchSeperate[0].ToString().Length >= 1 && rawSearchSeperate[1].ToString().Length >= 1)
                {
                    raw_health = Array.ConvertAll(rawSearchSeperate, int.Parse);
                }
            }
            if (raw_health != null)
            {
                if (getMaxhealth() != -1) raw_health[1] = getMaxhealth();
                else if ((raw_health[1] / 25) % 1 == 0)
                {
                    raw_health[1] = int.Parse(raw_health[1].ToString().Replace('8', '0'));
                    if ((raw_health[1] / 25) % 1 != 0)
                    {
                        AddFail(raw_health, raw);
                        raw_health[1] = 0;
                    }
                }
                if (raw_health[0] > raw_health[1])
                {
                    if (raw_health[0].ToString().StartsWith("7") && raw_health[0].ToString().Length > 1) raw_health[0] = 1 + int.Parse(raw_health[0].ToString().Substring(1));
                    else if (raw_health[0].ToString().Contains("8") && (raw_health[0].ToString().EndsWith("08") || raw_health[0].ToString().EndsWith("80") || last_value[0].ToString().EndsWith("88"))) raw_health[0] = int.Parse(raw_health[0].ToString().Replace('8', '0'));
                    if (raw_health[0] > raw_health[1])
                    {
                        AddFail(raw_health, raw);
                        raw_health[1] = 0;
                    }
                }
                if (raw_health[1] > 1000) raw_health[1] = 0;
                if (raw_health[1] != 0)
                {
                    last_value = raw_health;
                    return last_value;
                }
            }

            return last_value;
        }

        private void DisplayHealthColor(int[] health)
        {
            Color color = new Color();
            if ((double)health[0] / health[1] >= 0.5) color = Color.FromKnownColor(KnownColor.Green);
            else if ((double)health[0] / health[1] >= 0.15) color = Color.FromKnownColor(KnownColor.Yellow);
            else if ((double)health[0] / health[1] >= 0) color = Color.FromKnownColor(KnownColor.Red);
            else color = Color.FromKnownColor(KnownColor.Purple);
            if (last_color == color)
            {
                pictureBox2.BackColor = color;
                PythonInstance.ShowColor(color.R, color.G, color.B);
            }
            else last_color = color;
        }

        private string HealthToString(int[] health)
        {
            return health[0].ToString() + "/" + getMaxhealth(health[1]).ToString();
        }

        private int getMaxhealth(int healt = -1)
        {
            if (richTextBox2.Text != "")
            {
                int.TryParse(richTextBox2.Text, out healt);
            }
            return healt;
        }

        private void AddFail(int[] health, string raw)
        {
            richTextBox1.AppendText(HealthToString(health) + "  <|>  " + raw + "\n");
            fails++;
            label1.Text = "Fehler: " + fails.ToString();
            pictureBox3.Image = actImage;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            running = true;
        }
    }
}
