namespace BizDataLayerGen
{
    partial class frmTablesShow
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
            this.btnGenerate = new Guna.UI2.WinForms.Guna2Button();
            this.guna2HtmlLabel2 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.chBAllTables = new System.Windows.Forms.CheckBox();
            this.rbAll = new System.Windows.Forms.RadioButton();
            this.rbJustThis = new System.Windows.Forms.RadioButton();
            this.rbAddingStaticMethodsYes = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rbAddingStaticMethodsNo = new System.Windows.Forms.RadioButton();
            this.guna2CircleButton1 = new Guna.UI2.WinForms.Guna2CircleButton();
            this.LBTables = new System.Windows.Forms.CheckedListBox();
            this.switchAutoExcuteSP = new Guna.UI2.WinForms.Guna2ToggleSwitch();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.ckGenerateAPI = new Guna.UI2.WinForms.Guna2ToggleSwitch();
            this.label3 = new System.Windows.Forms.Label();
            this.ckAiCodeDocs = new Guna.UI2.WinForms.Guna2ToggleSwitch();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.switchUsingDTO = new Guna.UI2.WinForms.Guna2ToggleSwitch();
            this.progressBar = new Guna.UI2.WinForms.Guna2ProgressBar();
            this.lbCurrentFile = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.rbBoth = new Guna.UI2.WinForms.Guna2RadioButton();
            this.rbAsynchronous = new Guna.UI2.WinForms.Guna2RadioButton();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnGenerate
            // 
            this.btnGenerate.BorderRadius = 20;
            this.btnGenerate.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(86)))), ((int)(((byte)(142)))), ((int)(((byte)(163)))));
            this.btnGenerate.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGenerate.ForeColor = System.Drawing.Color.White;
            this.btnGenerate.Image = global::BizDataLayerGen.Properties.Resources.magic_wand;
            this.btnGenerate.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnGenerate.Location = new System.Drawing.Point(549, 636);
            this.btnGenerate.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(215, 59);
            this.btnGenerate.TabIndex = 13;
            this.btnGenerate.Text = "    Generate";
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // guna2HtmlLabel2
            // 
            this.guna2HtmlLabel2.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel2.Font = new System.Drawing.Font("Arial Narrow", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(86)))), ((int)(((byte)(142)))), ((int)(((byte)(163)))));
            this.guna2HtmlLabel2.Location = new System.Drawing.Point(243, 15);
            this.guna2HtmlLabel2.Margin = new System.Windows.Forms.Padding(8, 6, 8, 6);
            this.guna2HtmlLabel2.Name = "guna2HtmlLabel2";
            this.guna2HtmlLabel2.Size = new System.Drawing.Size(352, 71);
            this.guna2HtmlLabel2.TabIndex = 14;
            this.guna2HtmlLabel2.Text = "Choose Tables";
            // 
            // chBAllTables
            // 
            this.chBAllTables.AutoSize = true;
            this.chBAllTables.Checked = true;
            this.chBAllTables.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chBAllTables.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chBAllTables.Location = new System.Drawing.Point(56, 122);
            this.chBAllTables.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chBAllTables.Name = "chBAllTables";
            this.chBAllTables.Size = new System.Drawing.Size(195, 29);
            this.chBAllTables.TabIndex = 15;
            this.chBAllTables.Text = "Choose All Tables";
            this.chBAllTables.UseVisualStyleBackColor = true;
            this.chBAllTables.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // rbAll
            // 
            this.rbAll.AutoSize = true;
            this.rbAll.Checked = true;
            this.rbAll.Font = new System.Drawing.Font("Microsoft Tai Le", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbAll.Location = new System.Drawing.Point(31, 33);
            this.rbAll.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rbAll.Name = "rbAll";
            this.rbAll.Size = new System.Drawing.Size(56, 29);
            this.rbAll.TabIndex = 18;
            this.rbAll.TabStop = true;
            this.rbAll.Text = "All";
            this.rbAll.UseVisualStyleBackColor = true;
            // 
            // rbJustThis
            // 
            this.rbJustThis.AutoSize = true;
            this.rbJustThis.Font = new System.Drawing.Font("Microsoft Tai Le", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbJustThis.Location = new System.Drawing.Point(159, 33);
            this.rbJustThis.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rbJustThis.Name = "rbJustThis";
            this.rbJustThis.Size = new System.Drawing.Size(105, 29);
            this.rbJustThis.TabIndex = 19;
            this.rbJustThis.Text = "Just This";
            this.rbJustThis.UseVisualStyleBackColor = true;
            // 
            // rbAddingStaticMethodsYes
            // 
            this.rbAddingStaticMethodsYes.AutoSize = true;
            this.rbAddingStaticMethodsYes.Checked = true;
            this.rbAddingStaticMethodsYes.Font = new System.Drawing.Font("Microsoft Tai Le", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbAddingStaticMethodsYes.Location = new System.Drawing.Point(31, 33);
            this.rbAddingStaticMethodsYes.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rbAddingStaticMethodsYes.Name = "rbAddingStaticMethodsYes";
            this.rbAddingStaticMethodsYes.Size = new System.Drawing.Size(62, 29);
            this.rbAddingStaticMethodsYes.TabIndex = 21;
            this.rbAddingStaticMethodsYes.TabStop = true;
            this.rbAddingStaticMethodsYes.Text = "Yes";
            this.rbAddingStaticMethodsYes.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbJustThis);
            this.groupBox1.Controls.Add(this.rbAll);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Tai Le", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(860, 112);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Size = new System.Drawing.Size(352, 85);
            this.groupBox1.TabIndex = 23;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Searching FK OF:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rbAddingStaticMethodsNo);
            this.groupBox2.Controls.Add(this.rbAddingStaticMethodsYes);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Tai Le", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(860, 223);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox2.Size = new System.Drawing.Size(352, 84);
            this.groupBox2.TabIndex = 24;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Adding Static Methods:";
            // 
            // rbAddingStaticMethodsNo
            // 
            this.rbAddingStaticMethodsNo.AutoSize = true;
            this.rbAddingStaticMethodsNo.Font = new System.Drawing.Font("Microsoft Tai Le", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbAddingStaticMethodsNo.Location = new System.Drawing.Point(159, 33);
            this.rbAddingStaticMethodsNo.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rbAddingStaticMethodsNo.Name = "rbAddingStaticMethodsNo";
            this.rbAddingStaticMethodsNo.Size = new System.Drawing.Size(60, 29);
            this.rbAddingStaticMethodsNo.TabIndex = 20;
            this.rbAddingStaticMethodsNo.Text = "No";
            this.rbAddingStaticMethodsNo.UseVisualStyleBackColor = true;
            this.rbAddingStaticMethodsNo.CheckedChanged += new System.EventHandler(this.rbAddingStaticMethodsNo_CheckedChanged);
            // 
            // guna2CircleButton1
            // 
            this.guna2CircleButton1.BackgroundImage = global::BizDataLayerGen.Properties.Resources.Cancel;
            this.guna2CircleButton1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.guna2CircleButton1.FillColor = System.Drawing.Color.Transparent;
            this.guna2CircleButton1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.guna2CircleButton1.ForeColor = System.Drawing.Color.White;
            this.guna2CircleButton1.Location = new System.Drawing.Point(1153, 15);
            this.guna2CircleButton1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.guna2CircleButton1.Name = "guna2CircleButton1";
            this.guna2CircleButton1.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            this.guna2CircleButton1.Size = new System.Drawing.Size(47, 43);
            this.guna2CircleButton1.TabIndex = 25;
            this.guna2CircleButton1.Click += new System.EventHandler(this.guna2CircleButton1_Click);
            // 
            // LBTables
            // 
            this.LBTables.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.LBTables.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(112)))));
            this.LBTables.FormattingEnabled = true;
            this.LBTables.Location = new System.Drawing.Point(56, 159);
            this.LBTables.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.LBTables.Name = "LBTables";
            this.LBTables.Size = new System.Drawing.Size(472, 178);
            this.LBTables.Sorted = true;
            this.LBTables.TabIndex = 16;
            this.LBTables.SelectedIndexChanged += new System.EventHandler(this.LBTables_SelectedIndexChanged);
            // 
            // switchAutoExcuteSP
            // 
            this.switchAutoExcuteSP.CheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.switchAutoExcuteSP.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.switchAutoExcuteSP.CheckedState.InnerBorderColor = System.Drawing.Color.White;
            this.switchAutoExcuteSP.CheckedState.InnerColor = System.Drawing.Color.White;
            this.switchAutoExcuteSP.Location = new System.Drawing.Point(29, 131);
            this.switchAutoExcuteSP.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.switchAutoExcuteSP.Name = "switchAutoExcuteSP";
            this.switchAutoExcuteSP.Size = new System.Drawing.Size(47, 25);
            this.switchAutoExcuteSP.TabIndex = 26;
            this.switchAutoExcuteSP.UncheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
            this.switchAutoExcuteSP.UncheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
            this.switchAutoExcuteSP.UncheckedState.InnerBorderColor = System.Drawing.Color.White;
            this.switchAutoExcuteSP.UncheckedState.InnerColor = System.Drawing.Color.White;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.ckGenerateAPI);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.ckAiCodeDocs);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.switchUsingDTO);
            this.groupBox3.Controls.Add(this.switchAutoExcuteSP);
            this.groupBox3.Font = new System.Drawing.Font("Microsoft Tai Le", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(860, 315);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox3.Size = new System.Drawing.Size(361, 213);
            this.groupBox3.TabIndex = 25;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Pro Things:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(92, 81);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(125, 25);
            this.label4.TabIndex = 33;
            this.label4.Text = "Generate API";
            // 
            // ckGenerateAPI
            // 
            this.ckGenerateAPI.Checked = true;
            this.ckGenerateAPI.CheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.ckGenerateAPI.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.ckGenerateAPI.CheckedState.InnerBorderColor = System.Drawing.Color.White;
            this.ckGenerateAPI.CheckedState.InnerColor = System.Drawing.Color.White;
            this.ckGenerateAPI.Enabled = false;
            this.ckGenerateAPI.Location = new System.Drawing.Point(31, 81);
            this.ckGenerateAPI.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ckGenerateAPI.Name = "ckGenerateAPI";
            this.ckGenerateAPI.Size = new System.Drawing.Size(47, 25);
            this.ckGenerateAPI.TabIndex = 32;
            this.ckGenerateAPI.UncheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
            this.ckGenerateAPI.UncheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
            this.ckGenerateAPI.UncheckedState.InnerBorderColor = System.Drawing.Color.White;
            this.ckGenerateAPI.UncheckedState.InnerColor = System.Drawing.Color.White;
            this.ckGenerateAPI.CheckedChanged += new System.EventHandler(this.ckGenerateAPI_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(94, 169);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(129, 25);
            this.label3.TabIndex = 31;
            this.label3.Text = "AI Code Docs";
            // 
            // ckAiCodeDocs
            // 
            this.ckAiCodeDocs.CheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.ckAiCodeDocs.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.ckAiCodeDocs.CheckedState.InnerBorderColor = System.Drawing.Color.White;
            this.ckAiCodeDocs.CheckedState.InnerColor = System.Drawing.Color.White;
            this.ckAiCodeDocs.Location = new System.Drawing.Point(31, 171);
            this.ckAiCodeDocs.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ckAiCodeDocs.Name = "ckAiCodeDocs";
            this.ckAiCodeDocs.Size = new System.Drawing.Size(47, 25);
            this.ckAiCodeDocs.TabIndex = 30;
            this.ckAiCodeDocs.UncheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
            this.ckAiCodeDocs.UncheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
            this.ckAiCodeDocs.UncheckedState.InnerBorderColor = System.Drawing.Color.White;
            this.ckAiCodeDocs.UncheckedState.InnerColor = System.Drawing.Color.White;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(94, 33);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 25);
            this.label2.TabIndex = 29;
            this.label2.Text = "Use DTO";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(92, 129);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(153, 25);
            this.label1.TabIndex = 28;
            this.label1.Text = "Auto Execute SP";
            // 
            // switchUsingDTO
            // 
            this.switchUsingDTO.Checked = true;
            this.switchUsingDTO.CheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.switchUsingDTO.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.switchUsingDTO.CheckedState.InnerBorderColor = System.Drawing.Color.White;
            this.switchUsingDTO.CheckedState.InnerColor = System.Drawing.Color.White;
            this.switchUsingDTO.Enabled = false;
            this.switchUsingDTO.Location = new System.Drawing.Point(31, 34);
            this.switchUsingDTO.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.switchUsingDTO.Name = "switchUsingDTO";
            this.switchUsingDTO.Size = new System.Drawing.Size(47, 25);
            this.switchUsingDTO.TabIndex = 27;
            this.switchUsingDTO.UncheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
            this.switchUsingDTO.UncheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
            this.switchUsingDTO.UncheckedState.InnerBorderColor = System.Drawing.Color.White;
            this.switchUsingDTO.UncheckedState.InnerColor = System.Drawing.Color.White;
            this.switchUsingDTO.CheckedChanged += new System.EventHandler(this.switchUsingDTO_CheckedChanged);
            // 
            // progressBar
            // 
            this.progressBar.BorderRadius = 10;
            this.progressBar.Location = new System.Drawing.Point(56, 491);
            this.progressBar.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.progressBar.Name = "progressBar";
            this.progressBar.ProgressColor = System.Drawing.Color.FromArgb(((int)(((byte)(86)))), ((int)(((byte)(142)))), ((int)(((byte)(163)))));
            this.progressBar.ProgressColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(86)))), ((int)(((byte)(142)))), ((int)(((byte)(163)))));
            this.progressBar.Size = new System.Drawing.Size(484, 37);
            this.progressBar.TabIndex = 26;
            this.progressBar.Text = "guna2ProgressBar1";
            this.progressBar.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            // 
            // lbCurrentFile
            // 
            this.lbCurrentFile.BackColor = System.Drawing.Color.Transparent;
            this.lbCurrentFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCurrentFile.Location = new System.Drawing.Point(56, 449);
            this.lbCurrentFile.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.lbCurrentFile.Name = "lbCurrentFile";
            this.lbCurrentFile.Size = new System.Drawing.Size(99, 27);
            this.lbCurrentFile.TabIndex = 27;
            this.lbCurrentFile.Text = "CurrentFile";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.rbBoth);
            this.groupBox4.Controls.Add(this.rbAsynchronous);
            this.groupBox4.Font = new System.Drawing.Font("Microsoft Tai Le", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox4.Location = new System.Drawing.Point(860, 546);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox4.Size = new System.Drawing.Size(352, 149);
            this.groupBox4.TabIndex = 34;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Execution Mode:";
            // 
            // rbBoth
            // 
            this.rbBoth.AutoSize = true;
            this.rbBoth.Checked = true;
            this.rbBoth.CheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.rbBoth.CheckedState.BorderThickness = 0;
            this.rbBoth.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.rbBoth.CheckedState.InnerColor = System.Drawing.Color.White;
            this.rbBoth.CheckedState.InnerOffset = -4;
            this.rbBoth.Location = new System.Drawing.Point(31, 54);
            this.rbBoth.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.rbBoth.Name = "rbBoth";
            this.rbBoth.Size = new System.Drawing.Size(179, 29);
            this.rbBoth.TabIndex = 40;
            this.rbBoth.TabStop = true;
            this.rbBoth.Text = "Both Sync/Async";
            this.rbBoth.UncheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
            this.rbBoth.UncheckedState.BorderThickness = 2;
            this.rbBoth.UncheckedState.FillColor = System.Drawing.Color.Transparent;
            this.rbBoth.UncheckedState.InnerColor = System.Drawing.Color.Transparent;
            this.rbBoth.CheckedChanged += new System.EventHandler(this.rbBoth_CheckedChanged);
            // 
            // rbAsynchronous
            // 
            this.rbAsynchronous.AutoSize = true;
            this.rbAsynchronous.CheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.rbAsynchronous.CheckedState.BorderThickness = 0;
            this.rbAsynchronous.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.rbAsynchronous.CheckedState.InnerColor = System.Drawing.Color.White;
            this.rbAsynchronous.CheckedState.InnerOffset = -4;
            this.rbAsynchronous.Location = new System.Drawing.Point(31, 90);
            this.rbAsynchronous.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.rbAsynchronous.Name = "rbAsynchronous";
            this.rbAsynchronous.Size = new System.Drawing.Size(130, 29);
            this.rbAsynchronous.TabIndex = 35;
            this.rbAsynchronous.Text = "Async Only";
            this.rbAsynchronous.UncheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
            this.rbAsynchronous.UncheckedState.BorderThickness = 2;
            this.rbAsynchronous.UncheckedState.FillColor = System.Drawing.Color.Transparent;
            this.rbAsynchronous.UncheckedState.InnerColor = System.Drawing.Color.Transparent;
            this.rbAsynchronous.CheckedChanged += new System.EventHandler(this.rbAsynchronous_CheckedChanged);
            // 
            // frmTablesShow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.HighlightText;
            this.ClientSize = new System.Drawing.Size(1271, 708);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.lbCurrentFile);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.guna2CircleButton1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.LBTables);
            this.Controls.Add(this.chBAllTables);
            this.Controls.Add(this.guna2HtmlLabel2);
            this.Controls.Add(this.btnGenerate);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "frmTablesShow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Tables Shows";
            this.Load += new System.EventHandler(this.frmTablesShow_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Guna.UI2.WinForms.Guna2Button btnGenerate;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel2;
        private System.Windows.Forms.CheckBox chBAllTables;
        private System.Windows.Forms.RadioButton rbAll;
        private System.Windows.Forms.RadioButton rbJustThis;
        private System.Windows.Forms.RadioButton rbAddingStaticMethodsYes;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rbAddingStaticMethodsNo;
        private Guna.UI2.WinForms.Guna2CircleButton guna2CircleButton1;
        private System.Windows.Forms.CheckedListBox LBTables;
        private Guna.UI2.WinForms.Guna2ToggleSwitch switchAutoExcuteSP;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label1;
        private Guna.UI2.WinForms.Guna2ToggleSwitch switchUsingDTO;
        private System.Windows.Forms.Label label2;
        private Guna.UI2.WinForms.Guna2ToggleSwitch ckAiCodeDocs;
        private System.Windows.Forms.Label label3;
        private Guna.UI2.WinForms.Guna2ProgressBar progressBar;
        private Guna.UI2.WinForms.Guna2HtmlLabel lbCurrentFile;
        private System.Windows.Forms.Label label4;
        private Guna.UI2.WinForms.Guna2ToggleSwitch ckGenerateAPI;
        private System.Windows.Forms.GroupBox groupBox4;
        private Guna.UI2.WinForms.Guna2RadioButton rbAsynchronous;
        private Guna.UI2.WinForms.Guna2RadioButton rbBoth;
    }
}