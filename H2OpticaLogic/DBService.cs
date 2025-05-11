using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Migrations.Model;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Dynamic;
using System.IO;
using System.Security.Cryptography;
using System.Security.Policy;

namespace H2OpticaLogic
{
    public class DBService
    {
        private readonly string _dbPath;
        private const int MAX_SENSOR_ID = 5;

        //Costruttore
        public DBService(string dbPathFilePath)
        {
            _dbPath = dbPathFilePath;
        }

        //Metodi
        private SQLiteConnection GetConnection()
        {
            return new SQLiteConnection($"Data Source={_dbPath};Version=3;");
        }

        private bool TableExists(SQLiteConnection connection, string tableName)
        {
            if(connection.State != System.Data.ConnectionState.Open)
                connection.Open();  

            string query = @"SELECT name FROM sqlite_master WHERE type='table' AND name = @name;";

            try
            {
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@name", tableName);

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        return reader.Read();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                throw;
            }
            }
        private bool ColumnExists(SQLiteConnection connection, string tableName, string columnName)
        {
            if (connection.State != System.Data.ConnectionState.Open)
                connection.Open();

            string query = $"PRAGMA table_info({tableName});";

            try
            {
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string currentCol = reader.GetString(1);
                        if (string.Equals(currentCol, columnName, StringComparison.OrdinalIgnoreCase))
                            return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                throw;
            }

            return false;
        }

        private int AddColumn(SQLiteConnection connection, string tableName, string columnName, string columnType)
        {
            if (connection.State != System.Data.ConnectionState.Open)
                connection.Open();

            string query = $"ALTER {tableName} ADD COLUMN {columnName} {columnType};";

            try
            {
                Logger.Log($"Trying to add column type'{columnType}' named '{columnName}' to table '{tableName}'...");
                using (SQLiteCommand cmd = new SQLiteCommand(query, connection))
                    cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return -1;
            }

            return 0;
        }

        private int GetFirstAvailableSensorID(SQLiteConnection connection)
        {
            if (connection.State != System.Data.ConnectionState.Open)
                connection.Open();

            int? resultID = -1;

            string query = @"SELECT SensorId
                             FROM (SELECT 0 AS SensorId UNION SELECT 1 UNION SELECT 2 UNION SELECT 3 UNION SELECT 4 UNION SELECT 5)
                             WHERE SensorId NOT IN (SELECT SensorId FROM Sensors)
                             ORDER BY SensorId
                             LIMIT 1;";

            Logger.Log("Trying to retrieve first available sensor ID from database...");

            try
            {
                using (SQLiteCommand cmd = new SQLiteCommand(query, connection))
                {
                    resultID = (int?)cmd.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }

            if (resultID == null)
                return (int)-1;

            else return (int)resultID;
        }

        public void InitializeDB()
        {
            //Controllo esistenza file DB
            if (!File.Exists(_dbPath))
                SQLiteConnection.CreateFile(_dbPath);

            //Connessione
            using(SQLiteConnection connection = GetConnection())
            {
                connection.Open();

                //Query che crea la tabella dei sensori
                string sensorQuery = @"CREATE TABLE IF NOT EXISTS Sensori (
                                           SensorID INTEGER PRIMARY KEY,
                                           Name TEXT NOT NULL UNIQUE,
                                           SensorLimit REAL
                                    );";

                //Query che crea la tabella dei dati
                string dataQuery = @"CREATE TABLE IF NOT EXISTS SensorData (
                                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                        Timestamp DATETIME NOT NULL,
                                        Temp REAL NOT NULL,
                                        pH REAL NOT NULL,
                                        Flow0 REAL,
                                        Flow1 REAL,
                                        Flow2 REAL,                                   
                                        Flow3 REAL,
                                        Flow4 REAL,
                                        Flow5 REAL
                                       );";

                try
                {
                    using (SQLiteCommand cmd1 = new SQLiteCommand(sensorQuery, connection))
                        cmd1.ExecuteNonQuery();

                    Logger.Log("Successfully created table 'Sensori'");

                    using (SQLiteCommand cmd2 = new SQLiteCommand(dataQuery, connection))
                        cmd2.ExecuteNonQuery();

                    Logger.Log("Successfully created table 'SensorData'");
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                    throw;
                }
            }
        }

        public int CheckDB()
        {
            Logger.Log("Checking database integrity...");

            if (!File.Exists(_dbPath))
            {
                Logger.Log("Database file doesn't exist or not found");
                return -1;
            }

            using (SQLiteConnection connection = GetConnection())
            {
                connection.Open();

                //Controllo 'Sensori'
                string checkedTable = "Sensori";

                if (!TableExists(connection, checkedTable))
                    Logger.Log($"Table '{checkedTable}' doesn't exist or was not found");
                else
                {
                    Logger.Log($"Table '{checkedTable}' found");

                    //Controllo campi di 'Sensori'
                    string[] columns =
                    {
                        "SensorID",
                        "Name",
                        "SensorLimit"
                    };
                    string[] types = 
                    {
                        "INTEGER PRIMARY KEY",
                        "TEXT NOT NULL UNIQUE",
                        "REAL"
                    };

                    for (int i = 0; i < columns.Length; i++)
                    {
                        if (!ColumnExists(connection, checkedTable, columns[i]))
                        {
                            Logger.Log($"Column '{columns[i]}' was not found in table '{checkedTable}'");
                            AddColumn(connection, checkedTable, columns[i], types[i]);
                        }
                        else
                        {
                            Logger.Log($"Column '{columns[i]}' found in table '{checkedTable}'");
                        }
                    }
                }

                //Controllo 'SensorData'
                checkedTable = "SensorData";
                if (!TableExists(connection, checkedTable))
                    Logger.Log($"Table '{checkedTable}' doesn't exist or was not found");
                else
                {
                    Logger.Log($"Table '{checkedTable}' found");

                    //Controllo campi di 'SensorData'
                    string[] columns = 
                    {
                        "Id",
                        "Timestamp",
                        "Temp",
                        "pH",
                        "Flow0",
                        "Flow1",
                        "Flow2",
                        "Flow3",
                        "Flow4",
                        "Flow5"
                    };
                    string[] types = 
                    { 
                        "INTEGER PRIMARY KEY AUTOINCREMENT",
                        "DATETIME NOT NULL",
                        "REAL NOT NULL",
                        "REAL NOT NULL",
                        "REAL",
                        "REAL",
                        "REAL",
                        "REAL",
                        "REAL",
                        "REAL"
                    };

                    for (int i = 0; i < columns.Length; i++)
                    {
                        if (!ColumnExists(connection, checkedTable, columns[i]))
                        {
                            Logger.Log($"Column '{columns[i]}' was not found in table '{checkedTable}'");
                            AddColumn(connection, checkedTable, columns[i], types[i]);
                        }
                        else
                        {
                            Logger.Log($"Column '{columns[i]}' found in table '{checkedTable}'");
                        }
                    }
                }

                return 0;
            }
        }

        public void InsertNewFlowSens(string name)
        {

            using(SQLiteConnection connection = GetConnection())
            {
                connection.Open();

                string newSensQuery = @"INSERT INTO Sensori (SensorID,Name)
                                        VALUES (@id,@name);";

                int id = GetFirstAvailableSensorID(connection);

                //Aggiungo il sensore
                using (SQLiteCommand cmd = new SQLiteCommand(newSensQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public int SetSensorLimit(int sensId, double? limit)
        {
            if(sensId < 0 || sensId > MAX_SENSOR_ID)
            {
                Logger.Log(new ArgumentOutOfRangeException(nameof(sensId), $"Sensor ID: {sensId} does not exist"));
                Logger.Log("Exiting method with code -1...");
                return -1;
            }

            using (SQLiteConnection connection = GetConnection())
            {
                connection.Open();

                string query = @"UPDATE Sensori SET SensorLimit = @limit WHERE SensorID = @sensId;";

                try
                {
                    Logger.Log($"Trying to set limit {limit} on [ID: {sensId}]");

                    using (SQLiteCommand cmd = new SQLiteCommand(query, connection))
                    {
                        if(limit == null)
                            cmd.Parameters.AddWithValue("@limit", DBNull.Value);
                        else
                            cmd.Parameters.AddWithValue("@limit", limit);

                        cmd.Parameters.AddWithValue("@sensId", sensId);

                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                    return -1;
                }
            }
            
            Logger.Log("Sensor limit successfully set");

            return 0;
        }
        public void RemoveFlowSens(int id)
        {
            if(id < 0 || id > MAX_SENSOR_ID)
            {
                Logger.Log(new ArgumentOutOfRangeException(nameof(id), $"Sensor ID: {id} does not exist"));
                Logger.Log("Exiting method...");
                return;
            }

            using (SQLiteConnection connection = GetConnection())
            {
                connection.Open();

                string colName = $"Flow{id}";

                string delData = $"UPDATE SensorData SET {colName} = NULL WHERE {colName} IS NOT NULL";
                string delSens = @"DELETE FROM Sensori WHERE SensorID = @id";

                using (SQLiteCommand clearData = new SQLiteCommand(delData, connection))
                {
                    clearData.ExecuteNonQuery();
                }

                using (SQLiteCommand clearSensori =  new SQLiteCommand(delSens, connection))
                {
                    clearSensori.Parameters.AddWithValue("@id", id);

                    clearSensori.ExecuteNonQuery();
                }
            }
        }   

        public void ChangeSensName(int id, string newName)
        {
            using(SQLiteConnection connection = GetConnection())
            {
                connection.Open();

                string nameQuery = @"UPDATE Sensori
                                     SET Name = @newName
                                     WHERE SensorID = @id;";

                using (SQLiteCommand cmd = new SQLiteCommand(nameQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@newName", newName);
                    cmd.Parameters.AddWithValue("@id", id);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        /*public void InsertFlowData(int sensorID, double flow)
        {
            using(SQLiteConnection connection = GetConnection())
            {
                connection.Open();

                string sensorDataQuery = @"INSERT INTO SensorData 
                                           (SensorID, Volume)
                                           VALUES
                                           (@sensorID, @flow);";

                using (SQLiteCommand insertData = new SQLiteCommand(sensorDataQuery, connection))
                {
                    insertData.Parameters.AddWithValue("@sensorID", sensorID);
                    insertData.Parameters.AddWithValue("@flow", flow);

                    insertData.ExecuteNonQuery();
                }
            }
        }*/
        public void InsertWaterData(double temp, double ph)
        {
            try
            {
                using (SQLiteConnection connection = GetConnection())
                {
                    connection.Open();

                    string query = @"INSERT INTO SensorData
                                     (Timestamp, Temp, pH)
                                     VALUES
                                     (@dt, @temp, @ph);";

                    using (SQLiteCommand cmd = new SQLiteCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@dt", DateTime.Now);
                        cmd.Parameters.AddWithValue("@temp", temp);
                        cmd.Parameters.AddWithValue("@ph", ph);

                        cmd.ExecuteNonQuery();

                        Logger.Log("Successfully added 'Temp' and 'pH' data in table 'SensorData'");
                    }
                }
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
        }
        public void InsertAllData(DataCollection data)
        {
            using (SQLiteConnection connection = GetConnection())
            {
                connection.Open();

                try
                {
                    string query = @"INSERT INTO SensorData
                                     (Timestamp, Temp, pH, Flow0, Flow1, Flow2, Flow3, Flow4, Flow5)
                                     VALUES
                                     (@time, @temp, @ph, @f0, @f1, @f2, @f3, @f4, @f5)";

                    using (SQLiteCommand cmd = new SQLiteCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@time", DateTime.Now);
                        cmd.Parameters.AddWithValue("@temp", data.Stats.Temp);
                        cmd.Parameters.AddWithValue("@ph", data.Stats.pH);

                        for (int i = 0; i <= MAX_SENSOR_ID; i++)
                        {
                            if (data.Flows.Flux.TryGetValue(i, out double flow))
                                cmd.Parameters.AddWithValue($"@f{i}", flow);
                            else
                                cmd.Parameters.AddWithValue($"@f{i}", DBNull.Value);
                        }

                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
            }
        }

        //Ottenimento dati
        public string GetSensorName(int sensorID)
        {
            string name = null;

            if (sensorID >= 0 && sensorID < 6)
            {
                using (SQLiteConnection connection = GetConnection())
                {
                    connection.Open();

                    string selectQuery = @"SELECT Name FROM Sensori WHERE SensorID = @sensorID";

                    using (SQLiteCommand cmd = new SQLiteCommand(selectQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@sensorID", sensorID);

                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                                name = reader.GetString(0);
                        }
                    }
                }
            }
            else
            {
                Logger.Log(new ArgumentOutOfRangeException(nameof(sensorID), $"Sensor ID: {sensorID} does not exist"));
                Logger.Log("Returning null...");
                return null;
            }

            return name;
        }

        public int GetSensorID(string name)
        {
            int sensorID = -1;

            using (SQLiteConnection connection = GetConnection())
            {
                connection.Open();

                string idQuery = @"SELECT SensorID FROM Sensori WHERE Name = @name";

                using(SQLiteCommand cmd = new SQLiteCommand(idQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@name", name);

                    using(SQLiteDataReader reader = cmd.ExecuteReader())
                        if(reader.Read())
                            sensorID = reader.GetInt32(0);
                }
            }

            return sensorID;
        }

        public double? GetSensorLimit(int sensorID)
        {

            if (sensorID < 0 || sensorID > MAX_SENSOR_ID)
            {
                Logger.Log(new ArgumentOutOfRangeException(nameof(sensorID), $"Sensor ID: {sensorID} does not exist"));
                Logger.Log("Returning null...");
                return null;
            }

            double? sensorLimit = null;

            using (SQLiteConnection connection = GetConnection())
            {
                connection.Open();

                try
                {
                    string query = @"SELECT SensorLimit FROM Sensori WHERE SensorID = @sensorID";

                    using (SQLiteCommand cmd = new SQLiteCommand(query, connection))
                    {
                        using(SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            if(reader.Read())
                            {
                                sensorLimit = reader.IsDBNull(0) ? null : (double?)reader.GetDouble(0);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
            }

            return sensorLimit;
        }

        public double? GetTemp(DateTime dateTime)
        {
            double? temp = null;

            using (SQLiteConnection connection = GetConnection())
            {
                connection.Open();

                string selectQuery = @"SELECT Temp FROM SensorData WHERE Timestamp = @dateTime";

                using(SQLiteCommand cmd  = new SQLiteCommand(selectQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@dateTime", dateTime);

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                            temp = reader.IsDBNull(0) ? (double?)null : reader.GetDouble(0);
                    }
                }
            }
            
            return temp;
        }

        public double? GetLastTemp()
        {
            double? temp = null;

            using (SQLiteConnection connection = GetConnection())
            {
                connection.Open();

                string selectQuery = @"SELECT Temp FROM SensorData
                                       WHERE Temp IS NOT NULL
                                       ORDER BY Timestamp DESC
                                       LIMIT 1";

                using (SQLiteCommand cmd = new SQLiteCommand(selectQuery, connection))
                {
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                            temp = reader.IsDBNull(0) ? (double?)null : reader.GetDouble(0);
                    }
                }
            }

            return temp;
        }

        public double? GetpH(DateTime dateTime)
        {
            double? pH = null;

            using (SQLiteConnection connection = GetConnection())
            {
                connection.Open();

                string selectQuery = @"SELECT pH FROM SensorData WHERE Timestamp = @dateTime";

                using (SQLiteCommand cmd = new SQLiteCommand(selectQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@dateTime", dateTime);

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                            pH = reader.IsDBNull(0) ? (double?)null : reader.GetDouble(0);
                    }
                }
            }

            return pH;
        }

        public double? GetLastpH()
        {
            double? pH = null;

            using (SQLiteConnection connection = GetConnection())
            {
                connection.Open();

                string selectQuery = @"SELECT pH FROM SensorData
                                       WHERE pH IS NOT NULL
                                       ORDER BY Timestamp DESC
                                       LIMIT 1";

                using (SQLiteCommand cmd = new SQLiteCommand(selectQuery, connection))
                {
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                            pH = reader.IsDBNull(0) ? (double?)null : reader.GetDouble(0);
                    }
                }
            }

            return pH;
        }
        public double? GetVolume(int sensId, DateTime time)
        {
            if (sensId < 0 || sensId > MAX_SENSOR_ID)
            {
                Logger.Log(new ArgumentOutOfRangeException(nameof(sensId), $"Sensor ID: {sensId} does not exist"));
                Logger.Log("Returning null...");
                return null;
            }

            double? volume = null;

            using (SQLiteConnection connection = GetConnection())
            {
                connection.Open();

                string colName = $"Flow{sensId}";

                string volumeQuery = $@"SELECT {colName} FROM SensorData WHERE {colName} IS NOT NULL AND Timestamp = @time";

                using(SQLiteCommand cmd = new SQLiteCommand(volumeQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@time", time);

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                            volume = reader.IsDBNull(0) ? (double?)null : reader.GetDouble(0);
                    }
                }
            }

            return volume;
        }

        public double? GetLatestVolume(int sensId)
        {
            if(sensId < 0 || sensId > MAX_SENSOR_ID)
            {
                Logger.Log(new ArgumentOutOfRangeException(nameof(sensId), $"Sensor ID: {sensId} does not exist"));
                Logger.Log("Returning null...");
                return null;
            }

            using (SQLiteConnection connection = GetConnection())
            {
                connection.Open();

                string colName = $"Flow{sensId}";

                string query = $@"SELECT {colName} FROM SensorData WHERE {colName} IS NOT NULL
                                 ORDER BY Timestamp DESC LIMIT 1";

                using (SQLiteCommand cmd = new SQLiteCommand(query, connection))
                {
                    using(SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        if(reader.Read())
                        {
                            double? volume = reader.IsDBNull(0) ? null : (double?)reader.GetDouble(0);

                            return volume;
                        }
                    }
                }
            }

            return double.NaN;
        }

        public Dictionary<DateTime, double> GetChartData(int sensId, DateTime day)
        {
            if (sensId < 0 || sensId > MAX_SENSOR_ID)
                return null;

            Dictionary<DateTime, double> dataSet = new Dictionary<DateTime, double>();

            using (SQLiteConnection connection = GetConnection())
            {
                connection.Open();

                string colName = $"Flow{sensId}";

                DateTime dayStart = day.Date;
                DateTime dayEnd = day.Date.AddDays(1).AddSeconds(-1); //23:59:59

                string query = $@"SELECT Timestamp, {colName} FROM SensorData 
                                  WHERE {colName} IS NOT NULL
                                  AND Timestamp BETWEEN @dayStart AND @dayEnd
                                  ORDER BY Timestamp ASC";

                try
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@dayStart", dayStart);
                        cmd.Parameters.AddWithValue("@dayEnd", dayEnd);

                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DateTime timestamp = reader.GetDateTime(0);
                                double flowValue = reader.GetDouble(1);

                                dataSet[timestamp] = flowValue;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
            }

            return dataSet;
        }
        public List<Sensor> GetSensorList()
        {
            var sensList = new List<Sensor>();

            using (SQLiteConnection connection = GetConnection())
            {
                connection.Open();

                string sensorQuery = @"SELECT SensorID, Name, SensorLimit FROM Sensori";

                using(SQLiteCommand cmd = new SQLiteCommand(sensorQuery, connection))
                using(SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Sensor mySensor = new Sensor();

                        mySensor.SensorID = reader.GetInt32(0);
                        mySensor.SensorName = reader.GetString(1);
                        mySensor.SensorLimit = reader.IsDBNull(2) ? (double?)null : reader.GetDouble(2);
                        
                        sensList.Add(mySensor);
                    }
                }
            }

            return sensList;
        }

        /*public DataCollection GetLatestSensorData(int sensID)
        {
            DataCollection collection = new DataCollection();

            using (SQLiteConnection connection = GetConnection())
            {
                connection.Open();

                string dataQuery = @"SELECT DateTime, Volume, Temp, pH
                                     FROM SensorData
                                     WHERE SensorID = @sensID
                                     ORDER BY DateTime DESC
                                     LIMIT 1";
                try
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(dataQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@sensID", sensID);

                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                double? volume = reader.IsDBNull(1) ? (double?)null : reader.GetDouble(1);


                                if (!volume == null)
                                    collection.Flows.Flux.Add(sensID, (double)volume); ;

                                collection.Stats.Temp = reader.IsDBNull(2) ? (double?)null : reader.GetDouble(2);
                                collection.Stats.pH = reader.IsDBNull(3) ? (double?)null : reader.GetDouble(3);

                                return collection;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
            }

            return null;
        } */
        public DataCollection GetLatestWaterData()
        {
            DataCollection collection = new DataCollection();

            using (SQLiteConnection connection = GetConnection())
            {
                connection.Open();

                string dataQuery = @"SELECT Temp, pH
                                     FROM SensorData
                                     ORDER BY Timestamp DESC
                                     LIMIT 1";
                try
                {

                    using (SQLiteCommand cmd = new SQLiteCommand(dataQuery, connection))
                    {
                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                collection.Stats.Temp = reader.IsDBNull(0) ? (double?)null : reader.GetDouble(0);
                                collection.Stats.pH = reader.IsDBNull(1) ? (double?)null : reader.GetDouble(1);
                            }
                        }
                    }

                    List<Sensor> sensList = GetSensorList() ?? new List<Sensor>();

                    foreach (Sensor sens in sensList)
                    {
                        collection.Flows.Flux.Add(sens.SensorID, (double)GetLatestVolume(sens.SensorID));
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
            }

            return collection;
        }
    }
}
