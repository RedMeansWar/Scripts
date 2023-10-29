using System;
using System.Collections.Generic;
using System.Text;
using SharpConfig;
using Dapper;
using MySql.Data.MySqlClient;
using Red.Framework.Server.Misc;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using CitizenFX.Core.Native;

namespace Red.Framework.Server
{
    public class ServerMain : BaseScript
    {
        #region Variables
        protected static string dbUser, dbPassword, dbHost, dbName, connectionString;
        protected MySqlConnection Database = new(connectionString);
        protected int dbPort;
        private Character currentCharacter;
        #endregion

        #region Constructor
        public ServerMain() => OnReadConfigFile();
        #endregion

        #region Methods
        private void OnReadConfigFile()
        {
            var data = LoadResourceFile(GetCurrentResourceName(), "config.ini");

            if (Configuration.LoadFromString(data).Contains("Database", "DatabaseUsername") == true)
            {
                Configuration loaded = Configuration.LoadFromString(data);
                dbUser = loaded["Database"]["DatabaseUsername"].StringValue;
                dbPassword = loaded["Database"]["DatabasePassword"].StringValue;
                dbHost = loaded["Database"]["DatabaseHost"].StringValue;
                dbPort = loaded["Database"]["DatabasePort"].IntValue;
                dbName = loaded["Database"]["DatabaseName"].StringValue;
            }
            else
            {
                FrameworkLog.Error($"Config file has not been configured correctly.");
            }
        }
        #endregion

        #region Event Handlers
        [EventHandler("Framework:Server:getCharacter")]
        private void OnGetCharacters(int source)
        {
            var characters = new List<Character>();
            string license = Helper.GetPlayerLicense(source);

            Database.Open();

            string queryResult = "SELECT * FROM characters WHERE license = @license";
            MySqlCommand command = new(queryResult, Database);

            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Character character = new()
                    {
                        CharacterId = reader.GetString("character_id"),
                        FirstName = reader.GetString("first_name"),
                        LastName = reader.GetString("last_name"),
                        DoB = DateTime.ParseExact(reader.GetString("dob"), "yyyy-MM-dd", null),
                        Gender = reader.GetString("gender"),
                        Department = reader.GetString("department")
                    };
                }
            }

            TriggerClientEvent("Framework:Client:returnCharacters");
        }

        [EventHandler("Framework:Server:createCharacter")]
        private void OnCreateCharacter(string data)
        {
            var source = Helper.PlayerId();
            dynamic characterData = new Character();

            string characterId = characterData.CharacterId;
            string firstName = characterData.FirstName;
            string lastName = characterData.LastName;
            string gender = characterData.Gender;
            DateTime dob = characterData.DoB;
            string department = characterData.Department;

            Character createdCharacter = new()
            {
                CharacterId = characterId,
                FirstName = firstName,
                LastName = lastName,
                Gender = gender,
                DoB = dob,
                Department = department,
            };

            Database.Open();
            string insertQuery = "INSERT INTO characters (license, first_name, last_name, dob, gender, department) " +
                "VALUES (@license, @first_name, @last_name, @dob, @gender, @department)";

            MySqlCommand command = new(insertQuery, Database);

            command.Parameters.AddWithValue("@license", Helper.GetPlayerLicense(source));
            command.Parameters.AddWithValue("@first_name", createdCharacter.FirstName);
            command.Parameters.AddWithValue("@last_name", createdCharacter.LastName);
            command.Parameters.AddWithValue("@dob", createdCharacter.DoB);
            command.Parameters.AddWithValue("@gender", createdCharacter.Gender);
            command.Parameters.AddWithValue("@department", createdCharacter.Department);

            command.ExecuteNonQuery();
        }

        [EventHandler("Framework:Server:editCharacter")]
        private void OnEditCharacter()
        {

        }

        [EventHandler("Framework:Server:deleteCharacter")]
        private void OnDeleteCharacter()
        {

        }
        #endregion
    }
}
