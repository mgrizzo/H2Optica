namespace H2OpticaGUI
{
    partial class H2OpticaMain
    {
        /// <summary>
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Pulire le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione Windows Form

        /// <summary>
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(H2OpticaMain));
            this.MainLayout = new System.Windows.Forms.TableLayoutPanel();
            this.FlowAndLogo = new System.Windows.Forms.TableLayoutPanel();
            this.logoBox = new System.Windows.Forms.PictureBox();
            this.updateSensorCount = new System.Windows.Forms.Button();
            this.sensorLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.FixedAndGraph = new System.Windows.Forms.TableLayoutPanel();
            this.FixedSensors = new System.Windows.Forms.TableLayoutPanel();
            this.tempLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.phLabel = new System.Windows.Forms.Label();
            this.tempBarContainer = new System.Windows.Forms.Panel();
            this.tempBar = new System.Windows.Forms.Panel();
            this.DailyContainer = new System.Windows.Forms.Panel();
            this.phBar = new System.Windows.Forms.Panel();
            this.phBarContainer = new System.Windows.Forms.Panel();
            this.MainLayout.SuspendLayout();
            this.FlowAndLogo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logoBox)).BeginInit();
            this.FixedAndGraph.SuspendLayout();
            this.FixedSensors.SuspendLayout();
            this.tempBarContainer.SuspendLayout();
            this.DailyContainer.SuspendLayout();
            this.phBarContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainLayout
            // 
            resources.ApplyResources(this.MainLayout, "MainLayout");
            this.MainLayout.Controls.Add(this.FlowAndLogo, 0, 0);
            this.MainLayout.Controls.Add(this.FixedAndGraph, 1, 0);
            this.MainLayout.Name = "MainLayout";
            // 
            // FlowAndLogo
            // 
            resources.ApplyResources(this.FlowAndLogo, "FlowAndLogo");
            this.FlowAndLogo.Controls.Add(this.logoBox, 0, 0);
            this.FlowAndLogo.Controls.Add(this.updateSensorCount, 0, 2);
            this.FlowAndLogo.Controls.Add(this.sensorLayoutPanel, 0, 1);
            this.FlowAndLogo.Name = "FlowAndLogo";
            // 
            // logoBox
            // 
            resources.ApplyResources(this.logoBox, "logoBox");
            this.logoBox.Name = "logoBox";
            this.logoBox.TabStop = false;
            // 
            // updateSensorCount
            // 
            resources.ApplyResources(this.updateSensorCount, "updateSensorCount");
            this.updateSensorCount.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(103)))), ((int)(((byte)(181)))), ((int)(((byte)(226)))));
            this.updateSensorCount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(7)))), ((int)(((byte)(7)))), ((int)(((byte)(27)))));
            this.updateSensorCount.Name = "updateSensorCount";
            this.updateSensorCount.UseVisualStyleBackColor = false;
            // 
            // sensorLayoutPanel
            // 
            resources.ApplyResources(this.sensorLayoutPanel, "sensorLayoutPanel");
            this.sensorLayoutPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(26)))), ((int)(((byte)(44)))));
            this.sensorLayoutPanel.Name = "sensorLayoutPanel";
            // 
            // FixedAndGraph
            // 
            resources.ApplyResources(this.FixedAndGraph, "FixedAndGraph");
            this.FixedAndGraph.Controls.Add(this.FixedSensors, 0, 1);
            this.FixedAndGraph.Controls.Add(this.DailyContainer, 0, 0);
            this.FixedAndGraph.Name = "FixedAndGraph";
            // 
            // FixedSensors
            // 
            resources.ApplyResources(this.FixedSensors, "FixedSensors");
            this.FixedSensors.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(26)))), ((int)(((byte)(44)))));
            this.FixedSensors.Controls.Add(this.phBarContainer, 1, 1);
            this.FixedSensors.Controls.Add(this.tempLabel, 0, 0);
            this.FixedSensors.Controls.Add(this.phLabel, 0, 1);
            this.FixedSensors.Controls.Add(this.tempBarContainer, 1, 0);
            this.FixedSensors.Name = "FixedSensors";
            // 
            // tempLabel
            // 
            resources.ApplyResources(this.tempLabel, "tempLabel");
            this.tempLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(4)))), ((int)(((byte)(141)))), ((int)(((byte)(223)))));
            this.tempLabel.Name = "tempLabel";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(4)))), ((int)(((byte)(141)))), ((int)(((byte)(223)))));
            this.label2.Name = "label2";
            // 
            // phLabel
            // 
            resources.ApplyResources(this.phLabel, "phLabel");
            this.phLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(4)))), ((int)(((byte)(141)))), ((int)(((byte)(223)))));
            this.phLabel.Name = "phLabel";
            // 
            // tempBarContainer
            // 
            resources.ApplyResources(this.tempBarContainer, "tempBarContainer");
            this.tempBarContainer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(103)))), ((int)(((byte)(181)))), ((int)(((byte)(226)))));
            this.tempBarContainer.Controls.Add(this.tempBar);
            this.tempBarContainer.Name = "tempBarContainer";
            // 
            // tempBar
            // 
            resources.ApplyResources(this.tempBar, "tempBar");
            this.tempBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.tempBar.Name = "tempBar";
            this.tempBar.Resize += new System.EventHandler(this.tempBar_Resize);
            // 
            // DailyContainer
            // 
            resources.ApplyResources(this.DailyContainer, "DailyContainer");
            this.DailyContainer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(26)))), ((int)(((byte)(44)))));
            this.DailyContainer.Controls.Add(this.label2);
            this.DailyContainer.Name = "DailyContainer";
            // 
            // phBar
            // 
            resources.ApplyResources(this.phBar, "phBar");
            this.phBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(106)))), ((int)(((byte)(67)))), ((int)(((byte)(247)))));
            this.phBar.Name = "phBar";
            // 
            // phBarContainer
            // 
            resources.ApplyResources(this.phBarContainer, "phBarContainer");
            this.phBarContainer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(103)))), ((int)(((byte)(181)))), ((int)(((byte)(226)))));
            this.phBarContainer.Controls.Add(this.phBar);
            this.phBarContainer.Name = "phBarContainer";
            // 
            // H2OpticaMain
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(38)))), ((int)(((byte)(64)))));
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.MainLayout);
            this.Name = "H2OpticaMain";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.MainLayout.ResumeLayout(false);
            this.FlowAndLogo.ResumeLayout(false);
            this.FlowAndLogo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logoBox)).EndInit();
            this.FixedAndGraph.ResumeLayout(false);
            this.FixedSensors.ResumeLayout(false);
            this.FixedSensors.PerformLayout();
            this.tempBarContainer.ResumeLayout(false);
            this.DailyContainer.ResumeLayout(false);
            this.DailyContainer.PerformLayout();
            this.phBarContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel MainLayout;
        private System.Windows.Forms.TableLayoutPanel FlowAndLogo;
        private System.Windows.Forms.PictureBox logoBox;
        private System.Windows.Forms.Button updateSensorCount;
        private System.Windows.Forms.FlowLayoutPanel sensorLayoutPanel;
        private System.Windows.Forms.TableLayoutPanel FixedAndGraph;
        private System.Windows.Forms.TableLayoutPanel FixedSensors;
        private System.Windows.Forms.Label tempLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label phLabel;
        private System.Windows.Forms.Panel tempBarContainer;
        private System.Windows.Forms.Panel tempBar;
        private System.Windows.Forms.Panel DailyContainer;
        private System.Windows.Forms.Panel phBarContainer;
        private System.Windows.Forms.Panel phBar;
    }
}

