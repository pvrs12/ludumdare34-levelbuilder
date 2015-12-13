namespace LevelBuilder
{
    partial class Dialog
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
            this.propertiesCheckBox = new System.Windows.Forms.CheckedListBox();
            this.SuspendLayout();
            // 
            // propertiesCheckBox
            // 
            this.propertiesCheckBox.CheckOnClick = true;
            this.propertiesCheckBox.FormattingEnabled = true;
            this.propertiesCheckBox.Items.AddRange(new object[] {
            "North Wall",
            "East Wall",
            "South Wall",
            "West Wall",
            "Occupied",
            "Winning"});
            this.propertiesCheckBox.Location = new System.Drawing.Point(12, 12);
            this.propertiesCheckBox.Name = "propertiesCheckBox";
            this.propertiesCheckBox.Size = new System.Drawing.Size(125, 94);
            this.propertiesCheckBox.TabIndex = 1;
            // 
            // Dialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(149, 125);
            this.Controls.Add(this.propertiesCheckBox);
            this.Name = "Dialog";
            this.Text = "Dialog";
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.CheckedListBox propertiesCheckBox;
    }
}