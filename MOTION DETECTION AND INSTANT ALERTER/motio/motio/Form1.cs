using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video.DirectShow;
using AForge.Vision.Motion;
using System.IO.Ports;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace motio
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

        }
        // Configure the serial port for communication with the GSM modem
      

        FilterInfoCollection devices;
        VideoCaptureDevice camera = new VideoCaptureDevice();

        MotionDetector detector;
        float detectionLevel,sensitivity;
        
        BlobCountingObjectsProcessing motionProcessing = new BlobCountingObjectsProcessing();
        private SerialPort serialPort;


        private void Form1_Load(object sender, EventArgs e)
        {
            sensitivity = 0.02f;
            detector = new MotionDetector(new TwoFramesDifferenceDetector(), new BlobCountingObjectsProcessing());
            
            detectionLevel = 0;

            devices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo d in devices)
            {
                comboBox1.Items.Add(d.Name);
            }
            comboBox1.SelectedIndex = 0;
            // Configure the serial port for communication with the GSM modem
           
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            //timer1.Enabled = true;
            if (camera.IsRunning == true)
            {
                camera.Stop();
            }

            camera = new VideoCaptureDevice(devices[comboBox1.SelectedIndex].MonikerString);
            videoSourcePlayer1.VideoSource = camera;
            videoSourcePlayer1.Start();
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            detectionLevel = 0;
            videoSourcePlayer1.SignalToStop();
            camera.Stop();
        }
       
        private void timer1_Tick(object sender, EventArgs e)
        {
           // if ( checkBox1.Checked == true)
                if (detectionLevel > 0 && checkBox1.Checked == true)
            {
                System.Media.SoundPlayer player = new System.Media.SoundPlayer();
                player.SoundLocation = "sound.wav";
                player.Play();
                MessageBox.Show("motionProcessing");
                Bitmap bitmap = videoSourcePlayer1.GetCurrentVideoFrame();
                bitmap.Save(Application.StartupPath + "/" + textBox2.Text + "image.jpg");
                textBox2.Text = textBox2.Text + 1;
                // checkBox1.Checked = false;
                detectionLevel = 0;
                SendEmailNotification();
            }
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (camera.IsRunning == true)
            {
                detectionLevel = 0;
                videoSourcePlayer1.SignalToStop();
                camera.Stop();
            }
        }

        private void videoSourcePlayer1_NewFrame_1(object sender, ref Bitmap image)
        {
            // detectionLevel = detector.ProcessFrame(image);
            if (detector.ProcessFrame(image) > sensitivity)
            {
                detectionLevel = detector.ProcessFrame(image);
                //////MessageBox.Show("motionProcessing" );
                notify();
            }
        }
        private void notify()
        {
            if (checkBox1.Checked == true)
            {
                System.Media.SoundPlayer player = new System.Media.SoundPlayer();
                player.SoundLocation = "sound.wav";
                player.Play();
                MessageBox.Show("motionProcessing");
                Bitmap bitmap = videoSourcePlayer1.GetCurrentVideoFrame();
                bitmap.Save(Application.StartupPath + "/" + textBox2.Text + "image.jpg");
                //textBox2.Text = textBox2.Text + 1;
                // checkBox1.Checked = false;
                detectionLevel = 0;
                SendEmailNotification();
            }
        }

        private void videoSourcePlayer1_Click(object sender, EventArgs e)
        {

        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            if(trackBar1.Value == 1 )
            {
                sensitivity = 0.01f;
            }
            else if (trackBar1.Value == 2)
            {
                sensitivity = 0.02f;
            }
            else if (trackBar1.Value == 3)
            {
                sensitivity = 0.03f;
            }
            else if (trackBar1.Value == 4)
            {
                sensitivity = 0.04f;
            }
            else if (trackBar1.Value == 5)
            {
                sensitivity = 0.05f;
            }
            else if (trackBar1.Value == 6)
            {
                sensitivity = 0.06f;
            }
            else if (trackBar1.Value == 7)
            {
                sensitivity = 0.07f;
            }
            else if (trackBar1.Value == 8)
            {
                sensitivity = 0.08f;
            }
            else if (trackBar1.Value == 9)
            {
                sensitivity = 0.09f;
            }
        }
        public  void SendEmailNotification()

        {
            DateTime now = DateTime.Now;
            string time = now.ToString();
            // Sender credentials
            string senderEmail = "bsvssanthiya9@gmail.com";
            string senderPassword = "rbsaxzkxihgqhuao";

            // Recipient email address
            string recipientEmail = "chenningiri8889@gmail.com";
            // recipientEmail = tostrin(sender_mail.Text);
            Attachment att = new Attachment(Application.StartupPath + "/" + textBox2.Text + "image.jpg");
            // SMTP client configuration
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(senderEmail, senderPassword);

            // Email message
            MailMessage mailMessage = new MailMessage(senderEmail, recipientEmail);
            mailMessage.Subject = "Motion Detected!";
            mailMessage.Body = "Motion has been detected by the security system. @ " + time;
            mailMessage.Attachments.Add(att);
            client.Send(mailMessage);
            MessageBox.Show("email sent");
            Console.WriteLine("Email notification sent.");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SendEmailNotification();
            System.Media.SoundPlayer player = new System.Media.SoundPlayer();
            player.SoundLocation = "sound.wav";
            player.Play();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        static void CallUserPhone()
        {
          SerialPort serialPort = new SerialPort("COM3", 9600);
            serialPort.Open();
            // Send AT command to make a call
            serialPort.WriteLine("ATD+1234567890;"); // Replace +1234567890 with the user's phone number
           
        }
    }
    }
