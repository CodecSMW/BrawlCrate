using BrawlLib.SSBB.ResourceNodes;
using BrawlLib.SSBB.Types.Subspace;
using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;

namespace BrawlLib.Internal.Windows.Forms.Moveset
{
    public partial class FormEventList : Form
    {
        public DialogResult status;
        public long eventEvent;
        public bool resetParameter;
        public MoveDefNode p;
        public Dictionary<int, string> categoryGroup;
        public List<int> categoryIDs;
        public FormEventList()
        {
            InitializeComponent();
        }

        public void Setup(int catFilter = -1)
        {
            lstEvents.Items.Clear();
            //Add each event to the event list, but omit any events lacking a formal name.
            if (lstEvents.Items.Count <= 0)
            {
                foreach (ActionEventInfo e in MoveDefNode.EventDictionary.Values)
                {
                    if (!(e._name == null || e._name == ""))
                    {
                        long eventCat = e.idNumber & 0xFF000000;
                        eventCat /= 0x1000000;
                        if (catFilter == -1 || eventCat == catFilter) // -1 is the All category
                            lstEvents.Items.Add(e);
                    }
                }
            }

            txtEventId.Text = Helpers.Hex8(eventEvent);
            status = DialogResult.Cancel;
        }

        private void FormEventList_Load(object sender, EventArgs e)
        {
            resetCheckBox.Checked = true;
            resetParameter = true;
            categoryGroup = new Dictionary<int, string>();
            categoryIDs = new List<int>();
            categoryGroup.Clear();
            categoryIDs.Clear();
            categoryGroup.Add(-1, "All");
            categoryIDs.Add(-1);
            string loc = Application.StartupPath + "/MovesetData/EventCategories.txt";
            if (File.Exists(loc))
            {
                using (StreamReader sr = new StreamReader(loc))
                {
                    for (int i = 0; !sr.EndOfStream; i++)
                    {
                        string id = sr.ReadLine();
                        int idNumber = Convert.ToInt32(id.Substring(0,2), 16);
                        string categoryName = id.Substring(3);

                        categoryGroup.Add(idNumber,categoryName);
                        categoryIDs.Add(idNumber);
                    }
                }
            }
            for (int i = 0; i < categoryIDs.Count; i++) 
            {
                eventCategoryBox.Items.Add(categoryGroup[categoryIDs[i]]);
            }
            eventCategoryBox.SelectedIndex = 0;
            Setup();
        }

        private void FormEventList_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void lstEvents_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstEvents.SelectedIndex == -1)
            {
                return;
            }

            txtEventId.Text = Helpers.Hex8((lstEvents.SelectedItem as ActionEventInfo).idNumber);
        }

        private void txtEventId_TextChanged(object sender, EventArgs e)
        {
            if (txtEventId.Text.Length != 8)
            {
                return;
            }

            string eventId = txtEventId.Text;

            //Select the event corresponding to the id input.
            lstEvents.SelectedIndex = -1;
            for (int i = 0; i < lstEvents.Items.Count; i++)
            {
                if (eventId == Helpers.Hex8((lstEvents.Items[i] as ActionEventInfo).idNumber))
                {
                    lstEvents.SelectedIndex = i;
                    break;
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            status = DialogResult.Cancel;
            Close();
        }

        private void btnDone_Click(object sender, EventArgs e)
        {
            try
            {
                eventEvent = Helpers.UnHex(txtEventId.Text);
            }
            catch
            {
                eventEvent = 0;
            }

            if (eventEvent == 0)
            {
                MessageBox.Show("Invalid event Id.", "Invalid Id");
                return;
            }

            status = DialogResult.OK;
            Close();
        }

        private void resetCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            resetParameter = resetCheckBox.Checked;
        }

        private void eventCategoryBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Setup(categoryIDs[eventCategoryBox.SelectedIndex]);
        }
    }
}