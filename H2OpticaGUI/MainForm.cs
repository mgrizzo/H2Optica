using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using H2OpticaLogic;

namespace H2OpticaGUI
{
   /*
    * TO-DO
    * - Handle client-side data
    * - Graphs
    * - DataGrid for sensors??
    */

    public partial class H2OpticaMain : Form
    {
        private const int PANEL_RADIUS = 13;
        private const int BAR_RADIUS = 7;

        private DBService dbService;
        private ArduinoService arduinoService;

        private List<Sensor> sensorList = new List<Sensor>();

        internal DataCollection latestCollection = new DataCollection();
        internal DataCollection arduinoCollection = new DataCollection();

        internal double totalVolume = 0.0;

        private Timer updateTimer;
        private Timer arduinoRequest;

        public H2OpticaMain()
        {
            InitializeComponent();
            InitializeTimer();

            string _dbPath = Path.Combine(Application.StartupPath, "sensor_data.db");
            dbService = new DBService(_dbPath);

            dbService.InitializeDB();

            //arduinoService = new ArduinoService(ip_address);

            latestCollection = arduinoService.GetLatestReadingAsync().Result;
        }

        private void InitializeTimer()
        {
            updateTimer = new Timer();
            updateTimer.Interval = 1000;
            updateTimer.Tick += new EventHandler(updateTimer_Tick);
            updateTimer.Start();

            arduinoRequest = new Timer();
            arduinoRequest.Interval =500;
            arduinoRequest.Tick += new EventHandler(arduinoRequest_Tick);
            arduinoRequest.Start();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            dbService.CheckDB();

            //Volume giornaliero
            PanelRoundBorder(DailyContainer, PANEL_RADIUS);

            //Pannello temperatura e pH
            PanelRoundBorder(FixedSensors, PANEL_RADIUS);

            //Temperatura
            PanelRoundBorder(tempBarContainer, BAR_RADIUS);
            PanelRoundBorder(tempBar, BAR_RADIUS);

            //pH
            PanelRoundBorder(phBarContainer, BAR_RADIUS);
            PanelRoundBorder(phBar, BAR_RADIUS);

            //Sensori
            PanelRoundBorder(sensorLayoutPanel, PANEL_RADIUS); //Pannello principale

            latestCollection = dbService.GetLatestSensorData();

            UpdateSensorPanel(sensorList);
        }

        private void PanelRoundBorder(Panel panel, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int arcWidth = radius * 2;
            Rectangle bounds = panel.ClientRectangle;

            //Top left
            path.AddArc(bounds.X, bounds.Y, arcWidth, arcWidth, 180, 90);
            //Top right
            path.AddArc(bounds.Right - arcWidth, bounds.Y, arcWidth, arcWidth, 270, 90);
            //Bottom right
            path.AddArc(bounds.Right - arcWidth, bounds.Bottom - arcWidth, arcWidth, arcWidth, 0, 90);
            //Bottom left
            path.AddArc(bounds.X, bounds.Bottom - arcWidth, arcWidth, arcWidth, 90, 90);

            path.CloseFigure();

            panel.Region = new Region(path);
        }
        private void UpdateTempBar(double temp)
        {
            int maxTemp = 100;
            int maxWidth = tempBar.Width;

            int newWidth = (int)((temp / maxTemp) * maxWidth);

            newWidth = Math.Min(maxWidth, Math.Max(0, newWidth));

            tempBar.Width = newWidth;

            //Colore
            if (temp < 30)
                tempBar.BackColor = Color.FromArgb(247, 231, 79);
            else if (temp < 60)
                tempBar.BackColor = Color.FromArgb(245, 118, 64);
            else
                tempBar.BackColor = Color.FromArgb(245, 64, 64);
        }
        private void UpdatephBar(double pH)
        {
            int maxpH = 14;
            int maxWidth = phBar.Width;

            int newWidth = (int)((pH / maxpH) * maxWidth);

            newWidth = Math.Min(maxWidth, Math.Max(0, newWidth));

            phBar.Width = newWidth;

            //Colore
            if (pH < 4)
                phBar.BackColor = Color.FromArgb(245, 64, 64);
            else if (pH < 10)
                phBar.BackColor = Color.FromArgb(130, 247, 67);
            else
                phBar.BackColor = Color.FromArgb(106, 67, 247);
        }
        private void UpdateSensorPanel(List<Sensor> sensList)
        {
            foreach (Sensor sensor in sensList)
            {
                AddSensor(sensor.SensorName, dbService.GetLatestVolume(sensor.SensorID), sensor.SensorLimit);
            }
        }
        private double GetTotalVolume()
        {
            double totalVolume = 0;

            foreach (var flux in latestCollection.Flux)
            {
                totalVolume += flux.Value;
            }

            return totalVolume;
        }
        private void AddSensor(string name, double data, double? limit)
        {
            Panel sensorPanel = new Panel
            {
                Width = 292,
                Height = 161,
                BackColor = Color.FromArgb(9, 14, 23),
                Margin = new Padding(24)
            };

            PanelRoundBorder(sensorPanel, PANEL_RADIUS);

            Label SensorName = new Label
            {
                Text = name,
                Font = new Font("Microsoft Sans Serif", 22, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(4, 141, 223),
                Location = new Point(0, 0),
                AutoSize = true
            };

            Label SensorValue = new Label
            {
                Text = $"Quantità utilizzata: {data}L",
                Font = new Font("Microsoft Sans Serif", 14, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(4, 141, 223),
                Location = new Point(0, 0),
                AutoSize = true,
            };

            Label SensorLimit = new Label
            {
                Text = limit.HasValue ? $"Limite impostato: {limit}L" : "Limite impostato: --",
                Font = new Font("Microsoft Sans Serif", 14, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(4, 141, 223),
                AutoSize = true,
            };

            sensorPanel.Controls.Add(SensorName);
            sensorPanel.Controls.Add(SensorValue);
            sensorPanel.Controls.Add(SensorLimit);

            SensorName.Location = new Point((sensorPanel.Size.Width - SensorName.PreferredSize.Width) / 2, 20);
            SensorValue.Location = new Point((sensorPanel.Size.Width - SensorValue.PreferredSize.Width) / 2, SensorName.Bottom + 17);
            SensorLimit.Location = new Point((sensorPanel.Size.Width - SensorLimit.PreferredSize.Width) / 2, SensorValue.Bottom + 20);

            sensorLayoutPanel.Controls.Add(sensorPanel);
        }

        //Eventi
        private void updateTimer_Tick(object sender, EventArgs e)
        {
            latestCollection.Flux.Clear();
            latestCollection.Flux = arduinoCollection.Flux;

            latestCollection.Temp = dbService.GetLastTemp();
            latestCollection.pH = dbService.GetLastpH();

            double totalVolume = GetTotalVolume();

            totalVolLabel.Text = $"Oggi hai utilizzato {totalVolume}L";
            tempLabel.Text = $"Temperatura: {latestCollection.Temp}°C";
            phLabel.Text = $"pH: {latestCollection.pH}";

            UpdateTempBar((double)latestCollection.Temp);
            UpdatephBar((double)latestCollection.pH);
        }
        private void arduinoRequest_Tick(object sender, EventArgs e)
        {
            arduinoCollection = arduinoService.GetLatestReadingAsync().Result;
        }

        private void tempBar_Resize(object sender, EventArgs e)
        {
            PanelRoundBorder(tempBar, PANEL_RADIUS);
        }

        private void phBar_Resize(object sender, EventArgs e)
        {
            PanelRoundBorder(phBar, PANEL_RADIUS);
        }


        //Evento bottone aggiornamento pannello sensori
        private void updateSensorCount_Click(object sender, EventArgs e)
        {
            List<Sensor> latestList = new List<Sensor>();

            latestList = dbService.GetSensorList();

            bool congruent = sensorList.Count == latestList.Count && !sensorList.Except(latestList).Any() && !latestList.Except(sensorList).Any();

            if(!congruent)
            {
                sensorList.Clear();
                sensorList.AddRange(latestList);

                UpdateSensorPanel(sensorList);
            }
        }
    }
}
