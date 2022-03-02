using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Text.RegularExpressions;
using System.Windows.Forms.Integration;

namespace ESP_message
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            comboBox_Ports.Items.AddRange(SerialPort.GetPortNames());
            button_ComPort.Text = "Connect";
            //WPF ADD
            ElementHost host = new ElementHost();
            host.Dock = DockStyle.Right;
            graf.UserControl1 graf = new graf.UserControl1();
            host.Child = graf;
            this.Controls.Add(host);
            //WPF ADD
        }
        SerialPort ESP_SerialPort = new SerialPort();
        string PortName { get; set; }
        private void button_ComPort_Click(object sender, EventArgs e)
        {
            if (button_ComPort.Text == "Connect")
            {
                try
                {
                    ESP_SerialPort.PortName = PortName;
                }
                catch (Exception)
                {
                    MessageBox.Show("Выберите порт");
                    return;
                }
                ESP_SerialPort.BaudRate = 115200;
                ESP_SerialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
                try
                {
                    ESP_SerialPort.Open();
                    button_ComPort.Text = "Disconnect";
                }
                catch (Exception)
                {
                    MessageBox.Show("Порт закрыт");
                    return;
                }
            }
            else if (button_ComPort.Text == "Disconnect")
            {
                if (ESP_SerialPort.IsOpen)
                {
                    ESP_SerialPort.DiscardInBuffer();
                    ESP_SerialPort.DiscardOutBuffer();
                    ESP_SerialPort.Close();
                    button_ComPort.Text = "Connect";
                    ESP_SerialPort.DataReceived -= new SerialDataReceivedEventHandler(DataReceivedHandler);
                }
                else
                {
                    ESP_SerialPort.Close();
                    ESP_SerialPort.DataReceived -= new SerialDataReceivedEventHandler(DataReceivedHandler);
                    button_ComPort.Text = "Connect";
                }
            }
        }
        private delegate void SetTextDeleg(string text);
        void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            //string data = ESP_SerialPort.ReadExisting();
            string data = ESP_SerialPort.ReadLine();
            BeginInvoke(new SetTextDeleg(si_DataReceived), new object[] { data });
        }
        private void si_DataReceived(string data)
        {
            time(data, "Time_1");
            time(data, "Time_2");
            data_x_y_z(data, "Acc_1");
            data_x_y_z(data, "Acc_2");
            data_x_y_z(data, "Gyro_1");
            data_x_y_z(data, "Gyro_2");
            data_x_y_z(data, "Mag_1");
            data_x_y_z(data, "Mag_2");
            angel(data, "Roll_1");
            angel(data, "Roll_2");
            angel(data, "Pitch_1");
            angel(data, "Pitch_2");
            angel(data, "Yaw_1");
            angel(data, "Yaw_2");
            GPS(data, "Longitude_1");
            GPS(data, "Longitude_2");
            GPS(data, "Lattitude_1");
            GPS(data, "Lattitude_2");
            battery(data, "B1_1");
            battery(data, "B1_2");
            battery(data, "B2_1");
            battery(data, "B2_2");
            other_setting(data, "GPSHeight_1");
            other_setting(data, "GPSHeight_2");
            other_setting(data, "GPSYaw_1");
            other_setting(data, "GPSYaw_2");
            other_setting(data, "GPSV_1");
            other_setting(data, "GPSV_2");
            other_setting(data, "SN_1");
            other_setting(data, "HDOP");
            other_setting(data, "VDOP");
            other_setting(data, "PDOP");
        }

        private void button_refresh_Click(object sender, EventArgs e)
        {
            comboBox_Ports.Items.Clear();
            comboBox_Ports.Items.AddRange(SerialPort.GetPortNames());
        }

        private void comboBox_Ports_SelectedIndexChanged(object sender, EventArgs e)
        {
            PortName = comboBox_Ports.SelectedItem.ToString();
        }
        void time(string data, string name)
        {
            Regex regex = new Regex(name + @":(\d*\W+\d*\W+\d*\s+\d*\W+\d*\W+\d*\W+\d*)");
            MatchCollection matches = regex.Matches(data);
            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    if (name== "Time_1")
                    {
                        textBox_time_1.Text = match.ToString().Replace("Time_1:", "");
                    }
                    else if (name == "Time_2")
                    {
                        textBox_time_2.Text = match.ToString().Replace("Time_2:", "");
                    }
                }
            }
        }
        
        void data_x_y_z(string data,string name)
        {
            Regex regex = new Regex(name + @":(\W?\d+\W?\d*\s+\W?\d+\W?\d*\s+\W?\d+\W?\d*)");
            MatchCollection matches = regex.Matches(data);
            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    string text = match.ToString().Replace(name+":", "");
                    Regex regex_x = new Regex(@"\s?\W?\d+\S?\d*");
                    MatchCollection matches_x = regex_x.Matches(text);
                    if (matches_x.Count >= 3)
                    {
                        if (name == "Acc_1")
                        {
                            textBox_acc_x_1.Text = matches_x[0].ToString();
                            textBox_acc_y_1.Text = matches_x[1].ToString();
                            textBox_acc_z_1.Text = matches_x[2].ToString();
                        }
                        else if (name == "Acc_2")
                        {
                            textBox_acc_x_2.Text = matches_x[0].ToString();
                            textBox_acc_y_2.Text = matches_x[1].ToString();
                            textBox_acc_z_2.Text = matches_x[2].ToString();
                        }
                        else if (name == "Gyro_1")
                        {
                            textBox_gyro_x_1.Text = matches_x[0].ToString();
                            textBox_gyro_y_1.Text = matches_x[1].ToString();
                            textBox_gyro_z_1.Text = matches_x[2].ToString();
                        }
                        else if (name == "Gyro_2")
                        {
                            textBox_gyro_x_2.Text = matches_x[0].ToString();
                            textBox_gyro_y_2.Text = matches_x[1].ToString();
                            textBox_gyro_z_2.Text = matches_x[2].ToString();
                        }
                        else if (name == "Mag_1")
                        {
                            textBox_mag_x_1.Text = matches_x[0].ToString();
                            textBox_mag_y_1.Text = matches_x[1].ToString();
                            textBox_mag_z_1.Text = matches_x[2].ToString();
                        }
                        else if (name == "Mag_2")
                        {
                            textBox_mag_x_2.Text = matches_x[0].ToString();
                            textBox_mag_y_2.Text = matches_x[1].ToString();
                            textBox_mag_z_2.Text = matches_x[2].ToString();
                        }
                    }
                }
            }
        }
        void angel(string data, string name)
        {
            Regex regex = new Regex(name + @"\s+\D?\d*\D*\d*");
            MatchCollection matches = regex.Matches(data);
            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    if (name == "Roll_1")
                    {
                        textBox_angle_roll_1.Text = match.ToString().Replace(name + " ", "");
                    }
                    else if (name == "Roll_2")
                    {
                        textBox_angle_roll_2.Text = match.ToString().Replace(name + " ", "");
                    }
                    else if (name == "Pitch_1")
                    {
                        textBox_angle_pitch_1.Text = match.ToString().Replace(name + " ", "");
                    }
                    else if (name == "Pitch_2")
                    {
                        textBox_angle_pitch_2.Text = match.ToString().Replace(name + " ", "");
                    }
                    else if (name == "Yaw_1")
                    {
                        textBox_angle_yaw_1.Text = match.ToString().Replace(name + " ", "");
                    }
                    else if (name == "Yaw_2")
                    {
                        textBox_angle_yaw_2.Text = match.ToString().Replace(name + " ", "");
                    }
                }
            }
        }
        void GPS(string data, string name)
        {
            Regex regex = new Regex(name + @"\D?\d*");
            MatchCollection matches = regex.Matches(data);
            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    if (name == "Longitude_1")
                    {
                        textBox_Longitude_1.Text = match.ToString().Replace(name + ":", "");
                    }
                    else if (name == "Longitude_2")
                    {
                        textBox_Longitude_2.Text = match.ToString().Replace(name + ":", "");
                    }
                    else if (name == "Lattitude_1")
                    {
                        textBox_Lattitude_1.Text = match.ToString().Replace(name + ":", "");
                    }
                    else if (name == "Lattitude_2")
                    {
                        textBox_Lattitude_2.Text = match.ToString().Replace(name + ":", "");
                    }
                }
            }
        }
        void battery(string data, string name)
        {
            Regex regex = new Regex(name + @"\D+\d*\D*\d*");
            MatchCollection matches = regex.Matches(data);
            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    if (name == "B1_1")
                    {
                        textBox_B1_1.Text = match.ToString().Replace(name + ":", "");
                    }
                    else if (name == "B1_2")
                    {
                        textBox_B1_2.Text = match.ToString().Replace(name + ":", "");
                    }
                    else if (name == "B2_1")
                    {
                        textBox_B2_1.Text = match.ToString().Replace(name + ":", "");
                    }
                    else if (name == "B2_2")
                    {
                        textBox_B2_2.Text = match.ToString().Replace(name + ":", "");
                    }
                }
            }
        }
        void other_setting(string data, string name)
        {
            Regex regex = new Regex(name+@"\D?\s?\d+\D*\d*");
            MatchCollection matches = regex.Matches(data);
            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    if (name == "GPSHeight_1")
                    {
                        textBox_GPSHeight_1.Text = match.ToString().Replace(name + ": ", "") + " m";
                    }
                    else if (name == "GPSHeight_2")
                    {
                        textBox_GPSHeight_2.Text = match.ToString().Replace(name + ": ", "") + " m";
                    }
                    else if(name == "GPSYaw_1")
                    {
                        textBox_GPSYaw_1.Text = match.ToString().Replace(name + ":", "") + " Deg";
                    }
                    else if (name == "GPSYaw_2")
                    {
                        textBox_GPSYaw_2.Text = match.ToString().Replace(name + ":", "") + " Deg";
                    }
                    else if (name == "GPSV_1")
                    {
                        textBox_GPSV_1.Text = match.ToString().Replace(name + ":", "") + " km/h";
                    }
                    else if (name == "GPSV_2")
                    {
                        textBox_GPSV_2.Text = match.ToString().Replace(name + ":", "") + " km/h";
                    }
                    else if (name == "SN_1")
                    {
                        textBox_SN_1.Text = match.ToString().Replace(name + ":", "");
                    }
                    else if (name == "HDOP")
                    {
                        textBox_HDOP_1.Text = match.ToString().Replace(name, "");
                    }
                    else if (name == "VDOP")
                    {
                        textBox_VDOP_1.Text = match.ToString().Replace(name, "");
                    }
                    else if (name == "PDOP")
                    {
                        textBox_PDOP_1.Text = match.ToString().Replace(name, "");
                    }

                }
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.DrawRectangle(new Pen(Color.Black, 3), 800, 50, 150, 100);

        }
        void giro_graf(int x,int y, int z)
        {

        }
    }
}
