using System;
using System.Data.SQLite;
using System.IO;
using System.Security.Cryptography;

namespace H2OpticaLogic
{
    public class DBService
    {
        private readonly string _dbPath;

        public DBService(string dbPathFilePath)
        {
            _dbPath = dbPathFilePath;
        }

        public void InizializeDB()
        {
            //Controllo esistenza file DB
            if (!File.Exists(_dbPath))
                SQLiteConnection.CreateFile(_dbPath);

            //Connessione
            using(var connection = new SQLiteConnection($"Data Source={_dbPath};Version=3;"))
            {
                connection.Open();

                //Query che crea la tabella dei sensori
                string sensorQuery = @"CREATE TABLE IF NOT EXISTS Sensori (
                                           SensorID INTEGER PRIMARY KEY AUTOINCREMENT,
                                           Name TEXT NOT NULL UNIQUE
                                    );";

                //Query che crea la tabella dei dati
                string dataQuery = @"CREATE TABLE IF NOT EXISTS SensorData (
                                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                        SensorID INTEGER NOT NULL,
                                        DateTime DATETIME NOT NULL,
                                        Volume REAL,
                                        Temp REAL,
                                        pH REAL,
                                   
                                        FOREIGN KEY (SensorID) REFERENCES Sensori(SensorID)
                                    );";

                using (var cmd1 = new SQLiteCommand(sensorQuery, connection))
                    cmd1.ExecuteNonQuery();

                using (var cmd2 = new SQLiteCommand(dataQuery, connection))
                    cmd2.ExecuteNonQuery();
            }
        }

        public void InsertNewFlowSens(string name)
        {
            using(var connection = new SQLiteConnection($"Data Source={_dbPath};Version=3;"))
            {
                connection.Open();

                string newSensQuery = @"INSERT INTO Sensori (Name)
                                        VALUES (@name);";

                //Aggiungo il sensore
                using (var cmd = new SQLiteCommand(newSensQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void RemoveFlowSens(int id)
        {
            using (var connection = new SQLiteConnection($"Data Source={_dbPath};Version=3;"))
            {
                connection.Open();

                string delData = @"DELETE FROM SensorData WHERE SensorID = @id";
                string delSens = @"DELETE FROM Sensori WHERE SensorID = @id";

                using (var clearData = new SQLiteCommand(delData, connection))
                {
                    clearData.Parameters.AddWithValue("@id", id);

                    clearData.ExecuteNonQuery();
                }

                using (var clearSensori =  new SQLiteCommand(delSens, connection))
                {
                    clearSensori.Parameters.AddWithValue("@id", id);

                    clearSensori.ExecuteNonQuery();
                }
            }
        }   

        public void ChangeSensName(int id, string newName)
        {
            using(var connection = new SQLiteConnection($"Data Source={_dbPath};Version=3;"))
            {
                connection.Open();

                string nameQuery = @"UPDATE Sensori
                                     SET Name = @newName
                                     WHERE SensorID = @id;";

                using (var cmd = new SQLiteCommand(nameQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@newName", newName);
                    cmd.Parameters.AddWithValue("@id", id);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void InsertFlowData(int sensorID, double flow)
        {
            using(var connection = new SQLiteConnection($"Data Source={_dbPath};Version=3;"))
            {
                connection.Open();

                string sensorDataQuery = @"INSERT INTO SensorData 
                                           (SensorID, DateTime, Volume)
                                           VALUES
                                           (@id, @datetime, @flow);";

                using (var insertData = new SQLiteCommand(sensorDataQuery, connection))
                {
                    insertData.Parameters.AddWithValue("@id", sensorID);
                    insertData.Parameters.AddWithValue("@datetime", DateTime.Now);
                    insertData.Parameters.AddWithValue("@flow", flow);

                    insertData.ExecuteNonQuery();
                }
            }
        }

        public void PrintData()
        {
            using(var connection = new SQLiteConnection($"Data source={_dbPath};Version=3;"))
            {
                connection.Open();

                string query = "SELECT * FROM SensorData ORDER BY DateTime DESC;";

                using(var cmd = new SQLiteCommand(query, connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int ID = reader.GetInt32(0);
                            DateTime DT = DateTime.Parse(reader.GetString(1));
                            double? volume = reader.GetDouble(2);
                            double? temp = reader.GetDouble(3);
                            double? ph = reader.GetDouble(4);

                            //Stampa su form - DA FARE
                        }
                    }
                }
            }
        }
    }
}
