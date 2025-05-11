using H2OpticaLogic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace H2OpticaGUI
{
    public partial class ChangeNameForm : Form
    {
        private const int PANEL_RADIUS = 13;

        private DBService dbService;
        private Sensor Sensore;

        internal string NewName = null;

        public ChangeNameForm(DBService _dbService, Sensor sensore)
        {
            InitializeComponent();

            Sensore = sensore; //Passa per riferimento all'oggetto, quindi sarà modificato 'sensore'

            this.dbService = _dbService;
        }

        private void ChangeNameForm_Load(object sender, EventArgs e)
        {
            saveButton.Enabled = false;

            InfoLabel.Text = $"Cambia il nome per il sensore: {Sensore.SensorName}";
            InfoLabel.AutoSize = false;
            InfoLabel.Dock = DockStyle.Fill;
            InfoLabel.MaximumSize = new Size(labelContainer.Width - 20, 0);
            InfoLabel.TextAlign = ContentAlignment.MiddleCenter; ;

            invalidNameLabel.Visible = false;
            UIHelper.PanelRoundBorder(labelContainer, PANEL_RADIUS);

            if (Sensore != null)
                nameTextBox.Text = Sensore.SensorName;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if(!string.IsNullOrWhiteSpace(nameTextBox.Text))
            {
                NewName = nameTextBox.Text;

                //Aggiornamento del DB
                dbService.ChangeSensName(Sensore.SensorID, nameTextBox.Text);
                //Aggiornamento lato client
                Sensore.SensorName = nameTextBox.Text;

                MessageBox.Show("Nome aggiornato.");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                invalidNameLabel.Visible = true;
            }
        }

        private void nameTextBox_TextChanged(object sender, EventArgs e)
        {
            saveButton.Enabled = !string.IsNullOrWhiteSpace(nameTextBox.Text);
        }
    }
}
