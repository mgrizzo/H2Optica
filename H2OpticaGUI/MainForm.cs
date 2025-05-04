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
    public partial class H2OpticaMain : Form
    {
        private const int PANEL_RADIUS = 13;

        private DBService _dbService;

        public H2OpticaMain()
        {
            InitializeComponent();

            string _dbPath = Path.Combine(Application.StartupPath, "sensor_data.db");
            _dbService = new DBService(_dbPath);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Litraggio giornaliero
            PanelRoundBorder(DailyContainer, PANEL_RADIUS);

            //Pannello temperatura e pH
            PanelRoundBorder(FixedSensors, PANEL_RADIUS);

            //Temperatura
            PanelRoundBorder(tempBarContainer, PANEL_RADIUS);
            PanelRoundBorder(tempBar, PANEL_RADIUS);

            //pH
            PanelRoundBorder(phBarContainer, PANEL_RADIUS);
            PanelRoundBorder(phBar, PANEL_RADIUS);

            //Sensori
            PanelRoundBorder(sensorLayoutPanel, PANEL_RADIUS); //Pannello principale

            AddSensor("Rubinetto cucina", 131.0, null);
            AddSensor("Negro", 69, 31);
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
                AutoSize = true
            };

            Label SensorValue = new Label
            {
                Text = $"Quantità utilizzata: {data}L",
                Font = new Font("Microsoft Sans Serif", 14, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(4, 141, 223),
                Location = new Point(51, 54),
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

            /*
             * TO-DO
             * - Fix dynamic text placement(especially on the x axis)
             * - Code a way to set temp a pH dynamically from Arduino values
             * - ArduinoService class (to do on proj. work time)
             * - Handle events (buttons, other things???)
             * - Graphs
             * - DataGrid for sensors
             * - Details?
             */

            SensorName.Location = new Point((sensorPanel.Size.Width - SensorName.Size.Width) / 2, 20);
            SensorValue.Location = new Point((sensorPanel.Size.Width - SensorValue.Size.Width) / 2, SensorName.Height + 40);
            SensorLimit.Location = new Point((sensorPanel.Size.Width - SensorLimit.Size.Width) / 2, SensorValue.Height + 40);

            sensorPanel.Controls.Add(SensorName);
            sensorPanel.Controls.Add(SensorValue);
            sensorPanel.Controls.Add(SensorLimit);

            sensorLayoutPanel.Controls.Add(sensorPanel);
        }

        private void tempBar_Resize(object sender, EventArgs e)
        {
            PanelRoundBorder(tempBar, PANEL_RADIUS);
        }
    }
}
