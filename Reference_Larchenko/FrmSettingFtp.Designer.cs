namespace Reference_Larchenko
{
    partial class FrmSettingFtp
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
            this.tbLogin = new System.Windows.Forms.TextBox();
            this.tbPassword = new System.Windows.Forms.TextBox();
            this.btnSaveFtp = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tbIpAddress = new IPAddressControlLib.IPAddressControl();
            this.btnCheckConn = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.cbNoAuth = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnLocalBackupInstall = new System.Windows.Forms.Button();
            this.btnUploadBackup = new System.Windows.Forms.Button();
            this.btnCreateBackup = new System.Windows.Forms.Button();
            this.btnLocalBackup = new System.Windows.Forms.Button();
            this.btnBack = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbLogin
            // 
            this.tbLogin.Location = new System.Drawing.Point(97, 87);
            this.tbLogin.Margin = new System.Windows.Forms.Padding(4);
            this.tbLogin.Name = "tbLogin";
            this.tbLogin.Size = new System.Drawing.Size(200, 26);
            this.tbLogin.TabIndex = 0;
            // 
            // tbPassword
            // 
            this.tbPassword.Location = new System.Drawing.Point(97, 129);
            this.tbPassword.Margin = new System.Windows.Forms.Padding(4);
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.PasswordChar = '*';
            this.tbPassword.Size = new System.Drawing.Size(200, 26);
            this.tbPassword.TabIndex = 0;
            // 
            // btnSaveFtp
            // 
            this.btnSaveFtp.Enabled = false;
            this.btnSaveFtp.Location = new System.Drawing.Point(74, 307);
            this.btnSaveFtp.Margin = new System.Windows.Forms.Padding(4);
            this.btnSaveFtp.Name = "btnSaveFtp";
            this.btnSaveFtp.Size = new System.Drawing.Size(195, 50);
            this.btnSaveFtp.TabIndex = 2;
            this.btnSaveFtp.Text = "Зберегти";
            this.btnSaveFtp.UseVisualStyleBackColor = true;
            this.btnSaveFtp.Click += new System.EventHandler(this.btnSaveFtp_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 42);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 20);
            this.label1.TabIndex = 3;
            this.label1.Text = "IP адрес";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tbIpAddress);
            this.groupBox1.Controls.Add(this.btnCheckConn);
            this.groupBox1.Controls.Add(this.btnUpdate);
            this.groupBox1.Controls.Add(this.btnSaveFtp);
            this.groupBox1.Controls.Add(this.cbNoAuth);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.tbLogin);
            this.groupBox1.Controls.Add(this.tbPassword);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(329, 449);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Налаштування FTP";
            // 
            // tbIpAddress
            // 
            this.tbIpAddress.AllowInternalTab = false;
            this.tbIpAddress.AutoHeight = true;
            this.tbIpAddress.BackColor = System.Drawing.SystemColors.Window;
            this.tbIpAddress.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.tbIpAddress.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.tbIpAddress.Location = new System.Drawing.Point(97, 42);
            this.tbIpAddress.Name = "tbIpAddress";
            this.tbIpAddress.ReadOnly = false;
            this.tbIpAddress.Size = new System.Drawing.Size(200, 26);
            this.tbIpAddress.TabIndex = 7;
            this.tbIpAddress.Text = "...";
            // 
            // btnCheckConn
            // 
            this.btnCheckConn.Location = new System.Drawing.Point(74, 231);
            this.btnCheckConn.Name = "btnCheckConn";
            this.btnCheckConn.Size = new System.Drawing.Size(195, 50);
            this.btnCheckConn.TabIndex = 6;
            this.btnCheckConn.Text = "Перевірити з\'єднання";
            this.btnCheckConn.UseVisualStyleBackColor = true;
            this.btnCheckConn.Click += new System.EventHandler(this.btnCheckConn_Click);
            // 
            // btnUpdate
            // 
            this.btnUpdate.Location = new System.Drawing.Point(74, 383);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(195, 50);
            this.btnUpdate.TabIndex = 6;
            this.btnUpdate.Text = "Редагувати";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // cbNoAuth
            // 
            this.cbNoAuth.AutoSize = true;
            this.cbNoAuth.Location = new System.Drawing.Point(97, 170);
            this.cbNoAuth.Name = "cbNoAuth";
            this.cbNoAuth.Size = new System.Drawing.Size(202, 24);
            this.cbNoAuth.TabIndex = 4;
            this.cbNoAuth.Text = "без логіну та пароля";
            this.cbNoAuth.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 129);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 20);
            this.label3.TabIndex = 3;
            this.label3.Text = "Пароль";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 87);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "Логін";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnLocalBackupInstall);
            this.groupBox2.Controls.Add(this.btnUploadBackup);
            this.groupBox2.Controls.Add(this.btnCreateBackup);
            this.groupBox2.Controls.Add(this.btnLocalBackup);
            this.groupBox2.Location = new System.Drawing.Point(349, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(316, 449);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Резервне копіювання даних";
            // 
            // btnLocalBackupInstall
            // 
            this.btnLocalBackupInstall.Location = new System.Drawing.Point(29, 124);
            this.btnLocalBackupInstall.Name = "btnLocalBackupInstall";
            this.btnLocalBackupInstall.Size = new System.Drawing.Size(260, 60);
            this.btnLocalBackupInstall.TabIndex = 2;
            this.btnLocalBackupInstall.Text = "Встановити існуючу копію даних на комп\'ютері";
            this.btnLocalBackupInstall.UseVisualStyleBackColor = true;
            this.btnLocalBackupInstall.Click += new System.EventHandler(this.btnLocalBackupInstall_Click);
            // 
            // btnUploadBackup
            // 
            this.btnUploadBackup.Location = new System.Drawing.Point(29, 297);
            this.btnUploadBackup.Name = "btnUploadBackup";
            this.btnUploadBackup.Size = new System.Drawing.Size(260, 60);
            this.btnUploadBackup.TabIndex = 1;
            this.btnUploadBackup.Text = "Завантажити останню резервну копію з сервера";
            this.btnUploadBackup.UseVisualStyleBackColor = true;
            this.btnUploadBackup.Click += new System.EventHandler(this.btnUploadBackup_Click);
            // 
            // btnCreateBackup
            // 
            this.btnCreateBackup.Location = new System.Drawing.Point(29, 211);
            this.btnCreateBackup.Name = "btnCreateBackup";
            this.btnCreateBackup.Size = new System.Drawing.Size(260, 60);
            this.btnCreateBackup.TabIndex = 1;
            this.btnCreateBackup.Text = "Зробити резервну копію на сервер";
            this.btnCreateBackup.UseVisualStyleBackColor = true;
            this.btnCreateBackup.Click += new System.EventHandler(this.btnCreateBackup_Click);
            // 
            // btnLocalBackup
            // 
            this.btnLocalBackup.Location = new System.Drawing.Point(29, 39);
            this.btnLocalBackup.Name = "btnLocalBackup";
            this.btnLocalBackup.Size = new System.Drawing.Size(260, 60);
            this.btnLocalBackup.TabIndex = 0;
            this.btnLocalBackup.Text = "Зробити резервну копію на цьому комп\'ютері";
            this.btnLocalBackup.UseVisualStyleBackColor = true;
            this.btnLocalBackup.Click += new System.EventHandler(this.btnLocalBackup_Click);
            // 
            // btnBack
            // 
            this.btnBack.Location = new System.Drawing.Point(250, 474);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(195, 50);
            this.btnBack.TabIndex = 6;
            this.btnBack.Text = "Повернутись";
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // FrmSettingFtp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(677, 540);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.HelpButton = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmSettingFtp";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Розширені налаштування (Адміністратор)";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmSettingFtp_FormClosing);
            this.Load += new System.EventHandler(this.FrmSettingFtp_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox tbLogin;
        private System.Windows.Forms.TextBox tbPassword;
        private System.Windows.Forms.Button btnSaveFtp;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.CheckBox cbNoAuth;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnUploadBackup;
        private System.Windows.Forms.Button btnCreateBackup;
        private System.Windows.Forms.Button btnLocalBackup;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Button btnCheckConn;
        private IPAddressControlLib.IPAddressControl tbIpAddress;
        private System.Windows.Forms.Button btnLocalBackupInstall;
    }
}