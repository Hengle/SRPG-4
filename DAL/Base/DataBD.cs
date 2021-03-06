﻿
using System;
using Mono.Data.Sqlite;
using System.Diagnostics;

namespace DAL.Base
{
    /// <summary>
    /// 使用sqlite
    /// </summary>
    public class DbAccess
    {

        private SqliteConnection dbConnection;
        private SqliteCommand dbCommand;
        private SqliteDataReader reader;

        public DbAccess(string connectionString)
        {
            OpenDB(connectionString);
        }
        public DbAccess()
        {

        }

        public void OpenDB(string connectionString)
        {
            try
            {
                dbConnection = new SqliteConnection(connectionString);
                dbConnection.Open();
                Debug.WriteLine("Connected to db");
            }
            catch (Exception e)
            {
                string temp1 = e.ToString();
                Debug.WriteLine(temp1);
            }

        }

        public void CloseSqlConnection()
        {

            if (dbCommand != null)
            {
                dbCommand.Dispose();
            }

            dbCommand = null;
            if (reader != null)
            {
                reader.Dispose();
            }
            reader = null;
            if (dbConnection != null)
            {
                dbConnection.Close();
            }
            dbConnection = null;
            Debug.WriteLine("Disconnected from db.");
        }

        public SqliteDataReader ExecuteQuery(string sqlQuery)
        {
            dbCommand = dbConnection.CreateCommand();
            dbCommand.CommandText = sqlQuery;
            reader = dbCommand.ExecuteReader();
            return reader;
        }

        public SqliteDataReader ReadFullTable(string tableName)
        {
            string query = "SELECT * FROM " + tableName;
            return ExecuteQuery(query);
        }

        public SqliteDataReader InsertInto(string tableName, string[] values)
        {
            string query = "INSERT INTO " + tableName + " VALUES (" + values[0];
            for (int i = 1; i < values.Length; ++i)
            {
                query += ", " + values[i];
            }
            query += ")";
            return ExecuteQuery(query);
        }

        public SqliteDataReader UpdateInto(string tableName, string[] cols, string[] colsvalues, string selectkey, string selectvalue)
        {
            string query = "UPDATE " + tableName + " SET " + cols[0] + " = " + colsvalues[0];
            for (int i = 1; i < colsvalues.Length; ++i)
            {
                query += ", " + cols[i] + " =" + colsvalues[i];
            }
            query += " WHERE " + selectkey + " = " + selectvalue + " ";
            return ExecuteQuery(query);
        }

        public SqliteDataReader Delete(string tableName, string[] cols, string[] colsvalues)
        {
            string query = "DELETE FROM " + tableName + " WHERE " + cols[0] + " = " + colsvalues[0];
            for (int i = 1; i < colsvalues.Length; ++i)
            {
                query += " or " + cols[i] + " = " + colsvalues[i];
            }
            Debug.WriteLine(query);
            return ExecuteQuery(query);
        }

        public SqliteDataReader InsertIntoSpecific(string tableName, string[] cols, string[] values)
        {
            if (cols.Length != values.Length)
            {
                throw new SqliteException("columns.Length != values.Length");

            }
            string query = "INSERT INTO " + tableName + "(" + cols[0];
            for (int i = 1; i < cols.Length; ++i)
            {
                query += ", " + cols[i];
            }
            query += ") VALUES (" + values[0];
            for (int i = 1; i < values.Length; ++i)
            {
                query += ", " + values[i];
            }
            query += ")";
            return ExecuteQuery(query);
        }

        public SqliteDataReader DeleteContents(string tableName)
        {
            string query = "DELETE FROM " + tableName;
            return ExecuteQuery(query);
        }

        public SqliteDataReader CreateTable(string name, string[] col, string[] colType)
        {
            if (col.Length != colType.Length)
            {
                throw new SqliteException("columns.Length != colType.Length");
            }
            string query = "CREATE TABLE " + name + " (" + col[0] + " " + colType[0];
            for (int i = 1; i < col.Length; ++i)
            {
                query += ", " + col[i] + " " + colType[i];
            }
            query += ")";
            return ExecuteQuery(query);
        }
        public SqliteDataReader CreateTable(string name, string[] col)
        {
            string query = "CREATE TABLE " + name + " (" + col[0];
            for (int i = 1; i < col.Length; ++i)
            {
                query += ", " + col[i];
            }
            query += ")";
            Debug.WriteLine(query, "selectSql");
            return ExecuteQuery(query);
        }
        public SqliteDataReader SelectWhere(string tableName, string[] items, string[] col, string[] operation, string[] values)
        {
            if (col != null && (col.Length != operation.Length || operation.Length != values.Length))
            {
                throw new SqliteException("col.Length != operation.Length != values.Length");
            }
            string query = "SELECT ";
            if(items != null && items.Length > 0)
            {
                query += items[0];
                for (int i = 1; i < items.Length; ++i)
                {
                    query += ", " + items[i];
                }
            }
            else
            {
                query += "* ";
            }
            query += " FROM " + tableName;
            if(col != null)
            {
                query += " WHERE " + col[0] + operation[0] + values[0];
                for (int i = 1; i < col.Length; ++i)
                {
                    query += " AND " + col[i] + operation[i] + values[0];
                }
            }
            Debug.WriteLine(query, "查询SQL");
            return ExecuteQuery(query);
        }

        public SqliteDataReader SelectWhere(string query)
        {
            Debug.WriteLine(query, "selectSql");
            return ExecuteQuery(query);
        }

        public SqliteDataReader SelectJoinWhere(string mainTableName, string[] slaveTableNames, string[] items, string[] ids)
        {
            string query = "SELECT ";
            foreach (string s in items)
            {
                query += "m." + s + ",";
            }
            for (int i = 0; i < slaveTableNames.Length; i++)
            {
                query += "s" + i + ".Name AS " + slaveTableNames[i] + "_Name,";
            }
            query = query.Substring(0, query.Length - 1);
            query += " FROM " + mainTableName + " AS m ";
            for(int i = 0;i < slaveTableNames.Length; i++)
            {
                query += " JOIN " + slaveTableNames[i] + " AS s" + i + " ON m." + slaveTableNames[i] + "_Id = s" + i + ".Id ";
            }
            if(ids != null && ids.Length > 0)
            {
                query += "WHERE m.Id In (";
                foreach(string s in ids)
                {
                    query += s + ",";
                }
                query = query.Substring(0, query.Length - 1);
                query += ")";
            }
            return ExecuteQuery(query);
        }
    }
}
