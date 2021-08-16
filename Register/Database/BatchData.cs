using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Common;

namespace Register.Database
{
    public static  class BatchData
    {
        const string TABLE_NAME = "Batch_Table";
        const string BATCH_ID = "Batch_Id";
        const string BATCH_NAME = "Batch_Name";
        const string BATCH_STATUS = "Status";
        
        public static void CreateTable()
        {
            DataSet ds = new DataSet();
            if (!ds.Tables.Contains(TABLE_NAME))
            {
                DataTable batchTable = new DataTable(TABLE_NAME);
                DataColumn batchId = new DataColumn()
                {
                    DataType = typeof(Int32),
                    ColumnName = BATCH_ID
                };
                DataColumn batchName = new DataColumn()
                {
                    DataType = typeof(string),
                    ColumnName = BATCH_NAME
                };
                DataColumn status = new DataColumn()
                {
                    DataType = typeof(string),
                    ColumnName = BATCH_STATUS
                };
                batchTable.Columns.Add(batchId);
                batchTable.Columns.Add(batchName);
                batchTable.Columns.Add(status);

                //PrimaryKey
                DataColumn[] primaryKey = new DataColumn[1];
                primaryKey[0] = batchTable.Columns[BATCH_ID];
                batchTable.PrimaryKey = primaryKey;
                ds.Tables.Add(batchTable);
            }
            if(ds.Tables.Contains(TABLE_NAME))
            {
                Insert(TABLE_NAME);
            }
           
        }
        public static void Insert(string tableName)
        {
            DataSet ds = new DataSet();
            var table = ds.Tables[tableName];
            for(int i =1; i<=5; i++)
            {
                table.Rows.Add();
                DataRow dtrow = table.NewRow();
                dtrow[BATCH_ID] = i;
                dtrow[BATCH_NAME] = $"Batch: {i}";
                dtrow[BATCH_STATUS] = "Not Started";
                table.Rows.Add(dtrow);

            }
        }

        public static void Display()
        {
            DataSet ds = new DataSet();
            var table = ds.Tables[TABLE_NAME];
            foreach(DataRow row in table.Rows)
            {
                foreach(DataColumn column in table.Columns)
                {
                    Console.WriteLine(row[column]);
                }
            }
        }
    }
}
