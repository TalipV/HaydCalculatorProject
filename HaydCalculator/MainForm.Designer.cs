namespace HaydCalculator
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.istihadaListBox = new System.Windows.Forms.ListBox();
            this.menstruationListBox = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.inputListBox = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.clearDataButton = new System.Windows.Forms.Button();
            this.acceptInputButton = new System.Windows.Forms.Button();
            this.fromDateDateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.toDateDateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.haydTypeComboBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // istihadaListBox
            // 
            this.istihadaListBox.FormattingEnabled = true;
            this.istihadaListBox.ItemHeight = 20;
            this.istihadaListBox.Location = new System.Drawing.Point(440, 334);
            this.istihadaListBox.Name = "istihadaListBox";
            this.istihadaListBox.Size = new System.Drawing.Size(348, 104);
            this.istihadaListBox.TabIndex = 1;
            // 
            // menstruationListBox
            // 
            this.menstruationListBox.FormattingEnabled = true;
            this.menstruationListBox.ItemHeight = 20;
            this.menstruationListBox.Location = new System.Drawing.Point(12, 334);
            this.menstruationListBox.Name = "menstruationListBox";
            this.menstruationListBox.Size = new System.Drawing.Size(348, 104);
            this.menstruationListBox.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(440, 311);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 20);
            this.label1.TabIndex = 3;
            this.label1.Text = "Istihada";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 311);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(96, 20);
            this.label2.TabIndex = 4;
            this.label2.Text = "Menstruation";
            // 
            // inputListBox
            // 
            this.inputListBox.FormattingEnabled = true;
            this.inputListBox.ItemHeight = 20;
            this.inputListBox.Location = new System.Drawing.Point(230, 152);
            this.inputListBox.Name = "inputListBox";
            this.inputListBox.Size = new System.Drawing.Size(348, 144);
            this.inputListBox.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(230, 129);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 20);
            this.label3.TabIndex = 6;
            this.label3.Text = "Input";
            // 
            // clearDataButton
            // 
            this.clearDataButton.Location = new System.Drawing.Point(652, 189);
            this.clearDataButton.Name = "clearDataButton";
            this.clearDataButton.Size = new System.Drawing.Size(94, 29);
            this.clearDataButton.TabIndex = 8;
            this.clearDataButton.Text = "Clear";
            this.clearDataButton.UseVisualStyleBackColor = true;
            this.clearDataButton.Click += new System.EventHandler(this.clearDataButton_Click);
            // 
            // acceptInputButton
            // 
            this.acceptInputButton.Location = new System.Drawing.Point(230, 78);
            this.acceptInputButton.Name = "acceptInputButton";
            this.acceptInputButton.Size = new System.Drawing.Size(94, 29);
            this.acceptInputButton.TabIndex = 9;
            this.acceptInputButton.Text = "Accept";
            this.acceptInputButton.UseVisualStyleBackColor = true;
            this.acceptInputButton.Click += new System.EventHandler(this.acceptInputButton_Click);
            // 
            // fromDateDateTimePicker
            // 
            this.fromDateDateTimePicker.CustomFormat = "dd.MM.yyyy, HH:mm:ss";
            this.fromDateDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.fromDateDateTimePicker.Location = new System.Drawing.Point(232, 12);
            this.fromDateDateTimePicker.Name = "fromDateDateTimePicker";
            this.fromDateDateTimePicker.Size = new System.Drawing.Size(200, 27);
            this.fromDateDateTimePicker.TabIndex = 11;
            // 
            // toDateDateTimePicker
            // 
            this.toDateDateTimePicker.CustomFormat = "dd.MM.yyyy, HH:mm:ss";
            this.toDateDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.toDateDateTimePicker.Location = new System.Drawing.Point(232, 45);
            this.toDateDateTimePicker.Name = "toDateDateTimePicker";
            this.toDateDateTimePicker.Size = new System.Drawing.Size(200, 27);
            this.toDateDateTimePicker.TabIndex = 12;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(183, 17);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 20);
            this.label4.TabIndex = 13;
            this.label4.Text = "From";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(201, 47);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(25, 20);
            this.label5.TabIndex = 14;
            this.label5.Text = "To";
            // 
            // haydTypeComboBox
            // 
            this.haydTypeComboBox.FormattingEnabled = true;
            this.haydTypeComboBox.Location = new System.Drawing.Point(440, 9);
            this.haydTypeComboBox.Name = "haydTypeComboBox";
            this.haydTypeComboBox.Size = new System.Drawing.Size(151, 28);
            this.haydTypeComboBox.TabIndex = 15;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.haydTypeComboBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.toDateDateTimePicker);
            this.Controls.Add(this.fromDateDateTimePicker);
            this.Controls.Add(this.acceptInputButton);
            this.Controls.Add(this.clearDataButton);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.inputListBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.menstruationListBox);
            this.Controls.Add(this.istihadaListBox);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ListBox istihadaListBox;
        private ListBox menstruationListBox;
        private Label label1;
        private Label label2;
        private ListBox inputListBox;
        private Label label3;
        private Button clearDataButton;
        private Button acceptInputButton;
        private DateTimePicker fromDateDateTimePicker;
        private DateTimePicker toDateDateTimePicker;
        private Label label4;
        private Label label5;
        private ComboBox haydTypeComboBox;
    }
}