namespace Reference_Larchenko
{
    partial class FrmChangeMenu
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
            this.btnDepart = new System.Windows.Forms.Button();
            this.btnSpec = new System.Windows.Forms.Button();
            this.btnObj = new System.Windows.Forms.Button();
            this.btnBack = new System.Windows.Forms.Button();
            this.btnSetting = new System.Windows.Forms.Button();
            this.btnUser = new System.Windows.Forms.Button();
            this.btnSpecialization = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnDepart
            // 
            this.btnDepart.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.28571F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnDepart.Location = new System.Drawing.Point(39, 24);
            this.btnDepart.Name = "btnDepart";
            this.btnDepart.Size = new System.Drawing.Size(314, 78);
            this.btnDepart.TabIndex = 0;
            this.btnDepart.Text = "РЕДАГУВАТИ ВІДДІЛЕННЯ\r\n";
            this.btnDepart.UseVisualStyleBackColor = true;
            this.btnDepart.Click += new System.EventHandler(this.btnDepart_Click);
            // 
            // btnSpec
            // 
            this.btnSpec.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.28571F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnSpec.Location = new System.Drawing.Point(39, 126);
            this.btnSpec.Name = "btnSpec";
            this.btnSpec.Size = new System.Drawing.Size(314, 78);
            this.btnSpec.TabIndex = 1;
            this.btnSpec.Text = "РЕДАГУВАТИ СПЕЦІАЛЬНОСТІ";
            this.btnSpec.UseVisualStyleBackColor = true;
            this.btnSpec.Click += new System.EventHandler(this.btnSpec_Click);
            // 
            // btnObj
            // 
            this.btnObj.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.28571F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnObj.Location = new System.Drawing.Point(400, 24);
            this.btnObj.Name = "btnObj";
            this.btnObj.Size = new System.Drawing.Size(314, 78);
            this.btnObj.TabIndex = 2;
            this.btnObj.Text = "РЕДАГУВАТИ ДИСЦИПЛІНИ";
            this.btnObj.UseVisualStyleBackColor = true;
            this.btnObj.Click += new System.EventHandler(this.btnObj_Click);
            // 
            // btnBack
            // 
            this.btnBack.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.28571F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnBack.Location = new System.Drawing.Point(219, 334);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(314, 78);
            this.btnBack.TabIndex = 3;
            this.btnBack.Text = "ПОВЕРНУТИСЯ";
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // btnSetting
            // 
            this.btnSetting.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.28571F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnSetting.Location = new System.Drawing.Point(400, 230);
            this.btnSetting.Name = "btnSetting";
            this.btnSetting.Size = new System.Drawing.Size(314, 78);
            this.btnSetting.TabIndex = 3;
            this.btnSetting.Text = "РОЗШИРЕНІ НАЛАШТУВАННЯ";
            this.btnSetting.UseVisualStyleBackColor = true;
            this.btnSetting.Click += new System.EventHandler(this.btnSetting_Click);
            // 
            // btnUser
            // 
            this.btnUser.Location = new System.Drawing.Point(400, 126);
            this.btnUser.Name = "btnUser";
            this.btnUser.Size = new System.Drawing.Size(314, 78);
            this.btnUser.TabIndex = 4;
            this.btnUser.Text = "РЕДАГУВАТИ КОРИСТУВАЧІВ";
            this.btnUser.UseVisualStyleBackColor = true;
            this.btnUser.Click += new System.EventHandler(this.btnUser_Click);
            // 
            // btnSpecialization
            // 
            this.btnSpecialization.Location = new System.Drawing.Point(39, 230);
            this.btnSpecialization.Name = "btnSpecialization";
            this.btnSpecialization.Size = new System.Drawing.Size(314, 78);
            this.btnSpecialization.TabIndex = 5;
            this.btnSpecialization.Text = "РЕДАГУВАТИ СПЕЦІАЛІЗАЦІЇ";
            this.btnSpecialization.UseVisualStyleBackColor = true;
            this.btnSpecialization.Click += new System.EventHandler(this.btnSpecialization_Click);
            // 
            // FrmChangeMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(752, 436);
            this.Controls.Add(this.btnSpecialization);
            this.Controls.Add(this.btnUser);
            this.Controls.Add(this.btnSetting);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.btnObj);
            this.Controls.Add(this.btnSpec);
            this.Controls.Add(this.btnDepart);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.28571F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmChangeMenu";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Вибір редагування (Адміністратор)";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmChangeMenu_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnDepart;
        private System.Windows.Forms.Button btnSpec;
        private System.Windows.Forms.Button btnObj;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Button btnSetting;
        private System.Windows.Forms.Button btnUser;
        private System.Windows.Forms.Button btnSpecialization;
    }
}