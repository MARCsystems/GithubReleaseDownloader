namespace GithubDownloaderTest
{
    partial class TestForm
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cmb_Extension = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txt_Progress = new System.Windows.Forms.TextBox();
            this.btn_StartQuery = new System.Windows.Forms.Button();
            this.btn_Browse = new System.Windows.Forms.Button();
            this.txt_PrivateRepoKey = new System.Windows.Forms.TextBox();
            this.txt_TempInstallerPath = new System.Windows.Forms.TextBox();
            this.txt_RepoName = new System.Windows.Forms.TextBox();
            this.txt_RepoOwner = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.fbd_downloadpath = new System.Windows.Forms.FolderBrowserDialog();
            this.dgv_Releases = new System.Windows.Forms.DataGridView();
            this.Column5 = new System.Windows.Forms.DataGridViewButtonColumn();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Releases)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cmb_Extension);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.txt_Progress);
            this.groupBox1.Controls.Add(this.btn_StartQuery);
            this.groupBox1.Controls.Add(this.btn_Browse);
            this.groupBox1.Controls.Add(this.txt_PrivateRepoKey);
            this.groupBox1.Controls.Add(this.txt_TempInstallerPath);
            this.groupBox1.Controls.Add(this.txt_RepoName);
            this.groupBox1.Controls.Add(this.txt_RepoOwner);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(302, 221);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "groupBox1";
            // 
            // cmb_Extension
            // 
            this.cmb_Extension.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmb_Extension.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_Extension.FormattingEnabled = true;
            this.cmb_Extension.Items.AddRange(new object[] {
            ".exe",
            ".zip",
            ".gz"});
            this.cmb_Extension.Location = new System.Drawing.Point(135, 120);
            this.cmb_Extension.Name = "cmb_Extension";
            this.cmb_Extension.Size = new System.Drawing.Size(161, 21);
            this.cmb_Extension.TabIndex = 12;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(44, 124);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(85, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Asset Extension:";
            // 
            // txt_Progress
            // 
            this.txt_Progress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_Progress.Location = new System.Drawing.Point(6, 195);
            this.txt_Progress.Name = "txt_Progress";
            this.txt_Progress.ReadOnly = true;
            this.txt_Progress.Size = new System.Drawing.Size(290, 20);
            this.txt_Progress.TabIndex = 10;
            // 
            // btn_StartQuery
            // 
            this.btn_StartQuery.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_StartQuery.Location = new System.Drawing.Point(6, 166);
            this.btn_StartQuery.Name = "btn_StartQuery";
            this.btn_StartQuery.Size = new System.Drawing.Size(290, 23);
            this.btn_StartQuery.TabIndex = 9;
            this.btn_StartQuery.Text = "Start Query";
            this.btn_StartQuery.UseVisualStyleBackColor = true;
            this.btn_StartQuery.Click += new System.EventHandler(this.btn_StartQuery_Click);
            // 
            // btn_Browse
            // 
            this.btn_Browse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Browse.Location = new System.Drawing.Point(267, 68);
            this.btn_Browse.Name = "btn_Browse";
            this.btn_Browse.Size = new System.Drawing.Size(29, 20);
            this.btn_Browse.TabIndex = 8;
            this.btn_Browse.Text = "...";
            this.btn_Browse.UseVisualStyleBackColor = true;
            this.btn_Browse.Click += new System.EventHandler(this.btn_Browse_Click);
            // 
            // txt_PrivateRepoKey
            // 
            this.txt_PrivateRepoKey.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_PrivateRepoKey.Location = new System.Drawing.Point(135, 94);
            this.txt_PrivateRepoKey.Name = "txt_PrivateRepoKey";
            this.txt_PrivateRepoKey.Size = new System.Drawing.Size(161, 20);
            this.txt_PrivateRepoKey.TabIndex = 7;
            // 
            // txt_TempInstallerPath
            // 
            this.txt_TempInstallerPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_TempInstallerPath.Location = new System.Drawing.Point(135, 68);
            this.txt_TempInstallerPath.Name = "txt_TempInstallerPath";
            this.txt_TempInstallerPath.ReadOnly = true;
            this.txt_TempInstallerPath.Size = new System.Drawing.Size(126, 20);
            this.txt_TempInstallerPath.TabIndex = 6;
            // 
            // txt_RepoName
            // 
            this.txt_RepoName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_RepoName.Location = new System.Drawing.Point(135, 42);
            this.txt_RepoName.Name = "txt_RepoName";
            this.txt_RepoName.Size = new System.Drawing.Size(161, 20);
            this.txt_RepoName.TabIndex = 5;
            // 
            // txt_RepoOwner
            // 
            this.txt_RepoOwner.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_RepoOwner.Location = new System.Drawing.Point(135, 16);
            this.txt_RepoOwner.Name = "txt_RepoOwner";
            this.txt_RepoOwner.Size = new System.Drawing.Size(161, 20);
            this.txt_RepoOwner.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 98);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(117, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Private Repository Key:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 72);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(123, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Temporary Update Path:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(38, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(91, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Repository Name:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(35, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(94, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Repository Owner:";
            // 
            // dgv_Releases
            // 
            this.dgv_Releases.AllowUserToAddRows = false;
            this.dgv_Releases.AllowUserToDeleteRows = false;
            this.dgv_Releases.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgv_Releases.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_Releases.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column5,
            this.Column1,
            this.Column2,
            this.Column3,
            this.Column4,
            this.Column6});
            this.dgv_Releases.Location = new System.Drawing.Point(320, 12);
            this.dgv_Releases.Name = "dgv_Releases";
            this.dgv_Releases.ReadOnly = true;
            this.dgv_Releases.RowHeadersVisible = false;
            this.dgv_Releases.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgv_Releases.Size = new System.Drawing.Size(504, 221);
            this.dgv_Releases.TabIndex = 1;
            this.dgv_Releases.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_Releases_CellContentClick);
            // 
            // Column5
            // 
            this.Column5.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.Column5.HeaderText = "🧊";
            this.Column5.Name = "Column5";
            this.Column5.ReadOnly = true;
            this.Column5.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Column5.Width = 25;
            // 
            // Column1
            // 
            this.Column1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.Column1.HeaderText = "Version";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Column1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column1.Width = 48;
            // 
            // Column2
            // 
            this.Column2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.Column2.HeaderText = "Release Date";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            this.Column2.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Column2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column2.Width = 78;
            // 
            // Column3
            // 
            this.Column3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.Column3.HeaderText = "Asset Name";
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            this.Column3.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Column3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column3.Width = 70;
            // 
            // Column4
            // 
            this.Column4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column4.HeaderText = "Hash";
            this.Column4.Name = "Column4";
            this.Column4.ReadOnly = true;
            this.Column4.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Column4.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Column6
            // 
            this.Column6.HeaderText = "DownloadURL";
            this.Column6.Name = "Column6";
            this.Column6.ReadOnly = true;
            this.Column6.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Column6.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column6.Visible = false;
            // 
            // TestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(836, 245);
            this.Controls.Add(this.dgv_Releases);
            this.Controls.Add(this.groupBox1);
            this.Name = "TestForm";
            this.Text = "Github Download Test Form";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Releases)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt_PrivateRepoKey;
        private System.Windows.Forms.TextBox txt_TempInstallerPath;
        private System.Windows.Forms.TextBox txt_RepoName;
        private System.Windows.Forms.TextBox txt_RepoOwner;
        private System.Windows.Forms.Button btn_Browse;
        private System.Windows.Forms.TextBox txt_Progress;
        private System.Windows.Forms.Button btn_StartQuery;
        private System.Windows.Forms.FolderBrowserDialog fbd_downloadpath;
        private System.Windows.Forms.DataGridView dgv_Releases;
        private System.Windows.Forms.ComboBox cmb_Extension;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DataGridViewButtonColumn Column5;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column6;
    }
}

