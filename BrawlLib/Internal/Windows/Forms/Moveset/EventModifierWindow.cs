using System;
using System.Windows.Forms;

namespace BrawlLib.Internal.Windows.Forms.Moveset
{
    public class FormModifyEvent : Form
    {
        public EventModifier eventModifier1;
        public ScriptEditor handler;

        public FormModifyEvent(ScriptEditor ptr = null)
        {
            handler = ptr;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.eventModifier1 = new BrawlLib.Internal.Windows.Forms.Moveset.EventModifier();
            this.SuspendLayout();
            // 
            // eventModifier1
            // 
            this.eventModifier1.parentDesignMode = DesignMode;
            this.eventModifier1.AutoSize = true;
            this.eventModifier1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.eventModifier1.Location = new System.Drawing.Point(0, 0);
            this.eventModifier1.Name = "eventModifier1";
            this.eventModifier1.Size = new System.Drawing.Size(284, 262);
            this.eventModifier1.TabIndex = 0;
            this.eventModifier1.Completed += new System.EventHandler(this.eventModifier1_Completed);
            this.eventModifier1.Load += new System.EventHandler(this.eventModifier1_Load);
            // 
            // FormModifyEvent
            // 
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.eventModifier1);
            this.Name = "FormModifyEvent";
            this.Text = "Event Modifier";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void eventModifier1_Completed(object sender, EventArgs e)
        {
            DialogResult = eventModifier1.status;
            Close();
        }

        private void eventModifier1_Load(object sender, EventArgs e)
        {

        }
    }
}