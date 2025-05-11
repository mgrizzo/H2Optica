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
using System.Windows.Forms.DataVisualization;
using System.Windows.Forms.DataVisualization.Charting;
using H2OpticaLogic;

namespace H2OpticaGUI
{
   /*
    * TO-DO
    */

    public partial class H2OpticaMain : Form
    {
        private const int PANEL_RADIUS = 13;
        private const int BAR_RADIUS = 7;

        private const int DB_UPDATE_RATE = 12;

        private const string Hostname = "H2OpticaESP32";

        internal DBService dbService;
        private ArduinoService arduinoService;

        private List<Sensor> sensorList = new List<Sensor>();
        private Dictionary<int, Label> sensorValueLabels = new Dictionary<int, Label>();

        private DateTime lastUpdatedDate;
        private DateTime lastDBUpdateTime;
        private double totalVolume = 0.0;

        private DataCollection latestDBCollection;
        private DataCollection latestClientData = new DataCollection();
        private DataCollection latestArduinoData = new DataCollection();
        private FlowCollection flowCollection = new FlowCollection();
        private WaterStats waterStats = new WaterStats();

        private Timer updateTimer;
        private Timer arduinoRequest;
        private Timer dbRequest;

        public H2OpticaMain()
        {
            InitializeComponent();
            InitializeTimer();

            LoadGraph(sensorList);

            string _dbPath = Path.Combine(Application.StartupPath, "sensor_data.db");
            dbService = new DBService(_dbPath);

            dbService.InitializeDB();

            arduinoService = new ArduinoService(Hostname);
        }

        private void LoadGraph(List<Sensor> sensList)
        {
            VolumeChart.Series.Clear();

            foreach (Sensor sensor in sensList)
            {
                Dictionary<DateTime, double> data = dbService.GetChartData(sensor.SensorID, DateTime.Today.AddDays(-1));

                Series flowSeries = new Series
                {
                    Name = sensor.SensorName,
                    ChartType = SeriesChartType.Line,
                    XValueType = ChartValueType.DateTime,
                    YValueType = ChartValueType.Double,
                    BorderWidth = 2
                };

                foreach(var entry in data)
                {
                    flowSeries.Points.AddXY(entry.Key, entry.Value);
                }

                VolumeChart.Series.Add(flowSeries);
            }

            //Configurazione asse x
            VolumeChart.ChartAreas[0].AxisX.LabelStyle.Format = "HH:mm";
            VolumeChart.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.Minutes;
            VolumeChart.ChartAreas[0].AxisX.Interval = 10;

            //Configurazione asse y
            VolumeChart.ChartAreas[0].AxisY.Minimum = 0;
            VolumeChart.ChartAreas[0].AxisY.Title = "Flusso [L]";
        }

        private void InitializeTimer()
        {
            updateTimer = new Timer();
            updateTimer.Interval = 3000;
            updateTimer.Tick += new EventHandler(updateTimer_Tick);
            updateTimer.Start();

            arduinoRequest = new Timer();
            arduinoRequest.Interval = 2000;
            arduinoRequest.Tick += new EventHandler(arduinoRequest_Tick);
            arduinoRequest.Start();

            dbRequest = new Timer();
            dbRequest.Interval = 10000;
            dbRequest.Tick += new EventHandler(dbRequest_Tick);
            dbRequest.Start();
        }

        private void HandleNewDay()
        {
            latestClientData.Flows.ResetDailyData();

            totalVolume = 0.0;
            totalVolLabel.Text = $"Oggi hai utilizzato: {totalVolume}L";
        }

        private void UpdateUI()
        {
            if (latestArduinoData == null)
                return;

            //Somma del nuovo volume al precedente
            foreach(KeyValuePair<int, double> kvp in latestArduinoData.Flows.Flux)
            {
                if (flowCollection.Flux.ContainsKey(kvp.Key))
                    flowCollection.Flux[kvp.Key] += kvp.Value;
                else
                    flowCollection.Flux[kvp.Key] = kvp.Value;
            }

            //Update dei dati
            waterStats.Temp = latestArduinoData.Stats.Temp ?? waterStats.Temp;
            waterStats.pH = latestArduinoData.Stats.pH ?? waterStats.pH;

            totalVolume = flowCollection.CalculateTotalVolume();

            latestClientData.Stats = waterStats;
            latestClientData.Flows = flowCollection;

            //Update delle barre
            UpdateTempBar((double)waterStats.Temp);
            UpdatephBar((double)waterStats.pH);

            //Update dei label
            totalVolLabel.Text = $"Oggi hai usato {totalVolume}L";
            tempLabel.Text = $"Temperatura: {waterStats.Temp}°C";
            phLabel.Text = $"pH: {waterStats.pH}";

            //Update pannelli sensori di flusso
            foreach (KeyValuePair<int, Label> kvp in sensorValueLabels)
            {
                double newFlowValue = flowCollection.Flux[kvp.Key];

                kvp.Value.Text = $"Quantità utilizzata: {newFlowValue}L";

                //Controllo superamento del limite
                double? currentSensorLimit = sensorList[kvp.Key].SensorLimit;

                if (currentSensorLimit.HasValue && newFlowValue > currentSensorLimit)
                    kvp.Value.ForeColor = Color.FromArgb(245, 64, 64);
                else
                    kvp.Value.ForeColor = Color.FromArgb(4, 141, 223);
            }

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
                AddSensor(sensor, (double)dbService.GetLatestVolume(sensor.SensorID));
            }
        }
        private void AddSensor(Sensor mySensor, double data)
        {
            Panel sensorPanel = new Panel
            {
                Width = 292,
                Height = 161,
                BackColor = Color.FromArgb(9, 14, 23),
                Margin = new Padding(24)
            };
            sensorPanel.Tag = mySensor.SensorID;

            UIHelper.PanelRoundBorder(sensorPanel, PANEL_RADIUS);

            /*Label SensorName = new Label
            {
                Text = mySensor.SensorName,
                Font = new Font("Microsoft Sans Serif", 22, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(4, 141, 223),
                Location = new Point(0, 0),
                AutoSize = true
            };*/

            Label SensorName = UIHelper.CreateAutoScalingLabel(
                mySensor.SensorName,
                new Size(sensorPanel.Width - 20, 40), //Margine laterale di 10px
                new Font("Microsoft Sans Serif", 22, FontStyle.Bold)
                );
            SensorName.Location = new Point((sensorPanel.Width - SensorName.Width) / 2, 20);

            SensorName.Cursor = Cursors.Hand;
            SensorName.Click += SensorName_Click;

            /*Label SensorValue = new Label
            {
                Text = $"Quantità utilizzata: {data}L",
                Font = new Font("Microsoft Sans Serif", 14, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(4, 141, 223),
                Location = new Point(0, 0),
                AutoSize = true,
            };*/
            Label SensorValue = UIHelper.CreateAutoScalingLabel(
                $"Quantità utilizzata: {data}L",
                new Size(sensorPanel.Width - 20, 30),
                new Font("Microsoft Sans Serif", 14, FontStyle.Bold)
                );
            SensorValue.Location = new Point((sensorPanel.Width - SensorValue.Width) / 2, SensorName.Bottom + 17);

            /*Label SensorLimit = new Label
            {
                Text = mySensor.SensorLimit.HasValue ? $"Limite impostato: {mySensor.SensorLimit}L" : "Limite impostato: --",
                Font = new Font("Microsoft Sans Serif", 14, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(4, 141, 223),
                AutoSize = true,
            };*/
            Label SensorLimit = UIHelper.CreateAutoScalingLabel(
                mySensor.SensorLimit.HasValue ? $"Limite impostato: {mySensor.SensorLimit}L" : "Limite impostato: --",
                new Size(sensorPanel.Width - 20, 30),
                new Font("Microsoft Sans Serif", 14, FontStyle.Bold)
                );
            SensorLimit.Location = new Point((sensorPanel.Width - SensorLimit.Width) / 2, SensorValue.Bottom + 20);

            SensorLimit.Cursor = Cursors.Hand;
            SensorLimit.Click += SensorLimit_Click;

            sensorPanel.Controls.Add(SensorName);
            sensorPanel.Controls.Add(SensorValue);
            sensorPanel.Controls.Add(SensorLimit);

            sensorValueLabels[mySensor.SensorID] = SensorValue;

            sensorLayoutPanel.Controls.Add(sensorPanel);
        }

        //EVENTI
        private void Form1_Load(object sender, EventArgs e)
        {
            lastUpdatedDate = DateTime.Today;
            lastDBUpdateTime = DateTime.Now;

            dbService.CheckDB();

            //Volume giornaliero
            UIHelper.PanelRoundBorder(DailyContainer, PANEL_RADIUS);

            //Grafico
            UIHelper.PanelRoundBorder(ChartPanel, PANEL_RADIUS);

            //Pannello temperatura e pH
            UIHelper.PanelRoundBorder(FixedSensors, PANEL_RADIUS);

            //Temperatura
            UIHelper.PanelRoundBorder(tempBarContainer, BAR_RADIUS);
            UIHelper.PanelRoundBorder(tempBar, BAR_RADIUS);

            //pH
            UIHelper.PanelRoundBorder(phBarContainer, BAR_RADIUS);
            UIHelper.PanelRoundBorder(phBar, BAR_RADIUS);

            //Sensori
            UIHelper.PanelRoundBorder(sensorLayoutPanel, PANEL_RADIUS); //Pannello principale

            latestDBCollection = dbService.GetLatestWaterData();
            sensorList = dbService.GetSensorList();

            UpdateSensorPanel(sensorList);

            Sensor testSens = new Sensor(1, "Idiota", null);
            Sensor testSens2 = new Sensor(2, "Cazzone negro inculato");

            AddSensor(testSens, 101.3);
            AddSensor(testSens2, 10.1);
        }

        private void updateTimer_Tick(object sender, EventArgs e)
        {
            //Controllo del giorno
            if(DateTime.Today != lastUpdatedDate)
            {
                lastUpdatedDate = DateTime.Today;
                HandleNewDay();
            }

            UpdateUI();

            //Update del DB
            if ((DateTime.Now - lastDBUpdateTime).TotalSeconds > DB_UPDATE_RATE && !latestDBCollection.Equals(latestArduinoData))
            {
                try
                {
                    dbService.InsertAllData(latestArduinoData);
                    latestDBCollection = latestArduinoData;
                    lastDBUpdateTime = DateTime.Now;
                }
                catch (Exception ex)
                {
                    Logger.Log($"Scheduled DB update went wrong: {ex.Message}");
                }
            }
        }
        private void arduinoRequest_Tick(object sender, EventArgs e)
        {
            try
            {
                latestArduinoData = arduinoService.GetLatestReadingAsync().Result;
            }
            catch (Exception ex)
            {
                Logger.Log($"Couldn't fetch latest Arduino Data: {ex.Message}");
            }
        }
        private void dbRequest_Tick(object sender, EventArgs e)
        {
            try
            {
                sensorList = dbService.GetSensorList();
                latestDBCollection = dbService.GetLatestWaterData();
            }
            catch (Exception ex)
            {
                Logger.Log($"Couldn't fetch latest DB data: {ex.Message}");
            }
        }

        //Resize barre
        private void tempBar_Resize(object sender, EventArgs e)
        {
            UIHelper.PanelRoundBorder(tempBar, PANEL_RADIUS);
        }
        private void phBar_Resize(object sender, EventArgs e)
        {
            UIHelper.PanelRoundBorder(phBar, PANEL_RADIUS);
        }

        //Text change dei label
        private void totalVolLabel_TextChanged(object sender, EventArgs e)
        {
            totalVolLabel.Location = new Point((DailyContainer.Width - totalVolLabel.Width) / 2, 6);
        }
        private void tempLabel_TextChanged(object sender, EventArgs e)
        {
            tempLabel.Location = new Point((DailyContainer.Width - tempLabel.Width) / 2, 42);
        }
        private void phLabel_TextChanged(object sender, EventArgs e)
        {
            phLabel.Location = new Point((DailyContainer.Width - phLabel.Width) / 2, 195);
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

        //Evento click LimitLabel
        private void SensorLimit_Click(object sender, EventArgs e)
        {
            Label clickedLabel = sender as Label;
            Panel parentPanel = clickedLabel.Parent as Panel;

            if (parentPanel == null) 
                return;

            int sensorID = (int)parentPanel.Tag; //Prende il tag dal pannello

            Sensor sensore = sensorList.FirstOrDefault(sen => sen.SensorID == sensorID);

            if (sensore != null)
            {
                SetLimitForm setLimitForm = new SetLimitForm(dbService, sensore);

                if(setLimitForm.ShowDialog() == DialogResult.OK)
                {
                    if (sensore.SensorLimit == null)
                        clickedLabel.Text = "Limite impostato: --";
                    else
                        clickedLabel.Text = $"Limite impostato: {sensore.SensorLimit:F1}L";  
                }
            }
        }

        //Evento click NameLabel
        private void SensorName_Click(object sender, EventArgs e)
        {
            Label clickedLabel = sender as Label;
            Panel parentPanel = clickedLabel.Parent as Panel;

            if (parentPanel == null)
                return;

            int sensorID = (int)parentPanel.Tag;

            Sensor sensore = sensorList.FirstOrDefault(sen => sen.SensorID == sensorID);

            if(sensore != null)
            {
                ChangeNameForm changeNameForm = new ChangeNameForm(dbService, sensore);

                if(changeNameForm.ShowDialog() == DialogResult.OK)
                {
                    if (changeNameForm.NewName == null)
                        return;

                    clickedLabel.Text = changeNameForm.NewName;

                    //Riposiziono il label
                    clickedLabel.AutoSize = false;
                }
            }
        }
    }

    public static class UIHelper
    {
        public static void PanelRoundBorder(Panel panel, int radius)
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
        public static Label CreateAutoScalingLabel(string text, Size maxSize, Font baseFont)
        {
            //Creazione base label
            Label label = new Label
            {
                Text = text,
                Size = maxSize,
                AutoSize = false,
                AutoEllipsis = false,
                TextAlign = ContentAlignment.MiddleCenter,
                UseCompatibleTextRendering = true,
                ForeColor = Color.FromArgb(4, 141, 223)
            };

            //Creazione font di test
            float fontSize = baseFont.Size;
            Font testFont = new Font(baseFont.FontFamily, fontSize, baseFont.Style);

            //Contesto grafico per misurare lo spazio occupato dal testo con il font
            using(Graphics g = Graphics.FromHwnd(IntPtr.Zero))
            {
                SizeF textSize = g.MeasureString(text, testFont);

                //Se il testo non entra riduce gradualmente finché non entra o non è troppo piccolo
                while((textSize.Width > maxSize.Width || textSize.Height > maxSize.Height) && fontSize > 6)
                {
                    fontSize -= 0.5f;
                    testFont = new Font(baseFont.FontFamily, fontSize, baseFont.Style);
                    textSize = g.MeasureString(text, testFont);
                }
            }

            //Assegnazione al label una volta trovata la dimensione giusta
            label.Font = new Font(baseFont.FontFamily, fontSize, baseFont.Style);

            return label;
        }
    }
}
