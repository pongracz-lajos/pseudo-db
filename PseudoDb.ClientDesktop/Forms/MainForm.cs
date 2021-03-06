﻿using PseudoDb.ClientDesktop.Properties;
using PseudoDb.Engine;
using PseudoDb.Interfaces.Metadata;
using PseudoDb.Interfaces.Query;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace PseudoDb.ClientDesktop.Forms
{
    public partial class MainForm : Form 
    {
        private enum SelectedOperation
        {
            Unknown,
            Select,
            Delete
        }

        private TreeNode DatabaseTree;

        private DatabaseContext dbContext;

        private SelectedOperation selection;

        public MainForm()
        {
            InitializeComponent();

            dbContext = new DatabaseContext();

            BuildDatabaseTree();

            selectDataGridView.CellValueChanged += new DataGridViewCellEventHandler(selectDataGridView_CellValueChanged);
            filterDataGridView.CellValueChanged += new DataGridViewCellEventHandler(filterDataGridView_CellValueChanged);
            joinDataGridView.CellValueChanged += new DataGridViewCellEventHandler(joinDataGridView_CellValueChanged);
            queryDesignerTabControl.Visible = false;

            selection = SelectedOperation.Unknown;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void BuildDatabaseTree()
        {
            DatabaseTree = new TreeNode("Databases");

            foreach(var database in dbContext.SchemaQuery.GetDatabases())
            {
                TreeNode dbNode = new TreeNode(database.Name);

                foreach(var table in database.Tables)
                {
                    TreeNode tbNode = new TreeNode(table.Name);
                    dbNode.Nodes.Add(tbNode);
                }

                DatabaseTree.Nodes.Add(dbNode);
            }

            DatabaseTreeView.Nodes.Add(DatabaseTree);
            DatabaseTreeView.Refresh();

            // Add event handlers.
            DatabaseTreeView.NodeMouseClick += DatabaseTreeView_NodeMouseClick;
        }

        private void DatabaseTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            // Select the currently clicked item in the tree view.
            TreeNode sendeNode = ((TreeView)sender).SelectedNode;
            DatabaseTreeView.SelectedNode = e.Node;

            if (e.Button == MouseButtons.Right)
            {
                // Determine on which level was clicked.
                int clickLevel = e.Node.Level;
                ContextMenuStrip rightClickMenu = new ContextMenuStrip();

                switch(clickLevel)
                {
                    // Root node
                    case 0:
                        ToolStripMenuItem createDbItem = new ToolStripMenuItem("Create new database");
                        createDbItem.Click += OnCreateNewDbMenuItemClick;
                        rightClickMenu.Items.Add(createDbItem);
                        break;

                    // Database node
                    case 1:
                        ToolStripMenuItem actionWithDbItem = new ToolStripMenuItem("Delete database");
                        actionWithDbItem.Click += OnDeleteDbMenuItemClick;
                        rightClickMenu.Items.Add(actionWithDbItem);

                        actionWithDbItem = new ToolStripMenuItem("Create new table");
                        actionWithDbItem.Click += OnCreateNewTableMenuItemClick;
                        rightClickMenu.Items.Add(actionWithDbItem);
                        break;

                    // Table node
                    case 2:
                        ToolStripMenuItem actionWithTableItem;
                        actionWithTableItem = new ToolStripMenuItem("Select");
                        actionWithTableItem.Click += OnSelectFromTableMenuItemClick; ;
                        rightClickMenu.Items.Add(actionWithTableItem);

                        actionWithTableItem = new ToolStripMenuItem("Select all");
                        actionWithTableItem.Click += OnSelectAllFromTableMenuItemClick; ;
                        rightClickMenu.Items.Add(actionWithTableItem);

                        actionWithTableItem = new ToolStripMenuItem("Insert");
                        actionWithTableItem.Click += OnInsertIntoTableMenuItemClick; ;
                        rightClickMenu.Items.Add(actionWithTableItem);

                        actionWithTableItem = new ToolStripMenuItem("Delete");
                        actionWithTableItem.Click += OnDeleteFromTableMenuItemClick; ;
                        rightClickMenu.Items.Add(actionWithTableItem);

                        rightClickMenu.Items.Add(new ToolStripSeparator());

                        actionWithTableItem = new ToolStripMenuItem("Design table");
                        actionWithTableItem.Click += OnDesignTableMenuItemClick;
                        rightClickMenu.Items.Add(actionWithTableItem);

                        actionWithTableItem = new ToolStripMenuItem("Delete table");
                        actionWithTableItem.Click += OnDeleteTableMenuItemClick;
                        rightClickMenu.Items.Add(actionWithTableItem);

                        actionWithTableItem = new ToolStripMenuItem("Create index");
                        actionWithTableItem.Click += OnCreateIndexMenuItemClick;
                        rightClickMenu.Items.Add(actionWithTableItem);
                        break;
                }

                if (rightClickMenu.Items.Count > 0)
                {
                    rightClickMenu.Show(this, e.X, e.Y + 30);
                }
            }
        }

        private void OnSelectAllFromTableMenuItemClick(object sender, EventArgs e)
        {
            string selectedDbName = DatabaseTreeView.SelectedNode.Parent.Text.ToString();
            string selectedTableName = DatabaseTreeView.SelectedNode.Text.ToString();

            Database database = dbContext.SchemaQuery.GetDatabase(selectedDbName);
            Table tableSchema = database.GetTable(selectedTableName);
            DataTable resultTable = dbContext.Query.GetAll(database, tableSchema);

            resultDataGridView.DataSource = resultTable;
            resultDataGridView.Refresh();

            queryTabControl.SelectedIndex = 1;
        }

        private void OnCreateNewDbMenuItemClick(object sender, EventArgs e)
        {
            NewDatabaseForm newDatabaseForm = new NewDatabaseForm(dbContext);
            newDatabaseForm.ShowDialog(this);

            switch (newDatabaseForm.DialogResult)
            {
                case DialogResult.OK:
                    // Test if this database can be created!
                    if (newDatabaseForm.Database != null)
                    {
                        DatabaseTree.Nodes.Add(new TreeNode(newDatabaseForm.Database.Name));
                    }
                    else
                    {
                        MessageBox.Show(Resources.ResourceManager.GetString("DbCreateFail"), Resources.ResourceManager.GetString("DbCreateError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    break;
                case DialogResult.Cancel:
                    //MessageBox.Show("Cancel");
                    break;
            }
        }

        private void OnDeleteDbMenuItemClick(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Delete database?", "Delete database", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            switch (result)
            {
                case DialogResult.Yes:
                    dbContext.SchemaQuery.RemoveDatabase(DatabaseTreeView.SelectedNode.Text.ToString());
                    DatabaseTreeView.Nodes.Remove(DatabaseTreeView.SelectedNode);
                    break;
                case DialogResult.No:
                    break;
            }
        }

        private void OnCreateNewTableMenuItemClick(object sender, EventArgs e)
        {
            Database database = dbContext.SchemaQuery.GetDatabase(DatabaseTreeView.SelectedNode.Text.ToString());
            var newTableForm = new TableDesignForm(dbContext, database);
            newTableForm.ShowDialog(this);

            switch (newTableForm.DialogResult)
            {
                case DialogResult.OK:
                    dbContext.SchemaQuery.UpdateDatabase(database.Name);
                    Table table = newTableForm.GetTable();
                    DatabaseTreeView.SelectedNode.Nodes.Add(new TreeNode(table.Name));
                    break;
                default:
                    break;
            }
        }

        private void OnDesignTableMenuItemClick(object sender, EventArgs e)
        {
            Database database = dbContext.SchemaQuery.GetDatabase(DatabaseTreeView.SelectedNode.Parent.Text.ToString());
            var newTableForm = new TableDesignForm(dbContext, database, database.GetTable(DatabaseTreeView.SelectedNode.Text.ToString()));
            newTableForm.ShowDialog(this);

            switch (newTableForm.DialogResult)
            {
                case DialogResult.OK:
                    dbContext.SchemaQuery.UpdateDatabase(database.Name);
                    break;
                default:
                    break;
            }
        }

        private void OnDeleteTableMenuItemClick(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Delete table?", "Delete table", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            switch (result)
            {
                case DialogResult.Yes:
                    string databaseName = DatabaseTreeView.SelectedNode.Parent.Text.ToString();
                    Database database = dbContext.SchemaQuery.GetDatabase(databaseName);

                    Table table = database.GetTable(DatabaseTreeView.SelectedNode.Text.ToString());
                    if (table != null)
                    {
                        dbContext.Query.DeleteTable(database, table);
                        database.Tables.Remove(table);

                        dbContext.SchemaQuery.UpdateDatabase(database.Name);
                        DatabaseTreeView.Nodes.Remove(DatabaseTreeView.SelectedNode);
                    }
                    break;
                case DialogResult.No:
                    break;
            }
        }

        private void OnCreateIndexMenuItemClick(object sender, EventArgs e)
        {
            string selectedDbName = DatabaseTreeView.SelectedNode.Parent.Text.ToString();
            string selectedTableName = DatabaseTreeView.SelectedNode.Text.ToString();

            Database database = dbContext.SchemaQuery.GetDatabase(selectedDbName);
            Table tableSchema = database.GetTable(selectedTableName);

            var newIndexForm = new NewIndexForm(dbContext, database, tableSchema);
            newIndexForm.Show(this);

            switch (newIndexForm.DialogResult)
            {
                case DialogResult.OK:
                    break;
            }
        }

        private void OnInsertIntoTableMenuItemClick(object sender, EventArgs e)
        {
            string selectedDbName = DatabaseTreeView.SelectedNode.Parent.Text.ToString();
            string selectedTableName = DatabaseTreeView.SelectedNode.Text.ToString();

            Database database = dbContext.SchemaQuery.GetDatabase(selectedDbName);
            Table tableSchema = database.GetTable(selectedTableName);

            var insertForm = new InsertForm(dbContext, database, tableSchema);
            insertForm.Show(this);

            switch (insertForm.DialogResult)
            {
                case DialogResult.OK:
                    break;
            }
        }

        private void OnSelectFromTableMenuItemClick(object sender, EventArgs e)
        {
            queryDesignerTabControl.SelectedIndex = 0;
            string selectedTableName = DatabaseTreeView.SelectedNode.Text.ToString();

            // Populate selection table combo box from the data grid view with the possible tables.
            var tableColumnCombo = (DataGridViewComboBoxColumn)selectDataGridView.Columns["SelectTable"];
            tableColumnCombo.Items.Add(selectedTableName);

            // Populate filter table combo box from the data grid view with the possible tables.
            tableColumnCombo = (DataGridViewComboBoxColumn) filterDataGridView.Columns["Table"];
            tableColumnCombo.Items.Add(selectedTableName);

            // Populate join table combo box from the data grid view with the possible tables.
            //tableColumnCombo = (DataGridViewComboBoxColumn) joinDataGridView.Columns["LeftTable"];
            //tableColumnCombo.Items.Add(selectedTableName);

            executeToolStripButton.Enabled = true;
            cancelToolStripButton.Enabled = true;
            queryDesignerTabControl.Visible = true;

            selection = SelectedOperation.Select;
        }

        private void OnDeleteFromTableMenuItemClick(object sender, EventArgs e)
        {
            queryDesignerTabControl.SelectedIndex = 1;
            string selectedTableName = DatabaseTreeView.SelectedNode.Text.ToString();

            // Populate table combo box from the data grid view with the possible tables.
            var tableColumnCombo = (DataGridViewComboBoxColumn) filterDataGridView.Columns["Table"];
            tableColumnCombo.Items.Add(selectedTableName);

            executeToolStripButton.Enabled = true;
            cancelToolStripButton.Enabled = true;
            queryDesignerTabControl.Visible = true;

            selection = SelectedOperation.Delete;
        }

        private void selectDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            string selectedDbName = DatabaseTreeView.SelectedNode.Parent.Text.ToString();
            string selectedTableName = DatabaseTreeView.SelectedNode.Text.ToString();

            Database database = dbContext.SchemaQuery.GetDatabase(selectedDbName);
            Table table = database.GetTable(selectedTableName);

            if (e.ColumnIndex == 0)
            {
                var tableColumnCell = (DataGridViewComboBoxCell) selectDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
                var tableName = tableColumnCell.Value.ToString();

                var columnColumnCell = (DataGridViewComboBoxCell) selectDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex + 1];
                columnColumnCell.Items.Clear();
                columnColumnCell.Items.Add("*");
                columnColumnCell.Items.AddRange(table.Columns.Select(c => c.Name).ToArray());
            }
        }

        private void filterDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            string selectedDbName = DatabaseTreeView.SelectedNode.Parent.Text.ToString();
            string selectedTableName = DatabaseTreeView.SelectedNode.Text.ToString();

            Database database = dbContext.SchemaQuery.GetDatabase(selectedDbName);
            Table table = database.GetTable(selectedTableName);

            if (e.ColumnIndex == 0)
            {
                var tableColumnCell = (DataGridViewComboBoxCell) filterDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
                var tableName = tableColumnCell.Value.ToString();

                var columnColumnCell = (DataGridViewComboBoxCell) filterDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex + 1];
                columnColumnCell.Items.Clear();
                columnColumnCell.Items.AddRange(table.Columns.Select(c => c.Name).ToArray());
            }
        }

        private void joinDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void executeToolStripButton_Click(object sender, EventArgs e)
        {
            switch (selection)
            {
                case SelectedOperation.Select:
                    try
                    {
                        var selections = GetSelections();
                        var filters = GetFilters();
                        var joins = GetJoins();

                        string selectedDbName = DatabaseTreeView.SelectedNode.Parent.Text.ToString();
                        string selectedTableName = DatabaseTreeView.SelectedNode.Text.ToString();
                        Database database = dbContext.SchemaQuery.GetDatabase(selectedDbName);

                        DataTable resultTable = dbContext.Query.Select(database, selections, joins, filters);

                        resultDataGridView.DataSource = resultTable;
                        resultDataGridView.Refresh();

                        queryTabControl.SelectedIndex = 1;
                    }
                    catch (NullReferenceException exception)
                    {
                        MessageBox.Show("Complete all cells!\n" + exception.Message);
                    }
                    break;
                /*case SelectedOperation.Delete:
                    try
                    {
                        var filters = GetFilters();

                        string selectedDbName = DatabaseTreeView.SelectedNode.Parent.Text.ToString();
                        string selectedTableName = DatabaseTreeView.SelectedNode.Text.ToString();

                        Database database = dbContext.SchemaQuery.GetDatabase(selectedDbName);
                        Table table = database.GetTable(selectedTableName);

                        var status = dbContext.Query.Delete(database, table, filters);

                        messagesTextBox.Text = status.Message;
                        queryTabControl.SelectedIndex = 2;
                    }
                    catch (NullReferenceException exception)
                    {
                        MessageBox.Show("Complete all cells!\n" + exception.Message);
                    }

                    break;*/
            }

            ClearSelectDataGridView();
            ClearFilterDataGridView();
            ClearJoinDataGridView();

            executeToolStripButton.Enabled = false;
            cancelToolStripButton.Enabled = false;
            queryDesignerTabControl.Visible = false;

            selection = SelectedOperation.Unknown;
        }

        private ICollection<Selection> GetSelections()
        {
            var selections = new List<Selection>();

            var tableSelections = new Dictionary<string, List<string>>();

            for (int i = 0; i < selectDataGridView.Rows.Count - 1; i++)
            {
                var tableName = selectDataGridView.Rows[i].Cells[0].Value.ToString();
                var columnName = selectDataGridView.Rows[i].Cells[1].Value.ToString();

                if (!tableSelections.ContainsKey(tableName))
                {
                    tableSelections.Add(tableName, new List<string>());
                }
                tableSelections[tableName].Add(columnName);
            }

            foreach (var tableAsKey in tableSelections.Keys)
            {
                var selection = new Selection(tableAsKey, tableSelections[tableAsKey]);
                selections.Add(selection);
            }

            return selections;
        }

        private ICollection<Filter> GetFilters()
        {
            var filters = new List<Filter>();

            for (int i = 0; i < filterDataGridView.Rows.Count - 1; i++)
            {
                var tableName = filterDataGridView.Rows[i].Cells[0].Value.ToString();
                var columnName = filterDataGridView.Rows[i].Cells[1].Value.ToString();
                var operatorType = OperatorConverter.ToOperator(filterDataGridView.Rows[i].Cells[2].Value.ToString());
                var value = filterDataGridView.Rows[i].Cells[3].Value.ToString();

                var filter = new Filter(tableName, columnName, operatorType, value);
                filters.Add(filter);
            }

            return filters;
        }

        private ICollection<Join> GetJoins()
        {
            var joins = new List<Join>();

            for (int i = 0; i < joinDataGridView.Rows.Count - 1; i++)
            {
                var leftTableName = joinDataGridView.Rows[i].Cells[0].Value.ToString();
                var leftColumnName = joinDataGridView.Rows[i].Cells[1].Value.ToString();
                var rightTableName = joinDataGridView.Rows[i].Cells[2].Value.ToString();
                var rightColumnName = joinDataGridView.Rows[i].Cells[3].Value.ToString();
                var operatorType = OperatorConverter.ToOperator(filterDataGridView.Rows[i].Cells[4].Value.ToString());

                var join = new Join(leftTableName, leftColumnName, rightTableName, rightColumnName, operatorType);
                joins.Add(join);
            }

            return joins;
        }

        private void cancelToolStripButton_Click(object sender, EventArgs e)
        {
            ClearSelectDataGridView();
            ClearFilterDataGridView();
            ClearJoinDataGridView();

            executeToolStripButton.Enabled = false;
            cancelToolStripButton.Enabled = false;
            queryDesignerTabControl.Visible = false;

            selection = SelectedOperation.Unknown;
        }

        private void ClearSelectDataGridView()
        {
            selectDataGridView.Rows.Clear();

            var tableColumnCombo = (DataGridViewComboBoxColumn)selectDataGridView.Columns["SelectTable"];
            tableColumnCombo.Items.Clear();
        }

        private void ClearFilterDataGridView()
        {
            filterDataGridView.Rows.Clear();

            var tableColumnCombo = (DataGridViewComboBoxColumn)filterDataGridView.Columns["Table"];
            tableColumnCombo.Items.Clear();
        }

        private void ClearJoinDataGridView()
        {
            joinDataGridView.Rows.Clear();

            var tableColumnCombo = (DataGridViewComboBoxColumn)joinDataGridView.Columns["LeftTable"];
            tableColumnCombo.Items.Clear();

            tableColumnCombo = (DataGridViewComboBoxColumn)joinDataGridView.Columns["RightTable"];
            tableColumnCombo.Items.Clear();
        }
    }
}
