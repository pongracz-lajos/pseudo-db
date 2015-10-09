﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PseudoDb.ClientDesktop
{
    public partial class NewDatabaseForm : Form
    {
        //TODO: handle wrong imputs
        public string DatabaseName { get; set; }

        public NewDatabaseForm()
        {
            InitializeComponent();
            this.DialogResult = DialogResult.Ignore;
        }

        private void createButton_Click(object sender, EventArgs e)
        {
            DatabaseName = databaseNameTextBox.Text.ToString();
            this.DialogResult = DialogResult.OK;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}