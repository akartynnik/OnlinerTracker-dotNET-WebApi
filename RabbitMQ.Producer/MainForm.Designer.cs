namespace RabbitMQ.Producer
{
    partial class MainForm
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
            this.sendToMQ = new System.Windows.Forms.Button();
            this.messageBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // sendToMQ
            // 
            this.sendToMQ.Location = new System.Drawing.Point(14, 90);
            this.sendToMQ.Name = "sendToMQ";
            this.sendToMQ.Size = new System.Drawing.Size(242, 42);
            this.sendToMQ.TabIndex = 0;
            this.sendToMQ.Text = "Send to MQ";
            this.sendToMQ.UseVisualStyleBackColor = true;
            this.sendToMQ.Click += new System.EventHandler(this.sendToMQ1_Click);
            // 
            // messageBox
            // 
            this.messageBox.Location = new System.Drawing.Point(14, 35);
            this.messageBox.Name = "messageBox";
            this.messageBox.Size = new System.Drawing.Size(242, 20);
            this.messageBox.TabIndex = 1;
            this.messageBox.Text = "Hello, lold!";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(270, 147);
            this.Controls.Add(this.messageBox);
            this.Controls.Add(this.sendToMQ);
            this.Name = "MainForm";
            this.Text = "RabbitMQ Producer";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button sendToMQ;
        private System.Windows.Forms.TextBox messageBox;
    }
}

