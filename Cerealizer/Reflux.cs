﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Collections;
using System.Reflection;

namespace Cerealizer
{
    /// <summary>
    /// Static methods for dealing with Reflection and Cerealizer.
    /// </summary>
    public class Reflux
    {
        #region Instantiation

        /// <summary>
        /// Instantiates an object given its type.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static object Instantiate(Type t)
        {
            return Activator.CreateInstance(t);
        }

        /// <summary>
        /// Instantiates an object given its type with arguments for the constructor.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static object Instantiate(Type t, object[] param)
        {
            return Activator.CreateInstance(t, param);
        }

        /// <summary>
        /// Instantiates an object given the string of the object's type.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static object Instantiate(string t)
        {
            return Activator.CreateInstance(Type.GetType(t));
        }

        /// <summary>
        /// Instantiates an object given the string of the object's type with arguments for the constructor.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static object Instantiate(string t, object[] param)
        {
            return Activator.CreateInstance(Type.GetType(t), param);
        }

        #endregion

        private static ICerealizer InstaCastConstruct(Type innerType, object arg)
        {
            Type c = typeof(Cerealizer<>);
            Type constructed = c.MakeGenericType(innerType);
            ICerealizer toRet = (ICerealizer)Activator.CreateInstance(constructed, arg);
            return toRet;
        }


        private static bool NeedsCereal(Type t)
        {
            return !(t.IsPrimitive || t == typeof(Decimal) || t == typeof(String) || t == typeof(Boolean) || t == typeof(DateTime));
        }

        private static string ConvertTypeToDB(Type t)
        {
            string dataType = t.Name.ToString().ToLower();
            string dbType = "";

            if (dataType == "int32")
                dbType = "int";
            else if (dataType == "string")
                dbType = "varchar(30)";
            else if (dataType == "boolean")
                dbType = "bit";
            else
                dbType = dataType;

            return dbType;
        }

        /// <summary>
        /// Returns an SQL connection given database parameters.
        /// </summary>
        /// <param name="server"></param>
        /// <param name="database"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static SqlConnection GetSQLConnection(string server, string database, string user, string password)
        {
            string connectionString = string.Format("Server={0};Database={1};User={2};Password={3}", server, database, user, password);

            return new SqlConnection(connectionString);
        }

        /// <summary>
        /// Formats a datatable to a Cerealized object type.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="conn"></param>
        public static void FormatTable(ICerealizer obj, SqlConnection conn)
        {
            string tableCommand = "CREATE TABLE " + obj.OType.Name.ToString() + "_DATA (ID int PRIMARY KEY IDENTITY(1,1), ";

            foreach (KeyValuePair<string, object> kvp in obj.Serial)
            {
                string column = kvp.Key;
                string dbType = ConvertTypeToDB(kvp.Value.GetType());

                if (NeedsCereal(kvp.Value.GetType()))
                {
                    FormatTable(InstaCastConstruct(kvp.Value.GetType(), kvp.Value), conn);
                    dbType = "int";
                }

                tableCommand += column + " " + dbType + ", ";
            }

            tableCommand = tableCommand.Substring(0, tableCommand.Length - 2) + ");";

            SqlCommand command = new SqlCommand(tableCommand, conn);

            conn.Open();
            command.ExecuteNonQuery();
            conn.Close();

            Console.WriteLine(tableCommand);
        }

        /// <summary>
        /// Inserts a Cerealized object into a formatted database table.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int InsertIntoTable(ICerealizer obj, SqlConnection conn)
        {
            string tableCommand = "INSERT INTO " + obj.OType.Name.ToString() + "_DATA OUTPUT INSERTED.ID VALUES(";

            Dictionary<string, string> toInsert = new Dictionary<string, string>();


            foreach (KeyValuePair<string, object> kvp in obj.Serial)
            {
                string value = "";

                if (NeedsCereal(kvp.Value.GetType()))
                {
                    value = InsertIntoTable(InstaCastConstruct(kvp.Value.GetType(), kvp.Value), conn).ToString();
                }
                else
                    value = kvp.Value.ToString();

                toInsert.Add(kvp.Key, value);

                tableCommand += "@" + kvp.Key + ", ";
            }

            tableCommand = tableCommand.Substring(0, tableCommand.Length - 2) + ");";

            SqlCommand command = new SqlCommand(tableCommand, conn);

            foreach (KeyValuePair<string, string> kvp in toInsert)
            {
                command.Parameters.AddWithValue(kvp.Key, kvp.Value);
            }

            conn.Open();
            int id = (int)command.ExecuteScalar();
            conn.Close();

            Console.WriteLine(tableCommand);
            return id;
        }

        public static void UpdateTable(ICerealizer obj, SqlConnection conn, int ID)
        {
            string tableCommand = "UPDATE " + obj.OType.Name.ToString() + "_DATA SET ";

            Dictionary<string, string> toInsert = new Dictionary<string, string>();


            foreach (KeyValuePair<string, object> kvp in obj.Serial)
            {
                string value = "";

                if (NeedsCereal(kvp.Value.GetType()))
                {
                    value = InsertIntoTable(InstaCastConstruct(kvp.Value.GetType(), kvp.Value), conn).ToString();
                }
                else
                    value = kvp.Value.ToString();

                toInsert.Add(kvp.Key, value);

                tableCommand += kvp.Key + "=@" + kvp.Key + ", ";
            }

            tableCommand = tableCommand.Substring(0, tableCommand.Length - 2) + "WHERE ID=@ID;";

            SqlCommand command = new SqlCommand(tableCommand, conn);

            foreach (KeyValuePair<string, string> kvp in toInsert)
            {
                command.Parameters.AddWithValue(kvp.Key, kvp.Value);
            }

            conn.Open();
            int id = (int)command.ExecuteScalar();
            conn.Close();

            Console.WriteLine(tableCommand);
            //return id;
        }

        /// <summary>
        /// Queries a formatted database table based on an object type given WHERE constraints.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="conn"></param>
        /// <param name="where">Formatted as COLUMNNAME, VALUE.</param>
        /// <returns></returns>
        public static List<object> SelectFromTable(Type obj, SqlConnection conn, Dictionary<string, object> where)
        {
            string tableCommand = "SELECT * FROM " + obj.Name + "_DATA WHERE ";

            foreach (KeyValuePair<string, object> kvp in where)
            {
                tableCommand += kvp.Key + "=@" + kvp.Key + " AND ";
            }

            if (where.Count > 0)
                tableCommand = tableCommand.Substring(0, tableCommand.Length - 5) + ";";
            else
                tableCommand = tableCommand.Substring(0, tableCommand.Length - 7) + ";";

            object output = Instantiate(obj);
            ICerealizer temp = InstaCastConstruct(obj, output);

            List<object> toRet = new List<object>();



            SqlCommand command = new SqlCommand(tableCommand, conn);

            foreach (KeyValuePair<string, object> kvp in where)
            {
                command.Parameters.AddWithValue(kvp.Key, kvp.Value);
            }

            if (conn.State != System.Data.ConnectionState.Open)
            {
                conn.Open();
            }


            SqlDataReader reader = command.ExecuteReader();
            temp.DefaultProperties();

            while (reader.Read())
            {
                ICerealizer currInst = InstaCastConstruct(obj, output);

                foreach (KeyValuePair<string, object> kvp in temp.Serial)
                {
                    if (!NeedsCereal(kvp.Value.GetType()))
                    {
                        currInst[kvp.Key] = reader[kvp.Key];
                    }
                    else
                        currInst[kvp.Key] = SelectFromTable(kvp.Value.GetType(), new SqlConnection(conn.ConnectionString), new Dictionary<string, object> { { "ID", (int)reader[kvp.Key] } })[0];

                }

                toRet.Add(currInst.Deserialize());

            }

            conn.Close();

            Console.WriteLine(tableCommand);
            return toRet;
        }

    }
}