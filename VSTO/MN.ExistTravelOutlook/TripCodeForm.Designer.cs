namespace MN.ExistTravelOutlook
{
    partial class TripCodeForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.tripCodesTextBox = new System.Windows.Forms.TextBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.noTripCodeRequiredButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(439, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Your email subject does not have a trip code. Enter space-separated codes in E123" +
    "4 format";
            // 
            // tripCodesTextBox
            // 
            this.tripCodesTextBox.Location = new System.Drawing.Point(15, 35);
            this.tripCodesTextBox.Name = "tripCodesTextBox";
            this.tripCodesTextBox.Size = new System.Drawing.Size(439, 20);
            this.tripCodesTextBox.TabIndex = 1;
            this.tripCodesTextBox.TextChanged += new System.EventHandler(this.tripCodesTextBox_TextChanged);
            // 
            // okButton
            // 
            this.okButton.Enabled = false;
            this.okButton.Location = new System.Drawing.Point(119, 107);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 2;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(384, 107);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // noTripCodeRequiredButton
            // 
            this.noTripCodeRequiredButton.Location = new System.Drawing.Point(221, 107);
            this.noTripCodeRequiredButton.Name = "noTripCodeRequiredButton";
            this.noTripCodeRequiredButton.Size = new System.Drawing.Size(134, 23);
            this.noTripCodeRequiredButton.TabIndex = 4;
            this.noTripCodeRequiredButton.Text = "No trip code required";
            this.noTripCodeRequiredButton.UseVisualStyleBackColor = true;
            this.noTripCodeRequiredButton.Click += new System.EventHandler(this.noTripCodeRequiredButton_Click);
            // 
            // TripCodeForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(471, 142);
            this.Controls.Add(this.noTripCodeRequiredButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.tripCodesTextBox);
            this.Controls.Add(this.label1);
            this.Name = "TripCodeForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Trip Codes";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tripCodesTextBox;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button noTripCodeRequiredButton;
    }
}