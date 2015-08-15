using System;
using System.Drawing;
using System.IO.Ports;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace SerialLogger
{
    public partial class Form1 : Form
    {
        Series series1 = new Series("data");
        private int count = 0;

        public Form1()
        {
            InitializeComponent();
            chart_init();
            comboBox1.SelectedIndex = 0;
        }


        private void chart_init()
        {
            series1.ChartType = SeriesChartType.FastLine;
            series1.Color = Color.Blue;
            series1.BorderWidth = 2;
            series1.IsVisibleInLegend = false;

            chart1.Series.Add(series1);
            chart1.ChartAreas[0].AxisX.Title = "X";

            //X軸最小値、最大値、目盛間隔の設定
            chart1.ChartAreas[0].AxisX.Minimum = 0;
            chart1.ChartAreas[0].AxisX.Maximum = 100;
            chart1.ChartAreas[0].AxisX.Interval = 10;

            chart1.ChartAreas[0].AxisY.Title = "Y";
            chart1.ChartAreas[0].AxisY.Minimum = -100;
            chart1.ChartAreas[0].AxisY.Maximum = 1100;
            chart1.ChartAreas[0].AxisY.Interval = 100;
        }

        private void plot(int data)
        {
            series1.Points.Add(data);
            if(count > 100)
            {
                series1.Points.RemoveAt(0);
            }
            else
            {
                count++;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
                button1.Text = "接続";
            }
            else
            {
                serialPort1.PortName = portlist.SelectedItem.ToString();
                serialPort1.BaudRate = 115200;
                try
                {
                    serialPort1.Open();
                    button1.Text = "接続中";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

        }

        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            try
            {
                string data = serialPort1.ReadLine();
                if (!string.IsNullOrEmpty(data))
                {
                    //Invoke((MethodInvoker)(() => MessageBox.Show(data)));
                    BeginInvoke(new AppendTextDelegate(packet_restortion), data);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        //delegate（処理を委譲）
        delegate void AppendTextDelegate(string text);

        //委譲先の関数
        private void packet_restortion(string data)
        {
            int buf;
            if (data.Length > 0)
            {
                // 改行コードの削除
                data = data.Replace("\r", "").Replace("\n", "");
                // カンマ区切りで分割して配列に格納する

                string[] stArrayData = data.Split(',');
                if (int.TryParse(stArrayData[0], out buf))
                {
                    plot(buf);
                }
            }
        }

        private void portlist_Click(object sender, EventArgs e)
        {
            string[] PortList = SerialPort.GetPortNames();

            portlist.Items.Clear();

            //! シリアルポート名をコンボボックスにセットする.
            foreach (string PortName in PortList)
            {
                portlist.Items.Add(PortName);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 1)
            {
                series1.ChartType = SeriesChartType.StepLine;
                chart1.ChartAreas[0].AxisY.Minimum = -1;
                chart1.ChartAreas[0].AxisY.Maximum = 2;
                chart1.ChartAreas[0].AxisY.Interval = 1;
            }

        }
    }
}
