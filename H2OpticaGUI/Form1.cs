using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using H2OpticaLogic;

namespace H2OpticaGUI
{
    public partial class H2Optica : Form
    {
        private DBService _dbService;

        public H2Optica()
        {
            InitializeComponent();

            string _dbPath = Path.Combine(Application.StartupPath, "sensor_data.db");
            _dbService = new DBService(_dbPath);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
