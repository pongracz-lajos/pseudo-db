﻿namespace PseudoDb.ClientDesktop
{
    partial class NewTableForm
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
            this.TableNameTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.CreateTableDataGridView = new System.Windows.Forms.DataGridView();
            this.FieldName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FIeldType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.PK = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.FK = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ReferencedTable = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.ReferencedField = new System.Windows.Forms.DataGridViewComboBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.CreateTableDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // TableNameTextBox
            // 
            this.TableNameTextBox.Location = new System.Drawing.Point(82, 21);
            this.TableNameTextBox.Name = "TableNameTextBox";
            this.TableNameTextBox.Size = new System.Drawing.Size(100, 20);
            this.TableNameTextBox.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Table name";
            // 
            // CreateTableDataGridView
            // 
            this.CreateTableDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.CreateTableDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.FieldName,
            this.FIeldType,
            this.PK,
            this.FK,
            this.ReferencedTable,
            this.ReferencedField});
            this.CreateTableDataGridView.Location = new System.Drawing.Point(15, 50);
            this.CreateTableDataGridView.Name = "CreateTableDataGridView";
            this.CreateTableDataGridView.Size = new System.Drawing.Size(645, 281);
            this.CreateTableDataGridView.TabIndex = 2;
            // 
            // FieldName
            // 
            this.FieldName.HeaderText = "Field name";
            this.FieldName.Name = "FieldName";
            // 
            // FIeldType
            // 
            this.FIeldType.HeaderText = "Type";
            this.FIeldType.Items.AddRange(new object[] {
            "Int",
            "String"});
            this.FIeldType.Name = "FIeldType";
            // 
            // PK
            // 
            this.PK.HeaderText = "PK";
            this.PK.Name = "PK";
            // 
            // FK
            // 
            this.FK.HeaderText = "FK";
            this.FK.Name = "FK";
            // 
            // ReferencedTable
            // 
            this.ReferencedTable.HeaderText = "Referenced Table";
            this.ReferencedTable.Items.AddRange(new object[] {
            "A",
            "B",
            "C"});
            this.ReferencedTable.Name = "ReferencedTable";
            // 
            // ReferencedField
            // 
            this.ReferencedField.HeaderText = "Referenced Field";
            this.ReferencedField.Name = "ReferencedField";
            // 
            // NewTableForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(786, 368);
            this.Controls.Add(this.CreateTableDataGridView);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TableNameTextBox);
            this.Name = "NewTableForm";
            this.Text = "NewTableForm";
            ((System.ComponentModel.ISupportInitialize)(this.CreateTableDataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TableNameTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView CreateTableDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn FieldName;
        private System.Windows.Forms.DataGridViewComboBoxColumn FIeldType;
        private System.Windows.Forms.DataGridViewCheckBoxColumn PK;
        private System.Windows.Forms.DataGridViewCheckBoxColumn FK;
        private System.Windows.Forms.DataGridViewComboBoxColumn ReferencedTable;
        private System.Windows.Forms.DataGridViewComboBoxColumn ReferencedField;
    }
}