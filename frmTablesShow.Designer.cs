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
            this.LBTables = new System.Windows.Forms.CheckedListBox();
            this.rbAll = new System.Windows.Forms.RadioButton();
            this.rbJustThis = new System.Windows.Forms.RadioButton();
            this.rbAddingStaticMethodsYes = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rbAddingStaticMethodsNo = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnGenerate
            // 
            this.btnGenerate.BorderRadius = 15;
            this.btnGenerate.CheckedState.Parent = this.btnGenerate;
            this.btnGenerate.CustomImages.Parent = this.btnGenerate;
            this.btnGenerate.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(86)))), ((int)(((byte)(142)))), ((int)(((byte)(163)))));
            this.btnGenerate.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGenerate.ForeColor = System.Drawing.Color.White;
            this.btnGenerate.HoverState.Parent = this.btnGenerate;
            this.btnGenerate.Image = global::BizDataLayerGen.Properties.Resources.magic_wand;
            this.btnGenerate.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnGenerate.Location = new System.Drawing.Point(248, 309);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.ShadowDecoration.Parent = this.btnGenerate;
            this.btnGenerate.Size = new System.Drawing.Size(161, 48);
            this.btnGenerate.TabIndex = 13;
            this.btnGenerate.Text = "    Generate";
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // guna2HtmlLabel2
            // 
            this.guna2HtmlLabel2.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel2.Font = new System.Drawing.Font("Arial Narrow", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(86)))), ((int)(((byte)(142)))), ((int)(((byte)(163)))));
            this.guna2HtmlLabel2.Location = new System.Drawing.Point(179, 14);
            this.guna2HtmlLabel2.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.guna2HtmlLabel2.Name = "guna2HtmlLabel2";
            this.guna2HtmlLabel2.Size = new System.Drawing.Size(283, 59);
            this.guna2HtmlLabel2.TabIndex = 14;
            this.guna2HtmlLabel2.Text = "Choose Tables";
            // 
            // chBAllTables
            // 
            this.chBAllTables.AutoSize = true;
            this.chBAllTables.Checked = true;
            this.chBAllTables.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chBAllTables.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chBAllTables.Location = new System.Drawing.Point(26, 110);
            this.chBAllTables.Name = "chBAllTables";
            this.chBAllTables.Size = new System.Drawing.Size(155, 24);
            this.chBAllTables.TabIndex = 15;
            this.chBAllTables.Text = "Choose All Tables";
            this.chBAllTables.UseVisualStyleBackColor = true;
            this.chBAllTables.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // LBTables
            // 
            this.LBTables.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.LBTables.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(112)))));
            this.LBTables.FormattingEnabled = true;
            this.LBTables.Location = new System.Drawing.Point(26, 140);
            this.LBTables.Name = "LBTables";
            this.LBTables.Size = new System.Drawing.Size(355, 148);
            this.LBTables.Sorted = true;
            this.LBTables.TabIndex = 16;
            this.LBTables.SelectedIndexChanged += new System.EventHandler(this.LBTables_SelectedIndexChanged);
            // 
            // rbAll
            // 
            this.rbAll.AutoSize = true;
            this.rbAll.Checked = true;
            this.rbAll.Font = new System.Drawing.Font("Microsoft Tai Le", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbAll.Location = new System.Drawing.Point(23, 27);
            this.rbAll.Name = "rbAll";
            this.rbAll.Size = new System.Drawing.Size(46, 25);
            this.rbAll.TabIndex = 18;
            this.rbAll.TabStop = true;
            this.rbAll.Text = "All";
            this.rbAll.UseVisualStyleBackColor = true;
            // 
            // rbJustThis
            // 
            this.rbJustThis.AutoSize = true;
            this.rbJustThis.Font = new System.Drawing.Font("Microsoft Tai Le", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbJustThis.Location = new System.Drawing.Point(87, 27);
            this.rbJustThis.Name = "rbJustThis";
            this.rbJustThis.Size = new System.Drawing.Size(87, 25);
            this.rbJustThis.TabIndex = 19;
            this.rbJustThis.Text = "Just This";
            this.rbJustThis.UseVisualStyleBackColor = true;
            // 
            // rbAddingStaticMethodsYes
            // 
            this.rbAddingStaticMethodsYes.AutoSize = true;
            this.rbAddingStaticMethodsYes.Checked = true;
            this.rbAddingStaticMethodsYes.Font = new System.Drawing.Font("Microsoft Tai Le", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbAddingStaticMethodsYes.Location = new System.Drawing.Point(23, 27);
            this.rbAddingStaticMethodsYes.Name = "rbAddingStaticMethodsYes";
            this.rbAddingStaticMethodsYes.Size = new System.Drawing.Size(52, 25);
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
            this.groupBox1.Location = new System.Drawing.Point(387, 98);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 69);
            this.groupBox1.TabIndex = 23;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Searching FK OF:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rbAddingStaticMethodsNo);
            this.groupBox2.Controls.Add(this.rbAddingStaticMethodsYes);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Tai Le", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(387, 188);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(200, 68);
            this.groupBox2.TabIndex = 24;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Adding Static Methods:";
            // 
            // rbAddingStaticMethodsNo
            // 
            this.rbAddingStaticMethodsNo.AutoSize = true;
            this.rbAddingStaticMethodsNo.Font = new System.Drawing.Font("Microsoft Tai Le", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbAddingStaticMethodsNo.Location = new System.Drawing.Point(100, 27);
            this.rbAddingStaticMethodsNo.Name = "rbAddingStaticMethodsNo";
            this.rbAddingStaticMethodsNo.Size = new System.Drawing.Size(49, 25);
            this.rbAddingStaticMethodsNo.TabIndex = 20;
            this.rbAddingStaticMethodsNo.Text = "No";
            this.rbAddingStaticMethodsNo.UseVisualStyleBackColor = true;
            this.rbAddingStaticMethodsNo.CheckedChanged += new System.EventHandler(this.rbAddingStaticMethodsNo_CheckedChanged);
            // 
            // frmTablesShow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(608, 379);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.LBTables);
            this.Controls.Add(this.chBAllTables);
            this.Controls.Add(this.guna2HtmlLabel2);
            this.Controls.Add(this.btnGenerate);
            this.Name = "frmTablesShow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Tables Shows";
            this.Load += new System.EventHandler(this.frmTablesShow_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Guna.UI2.WinForms.Guna2Button btnGenerate;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel2;
        private System.Windows.Forms.CheckBox chBAllTables;
        private System.Windows.Forms.CheckedListBox LBTables;
        private System.Windows.Forms.RadioButton rbAll;
        private System.Windows.Forms.RadioButton rbJustThis;
        private System.Windows.Forms.RadioButton rbAddingStaticMethodsYes;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rbAddingStaticMethodsNo;
    }
}