using Npgsql;
using Register.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Register.Database
{
    public class BatchDataService : IBatchDataService
    {
        const string connectionString = "Server=localhost; Port=5432; User Id=postgres; Password=ram0978; Database=Test; Pooling=false; Timeout=300; CommandTimeout=300";
        public List<BatchModel> Get()
        {
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"select * from Batch_Table";
            NpgsqlDataReader dataReader = command.ExecuteReader();
            List<BatchModel> batchList = new List<BatchModel>();
            while(dataReader.Read())
            {
                BatchModel model = new BatchModel
                {
                    BatchId = Int32.Parse(dataReader[0].ToString()),
                    BatchName = dataReader[1].ToString(),
                    Status = dataReader[2].ToString()
                };
                batchList.Add(model);
            }
            connection.Close();
            return batchList;
        }

        public void Update(int id, string status)
        {
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            connection.Open();
            int result = 0;
            var command = connection.CreateCommand();
            command.CommandText = @"update Batch_Table set status='" +status+ "' where id = " +id+ "";
            var temp = $"update Batch_Table set status = '{status}', where id = '{id}'";
            result = command.ExecuteNonQuery();
        }
    }
}
