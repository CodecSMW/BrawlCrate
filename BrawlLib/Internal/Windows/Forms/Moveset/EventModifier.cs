using BrawlLib.SSBB.ResourceNodes;
using BrawlLib.SSBB.Types;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace BrawlLib.Internal.Windows.Forms.Moveset
{
    public class EventModifier : UserControl
    {
        public bool parentDesignMode = true;

        private int iReq9999loc;
        private ComboBox cboType;
        private ListBox lstParameters;
        private Button btnChangeEvent;
        private Label lblEventId;
        private Label lblEventName;
        private Label lblParamDescription;
        private Button btnCancel;
        private Button btnDone;
        private Label lblName2;
        private PropertyGrid valueGrid;
        private Panel requirementPanel;
        private Panel mainPanel;
        private SplitContainer splitContainer1;
        private Panel typePanel;
        private CheckBox chkNot;
        private Label label1;
        private ComboBox cboRequirement;
        private Panel offsetPanel;
        private Button offsetOkay;
        private ComboBox comboBox1;
        private ComboBox comboBox2;
        private ComboBox comboBox3;
        private Label label2;
        private Label label3;
        private Label label4;
        private Panel hitboxFlagPanel;
        private Label label8;
        private Label label7;
        private ComboBox comboHit_SoundVol;
        private ComboBox comboHit_Sound;
        private ComboBox comboHit_Attribute;
        private Label label6;
        private ComboBox comboHit_Type;
        private Label label5;
        private CheckBox comboHit_check_Hit_Odd;
        private CheckBox comboHit_check_Hit_Air;
        private CheckBox comboHit_check_Hit_Ground;
        private CheckBox comboHit_check_Clank;
        private CheckBox comboHit_check_Rebound;
        private CheckBox comboHit_check_Direct;
        private Panel spHitboxFlagPanel;
        private CheckedListBox comboSpHit_TargetBox;
        private ComboBox comboSpHit_KB_Box;
        private CheckedListBox comboSpHit_RegionBox;
        private CheckedListBox comboSpHit_BehaviorBox;
        private Label label9;
        private Label lblName1;

        #region Designer

        private void InitializeComponent()
        {
            this.cboType = new System.Windows.Forms.ComboBox();
            this.lstParameters = new System.Windows.Forms.ListBox();
            this.btnChangeEvent = new System.Windows.Forms.Button();
            this.lblEventId = new System.Windows.Forms.Label();
            this.lblEventName = new System.Windows.Forms.Label();
            this.lblParamDescription = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnDone = new System.Windows.Forms.Button();
            this.lblName2 = new System.Windows.Forms.Label();
            this.lblName1 = new System.Windows.Forms.Label();
            this.valueGrid = new System.Windows.Forms.PropertyGrid();
            this.requirementPanel = new System.Windows.Forms.Panel();
            this.chkNot = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cboRequirement = new System.Windows.Forms.ComboBox();
            this.offsetPanel = new System.Windows.Forms.Panel();
            this.offsetOkay = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.comboBox3 = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.mainPanel = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.spHitboxFlagPanel = new System.Windows.Forms.Panel();
            this.comboSpHit_BehaviorBox = new System.Windows.Forms.CheckedListBox();
            this.label9 = new System.Windows.Forms.Label();
            this.comboSpHit_KB_Box = new System.Windows.Forms.ComboBox();
            this.comboSpHit_RegionBox = new System.Windows.Forms.CheckedListBox();
            this.comboSpHit_TargetBox = new System.Windows.Forms.CheckedListBox();
            this.hitboxFlagPanel = new System.Windows.Forms.Panel();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.comboHit_SoundVol = new System.Windows.Forms.ComboBox();
            this.comboHit_Sound = new System.Windows.Forms.ComboBox();
            this.comboHit_Attribute = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.comboHit_Type = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.comboHit_check_Hit_Odd = new System.Windows.Forms.CheckBox();
            this.comboHit_check_Hit_Air = new System.Windows.Forms.CheckBox();
            this.comboHit_check_Hit_Ground = new System.Windows.Forms.CheckBox();
            this.comboHit_check_Clank = new System.Windows.Forms.CheckBox();
            this.comboHit_check_Rebound = new System.Windows.Forms.CheckBox();
            this.comboHit_check_Direct = new System.Windows.Forms.CheckBox();
            this.typePanel = new System.Windows.Forms.Panel();
            this.requirementPanel.SuspendLayout();
            this.offsetPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.spHitboxFlagPanel.SuspendLayout();
            this.hitboxFlagPanel.SuspendLayout();
            this.typePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // cboType
            // 
            this.cboType.FormattingEnabled = true;
            this.cboType.Items.AddRange(new object[] {
            "Value",
            "Scalar",
            "Drawing.Pointer",
            "Boolean",
            "Unknown",
            "Variable",
            "Requirement"});
            this.cboType.Location = new System.Drawing.Point(46, 0);
            this.cboType.Name = "cboType";
            this.cboType.Size = new System.Drawing.Size(82, 21);
            this.cboType.TabIndex = 63;
            this.cboType.SelectedIndexChanged += new System.EventHandler(this.cboType_SelectedIndexChanged);
            // 
            // lstParameters
            // 
            this.lstParameters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstParameters.FormattingEnabled = true;
            this.lstParameters.Location = new System.Drawing.Point(0, 21);
            this.lstParameters.Name = "lstParameters";
            this.lstParameters.Size = new System.Drawing.Size(137, 197);
            this.lstParameters.TabIndex = 62;
            this.lstParameters.SelectedIndexChanged += new System.EventHandler(this.lstParameters_SelectedIndexChanged);
            // 
            // btnChangeEvent
            // 
            this.btnChangeEvent.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnChangeEvent.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F);
            this.btnChangeEvent.Location = new System.Drawing.Point(279, 2);
            this.btnChangeEvent.Name = "btnChangeEvent";
            this.btnChangeEvent.Size = new System.Drawing.Size(56, 23);
            this.btnChangeEvent.TabIndex = 61;
            this.btnChangeEvent.Text = "Change";
            this.btnChangeEvent.UseVisualStyleBackColor = true;
            this.btnChangeEvent.Click += new System.EventHandler(this.btnChangeEvent_Click);
            // 
            // lblEventId
            // 
            this.lblEventId.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblEventId.BackColor = System.Drawing.Color.WhiteSmoke;
            this.lblEventId.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblEventId.Location = new System.Drawing.Point(215, 3);
            this.lblEventId.Name = "lblEventId";
            this.lblEventId.Size = new System.Drawing.Size(66, 20);
            this.lblEventId.TabIndex = 60;
            this.lblEventId.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblEventId.Click += new System.EventHandler(this.lblEventId_Click);
            // 
            // lblEventName
            // 
            this.lblEventName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblEventName.BackColor = System.Drawing.Color.WhiteSmoke;
            this.lblEventName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblEventName.Location = new System.Drawing.Point(2, 3);
            this.lblEventName.Name = "lblEventName";
            this.lblEventName.Size = new System.Drawing.Size(213, 20);
            this.lblEventName.TabIndex = 59;
            this.lblEventName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblEventName.Click += new System.EventHandler(this.lblEventName_Click);
            // 
            // lblParamDescription
            // 
            this.lblParamDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblParamDescription.BackColor = System.Drawing.SystemColors.Control;
            this.lblParamDescription.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblParamDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblParamDescription.Location = new System.Drawing.Point(2, 245);
            this.lblParamDescription.Name = "lblParamDescription";
            this.lblParamDescription.Size = new System.Drawing.Size(333, 63);
            this.lblParamDescription.TabIndex = 58;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(277, 311);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(58, 24);
            this.btnCancel.TabIndex = 57;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click_1);
            // 
            // btnDone
            // 
            this.btnDone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDone.Location = new System.Drawing.Point(213, 311);
            this.btnDone.Name = "btnDone";
            this.btnDone.Size = new System.Drawing.Size(58, 24);
            this.btnDone.TabIndex = 56;
            this.btnDone.Text = "Done";
            this.btnDone.UseVisualStyleBackColor = true;
            this.btnDone.Click += new System.EventHandler(this.btnDone_Click_1);
            // 
            // lblName2
            // 
            this.lblName2.BackColor = System.Drawing.Color.WhiteSmoke;
            this.lblName2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblName2.Location = new System.Drawing.Point(0, 0);
            this.lblName2.Name = "lblName2";
            this.lblName2.Size = new System.Drawing.Size(45, 21);
            this.lblName2.TabIndex = 55;
            this.lblName2.Text = "Type:";
            this.lblName2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblName2.Click += new System.EventHandler(this.lblName2_Click);
            // 
            // lblName1
            // 
            this.lblName1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.lblName1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblName1.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblName1.Location = new System.Drawing.Point(0, 0);
            this.lblName1.Name = "lblName1";
            this.lblName1.Size = new System.Drawing.Size(137, 21);
            this.lblName1.TabIndex = 54;
            this.lblName1.Text = "Parameter:";
            this.lblName1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // valueGrid
            // 
            this.valueGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.valueGrid.HelpVisible = false;
            this.valueGrid.Location = new System.Drawing.Point(0, 0);
            this.valueGrid.Name = "valueGrid";
            this.valueGrid.Size = new System.Drawing.Size(180, 371);
            this.valueGrid.TabIndex = 8;
            this.valueGrid.ToolbarVisible = false;
            // 
            // requirementPanel
            // 
            this.requirementPanel.Controls.Add(this.chkNot);
            this.requirementPanel.Controls.Add(this.label1);
            this.requirementPanel.Controls.Add(this.cboRequirement);
            this.requirementPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.requirementPanel.Location = new System.Drawing.Point(0, 0);
            this.requirementPanel.Name = "requirementPanel";
            this.requirementPanel.Size = new System.Drawing.Size(180, 371);
            this.requirementPanel.TabIndex = 64;
            // 
            // chkNot
            // 
            this.chkNot.AutoSize = true;
            this.chkNot.Location = new System.Drawing.Point(81, 3);
            this.chkNot.Name = "chkNot";
            this.chkNot.Size = new System.Drawing.Size(43, 17);
            this.chkNot.TabIndex = 65;
            this.chkNot.Text = "Not";
            this.chkNot.UseVisualStyleBackColor = true;
            this.chkNot.CheckedChanged += new System.EventHandler(this.Requirement_Handle);
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 21);
            this.label1.TabIndex = 64;
            this.label1.Text = "Requirement:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cboRequirement
            // 
            this.cboRequirement.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboRequirement.FormattingEnabled = true;
            this.cboRequirement.Location = new System.Drawing.Point(0, 22);
            this.cboRequirement.Name = "cboRequirement";
            this.cboRequirement.Size = new System.Drawing.Size(141, 21);
            this.cboRequirement.TabIndex = 0;
            this.cboRequirement.SelectedIndexChanged += new System.EventHandler(this.Requirement_Handle);
            // 
            // offsetPanel
            // 
            this.offsetPanel.AutoScroll = true;
            this.offsetPanel.AutoSize = true;
            this.offsetPanel.Controls.Add(this.offsetOkay);
            this.offsetPanel.Controls.Add(this.comboBox1);
            this.offsetPanel.Controls.Add(this.comboBox2);
            this.offsetPanel.Controls.Add(this.comboBox3);
            this.offsetPanel.Controls.Add(this.label2);
            this.offsetPanel.Controls.Add(this.label3);
            this.offsetPanel.Controls.Add(this.label4);
            this.offsetPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.offsetPanel.Location = new System.Drawing.Point(0, 21);
            this.offsetPanel.Name = "offsetPanel";
            this.offsetPanel.Size = new System.Drawing.Size(180, 350);
            this.offsetPanel.TabIndex = 66;
            // 
            // offsetOkay
            // 
            this.offsetOkay.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.offsetOkay.BackColor = System.Drawing.SystemColors.Control;
            this.offsetOkay.Location = new System.Drawing.Point(-1, 69);
            this.offsetOkay.Name = "offsetOkay";
            this.offsetOkay.Size = new System.Drawing.Size(142, 23);
            this.offsetOkay.TabIndex = 13;
            this.offsetOkay.Text = "Okay";
            this.offsetOkay.UseVisualStyleBackColor = false;
            this.offsetOkay.Click += new System.EventHandler(this.offsetOkay_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "Actions",
            "SubActions",
            "SubRoutines",
            "External",
            "Null",
            "Screen Tints",
            "Flash Overlays"});
            this.comboBox1.Location = new System.Drawing.Point(46, 0);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(95, 21);
            this.comboBox1.TabIndex = 7;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // comboBox2
            // 
            this.comboBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Location = new System.Drawing.Point(46, 24);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(95, 21);
            this.comboBox2.TabIndex = 9;
            // 
            // comboBox3
            // 
            this.comboBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox3.FormattingEnabled = true;
            this.comboBox3.Location = new System.Drawing.Point(46, 48);
            this.comboBox3.Name = "comboBox3";
            this.comboBox3.Size = new System.Drawing.Size(95, 21);
            this.comboBox3.TabIndex = 11;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.WhiteSmoke;
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F);
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 21);
            this.label2.TabIndex = 8;
            this.label2.Text = "List:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.WhiteSmoke;
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F);
            this.label3.Location = new System.Drawing.Point(0, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 21);
            this.label3.TabIndex = 10;
            this.label3.Text = "Action:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.WhiteSmoke;
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F);
            this.label4.Location = new System.Drawing.Point(0, 48);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(45, 21);
            this.label4.TabIndex = 12;
            this.label4.Text = "Type:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // mainPanel
            // 
            this.mainPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mainPanel.Location = new System.Drawing.Point(2, 25);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(333, 218);
            this.mainPanel.TabIndex = 0;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(2, 25);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.lstParameters);
            this.splitContainer1.Panel1.Controls.Add(this.lblName1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.AutoScroll = true;
            this.splitContainer1.Panel2.Controls.Add(this.spHitboxFlagPanel);
            this.splitContainer1.Panel2.Controls.Add(this.hitboxFlagPanel);
            this.splitContainer1.Panel2.Controls.Add(this.offsetPanel);
            this.splitContainer1.Panel2.Controls.Add(this.typePanel);
            this.splitContainer1.Panel2.Controls.Add(this.requirementPanel);
            this.splitContainer1.Panel2.Controls.Add(this.valueGrid);
            this.splitContainer1.Size = new System.Drawing.Size(333, 218);
            this.splitContainer1.SplitterDistance = 137;
            this.splitContainer1.TabIndex = 9;
            // 
            // spHitboxFlagPanel
            // 
            this.spHitboxFlagPanel.Controls.Add(this.comboSpHit_BehaviorBox);
            this.spHitboxFlagPanel.Controls.Add(this.label9);
            this.spHitboxFlagPanel.Controls.Add(this.comboSpHit_KB_Box);
            this.spHitboxFlagPanel.Controls.Add(this.comboSpHit_RegionBox);
            this.spHitboxFlagPanel.Controls.Add(this.comboSpHit_TargetBox);
            this.spHitboxFlagPanel.Location = new System.Drawing.Point(-1, 2);
            this.spHitboxFlagPanel.Name = "spHitboxFlagPanel";
            this.spHitboxFlagPanel.Size = new System.Drawing.Size(181, 369);
            this.spHitboxFlagPanel.TabIndex = 68;
            // 
            // comboSpHit_BehaviorBox
            // 
            this.comboSpHit_BehaviorBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.comboSpHit_BehaviorBox.FormattingEnabled = true;
            this.comboSpHit_BehaviorBox.Items.AddRange(new object[] {
            "Flinchless",
            "Hide Hit Effect",
            "No Hitstop",
            "Friendly Fire",
            "Grabbed Targets Only",
            "Ignore Intangibility",
            "Ignore Invincibility",
            "Absorbable",
            "Reflectable",
            "Blockable",
            "Capsule Hitbox",
            "Front-Only (Custom)",
            "KO after 100% (Custom)"});
            this.comboSpHit_BehaviorBox.Location = new System.Drawing.Point(10, 216);
            this.comboSpHit_BehaviorBox.Name = "comboSpHit_BehaviorBox";
            this.comboSpHit_BehaviorBox.Size = new System.Drawing.Size(158, 79);
            this.comboSpHit_BehaviorBox.TabIndex = 4;
            this.comboSpHit_BehaviorBox.SelectedIndexChanged += new System.EventHandler(this.comboSpHit_BehaviorBox_SelectedIndexChanged);
            this.comboSpHit_BehaviorBox.MouseLeave += new System.EventHandler(this.combooSpHit_checkedBoxLeave);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 5.2F);
            this.label9.Location = new System.Drawing.Point(6, 34);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(45, 7);
            this.label9.TabIndex = 3;
            this.label9.Text = "KB Direction";
            // 
            // comboSpHit_KB_Box
            // 
            this.comboSpHit_KB_Box.DropDownWidth = 240;
            this.comboSpHit_KB_Box.FormattingEnabled = true;
            this.comboSpHit_KB_Box.Items.AddRange(new object[] {
            "Normal",
            "In the direction of the attacker\'s momentum.",
            "The direction the attacked faces.",
            "Forward",
            "Behind",
            "Away from center of hitbox.",
            "Unused (Reserved for future Back Slash attempt)",
            "Unused"});
            this.comboSpHit_KB_Box.Location = new System.Drawing.Point(68, 28);
            this.comboSpHit_KB_Box.Name = "comboSpHit_KB_Box";
            this.comboSpHit_KB_Box.Size = new System.Drawing.Size(100, 21);
            this.comboSpHit_KB_Box.TabIndex = 2;
            this.comboSpHit_KB_Box.SelectedIndexChanged += new System.EventHandler(this.comboSpHit_KB_Box_SelectedIndexChanged);
            // 
            // comboSpHit_RegionBox
            // 
            this.comboSpHit_RegionBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.comboSpHit_RegionBox.FormattingEnabled = true;
            this.comboSpHit_RegionBox.Items.AddRange(new object[] {
            "Leg",
            "Knee",
            "Arm",
            "Body"});
            this.comboSpHit_RegionBox.Location = new System.Drawing.Point(11, 146);
            this.comboSpHit_RegionBox.Name = "comboSpHit_RegionBox";
            this.comboSpHit_RegionBox.Size = new System.Drawing.Size(157, 64);
            this.comboSpHit_RegionBox.TabIndex = 1;
            this.comboSpHit_RegionBox.SelectedIndexChanged += new System.EventHandler(this.comboSpHit_RegionBox_SelectedIndexChanged);
            this.comboSpHit_RegionBox.MouseLeave += new System.EventHandler(this.combooSpHit_checkedBoxLeave);
            // 
            // comboSpHit_TargetBox
            // 
            this.comboSpHit_TargetBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.comboSpHit_TargetBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.comboSpHit_TargetBox.FormattingEnabled = true;
            this.comboSpHit_TargetBox.Items.AddRange(new object[] {
            "Destroyable Floors",
            "Items (SSE Enemies Can Hit)",
            "Destroyable Walls",
            "Destroyable Objects",
            "Unknown",
            "Unknown",
            "Items",
            "Unknown",
            "Enemies",
            "Fighters"});
            this.comboSpHit_TargetBox.Location = new System.Drawing.Point(10, 58);
            this.comboSpHit_TargetBox.Name = "comboSpHit_TargetBox";
            this.comboSpHit_TargetBox.ScrollAlwaysVisible = true;
            this.comboSpHit_TargetBox.Size = new System.Drawing.Size(158, 82);
            this.comboSpHit_TargetBox.TabIndex = 0;
            this.comboSpHit_TargetBox.SelectedIndexChanged += new System.EventHandler(this.comboSpHit_TargetBox_SelectedIndexChanged);
            this.comboSpHit_TargetBox.MouseLeave += new System.EventHandler(this.combooSpHit_checkedBoxLeave);
            // 
            // hitboxFlagPanel
            // 
            this.hitboxFlagPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.hitboxFlagPanel.Controls.Add(this.label8);
            this.hitboxFlagPanel.Controls.Add(this.label7);
            this.hitboxFlagPanel.Controls.Add(this.comboHit_SoundVol);
            this.hitboxFlagPanel.Controls.Add(this.comboHit_Sound);
            this.hitboxFlagPanel.Controls.Add(this.comboHit_Attribute);
            this.hitboxFlagPanel.Controls.Add(this.label6);
            this.hitboxFlagPanel.Controls.Add(this.comboHit_Type);
            this.hitboxFlagPanel.Controls.Add(this.label5);
            this.hitboxFlagPanel.Controls.Add(this.comboHit_check_Hit_Odd);
            this.hitboxFlagPanel.Controls.Add(this.comboHit_check_Hit_Air);
            this.hitboxFlagPanel.Controls.Add(this.comboHit_check_Hit_Ground);
            this.hitboxFlagPanel.Controls.Add(this.comboHit_check_Clank);
            this.hitboxFlagPanel.Controls.Add(this.comboHit_check_Rebound);
            this.hitboxFlagPanel.Controls.Add(this.comboHit_check_Direct);
            this.hitboxFlagPanel.Location = new System.Drawing.Point(-1, 0);
            this.hitboxFlagPanel.Name = "hitboxFlagPanel";
            this.hitboxFlagPanel.Size = new System.Drawing.Size(162, 280);
            this.hitboxFlagPanel.TabIndex = 67;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F);
            this.label8.Location = new System.Drawing.Point(7, 87);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(42, 13);
            this.label8.TabIndex = 12;
            this.label8.Text = "Volume";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F);
            this.label7.Location = new System.Drawing.Point(7, 59);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(38, 13);
            this.label7.TabIndex = 11;
            this.label7.Text = "Sound";
            // 
            // comboHit_SoundVol
            // 
            this.comboHit_SoundVol.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboHit_SoundVol.FormattingEnabled = true;
            this.comboHit_SoundVol.Items.AddRange(new object[] {
            "Soft",
            "Medium",
            "Loud",
            "Very Loud"});
            this.comboHit_SoundVol.Location = new System.Drawing.Point(66, 84);
            this.comboHit_SoundVol.Name = "comboHit_SoundVol";
            this.comboHit_SoundVol.Size = new System.Drawing.Size(93, 21);
            this.comboHit_SoundVol.TabIndex = 10;
            this.comboHit_SoundVol.SelectedIndexChanged += new System.EventHandler(this.comboHit_SoundVol_SelectedIndexChanged);
            // 
            // comboHit_Sound
            // 
            this.comboHit_Sound.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboHit_Sound.FormattingEnabled = true;
            this.comboHit_Sound.Items.AddRange(new object[] {
            "None",
            "Punch",
            "Kick",
            "Slash",
            "Coin",
            "Bat",
            "Paper",
            "Shock",
            "Burn",
            "Splash",
            "Grass",
            "Explosion",
            "Unused? (12: Peach-Only)",
            "Thud (Snake-Only)",
            "Slam (Ike-Only)",
            "Hammer (Dedede-Only)",
            "Magic Zap",
            "Shell",
            "Slap (Peach-Only)",
            "Pan (Peach-Only)",
            "Club (Peach-Only)",
            "Racket (Peach-Only)",
            "Aura (Lucario-Only)",
            "Treble (Marth-Only)",
            "Coin (Mario-Only)",
            "Finale (Mario-Only)",
            "Coin (Luigi-Only)",
            "Bat (Ness-Only)",
            "Freeze"});
            this.comboHit_Sound.Location = new System.Drawing.Point(66, 56);
            this.comboHit_Sound.Name = "comboHit_Sound";
            this.comboHit_Sound.Size = new System.Drawing.Size(93, 21);
            this.comboHit_Sound.TabIndex = 9;
            this.comboHit_Sound.SelectedIndexChanged += new System.EventHandler(this.comboHit_Sound_SelectedIndexChanged);
            // 
            // comboHit_Attribute
            // 
            this.comboHit_Attribute.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboHit_Attribute.FormattingEnabled = true;
            this.comboHit_Attribute.Items.AddRange(new object[] {
            "Normal",
            "None",
            "Slash",
            "Electric",
            "Ice",
            "Fire",
            "Coin",
            "Reverse",
            "Trip",
            "Sleep",
            "Unused? (10)",
            "Bury",
            "Stun",
            "Magic (PM)",
            "Flower",
            "Green Fire (PM)",
            "Unused? (16)",
            "Grass",
            "Water",
            "Darkness",
            "Paralyze",
            "Aura",
            "Plunge",
            "Down",
            "Flinchless",
            "Unused (25)",
            "Unused (26)",
            "Unused (27)",
            "Unused (28)",
            "Unused (29)",
            "Unused (30)",
            "Unused (31)"});
            this.comboHit_Attribute.Location = new System.Drawing.Point(66, 30);
            this.comboHit_Attribute.Name = "comboHit_Attribute";
            this.comboHit_Attribute.Size = new System.Drawing.Size(93, 21);
            this.comboHit_Attribute.TabIndex = 8;
            this.comboHit_Attribute.SelectedIndexChanged += new System.EventHandler(this.comboHit_Attribute_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 33);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(46, 13);
            this.label6.TabIndex = 7;
            this.label6.Text = "Attribute";
            // 
            // comboHit_Type
            // 
            this.comboHit_Type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboHit_Type.FormattingEnabled = true;
            this.comboHit_Type.Items.AddRange(new object[] {
            "Typeless",
            "Head",
            "Body",
            "Hip",
            "Hand",
            "Elbow",
            "Foot",
            "Knee",
            "Throw",
            "Weapon",
            "Sword",
            "Hammer",
            "Bomb",
            "Spin",
            "Bite",
            "Magic",
            "PSI",
            "Bow",
            "Aura",
            "Bat",
            "Parasol",
            "Pikmin",
            "Water",
            "Whip",
            "Tail",
            "Energy"});
            this.comboHit_Type.Location = new System.Drawing.Point(66, 3);
            this.comboHit_Type.Name = "comboHit_Type";
            this.comboHit_Type.Size = new System.Drawing.Size(93, 21);
            this.comboHit_Type.TabIndex = 6;
            this.comboHit_Type.SelectedIndexChanged += new System.EventHandler(this.comboHit_Type_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 6);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(31, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "Type";
            // 
            // comboHit_check_Hit_Odd
            // 
            this.comboHit_check_Hit_Odd.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.comboHit_check_Hit_Odd.Location = new System.Drawing.Point(78, 169);
            this.comboHit_check_Hit_Odd.Name = "comboHit_check_Hit_Odd";
            this.comboHit_check_Hit_Odd.Size = new System.Drawing.Size(55, 20);
            this.comboHit_check_Hit_Odd.TabIndex = 0;
            this.comboHit_check_Hit_Odd.Text = "Odd";
            this.comboHit_check_Hit_Odd.UseMnemonic = false;
            this.comboHit_check_Hit_Odd.UseVisualStyleBackColor = true;
            this.comboHit_check_Hit_Odd.CheckedChanged += new System.EventHandler(this.comboHit_check_Hit_Odd_CheckedChanged);
            // 
            // comboHit_check_Hit_Air
            // 
            this.comboHit_check_Hit_Air.AutoSize = true;
            this.comboHit_check_Hit_Air.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.comboHit_check_Hit_Air.Location = new System.Drawing.Point(78, 141);
            this.comboHit_check_Hit_Air.Name = "comboHit_check_Hit_Air";
            this.comboHit_check_Hit_Air.Size = new System.Drawing.Size(38, 17);
            this.comboHit_check_Hit_Air.TabIndex = 4;
            this.comboHit_check_Hit_Air.Text = "Air";
            this.comboHit_check_Hit_Air.UseVisualStyleBackColor = true;
            this.comboHit_check_Hit_Air.CheckedChanged += new System.EventHandler(this.comboHit_check_Hit_Air_CheckedChanged);
            // 
            // comboHit_check_Hit_Ground
            // 
            this.comboHit_check_Hit_Ground.AutoSize = true;
            this.comboHit_check_Hit_Ground.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.comboHit_check_Hit_Ground.Location = new System.Drawing.Point(10, 141);
            this.comboHit_check_Hit_Ground.Name = "comboHit_check_Hit_Ground";
            this.comboHit_check_Hit_Ground.Size = new System.Drawing.Size(61, 17);
            this.comboHit_check_Hit_Ground.TabIndex = 3;
            this.comboHit_check_Hit_Ground.Text = "Ground";
            this.comboHit_check_Hit_Ground.UseVisualStyleBackColor = true;
            this.comboHit_check_Hit_Ground.CheckedChanged += new System.EventHandler(this.comboHit_check_Hit_Ground_CheckedChanged);
            // 
            // comboHit_check_Clank
            // 
            this.comboHit_check_Clank.AutoSize = true;
            this.comboHit_check_Clank.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.comboHit_check_Clank.Location = new System.Drawing.Point(10, 114);
            this.comboHit_check_Clank.Name = "comboHit_check_Clank";
            this.comboHit_check_Clank.Size = new System.Drawing.Size(52, 17);
            this.comboHit_check_Clank.TabIndex = 2;
            this.comboHit_check_Clank.Text = "Clank";
            this.comboHit_check_Clank.UseVisualStyleBackColor = true;
            this.comboHit_check_Clank.CheckedChanged += new System.EventHandler(this.comboHit_check_Clank_CheckedChanged);
            // 
            // comboHit_check_Rebound
            // 
            this.comboHit_check_Rebound.AutoSize = true;
            this.comboHit_check_Rebound.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.comboHit_check_Rebound.Location = new System.Drawing.Point(78, 114);
            this.comboHit_check_Rebound.Name = "comboHit_check_Rebound";
            this.comboHit_check_Rebound.Size = new System.Drawing.Size(69, 17);
            this.comboHit_check_Rebound.TabIndex = 1;
            this.comboHit_check_Rebound.Text = "Rebound";
            this.comboHit_check_Rebound.UseVisualStyleBackColor = true;
            this.comboHit_check_Rebound.CheckedChanged += new System.EventHandler(this.comboHit_check_Rebound_CheckedChanged);
            // 
            // comboHit_check_Direct
            // 
            this.comboHit_check_Direct.AutoSize = true;
            this.comboHit_check_Direct.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.comboHit_check_Direct.Location = new System.Drawing.Point(10, 169);
            this.comboHit_check_Direct.Name = "comboHit_check_Direct";
            this.comboHit_check_Direct.Size = new System.Drawing.Size(53, 17);
            this.comboHit_check_Direct.TabIndex = 0;
            this.comboHit_check_Direct.Text = "Direct";
            this.comboHit_check_Direct.UseVisualStyleBackColor = true;
            this.comboHit_check_Direct.CheckedChanged += new System.EventHandler(this.comboHit_check_Direct_CheckedChanged);
            // 
            // typePanel
            // 
            this.typePanel.Controls.Add(this.lblName2);
            this.typePanel.Controls.Add(this.cboType);
            this.typePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.typePanel.Location = new System.Drawing.Point(0, 0);
            this.typePanel.Name = "typePanel";
            this.typePanel.Size = new System.Drawing.Size(180, 21);
            this.typePanel.TabIndex = 64;
            // 
            // EventModifier
            // 
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.mainPanel);
            this.Controls.Add(this.btnChangeEvent);
            this.Controls.Add(this.lblEventId);
            this.Controls.Add(this.lblEventName);
            this.Controls.Add(this.lblParamDescription);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnDone);
            this.Name = "EventModifier";
            this.Size = new System.Drawing.Size(338, 338);
            this.Load += new System.EventHandler(this.EventModifier_Load);
            this.requirementPanel.ResumeLayout(false);
            this.requirementPanel.PerformLayout();
            this.offsetPanel.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.spHitboxFlagPanel.ResumeLayout(false);
            this.spHitboxFlagPanel.PerformLayout();
            this.hitboxFlagPanel.ResumeLayout(false);
            this.hitboxFlagPanel.PerformLayout();
            this.typePanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public EventModifier()
        {
            InitializeComponent();
            frmEventList = new FormEventList();
        }

        public DialogResult status;
        public MoveDefEventNode origEvent;

        private MoveDefEventNode eventData => newEvent;
        private MoveDefEventNode newEv;

        private MoveDefEventNode newEvent
        {
            get
            {
                if (newEv == null)
                {
                    newEv = new MoveDefEventNode {_parent = origEvent.Parent};

                    newEv.EventID = origEvent._event;
                    ActionEventInfo info = origEvent.EventInfo;

                    for (int i = 0; i < newEv.numArguments; i++)
                    {
                        int type = 0, value = 0;
                        if (origEvent.Children.Count > i)
                        {
                            type = (int) (origEvent.Children[i] as MoveDefEventParameterNode)._type;
                            value = (int) (origEvent.Children[i] as MoveDefEventParameterNode)._value;
                        }

                        newEv.NewParam(i, value, type);
                        if (type == (int) ArgVarType.Offset)
                        {
                            MoveDefEventOffsetNode oldoff = origEvent.Children[i] as MoveDefEventOffsetNode;
                            MoveDefEventOffsetNode newoff = newEv.Children[i] as MoveDefEventOffsetNode;
                            newoff.list = oldoff.list;
                            newoff.index = oldoff.index;
                            newoff.type = oldoff.type;
                            newoff.action = oldoff.action;
                        }
                    }
                }

                return newEv;
            }
        }

        public MoveDefEventParameterNode param;

        //Display the event's name, offset and parameters.
        public void DisplayEvent()
        {
            lstParameters.Items.Clear();
            cboType.SelectedIndex = -1;
            cboType.Text = "";
            cboType.Enabled = false;
            lblParamDescription.Text = "No Description Available.";

            PanelMode(panelMode.none);

            ActionEventInfo info = null;
            if (MoveDefNode.EventDictionary.ContainsKey(eventData._event))
            {
                info = MoveDefNode.EventDictionary[eventData._event];
            }

            if (info != null)
            {
                lblEventName.Text = info._name;
                lblParamDescription.Text = info._description;
            }

            lblEventId.Text = Helpers.Hex8(eventData._event);

            foreach (MoveDefEventParameterNode n in eventData.Children)
            {
                if (!string.IsNullOrEmpty(n.Name))
                {
                    lstParameters.Items.Add(n.Name);
                }
            }
        }

        //Display the selected parameter's value, type and description.
        private void DisplayParameter(int index)
        {
            param = eventData.Children[index] as MoveDefEventParameterNode;

            cboType.Enabled = true;
            try
            {
                cboType.SelectedIndex = (int) param._type;
            }
            catch
            {
                cboType.SelectedIndex = -1;
                cboType.Text = "(" + param._type + ")";
            }

            DisplayInType(param);

            lblParamDescription.Text = param.Description;
        }

        //Display the parameter's value according to its type.
        public void DisplayInType(MoveDefEventParameterNode value)
        {
            if (value is MoveDefEventOffsetNode)
            {
                PanelMode(panelMode.offsetPanel);
                _updating = true;
                MoveDefEventOffsetNode offset = value as MoveDefEventOffsetNode;

                comboBox1.SelectedIndex = offset.list;
                if (comboBox1.SelectedIndex != 4)
                {
                    if (offset.type != -1)
                    {
                        comboBox3.SelectedIndex = offset.type;
                    }

                    if (offset.index != -1)
                    {
                        comboBox2.SelectedIndex = offset.index;
                    }
                }
                else
                {
                    offset.type = -1;
                    offset.index = -1;
                }
 

                _updating = false;
            }
            else if (value is MoveDefEventRequirementNode)
            {
                PanelMode(panelMode.requirement);
                _updating = true;
                MoveDefEventRequirementNode offset = value as MoveDefEventRequirementNode;

                if (cboRequirement.SelectedIndex == -1)
                {

                    chkNot.Checked = offset.Not;
                    int convert = offset._value & 0x7FFFFFFF;
                    convert = (convert >= 9999) ? convert - 9999 + iReq9999loc : convert; //270F is 9999
                    cboRequirement.SelectedIndex = convert;
                }
            }
            else if (value is HitboxFlagsNode)
            {
                PanelMode(panelMode.hitboxPanel);
                _updating = true;
                refreshHitboxInfo(value as  HitboxFlagsNode);
            }
            else if (value is SpecialHitboxFlagsNode)
            {
                PanelMode(panelMode.spHitboxPanel);
                _updating = true;
                refreshSpHitboxInfo(value as SpecialHitboxFlagsNode);
            }
            else
            {
                PanelMode(panelMode.valueGrid);

                valueGrid.SelectedObject = value;
            }
        }

        private void refreshHitboxInfo(HitboxFlagsNode hitboxInfo)
        {
            comboHit_Type.SelectedIndex = (int)hitboxInfo.Type;
            comboHit_Attribute.SelectedIndex = (int)hitboxInfo.Effect;
            comboHit_Sound.SelectedIndex = (int)hitboxInfo.Sound;
            comboHit_SoundVol.SelectedIndex = (int)hitboxInfo.SoundVol;
            comboHit_check_Clank.Checked = hitboxInfo.Clang;
            comboHit_check_Rebound.Checked = hitboxInfo.Rebound;
            comboHit_check_Direct.Checked = hitboxInfo.Direct;
            comboHit_check_Hit_Air.Checked = hitboxInfo.Aerial;
            comboHit_check_Hit_Ground.Checked = hitboxInfo.Grounded;
            comboHit_check_Hit_Odd.Checked = hitboxInfo.Odd;
        }

        private void refreshSpHitboxInfo(SpecialHitboxFlagsNode hitboxInfo)
        {
            comboSpHit_KB_Box.SelectedIndex = hitboxInfo.AngleFlipping;

            comboSpHit_TargetBox.SetItemChecked(0, hitboxInfo.CanHitDamageableFloors);
            comboSpHit_TargetBox.SetItemChecked(1, hitboxInfo.SSE_CanHitDamageableItems);
            comboSpHit_TargetBox.SetItemChecked(2, hitboxInfo.CanHitDamageableWalls);
            comboSpHit_TargetBox.SetItemChecked(3, hitboxInfo.CanHitDestroyableObjects);
            comboSpHit_TargetBox.SetItemChecked(4, hitboxInfo.CanHitUnk3);
            comboSpHit_TargetBox.SetItemChecked(5, hitboxInfo.CanHitUnk2);
            comboSpHit_TargetBox.SetItemChecked(6, hitboxInfo.CanHitItems);
            comboSpHit_TargetBox.SetItemChecked(7, hitboxInfo.CanHitUnk1);
            comboSpHit_TargetBox.SetItemChecked(8, hitboxInfo.CanHitSSEenemies);
            comboSpHit_TargetBox.SetItemChecked(9, hitboxInfo.CanHitMultiplayerCharacters);

            comboSpHit_RegionBox.SetItemChecked(0, hitboxInfo.CanHitRegion_Leg);
            comboSpHit_RegionBox.SetItemChecked(1, hitboxInfo.CanHitRegion_Knee);
            comboSpHit_RegionBox.SetItemChecked(2, hitboxInfo.CanHitRegion_Arm);
            comboSpHit_RegionBox.SetItemChecked(3, hitboxInfo.CanHitRegion_Body);

            comboSpHit_BehaviorBox.SetItemChecked(0, hitboxInfo.Flinchless);
            comboSpHit_BehaviorBox.SetItemChecked(1, hitboxInfo.NoHitEffect);
            comboSpHit_BehaviorBox.SetItemChecked(2, hitboxInfo.FreezeFrameDisable);
            comboSpHit_BehaviorBox.SetItemChecked(3, hitboxInfo.IgnoreTeamSettings);
            comboSpHit_BehaviorBox.SetItemChecked(4, hitboxInfo.HittingGrippedCharacter);
            comboSpHit_BehaviorBox.SetItemChecked(5, hitboxInfo.IgnoreIntangibility);
            comboSpHit_BehaviorBox.SetItemChecked(6, hitboxInfo.IgnoreInvincibility);
            comboSpHit_BehaviorBox.SetItemChecked(7, hitboxInfo.CanBeAbsorbed);
            comboSpHit_BehaviorBox.SetItemChecked(8, hitboxInfo.CanBeReflected);
            comboSpHit_BehaviorBox.SetItemChecked(9, hitboxInfo.CanBeShielded);
            comboSpHit_BehaviorBox.SetItemChecked(10, hitboxInfo.Stretches);

            comboSpHit_BehaviorBox.SetItemChecked(11, hitboxInfo.Custom_Front);
            comboSpHit_BehaviorBox.SetItemChecked(12, hitboxInfo.Custom_KO_At_100);

            /*
            Currently Unused:

            hitboxInfo.Unk1
            hitboxInfo.Unk2
            hitboxInfo.CanHitUnk1
            hitboxInfo.CanHitUnk2
            hitboxInfo.CanHitUnk3
             */
        }

        enum panelMode
        {
            requirement,
            valueGrid,
            hitboxPanel,
            spHitboxPanel,
            offsetPanel,
            none,
        }
        private void PanelMode(panelMode mode)
        {
            requirementPanel.Visible = false;
            valueGrid.Visible = false;
            hitboxFlagPanel.Visible = false;
            spHitboxFlagPanel.Visible = false;
            offsetPanel.Visible = false;

            switch (mode)
            {
                case panelMode.requirement:
                    requirementPanel.Visible = true; break;
                case panelMode.valueGrid:
                    valueGrid.Visible = true; break;
                case panelMode.hitboxPanel:
                    hitboxFlagPanel.Visible = true; break;
                case panelMode.spHitboxPanel:
                    spHitboxFlagPanel.Visible = true; break;
                case panelMode.offsetPanel:
                    offsetPanel.Visible = true; break; 
            }
        }

        public bool _updating;

        public FormEventList frmEventList;

        private void btnChangeEvent_Click(object sender, EventArgs e)
        {
            //Pass in the event Event.
            frmEventList.eventEvent = eventData._event;
            frmEventList.p = eventData.Root;
            frmEventList.ShowDialog();

            //Retrieve and setup the new event according to the new event Event.
            if (frmEventList.status == DialogResult.OK)
            {
                newEv = new MoveDefEventNode {_parent = origEvent.Parent};

                newEvent.EventID = (uint) frmEventList.eventEvent;
                ActionEventInfo info = newEvent.EventInfo;

                newEvent.NewChildren();
                if (!frmEventList.resetParameter) //TODO: Reset Params check
                {
                    newEvent.Replace(origEvent, false);
                }
                else
                {
                    chkNot.Checked = false;
                }
                if (newEvent.Children.Count > 0)
                {
                    param = newEvent.Children[0] as MoveDefEventParameterNode;
                    cboRequirement.SelectedIndex = param._value & 0x7FFFFFFF;
                }
                else
                {
                    param = null;
                }
            }
            
            DisplayEvent();
        }

        private void lstParameters_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstParameters.SelectedIndex == -1)
            {
                return;
            };
            int index = lstParameters.SelectedIndex;
            DisplayParameter(index);
        }

        private void cboType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboType.SelectedIndex == -1)
            {
                return;
            }

            if (lstParameters.SelectedIndex == -1)
            {
                return;
            }

            int index = lstParameters.SelectedIndex;

            //Change the type to the type selected and update the view window.

            param = eventData.Children[index] as MoveDefEventParameterNode;

            if (param._type != (ArgVarType) cboType.SelectedIndex)
            {
                int ind = param.Index;
                ActionEventInfo info = eventData.EventInfo;
                string name = ((ArgVarType) cboType.SelectedIndex).ToString();

                int value = 0;

                MoveDefEventParameterNode p = newEvent.Children[ind] as MoveDefEventParameterNode;
                if (p is MoveDefEventValueNode || p is MoveDefEventScalarNode || p is MoveDefEventBoolNode)
                {
                    value = p._value;
                }

                newEvent.Children[ind].Remove();

                ArgVarType t = (ArgVarType) cboType.SelectedIndex;

                newEvent.NewParam(ind, value, (int) t);
            }

            DisplayParameter(index);
        }

        private void Requirement_Handle(object sender, EventArgs e)
        {
            
            if (cboRequirement.SelectedIndex == -1)
            {
                return;
            }

            if (lstParameters.SelectedIndex == -1)
            {
                return;
            }

            int index = lstParameters.SelectedIndex;
            long value = cboRequirement.SelectedIndex;
            if (index >= iReq9999loc)
                value += 9999 - iReq9999loc; //0x270F
            if (chkNot.Checked)
            {
                value |= 0x80000000;
            }

            (newEvent.Children[index] as MoveDefEventParameterNode)._value = (int) value;
        }

        public event EventHandler Completed;

        private void btnCancel_Click(object sender, EventArgs e)
        {
            status = DialogResult.Cancel;

            Completed?.Invoke(this, null);
        }

        private void btnDone_Click(object sender, EventArgs e)
        {
            if (newEv == null) //No changes were made.
            {
                btnCancel_Click(sender, e);
                return;
            }

            status = DialogResult.OK;
            int index = origEvent.Index;
            MoveDefActionNode action = origEvent.Parent as MoveDefActionNode;
            origEvent.Remove();
            action.InsertChild(newEvent, true, index);

            Completed?.Invoke(this, null);
        }

        public object _oldSelectedObject;

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
            {
                comboBox3.Items.Clear();
                comboBox3.Items.Add("Entry");
                comboBox3.Items.Add("Exit");

                comboBox2.Items.Clear();
                comboBox2.Items.AddRange(param.Root._actions.Children.ToArray());
            }

            if (comboBox1.SelectedIndex == 1 && param.Root.dataCommon == null) //TODO: Doesn't remove from Fighter.pac! Should.
            {
                comboBox3.Items.Clear();
                comboBox3.Items.Add("Main");
                comboBox3.Items.Add("GFX");
                comboBox3.Items.Add("SFX");
                comboBox3.Items.Add("Other");

                comboBox2.Items.Clear();
                comboBox2.Items.AddRange(param.Root._subActions.Children.ToArray());
            }

            if (comboBox1.SelectedIndex >= 2)
            {
                comboBox3.Visible = label4.Visible = false;
            }
            else
            {
                comboBox3.Visible = label4.Visible = true;
            }

            if (comboBox1.SelectedIndex == 4)
            {
                comboBox2.Visible = label3.Visible = label2.Visible = false;
            }
            else
            {
                comboBox2.Visible = label3.Visible = label2.Visible = true;
            }

            if (comboBox1.SelectedIndex == 2)
            {
                comboBox2.Items.Clear();
                comboBox2.Items.AddRange(param.Root._subRoutineList.ToArray());
            }

            if (comboBox1.SelectedIndex == 3)
            {
                comboBox2.Items.Clear();
                comboBox2.Items.AddRange(param.Root._externalRefs.ToArray());
            }
            if (comboBox1.SelectedIndex == 5 && param.Root.dataCommon != null)
            {
                comboBox2.Items.Clear();
                comboBox2.Items.AddRange(param.Root.dataCommon._screenTint.Children.ToArray());
            }
            if (comboBox1.SelectedIndex == 6 && param.Root.dataCommon != null)
            {
                comboBox2.Items.Clear(); 
                comboBox2.Items.AddRange(param.Root.dataCommon._flashOverlay.Children.ToArray());
            }
        }

        private void offsetOkay_Click(object sender, EventArgs e)
        {
            MoveDefEventOffsetNode _targetNode = param as MoveDefEventOffsetNode;
            if (_targetNode.action != null)
            {
                _targetNode._value = -1;
                _targetNode.action._actionRefs.Remove(param);
            }

            if (comboBox1.SelectedIndex >= 3)
            {
                if (comboBox1.SelectedIndex == 3 && comboBox2.SelectedIndex >= 0 &&
                    comboBox2.SelectedIndex < param.Root._externalRefs.Count)
                {
                    if (_targetNode.External)
                    {
                        _targetNode._extNode._refs.Remove(_targetNode);
                        _targetNode._extNode = null;
                    }

                    (param._extNode = param.Root._externalRefs[comboBox2.SelectedIndex] as MoveDefExternalNode)._refs
                        .Add(
                            param);
                }
            }
            else
            {
                if (param._extNode != null)
                {
                    param._extNode._refs.Remove(param);
                    param._extNode = null;
                }
            }

            _targetNode.list = comboBox1.SelectedIndex;
            _targetNode.type = comboBox1.SelectedIndex >= 2 ? -1 : comboBox3.SelectedIndex;
            _targetNode.index = comboBox1.SelectedIndex == 4 ? -1 : comboBox2.SelectedIndex;
            _targetNode.action = param.Root.GetAction(_targetNode.list, _targetNode.type, _targetNode.index);
            if (_targetNode.action != null)
            {
                param._value = _targetNode.action._offset;
                _targetNode.action._actionRefs.Add(param);
            }
            else
            {
                param._value = -1;
            }

            param.SignalPropertyChange();
        }

        private void lblName2_Click(object sender, EventArgs e)
        {

        }

        private void btnCancel_Click_1(object sender, EventArgs e)
        {
            (Parent as FormModifyEvent).Close();
        }

        private void btnDone_Click_1(object sender, EventArgs e) // Used to update the event modified.
        {
            FormModifyEvent parentWindow = (FormModifyEvent)Parent;

            int selectedEvent = parentWindow.handler.EventList.SelectedIndex;
            int topEvent = parentWindow.handler.EventList.TopIndex;
            origEvent.Replace(newEvent);
            parentWindow.handler.MakeScript();
            parentWindow.handler.EventList.SetSelected(selectedEvent, true);
            parentWindow.handler.EventList.TopIndex = topEvent;
            parentWindow.Close();
        }

        private void lblEventName_Click(object sender, EventArgs e) //Clicking the name of the event.
        {
            DisplayEvent();
        }

        private void lblEventId_Click(object sender, EventArgs e) //Clicking the ID of the event.
        {
            DisplayEvent();
        }

        private void EventModifier_Load(object sender, EventArgs e) //Initialize window
        {
            //Link the requirements!
            if (!parentDesignMode) //cheese a private variable and a stubborn IDE that hates the below conversion
            {
                cboRequirement.Items.Clear();
                cboRequirement.Items.AddRange((Parent as FormModifyEvent).handler.currentMoveDef.iRequirements);
                iReq9999loc = cboRequirement.Items.Count;
                cboRequirement.Items.AddRange((Parent as FormModifyEvent).handler.currentMoveDef.iRequirementsUniq);
                for (int i = 0; i < cboRequirement.Items.Count; i++)
                {
                    if ((string)cboRequirement.Items[i] == "270F")
                    {
                        iReq9999loc = i;
                        break;
                    }
                }
            }
        }

        #region Hitbox Flag Info Modification

        #region Hitbox Dropdown
        private void comboHit_Type_SelectedIndexChanged(object sender, EventArgs e)
        {
            HitboxFlagsNode hitboxInfo = param as HitboxFlagsNode;
            hitboxInfo.Type = (HitboxType)comboHit_Type.SelectedIndex;
            refreshHitboxInfo(hitboxInfo);
        }

        private void comboHit_Attribute_SelectedIndexChanged(object sender, EventArgs e)
        {
            HitboxFlagsNode hitboxInfo = param as HitboxFlagsNode;
            hitboxInfo.Effect = (HitboxEffect)comboHit_Attribute.SelectedIndex;
            refreshHitboxInfo(hitboxInfo);
        }

        private void comboHit_Sound_SelectedIndexChanged(object sender, EventArgs e)
        {
            HitboxFlagsNode hitboxInfo = param as HitboxFlagsNode;
            hitboxInfo.Sound = (HitboxSFX)comboHit_Sound.SelectedIndex;
            refreshHitboxInfo(hitboxInfo);
        }

        private void comboHit_SoundVol_SelectedIndexChanged(object sender, EventArgs e)
        {
            HitboxFlagsNode hitboxInfo = param as HitboxFlagsNode;
            hitboxInfo.SoundVol = (HitboxSFXLevel)comboHit_SoundVol.SelectedIndex;
            refreshHitboxInfo(hitboxInfo);
        }
        #endregion

        #region Hitbox Checks

        private void comboHit_check_Clank_CheckedChanged(object sender, EventArgs e)
        {
            HitboxFlagsNode hitboxInfo = param as HitboxFlagsNode;
            hitboxInfo.Clang = comboHit_check_Clank.Checked;
            refreshHitboxInfo(hitboxInfo);
        }

        private void comboHit_check_Rebound_CheckedChanged(object sender, EventArgs e)
        {
            HitboxFlagsNode hitboxInfo = param as HitboxFlagsNode;
            hitboxInfo.Rebound = comboHit_check_Rebound.Checked;
            refreshHitboxInfo(hitboxInfo);
        }
        private void comboHit_check_Direct_CheckedChanged(object sender, EventArgs e)
        {
            HitboxFlagsNode hitboxInfo = param as HitboxFlagsNode;
            hitboxInfo.Direct = comboHit_check_Direct.Checked;
            refreshHitboxInfo(hitboxInfo);
        }

        private void comboHit_check_Hit_Ground_CheckedChanged(object sender, EventArgs e)
        {
            HitboxFlagsNode hitboxInfo = param as HitboxFlagsNode;
            hitboxInfo.Grounded = comboHit_check_Hit_Ground.Checked;
            refreshHitboxInfo(hitboxInfo);
        }

        private void comboHit_check_Hit_Air_CheckedChanged(object sender, EventArgs e)
        {
            HitboxFlagsNode hitboxInfo = param as HitboxFlagsNode;
            hitboxInfo.Aerial = comboHit_check_Hit_Air.Checked;
            refreshHitboxInfo(hitboxInfo);
        }

        private void comboHit_check_Hit_Odd_CheckedChanged(object sender, EventArgs e)
        {
            HitboxFlagsNode hitboxInfo = param as HitboxFlagsNode;
            hitboxInfo.Odd = comboHit_check_Hit_Odd.Checked;
            refreshHitboxInfo(hitboxInfo);
        }

        #endregion


        #endregion

        #region Hitbox Special Flag Info Modification
        private void comboSpHit_KB_Box_SelectedIndexChanged(object sender, EventArgs e)
        {
            SpecialHitboxFlagsNode hitboxInfo = param as SpecialHitboxFlagsNode;
            hitboxInfo.AngleFlipping = comboSpHit_KB_Box.SelectedIndex;
        }

        private void comboSpHit_RegionBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SpecialHitboxFlagsNode hitboxInfo = param as SpecialHitboxFlagsNode;

            hitboxInfo.CanHitRegion_Leg = comboSpHit_RegionBox.GetItemChecked(0);
            hitboxInfo.CanHitRegion_Knee = comboSpHit_RegionBox.GetItemChecked(1);
            hitboxInfo.CanHitRegion_Arm = comboSpHit_RegionBox.GetItemChecked(2);
            hitboxInfo.CanHitRegion_Body = comboSpHit_RegionBox.GetItemChecked(3);
        }

        private void comboSpHit_TargetBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SpecialHitboxFlagsNode hitboxInfo = param as SpecialHitboxFlagsNode;
            hitboxInfo.CanHitDamageableFloors = comboSpHit_TargetBox.GetItemChecked(0);
            hitboxInfo.SSE_CanHitDamageableItems = comboSpHit_TargetBox.GetItemChecked(1);
            hitboxInfo.CanHitDamageableWalls = comboSpHit_TargetBox.GetItemChecked(2);
            hitboxInfo.CanHitDestroyableObjects = comboSpHit_TargetBox.GetItemChecked(3);
            hitboxInfo.CanHitUnk3 = comboSpHit_TargetBox.GetItemChecked(4);
            hitboxInfo.CanHitUnk2 = comboSpHit_TargetBox.GetItemChecked(5);
            hitboxInfo.CanHitItems = comboSpHit_TargetBox.GetItemChecked(6);
            hitboxInfo.CanHitUnk1 = comboSpHit_TargetBox.GetItemChecked(7);
            hitboxInfo.CanHitSSEenemies = comboSpHit_TargetBox.GetItemChecked(8);
            hitboxInfo.CanHitMultiplayerCharacters = comboSpHit_TargetBox.GetItemChecked(9);
        }

        private void comboSpHit_BehaviorBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SpecialHitboxFlagsNode hitboxInfo = param as SpecialHitboxFlagsNode;

            hitboxInfo.Flinchless = comboSpHit_BehaviorBox.GetItemChecked(0);
            hitboxInfo.NoHitEffect = comboSpHit_BehaviorBox.GetItemChecked(1);
            hitboxInfo.FreezeFrameDisable = comboSpHit_BehaviorBox.GetItemChecked(2);
            hitboxInfo.IgnoreTeamSettings = comboSpHit_BehaviorBox.GetItemChecked(3);
            hitboxInfo.HittingGrippedCharacter = comboSpHit_BehaviorBox.GetItemChecked(4);

            hitboxInfo.IgnoreIntangibility = comboSpHit_BehaviorBox.GetItemChecked(5);
            hitboxInfo.IgnoreInvincibility = comboSpHit_BehaviorBox.GetItemChecked(6);

            hitboxInfo.CanBeAbsorbed = comboSpHit_BehaviorBox.GetItemChecked(7);
            hitboxInfo.CanBeReflected = comboSpHit_BehaviorBox.GetItemChecked(8);
            hitboxInfo.CanBeShielded = comboSpHit_BehaviorBox.GetItemChecked(9);
            hitboxInfo.Stretches = comboSpHit_BehaviorBox.GetItemChecked(10);

            hitboxInfo.Custom_Front = comboSpHit_BehaviorBox.GetItemChecked(11);
            hitboxInfo.Custom_KO_At_100 = comboSpHit_BehaviorBox.GetItemChecked(12);

            //unk1 and unk2 available for future additions
            //CanHitUnk1, CanHitUnk2 and CanHitUnk3 are also available
        }
        private void combooSpHit_checkedBoxLeave(object sender, EventArgs e)
        {
            comboSpHit_BehaviorBox.ClearSelected();
            comboSpHit_RegionBox.ClearSelected();
            comboSpHit_TargetBox.ClearSelected();
        }
        #endregion


    }
}