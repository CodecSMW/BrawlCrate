using System.Windows.Forms;

namespace BrawlLib.Internal.Windows.Forms.Moveset
{
    partial class FormEventList
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
            this.lstEvents = new System.Windows.Forms.ListBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnDone = new System.Windows.Forms.Button();
            this.txtEventId = new System.Windows.Forms.TextBox();
            this.resetCheckBox = new System.Windows.Forms.CheckBox();
            this.eventCategoryBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // lstEvents
            // 
            this.lstEvents.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstEvents.FormattingEnabled = true;
            this.lstEvents.ItemHeight = 16;
            this.lstEvents.Location = new System.Drawing.Point(13, 13);
            this.lstEvents.Margin = new System.Windows.Forms.Padding(4);
            this.lstEvents.Name = "lstEvents";
            this.lstEvents.Size = new System.Drawing.Size(373, 244);
            this.lstEvents.TabIndex = 0;
            this.lstEvents.SelectedIndexChanged += new System.EventHandler(this.lstEvents_SelectedIndexChanged);
            this.lstEvents.DoubleClick += new System.EventHandler(this.btnDone_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCancel.Location = new System.Drawing.Point(96, 303);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 30);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnDone
            // 
            this.btnDone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDone.Location = new System.Drawing.Point(11, 303);
            this.btnDone.Margin = new System.Windows.Forms.Padding(4);
            this.btnDone.Name = "btnDone";
            this.btnDone.Size = new System.Drawing.Size(75, 30);
            this.btnDone.TabIndex = 2;
            this.btnDone.Text = "Done";
            this.btnDone.UseVisualStyleBackColor = true;
            this.btnDone.Click += new System.EventHandler(this.btnDone_Click);
            // 
            // txtEventId
            // 
            this.txtEventId.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.txtEventId.Location = new System.Drawing.Point(276, 307);
            this.txtEventId.Margin = new System.Windows.Forms.Padding(4);
            this.txtEventId.MaxLength = 8;
            this.txtEventId.Name = "txtEventId";
            this.txtEventId.Size = new System.Drawing.Size(105, 22);
            this.txtEventId.TabIndex = 3;
            this.txtEventId.TextChanged += new System.EventHandler(this.txtEventId_TextChanged);
            // 
            // resetCheckBox
            // 
            this.resetCheckBox.AutoSize = true;
            this.resetCheckBox.Checked = true;
            this.resetCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.resetCheckBox.Location = new System.Drawing.Point(13, 269);
            this.resetCheckBox.Name = "resetCheckBox";
            this.resetCheckBox.Size = new System.Drawing.Size(138, 20);
            this.resetCheckBox.TabIndex = 4;
            this.resetCheckBox.Text = "Reset Parameters";
            this.resetCheckBox.UseVisualStyleBackColor = true;
            this.resetCheckBox.CheckedChanged += new System.EventHandler(this.resetCheckBox_CheckedChanged);
            // 
            // eventCategoryBox
            // 
            this.eventCategoryBox.FormattingEnabled = true;
            this.eventCategoryBox.Location = new System.Drawing.Point(182, 267);
            this.eventCategoryBox.Name = "eventCategoryBox";
            this.eventCategoryBox.Size = new System.Drawing.Size(199, 24);
            this.eventCategoryBox.TabIndex = 5;
            this.eventCategoryBox.SelectedIndexChanged += new System.EventHandler(this.eventCategoryBox_SelectedIndexChanged);
            // 
            // FormEventList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(394, 340);
            this.Controls.Add(this.eventCategoryBox);
            this.Controls.Add(this.resetCheckBox);
            this.Controls.Add(this.txtEventId);
            this.Controls.Add(this.btnDone);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.lstEvents);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(302, 344);
            this.Name = "FormEventList";
            this.ShowIcon = false;
            this.Text = "Events";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormEventList_FormClosing);
            this.Load += new System.EventHandler(this.FormEventList_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lstEvents;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnDone;
        private System.Windows.Forms.TextBox txtEventId;
        private CheckBox resetCheckBox;
        private ComboBox eventCategoryBox;
    }
}