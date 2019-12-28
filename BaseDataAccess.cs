using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Diagnostics;

public class BaseDataAccess
{
    protected string ConnectionString { get; set; }

    public BaseDataAccess()
    {
    }

    public BaseDataAccess(string connectionString)
    {
        this.ConnectionString = connectionString;
    }

    private SqlConnection GetConnection()
    {
        var connection = new SqlConnection(this.ConnectionString);
        if (connection.State != ConnectionState.Open)
            connection.Open();
        return connection;
    }

    protected SqlCommand GetCommand(DbConnection connection, string commandText, CommandType commandType)
    {
        var command = new SqlCommand(commandText, connection as SqlConnection);
        command.CommandType = commandType;
        return command;
    }

    protected SqlParameter GetParameter(string parameter, object value)
    {
        var parameterObject = new SqlParameter(parameter, value != null ? value : DBNull.Value);
        parameterObject.Direction = ParameterDirection.Input;
        return parameterObject;
    }

    protected SqlParameter GetParameterOut(string parameter, SqlDbType type, object value = null, ParameterDirection parameterDirection = ParameterDirection.InputOutput)
    {
        var parameterObject = new SqlParameter(parameter, type); ;

        if (type == SqlDbType.NVarChar || type == SqlDbType.VarChar || type == SqlDbType.NText || type == SqlDbType.Text)
        {
            parameterObject.Size = -1;
        }

        parameterObject.Direction = parameterDirection;

        if (value != null)
        {
            parameterObject.Value = value;
        }
        else
        {
            parameterObject.Value = DBNull.Value;
        }

        return parameterObject;
    }

    public int ExecuteNonQuery(string procedureName, IEnumerable<DbParameter> parameters, CommandType commandType)
    {
        var returnValue = -1;

        try
        {
            using (SqlConnection connection = this.GetConnection())
            {
                var cmd = this.GetCommand(connection, procedureName, commandType);

                if (parameters != null && parameters.Any())
                {
                    cmd.Parameters.AddRange(parameters.ToArray());
                }

                returnValue = cmd.ExecuteNonQuery();
            }
        }
        catch (Exception)
        {
            //LogException("Failed to ExecuteNonQuery for " + procedureName, ex, parameters);
            throw;
        }

        return returnValue;
    }


    public DataSet ExecuteCommand(string procedureName, CommandType commandType, IEnumerable<DbParameter> parameters)
    {
        var dataTable = new DataSet();
        try
        {
            using (var connection = this.GetConnection())
            {
                var cmd = this.GetCommand(connection, procedureName, commandType);
                if (parameters != null && parameters.Any())
                {
                    cmd.Parameters.AddRange(parameters.ToArray());
                }

                var da = new SqlDataAdapter(cmd);
                // this will query your database and return the result to your datatable
                da.Fill(dataTable);

                return dataTable;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to ExecuteNonQuery for " + procedureName, ex, parameters);
            throw;
        }
    }

    protected object ExecuteScalar(string procedureName, List<SqlParameter> parameters)
    {
        object returnValue = null;

        try
        {
            using (DbConnection connection = this.GetConnection())
            {
                DbCommand cmd = this.GetCommand(connection, procedureName, CommandType.StoredProcedure);

                if (parameters != null && parameters.Count > 0)
                {
                    cmd.Parameters.AddRange(parameters.ToArray());
                }

                returnValue = cmd.ExecuteScalar();
            }
        }
        catch (Exception)
        {
            //LogException("Failed to ExecuteScalar for " + procedureName, ex, parameters);
            throw;
        }

        return returnValue;
    }

    protected DbDataReader GetDataReader(string procedureName, List<DbParameter> parameters, CommandType commandType = CommandType.StoredProcedure)
    {
        DbDataReader ds;

        try
        {
            DbConnection connection = this.GetConnection();
            {
                DbCommand cmd = this.GetCommand(connection, procedureName, commandType);
                if (parameters != null && parameters.Count > 0)
                {
                    cmd.Parameters.AddRange(parameters.ToArray());
                }

                ds = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
        }
        catch (Exception)
        {
            //LogException("Failed to GetDataReader for " + procedureName, ex, parameters);
            throw;
        }

        return ds;
    }
}