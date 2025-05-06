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
    * - Handle events (buttons, other things???)
    * - Graphs
    * - DataGrid for sensors
    * - Details?
    */

    public partial class H2OpticaMain : Form
    {
        private const int PANEL_RADIUS = 13;
        private const int BAR_RADIUS = 7;

        private DBService dbService;

        private List<Sensor> sensorList;

        public H2OpticaMain()
        {
            InitializeComponent();

            string _dbPath = Path.Combine(Application.StartupPath, "sensor_data.db");
            dbService = new DBService(_dbPath);

            int dbStatus = dbService.CheckDB();

            if (dbStatus == -1)
                dbService.InitializeDB(); //Da sistemare (viva la corruzione!!!)

            sensorList = dbService.GetSensorList();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
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

            foreach(Sensor sensor in sensorList)
            {
                AddSensor(sensor.SensorName, dbService.GetLatestVolume(sensor.SensorID), sensor.SensorLimit);
            }

            /*
            AddSensor("Negro", 91, null);
            AddSensor("Negro2", 10, 40);
            AddSensor("Rubinetto cucina", 104, 69);
            AddSensor("Tua mamma", 21, null);
            */
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

        }
    }
}
