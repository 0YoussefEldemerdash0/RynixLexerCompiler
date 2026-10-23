using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace RynixLexerCompiler
{
    public partial class Form1 : Form
    {
        private DataTable _tokenTable = new DataTable();

        public Form1()
        {
            InitializeComponent();
            SetupTable();

            btnScan.Click += BtnScan_Click;
        }

        private void SetupTable()
        {
            _tokenTable = new DataTable();
            _tokenTable.Columns.Add("Line", typeof(int));
            _tokenTable.Columns.Add("Col", typeof(int));
            _tokenTable.Columns.Add("Lexeme", typeof(string));
            _tokenTable.Columns.Add("Token Class", typeof(string));

            dataGridViewTokens.DataSource = _tokenTable;
            dataGridViewTokens.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void BtnScan_Click(object? sender, EventArgs e)
        {
            _tokenTable.Clear();
            listBoxErrors.Items.Clear();

            string code = txtInputCode.Text;

            if (string.IsNullOrWhiteSpace(code))
            {
                MessageBox.Show("Please enter some Rynix code to scan!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Scanner scanner = new Scanner();
            ScanResult result = scanner.Scan(code);

            foreach (var token in result.Tokens)
            {
                _tokenTable.Rows.Add(token.Line, token.Column, token.Lexeme, token.Class.ToString());
            }

            foreach (var err in result.Errors)
            {
                listBoxErrors.Items.Add(err.ToString());
            }

            if (result.Errors.Count == 0)
            {
                MessageBox.Show("Scanning completed successfully with 0 errors!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}