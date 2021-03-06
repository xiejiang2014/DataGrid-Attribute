﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace DataGridAttributeDemo
{
    public class DataGridEx : DataGrid
    {
        private List<ColumnAndOrder> _columnAndOrderList = new List<ColumnAndOrder>();

        public DataGridEx()
        {
            AutoGeneratingColumn += DataGridEx_AutoGeneratingColumn;
            AutoGeneratedColumns += DataGridEx_AutoGeneratedColumns;
        }

        private void DataGridEx_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            //将创建过程标记为已取消,以阻止DataGrid将生成的列对象加入到列集合中.
            //Set event to cancel,DataGrid will not add current column to the Columns collection.
            e.Cancel = true;

            //从Attribute中读取显示名称.
            //Read display name from Attribute.
            var displayName = ((System.ComponentModel.MemberDescriptor)e.PropertyDescriptor).DisplayName;
            if (!string.IsNullOrWhiteSpace(displayName))
            {
                e.Column.Header = displayName;
            }

            //从Attribute中读取顺序,以及是否自动生成列的标识.
            //Read order from Attribute and whether to automatically generate column.
            var attributes = ((System.ComponentModel.MemberDescriptor)e.PropertyDescriptor).Attributes;
            var order = 0;
            foreach (Attribute attribute in attributes)
            {
                if (attribute is DataGridColumnOrderAttribute orderAttribute)
                {
                    order = orderAttribute.DataGridColumnOrder;
                }
                else if (attribute is DoNotAutoGenerateDataGridColumnAttribute)
                {
                    //发现 DoNotAutoGenerateDataGridColumnAttribute 时,丢弃已生成的列
                    //Discard generated column when DoNotAutoGenerateDataGridColumnAttribute is found
                    return;
                }
            }

            //将创建的列及顺序保存
            //Save the column and order
            _columnAndOrderList.Add(new ColumnAndOrder(order, e.Column));
        }

        private void DataGridEx_AutoGeneratedColumns(object sender, EventArgs e)
        {
            //按顺序将所有列加入到Columns集合中
            //Add all columns to the Columns collection in order
            foreach (var columnAndOrder in _columnAndOrderList.OrderBy(v => v.Order))
            {
                Columns.Add(columnAndOrder.DataGridColumn);
            }

            //不需要了
            //No longer useful
            _columnAndOrderList = null;
        }

        private class ColumnAndOrder
        {
            public ColumnAndOrder(int order, DataGridColumn dataGridColumn)
            {
                Order = order;
                DataGridColumn = dataGridColumn;
            }

            public int Order { get; }

            public DataGridColumn DataGridColumn { get; }
        }
    }
}