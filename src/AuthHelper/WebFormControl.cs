using System;
using System.Windows.Forms;

namespace AuthHelper
{
    public partial class WebFormControl : Form
    {
        private System.Windows.Forms.WebBrowser webBrowser1;

        public Uri targetUri { get; set; }
        public WebFormControl(Uri target)
        {
            InitializeComponent();
            targetUri = target;
        }

        private void InitializeComponent()
        {
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // webBrowser1
            // 
            this.webBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser1.Location = new System.Drawing.Point(0, 0);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(1024, 768);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.Size = new System.Drawing.Size(1024, 768);
            this.webBrowser1.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1024, 768);
            this.Controls.Add(this.webBrowser1);
            this.Name = "WebFormControl";
            this.Text = "OAuth Helper";
            this.ResumeLayout(false);

        }
    }
}
