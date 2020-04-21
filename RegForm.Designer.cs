namespace ABS_C
{
    partial class RegForm
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
            this.Reg_Label_Login = new System.Windows.Forms.Label();
            this.Reg_Label_Password = new System.Windows.Forms.Label();
            this.Reg_Textbox_Password = new System.Windows.Forms.TextBox();
            this.Reg_Button_Ok = new System.Windows.Forms.Button();
            this.Reg_Button_Chancel = new System.Windows.Forms.Button();
            this.Reg_PickBox_icon = new System.Windows.Forms.PictureBox();
            this.ComboBox_Reg_Login = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.Reg_PickBox_icon)).BeginInit();
            this.SuspendLayout();
            // 
            // Reg_Label_Login
            // 
            this.Reg_Label_Login.AutoSize = true;
            this.Reg_Label_Login.Location = new System.Drawing.Point(131, 31);
            this.Reg_Label_Login.Name = "Reg_Label_Login";
            this.Reg_Label_Login.Size = new System.Drawing.Size(38, 13);
            this.Reg_Label_Login.TabIndex = 1;
            this.Reg_Label_Login.Text = "Логин";
            // 
            // Reg_Label_Password
            // 
            this.Reg_Label_Password.AutoSize = true;
            this.Reg_Label_Password.Location = new System.Drawing.Point(131, 62);
            this.Reg_Label_Password.Name = "Reg_Label_Password";
            this.Reg_Label_Password.Size = new System.Drawing.Size(45, 13);
            this.Reg_Label_Password.TabIndex = 2;
            this.Reg_Label_Password.Text = "Пароль";
            // 
            // Reg_Textbox_Password
            // 
            this.Reg_Textbox_Password.Location = new System.Drawing.Point(187, 59);
            this.Reg_Textbox_Password.Name = "Reg_Textbox_Password";
            this.Reg_Textbox_Password.PasswordChar = '*';
            this.Reg_Textbox_Password.Size = new System.Drawing.Size(232, 20);
            this.Reg_Textbox_Password.TabIndex = 4;
            this.Reg_Textbox_Password.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Reg_Textbox_Password_KeyDown);
            // 
            // Reg_Button_Ok
            // 
            this.Reg_Button_Ok.Location = new System.Drawing.Point(257, 103);
            this.Reg_Button_Ok.Name = "Reg_Button_Ok";
            this.Reg_Button_Ok.Size = new System.Drawing.Size(75, 23);
            this.Reg_Button_Ok.TabIndex = 5;
            this.Reg_Button_Ok.Text = "Вход";
            this.Reg_Button_Ok.UseVisualStyleBackColor = true;
            this.Reg_Button_Ok.Click += new System.EventHandler(this.Reg_Button_Ok_Click);
            this.Reg_Button_Ok.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Reg_Button_Ok_KeyDown);
            // 
            // Reg_Button_Chancel
            // 
            this.Reg_Button_Chancel.Location = new System.Drawing.Point(344, 103);
            this.Reg_Button_Chancel.Name = "Reg_Button_Chancel";
            this.Reg_Button_Chancel.Size = new System.Drawing.Size(75, 23);
            this.Reg_Button_Chancel.TabIndex = 6;
            this.Reg_Button_Chancel.Text = "Выход";
            this.Reg_Button_Chancel.UseVisualStyleBackColor = true;
            this.Reg_Button_Chancel.Click += new System.EventHandler(this.Reg_Button_Chancel_Click);
            // 
            // Reg_PickBox_icon
            // 
            this.Reg_PickBox_icon.Image = global::ABS_C.Properties.Resources.logo_ico;
            this.Reg_PickBox_icon.Location = new System.Drawing.Point(12, 12);
            this.Reg_PickBox_icon.Name = "Reg_PickBox_icon";
            this.Reg_PickBox_icon.Size = new System.Drawing.Size(103, 114);
            this.Reg_PickBox_icon.TabIndex = 0;
            this.Reg_PickBox_icon.TabStop = false;
            // 
            // ComboBox_Reg_Login
            // 
            this.ComboBox_Reg_Login.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ComboBox_Reg_Login.FormattingEnabled = true;
            this.ComboBox_Reg_Login.Location = new System.Drawing.Point(187, 28);
            this.ComboBox_Reg_Login.Name = "ComboBox_Reg_Login";
            this.ComboBox_Reg_Login.Size = new System.Drawing.Size(232, 24);
            this.ComboBox_Reg_Login.TabIndex = 7;
            this.ComboBox_Reg_Login.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ComboBox_Reg_Login_KeyDown);
            // 
            // RegForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(439, 139);
            this.Controls.Add(this.ComboBox_Reg_Login);
            this.Controls.Add(this.Reg_Button_Chancel);
            this.Controls.Add(this.Reg_Button_Ok);
            this.Controls.Add(this.Reg_Textbox_Password);
            this.Controls.Add(this.Reg_Label_Password);
            this.Controls.Add(this.Reg_Label_Login);
            this.Controls.Add(this.Reg_PickBox_icon);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(455, 178);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(455, 178);
            this.Name = "RegForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Регистрация";
            this.Load += new System.EventHandler(this.RegForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.Reg_PickBox_icon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox Reg_PickBox_icon;
        private System.Windows.Forms.Label Reg_Label_Login;
        private System.Windows.Forms.Label Reg_Label_Password;
        private System.Windows.Forms.TextBox Reg_Textbox_Password;
        private System.Windows.Forms.Button Reg_Button_Ok;
        private System.Windows.Forms.Button Reg_Button_Chancel;
        private System.Windows.Forms.ComboBox ComboBox_Reg_Login;
    }
}