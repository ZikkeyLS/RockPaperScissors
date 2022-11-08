using MySql.Data.MySqlClient;
using System.Data;

namespace RockPaperScissors.DB
{
    public class FlexibleDB
    {
        public class Value
        {
            public readonly string Name = "";
            public readonly object Result;
            public readonly string Type = "string";

            public Value(string name, object result)
            {
                Name = name;
                Result = result;

                if (result.GetType() == typeof(string))
                    Type = "string";
                else if (result.GetType() == typeof(int))
                    Type = "int";
            }
        }

        private readonly string _connectionData = "";

        public FlexibleDB(string databaseName, string ip = "localhost", int port = 3306, string username = "root", string password = "root")
        {
            _connectionData = $"server={ip};port={port};username={username};password={password};database={databaseName};";
        }

        public DataRowCollection CreateGetRequest(string tableName, Value[] values)
        {
            DataTable table = new();
            MySqlDataAdapter adapter = new();
            MySqlCommand command = new();

            MySqlConnection connection = new(_connectionData);
            _ = connection.OpenAsync();

            string sqlRequest = $"SELECT * FROM `{tableName}` WHERE ";

            for(int i = 0; i < values.Length; i++)
            {
                Value value = values[i];

                string symbol = value.Type == "string" ? "LIKE" : "=";
                string? finalValue = value.Type == "string" ? "'" + value.Result.ToString()?.Replace(" ", "") + "'" : value.Result.ToString()?.Replace(" ", "");

                sqlRequest += $"`{value.Name}` {symbol} {finalValue}";
                
                if (i != values.Length - 1)
                    sqlRequest += " AND ";
            }

            command = new MySqlCommand(sqlRequest, connection);
            adapter.SelectCommand = command;
            adapter.Fill(table);

            _ = connection.CloseAsync();
            _ = connection.DisposeAsync();

            return table.Rows;
        }

        public int CreateChangeRequest(string tableName, Value valueToChange, Value valueToCheck)
        {
            MySqlConnection connection = new(_connectionData);
            _ = connection.OpenAsync();

            string? finalValue = valueToChange.Type == "string" ? "'" + valueToChange.Result.ToString()?.Replace(" ", "") + "'" : valueToChange.Result.ToString()?.Replace(" ", "");

            string checkSymbol = valueToCheck.Type == "string" ? "LIKE" : "=";
            string? checkFinalValue = valueToCheck.Type == "string" ? "'" + valueToCheck.Result.ToString()?.Replace(" ", "") + "'" : valueToCheck.Result.ToString()?.Replace(" ", "");

            string sqlRequest = $"UPDATE `{tableName}` SET `{valueToChange.Name}` = {finalValue} WHERE `{valueToCheck.Name}` {checkSymbol} {checkFinalValue}";

            MySqlCommand command = new(sqlRequest, connection);

            Task<int> task = command.ExecuteNonQueryAsync();

            _ = connection.CloseAsync();
            _ = connection.DisposeAsync();

            return task.Result;
        }

        public int CreateChangeRequest(string tableName, Value valueToChange)
        {
            MySqlConnection connection = new(_connectionData);
            _ = connection.OpenAsync();

            string? finalValue = valueToChange.Type == "string" ? "'" + valueToChange.Result.ToString()?.Replace(" ", "") + "'" : valueToChange.Result.ToString()?.Replace(" ", "");

            string sqlRequest = $"UPDATE `{tableName}` SET `{valueToChange.Name}` = {finalValue}";

            MySqlCommand command = new(sqlRequest, connection);

            Task<int> task = command.ExecuteNonQueryAsync();

            _ = connection.CloseAsync();
            _ = connection.DisposeAsync();

            return task.Result;
        }
    }
}
