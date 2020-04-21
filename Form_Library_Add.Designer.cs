namespace ABS_C
{
    partial class Form_Library_Add
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
            this.txt_name = new System.Windows.Forms.TextBox();
            this.txt_uroven3 = new System.Windows.Forms.TextBox();
            this.txt_uroven2 = new System.Windows.Forms.TextBox();
            this.btn_add = new System.Windows.Forms.Button();
            this.btn_cancel = new System.Windows.Forms.Button();
            this.cb_active = new System.Windows.Forms.CheckBox();
            this.cbm_en = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lab_uroven2 = new System.Windows.Forms.Label();
            this.lab_uroven3 = new System.Windows.Forms.Label();
            this.lab_en = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txt_name
            // 
            this.txt_name.Location = new System.Drawing.Point(131, 15);
            this.txt_name.Name = "txt_name";
            this.txt_name.Size = new System.Drawing.Size(315, 20);
            this.txt_name.TabIndex = 0;
            // 
            // txt_uroven3
            // 
            this.txt_uroven3.Location = new System.Drawing.Point(131, 67);
            this.txt_uroven3.Name = "txt_uroven3";
            this.txt_uroven3.Size = new System.Drawing.Size(315, 20);
            this.txt_uroven3.TabIndex = 1;
            // 
            // txt_uroven2
            // 
            this.txt_uroven2.Location = new System.Drawing.Point(131, 41);
            this.txt_uroven2.Name = "txt_uroven2";
            this.txt_uroven2.Size = new System.Drawing.Size(315, 20);
            this.txt_uroven2.TabIndex = 2;
            // 
            // btn_add
            // 
            this.btn_add.Location = new System.Drawing.Point(290, 136);
            this.btn_add.Name = "btn_add";
            this.btn_add.Size = new System.Drawing.Size(75, 23);
            this.btn_add.TabIndex = 3;
            this.btn_add.Text = "ОК";
            this.btn_add.UseVisualStyleBackColor = true;
            this.btn_add.Click += new System.EventHandler(this.Btn_add_Click);
            // 
            // btn_cancel
            // 
            this.btn_cancel.Location = new System.Drawing.Point(371, 136);
            this.btn_cancel.Name = "btn_cancel";
            this.btn_cancel.Size = new System.Drawing.Size(75, 23);
            this.btn_cancel.TabIndex = 4;
            this.btn_cancel.Text = "Отмена";
            this.btn_cancel.UseVisualStyleBackColor = true;
            this.btn_cancel.Click += new System.EventHandler(this.Btn_cancel_Click);
            // 
            // cb_active
            // 
            this.cb_active.AutoSize = true;
            this.cb_active.Location = new System.Drawing.Point(68, 136);
            this.cb_active.Name = "cb_active";
            this.cb_active.Size = new System.Drawing.Size(56, 17);
            this.cb_active.TabIndex = 5;
            this.cb_active.Text = "Актив";
            this.cb_active.UseVisualStyleBackColor = true;
            // 
            // cbm_en
            // 
            this.cbm_en.FormattingEnabled = true;
            this.cbm_en.Location = new System.Drawing.Point(131, 93);
            this.cbm_en.Name = "cbm_en";
            this.cbm_en.Size = new System.Drawing.Size(315, 21);
            this.cbm_en.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(38, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Наименование:";
            // 
            // lab_uroven2
            // 
            this.lab_uroven2.AutoSize = true;
            this.lab_uroven2.Location = new System.Drawing.Point(61, 44);
            this.lab_uroven2.Name = "lab_uroven2";
            this.lab_uroven2.Size = new System.Drawing.Size(63, 13);
            this.lab_uroven2.TabIndex = 8;
            this.lab_uroven2.Text = "Уровень 2:";
            // 
            // lab_uroven3
            // 
            this.lab_uroven3.AutoSize = true;
            this.lab_uroven3.Location = new System.Drawing.Point(61, 70);
            this.lab_uroven3.Name = "lab_uroven3";
            this.lab_uroven3.Size = new System.Drawing.Size(63, 13);
            this.lab_uroven3.TabIndex = 9;
            this.lab_uroven3.Text = "Уровень 3:";
            // 
            // lab_en
            // 
            this.lab_en.AutoSize = true;
            this.lab_en.Location = new System.Drawing.Point(12, 96);
            this.lab_en.Name = "lab_en";
            this.lab_en.Size = new System.Drawing.Size(112, 13);
            this.lab_en.TabIndex = 10;
            this.lab_en.Text = "Единица измерения:";
            // 
            // Form_Library_Add
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(470, 175);
            this.Controls.Add(this.lab_en);
            this.Controls.Add(this.lab_uroven3);
            this.Controls.Add(this.lab_uroven2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbm_en);
            this.Controls.Add(this.cb_active);
            this.Controls.Add(this.btn_cancel);
            this.Controls.Add(this.btn_add);
            this.Controls.Add(this.txt_uroven2);
            this.Controls.Add(this.txt_uroven3);
            this.Controls.Add(this.txt_name);
            this.Name = "Form_Library_Add";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.Form_Library_Add_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txt_name;
        private System.Windows.Forms.TextBox txt_uroven3;
        private System.Windows.Forms.TextBox txt_uroven2;
        private System.Windows.Forms.Button btn_add;
        private System.Windows.Forms.Button btn_cancel;
        private System.Windows.Forms.CheckBox cb_active;
        private System.Windows.Forms.ComboBox cbm_en;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lab_uroven2;
        private System.Windows.Forms.Label lab_uroven3;
        private System.Windows.Forms.Label lab_en;
    }
}