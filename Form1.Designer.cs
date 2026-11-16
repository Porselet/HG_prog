namespace HG_prog
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Button btnOn;
        private System.Windows.Forms.Button btnOff;
        private System.Windows.Forms.TextBox textBoxSetpoint;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Label labelPress;
        private System.Windows.Forms.Label labelFlow;
        private System.Windows.Forms.Label labelVolt;
        private System.Windows.Forms.Label labelAmp;
        private System.Windows.Forms.Label labelTemp;
        private System.Windows.Forms.Label lblPressTitle;
        private System.Windows.Forms.Label lblFlowTitle;
        private System.Windows.Forms.Label lblVoltTitle;
        private System.Windows.Forms.Label lblAmpTitle;
        private System.Windows.Forms.Label lblTempTitle;
        private System.Windows.Forms.Label lblSetpointTitle;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) { components.Dispose(); }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.btnOn = new System.Windows.Forms.Button();
            this.btnOff = new System.Windows.Forms.Button();
            this.textBoxSetpoint = new System.Windows.Forms.TextBox();
            this.btnApply = new System.Windows.Forms.Button();
            this.labelPress = new System.Windows.Forms.Label();
            this.labelFlow = new System.Windows.Forms.Label();
            this.labelVolt = new System.Windows.Forms.Label();
            this.labelAmp = new System.Windows.Forms.Label();
            this.labelTemp = new System.Windows.Forms.Label();
            this.lblPressTitle = new System.Windows.Forms.Label();
            this.lblFlowTitle = new System.Windows.Forms.Label();
            this.lblVoltTitle = new System.Windows.Forms.Label();
            this.lblAmpTitle = new System.Windows.Forms.Label();
            this.lblTempTitle = new System.Windows.Forms.Label();
            this.lblSetpointTitle = new System.Windows.Forms.Label();
            this.SuspendLayout();

            // ON
            this.btnOn.Location = new System.Drawing.Point(12, 12);
            this.btnOn.Size = new System.Drawing.Size(90, 32);
            this.btnOn.Text = "ON";
            this.btnOn.Click += new System.EventHandler(this.btnOn_Click);

            // OFF
            this.btnOff.Location = new System.Drawing.Point(110, 12);
            this.btnOff.Size = new System.Drawing.Size(90, 32);
            this.btnOff.Text = "OFF";
            this.btnOff.Click += new System.EventHandler(this.btnOff_Click);

            // Подпись поля
            this.lblSetpointTitle.Location = new System.Drawing.Point(12, 54);
            this.lblSetpointTitle.Size = new System.Drawing.Size(160, 18);
            this.lblSetpointTitle.Text = "Желаемый расход, л/ч";
            this.lblSetpointTitle.Font = new System.Drawing.Font("Segoe UI", 8F);

            // Поле ввода
            this.textBoxSetpoint.Location = new System.Drawing.Point(12, 74);
            this.textBoxSetpoint.Size = new System.Drawing.Size(90, 22);
            this.textBoxSetpoint.Text = "300";

            // Применить
            this.btnApply.Location = new System.Drawing.Point(110, 72);
            this.btnApply.Size = new System.Drawing.Size(90, 26);
            this.btnApply.Text = "Применить";
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);

            // Заголовки и значения
            int y = 112;
            System.Drawing.Font titleFont = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            System.Drawing.Font valueFont = new System.Drawing.Font("Segoe UI", 9F);

            this.lblPressTitle = MakeTitle("Давление", 12, y); y += 22;
            this.labelPress = MakeValue("0", 160, y - 22);
            this.lblFlowTitle = MakeTitle("Расход", 12, y); y += 22;
            this.labelFlow = MakeValue("0", 160, y - 22);
            this.lblVoltTitle = MakeTitle("Напряжение", 12, y); y += 22;
            this.labelVolt = MakeValue("0", 160, y - 22);
            this.lblAmpTitle = MakeTitle("Ток", 12, y); y += 22;
            this.labelAmp = MakeValue("0", 160, y - 22);
            this.lblTempTitle = MakeTitle("Температура", 12, y); y += 22;
            this.labelTemp = MakeValue("0", 160, y - 22);

            // Форма
            this.ClientSize = new System.Drawing.Size(220, y + 10);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "HG_prog";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);

            this.Controls.Add(this.btnOn);
            this.Controls.Add(this.btnOff);
            this.Controls.Add(this.lblSetpointTitle);
            this.Controls.Add(this.textBoxSetpoint);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.lblPressTitle);
            this.Controls.Add(this.labelPress);
            this.Controls.Add(this.lblFlowTitle);
            this.Controls.Add(this.labelFlow);
            this.Controls.Add(this.lblVoltTitle);
            this.Controls.Add(this.labelVolt);
            this.Controls.Add(this.lblAmpTitle);
            this.Controls.Add(this.labelAmp);
            this.Controls.Add(this.lblTempTitle);
            this.Controls.Add(this.labelTemp);

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label MakeTitle(string text, int x, int y)
        {
            return new System.Windows.Forms.Label
            {
                Location = new System.Drawing.Point(x, y),
                Size = new System.Drawing.Size(140, 18),
                Text = text,
                Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold)
            };
        }

        private System.Windows.Forms.Label MakeValue(string text, int x, int y)
        {
            return new System.Windows.Forms.Label
            {
                Location = new System.Drawing.Point(x, y),
                Size = new System.Drawing.Size(60, 18),
                Text = text,
                TextAlign = System.Drawing.ContentAlignment.MiddleRight,
                Font = new System.Drawing.Font("Segoe UI", 9F)
            };
        }
    }
}