namespace H2OpticaGUI
{
    partial class SetLimitForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetLimitForm));
            this.MainLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.labelContainer = new System.Windows.Forms.Panel();
            this.InfoLabel = new System.Windows.Forms.Label();
            this.invalidLimitLabel = new System.Windows.Forms.Label();
            this.SaveButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.limitTextBox = new System.Windows.Forms.TextBox();
            this.removeLimitButton = new System.Windows.Forms.Button();
            this.MainLayoutPanel.SuspendLayout();
            this.labelContainer.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainLayoutPanel
            // 
            this.MainLayoutPanel.ColumnCount = 1;
            this.MainLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.MainLayoutPanel.Controls.Add(this.labelContainer, 0, 0);
            this.MainLayoutPanel.Controls.Add(this.invalidLimitLabel, 0, 2);
            this.MainLayoutPanel.Controls.Add(this.tableLayoutPanel1, 0, 3);
            this.MainLayoutPanel.Controls.Add(this.limitTextBox, 0, 1);
            this.MainLayoutPanel.Location = new System.Drawing.Point(12, 12);
            this.MainLayoutPanel.Name = "MainLayoutPanel";
            this.MainLayoutPanel.RowCount = 4;
            this.MainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 35F));
            this.MainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 23F));
            this.MainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 27F));
            this.MainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.MainLayoutPanel.Size = new System.Drawing.Size(600, 419);
            this.MainLayoutPanel.TabIndex = 0;
            // 
            // labelContainer
            // 
            this.labelContainer.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.labelContainer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(26)))), ((int)(((byte)(44)))));
            this.labelContainer.Controls.Add(this.InfoLabel);
            this.labelContainer.Location = new System.Drawing.Point(8, 6);
            this.labelContainer.Name = "labelContainer";
            this.labelContainer.Size = new System.Drawing.Size(583, 133);
            this.labelContainer.TabIndex = 1;
            // 
            // InfoLabel
            // 
            this.InfoLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.InfoLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(4)))), ((int)(((byte)(141)))), ((int)(((byte)(223)))));
            this.InfoLabel.Location = new System.Drawing.Point(21, 11);
            this.InfoLabel.Name = "InfoLabel";
            this.InfoLabel.Size = new System.Drawing.Size(547, 110);
            this.InfoLabel.TabIndex = 0;
            this.InfoLabel.Text = "Imposta il limite per il sensore: {sensorName}";
            this.InfoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // invalidLimitLabel
            // 
            this.invalidLimitLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.invalidLimitLabel.AutoSize = true;
            this.invalidLimitLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.invalidLimitLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.invalidLimitLabel.Location = new System.Drawing.Point(24, 259);
            this.invalidLimitLabel.Name = "invalidLimitLabel";
            this.invalidLimitLabel.Size = new System.Drawing.Size(552, 78);
            this.invalidLimitLabel.TabIndex = 3;
            this.invalidLimitLabel.Text = "Hai inserito un valore non valido. Per favore, riprova.";
            this.invalidLimitLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.invalidLimitLabel.Visible = false;
            // 
            // SaveButton
            // 
            this.SaveButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.SaveButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(103)))), ((int)(((byte)(181)))), ((int)(((byte)(226)))));
            this.SaveButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.SaveButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SaveButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SaveButton.Location = new System.Drawing.Point(328, 7);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(234, 43);
            this.SaveButton.TabIndex = 4;
            this.SaveButton.Text = "Salva";
            this.SaveButton.UseVisualStyleBackColor = false;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.SaveButton, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.removeLimitButton, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 358);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(594, 58);
            this.tableLayoutPanel1.TabIndex = 5;
            // 
            // limitTextBox
            // 
            this.limitTextBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.limitTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.limitTextBox.Location = new System.Drawing.Point(16, 172);
            this.limitTextBox.Name = "limitTextBox";
            this.limitTextBox.Size = new System.Drawing.Size(567, 44);
            this.limitTextBox.TabIndex = 6;
            // 
            // removeLimitButton
            // 
            this.removeLimitButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.removeLimitButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(103)))), ((int)(((byte)(181)))), ((int)(((byte)(226)))));
            this.removeLimitButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.removeLimitButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.removeLimitButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.removeLimitButton.Location = new System.Drawing.Point(31, 7);
            this.removeLimitButton.Name = "removeLimitButton";
            this.removeLimitButton.Size = new System.Drawing.Size(234, 43);
            this.removeLimitButton.TabIndex = 5;
            this.removeLimitButton.Text = "Rimuovi limite";
            this.removeLimitButton.UseVisualStyleBackColor = false;
            this.removeLimitButton.Click += new System.EventHandler(this.removeLimitButton_Click);
            // 
            // SetLimitForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(38)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(624, 441);
            this.Controls.Add(this.MainLayoutPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SetLimitForm";
            this.ShowInTaskbar = false;
            this.Text = "Impostazione limite";
            this.Load += new System.EventHandler(this.SetLimitForm_Load);
            this.MainLayoutPanel.ResumeLayout(false);
            this.MainLayoutPanel.PerformLayout();
            this.labelContainer.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel MainLayoutPanel;
        private System.Windows.Forms.Label InfoLabel;
        private System.Windows.Forms.Panel labelContainer;
        private System.Windows.Forms.Label invalidLimitLabel;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox limitTextBox;
        private System.Windows.Forms.Button removeLimitButton;
    }
}