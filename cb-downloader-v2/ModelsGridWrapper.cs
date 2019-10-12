using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace cb_downloader_v2
{
    class ModelsGridWrapper
    {
        private readonly DataGridView _grid;

        public ModelsGridWrapper(DataGridView grid)
        {
            _grid = grid;
        }

        public void SortByModelNameAscending()
        {
            _grid.Sort(_grid.Columns[0], ListSortDirection.Ascending);
        }

        public int? SelectedIndex => _grid.CurrentRow?.Index;

        public IEnumerable<string> SelectedModelNames => _grid.SelectedRows.Cast<DataGridViewRow>()
            .Select(r => r.Cells[0].Value.ToString());

        public void SetStatus(string modelName, Status status)
        {
            var row = GetRow(modelName);

            if (row == null)
            {
                return;
            }
            _grid.Invoke((MethodInvoker)(() => row.Cells[1].Value = status));
        }

        public bool Contains(string modelName)
        {
            return GetRow(modelName) != null;
        }

        public Status GetStatus(int idx)
        {
            return (Status) _grid.Rows[idx].Cells[1].Value;
        }

        public void AddModel(string modelName)
        {
            _grid.Rows.Add(modelName, Status.Disconnected);

            if (_grid.SortedColumn == _grid.Columns[0])
            {
                var sortOrder = _grid.SortOrder == SortOrder.Descending
                    ? ListSortDirection.Descending
                    : ListSortDirection.Ascending;
                _grid.Sort(_grid.Columns[0], sortOrder);
            }
        }

        public void RemoveModel(string modelName)
        {
            var row = GetRow(modelName);
            _grid.Invoke((MethodInvoker)(() => _grid.Rows.RemoveAt(row.Index)));
        }

        public string CreateModelsFileContent()
        {
            return _grid.Rows.Cast<DataGridViewRow>()
                .Aggregate("", (current, row) => current + row.Cells[0].Value.ToString() + Environment.NewLine);
        }

        public List<string> ModelsWithStatus(Status status)
        {
            return _grid.Rows.Cast<DataGridViewRow>()
                .Where(r => (Status) r.Cells[1].Value == status)
                .Select(r => r.Cells[0].Value.ToString())
                .ToList();
        }

        private DataGridViewRow GetRow(string modelName)
        {
            return GetRow(0, modelName);
        }

        private DataGridViewRow GetRow(int columnIdx, object value)
        {
            return _grid.Rows.Cast<DataGridViewRow>()
                .FirstOrDefault(row => row.Cells[columnIdx].Value.Equals(value));
        }
    }
}
