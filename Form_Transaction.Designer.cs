namespace ABS_C
{
    partial class Form_Transaction
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.DGW_list = new System.Windows.Forms.DataGridView();
            this.panel3 = new System.Windows.Forms.Panel();
            this.txt_messanger = new System.Windows.Forms.TextBox();
            this.txt_Edit_User = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DGW_list)).BeginInit();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.DGW_list);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(200, 714);
            this.panel1.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.txt_Edit_User);
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(200, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1046, 714);
            this.panel2.TabIndex = 1;
            // 
            // DGW_list
            // 
            this.DGW_list.BackgroundColor = System.Drawing.Color.White;
            this.DGW_list.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGW_list.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DGW_list.Location = new System.Drawing.Point(0, 0);
            this.DGW_list.Name = "DGW_list";
            this.DGW_list.Size = new System.Drawing.Size(200, 714);
            this.DGW_list.TabIndex = 0;
            this.DGW_list.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DGW_list_CellClick);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.txt_messanger);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(0, 56);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1046, 658);
            this.panel3.TabIndex = 1;
            // 
            // txt_messanger
            // 
            this.txt_messanger.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_messanger.Location = new System.Drawing.Point(6, 3);
            this.txt_messanger.Multiline = true;
            this.txt_messanger.Name = "txt_messanger";
            this.txt_messanger.Size = new System.Drawing.Size(1037, 652);
            this.txt_messanger.TabIndex = 0;
            // 
            // txt_Edit_User
            // 
            this.txt_Edit_User.Enabled = false;
            this.txt_Edit_User.Location = new System.Drawing.Point(76, 12);
            this.txt_Edit_User.Name = "txt_Edit_User";
            this.txt_Edit_User.Size = new System.Drawing.Size(251, 20);
            this.txt_Edit_User.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Сотрудник:";
            // 
            // Form_Transaction
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1246, 714);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1262, 753);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(1262, 753);
            this.Name = "Form_Transaction";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.Form_Transaction_Load);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DGW_list)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataGridView DGW_list;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.TextBox txt_messanger;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt_Edit_User;
    }
}