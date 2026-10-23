namespace RynixLexerCompiler
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            txtInputCode = new RichTextBox();
            btnScan = new Button();
            dataGridViewTokens = new DataGridView();
            listBoxErrors = new ListBox();
            ((System.ComponentModel.ISupportInitialize)dataGridViewTokens).BeginInit();
            SuspendLayout();
            // 
            // txtInputCode
            // 
            txtInputCode.Location = new Point(12, 12);
            txtInputCode.Name = "txtInputCode";
            txtInputCode.Size = new Size(367, 363);
            txtInputCode.TabIndex = 0;
            txtInputCode.Text = "";
            // 
            // btnScan
            // 
            btnScan.Location = new Point(133, 406);
            btnScan.Name = "btnScan";
            btnScan.Size = new Size(94, 29);
            btnScan.TabIndex = 1;
            btnScan.Text = "button1";
            btnScan.UseVisualStyleBackColor = true;
            // 
            // dataGridViewTokens
            // 
            dataGridViewTokens.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewTokens.Location = new Point(403, 12);
            dataGridViewTokens.Name = "dataGridViewTokens";
            dataGridViewTokens.RowHeadersWidth = 51;
            dataGridViewTokens.Size = new Size(385, 275);
            dataGridViewTokens.TabIndex = 2;
            // 
            // listBoxErrors
            // 
            listBoxErrors.FormattingEnabled = true;
            listBoxErrors.Location = new Point(403, 311);
            listBoxErrors.Name = "listBoxErrors";
            listBoxErrors.Size = new Size(385, 124);
            listBoxErrors.TabIndex = 3;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(listBoxErrors);
            Controls.Add(dataGridViewTokens);
            Controls.Add(btnScan);
            Controls.Add(txtInputCode);
            Name = "Form1";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)dataGridViewTokens).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private RichTextBox txtInputCode;
        private Button btnScan;
        private DataGridView dataGridViewTokens;
        private ListBox listBoxErrors;
    }
}
