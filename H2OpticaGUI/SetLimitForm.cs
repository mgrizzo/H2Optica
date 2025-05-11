using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using H2OpticaLogic;

namespace H2OpticaGUI
{
    public partial class SetLimitForm : Form
    {
        private const int PANEL_RADIUS = 13;

        private Sensor Sensore;
        private double newLimit;
        private DBService dbService;

        public SetLimitForm(DBService _dbService, Sensor sensore)
        {
            InitializeComponent();

            Sensore = sensore; //Passa per riferimento all'oggetto, quindi sarà modificato 'sensore'

            this.dbService = _dbService;
        }

        private void SetLimitForm_Load(object sender, EventArgs e)
        {
            //Update di testo e posizione del label
            InfoLabel.Text = $"Imposta il limite per il sensore: {Sensore.SensorName}";
            InfoLabel.AutoSize = false;
            InfoLabel.Dock = DockStyle.Fill;
            InfoLabel.MaximumSize = new Size(labelContainer.Width - 20, 0);
            InfoLabel.TextAlign = ContentAlignment.MiddleCenter;;

            invalidLimitLabel.Visible = false;
            UIHelper.PanelRoundBorder(labelContainer, PANEL_RADIUS);
            Sensore.SensorLimit = dbService.GetSensorLimit(Sensore.SensorID);
            
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if(double.TryParse(limitTextBox.Text, out newLimit))
            {
                //Aggiorna il DB
                dbService.SetSensorLimit(Sensore.SensorID, newLimit);
                //Aggiorna il limite lato client
                Sensore.SensorLimit = newLimit;

                MessageBox.Show("Limite aggiornato.");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                invalidLimitLabel.Visible = true;
            }
        }

        private void removeLimitButton_Click(object sender, EventArgs e)
        {
            //Aggiornamento del DB
            dbService.SetSensorLimit(Sensore.SensorID, null);
            //Aggiornamento limite lato client
            Sensore.SensorLimit = null;

            MessageBox.Show("Limite rimosso.");
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
