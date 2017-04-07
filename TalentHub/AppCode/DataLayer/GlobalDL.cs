using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using TalentHub.AppCode.Models;
using TalentHub.AppCode.Utilities;

namespace TalentHub.AppCode.DataLayer {
	public class GlobalDL {
		private List<KeyValuePair<string, object>> parameterCollection;
		private MySqlConnection sqlConnection;

		public GlobalDL() {
			parameterCollection = new List<KeyValuePair<string, object>>();
		}

		internal void AddParameter(string parameterName, object parameterValue) {
			parameterCollection.Add(new KeyValuePair<string, object>(parameterName, parameterValue));
		}

		internal object ExecuteSqlReturnScalar(string connectionString, string sql) {
			const string methodName = "ExecuteSqlReturnScalar";
			this.LogCall(methodName, sql);

			try {
				using (MySqlConnection connection = new MySqlConnection(connectionString)) {
					using (MySqlCommand command = connection.CreateCommand()) {
						command.CommandType = CommandType.Text;
						command.CommandText = sql;
						connection.Open();

						foreach (KeyValuePair<string, object> item in parameterCollection) {
							command.Parameters.AddWithValue(item.Key, item.Value);
						}

						return command.ExecuteScalar();
					}
				}
			}
			catch (Exception ex) {
				// LoggerBL.LogException (className, methodName, new SqlException (GetSqlWithParams (sql), ex));
				throw;
			}
			finally {
				ClearParams();
				// LoggerBL.LogMethodEnd (className, methodName);
			}
		}

		internal T ExecuteSqlReturnScalar<T>(string connectionString, string sql) {
			object val = this.ExecuteSqlReturnScalar(connectionString, sql);

			if (val == null || val == DBNull.Value) {
				return default(T);
			}
			else {
				return (T) Convert.ChangeType(val, typeof(T));
			}
		}

		internal MySqlDataReader ExecuteSqlReturnReader(string connectionString, string sql) {
			const string methodName = "ExecuteSqlReturnReader";
			this.LogCall(methodName, sql);
			MySqlConnection connection = new MySqlConnection(connectionString);

			try {
				using (MySqlCommand command = connection.CreateCommand()) {
					command.CommandType = CommandType.Text;
					command.CommandText = sql;

					foreach (KeyValuePair<string, object> item in parameterCollection) {
						command.Parameters.AddWithValue(item.Key, item.Value);
					}

					connection.Open();

					return command.ExecuteReader(CommandBehavior.CloseConnection);
				}
			}
			catch (Exception ex) {
				connection.Close();
				// LoggerBL.LogException (className, methodName, new SqlException (GetSqlWithParams (sql), ex));
				throw;
			}
			finally {
				ClearParams();
				// LoggerBL.LogMethodEnd (className, methodName);
			}
		}

		internal void AddDateParameter(string parameterName, DateTime? parameterValue) {
			// DateTime.MinValue = January 01, 0001 00:00:00:0000
			DateTime value = (parameterValue == null) ? DateTime.MinValue : (DateTime) parameterValue;

			parameterCollection.Add(new KeyValuePair<string, object>(parameterName, DateUtility.GetDateForSqlParam(value)));
		}

		internal void AddDateParameterWithNull(string parameterName, DateTime? parameterValue) {
			if (parameterValue == null) {
				parameterCollection.Add(new KeyValuePair<string, object>(parameterName, DBNull.Value));
			}
			else {
				parameterCollection.Add(new KeyValuePair<string, object>(parameterName, DateUtility.GetDateForSqlParam((DateTime) parameterValue)));
			}
		}

		internal void AddIntParameter(string parameterName, object parameterValue) {
			if (parameterValue == null || String.IsNullOrWhiteSpace(parameterValue.ToString())) {
				parameterValue = DBNull.Value;
			}

			AddParameter(parameterName, parameterValue);
		}

		internal void AddLikeParameter(string parameterName, string parameterValue) {
			parameterCollection.Add(new KeyValuePair<string, object>(parameterName, Utility.AddWildCard(parameterValue)));
		}

		internal void ClearParams() {
			parameterCollection.Clear();
		}

		internal void ExecuteProcedureNonQuery(string connectionString, string procedureName) {
			const string methodName = "ExecuteProcedureNonQuery";
			this.LogCall(methodName, procedureName);
			try {
				using (MySqlConnection connection = new MySqlConnection(connectionString)) {
					using (MySqlCommand command = connection.CreateCommand()) {
						command.CommandType = CommandType.StoredProcedure;
						command.CommandText = procedureName;

						foreach (KeyValuePair<string, object> item in parameterCollection) {
							command.Parameters.AddWithValue(item.Key, item.Value);
						}

						connection.Open();

						command.ExecuteNonQuery();
					}
				}
			}
			catch (Exception ex) {
				// LoggerBL.LogException (className, methodName, new SqlException (GetSqlWithParams (procedureName), ex));
				throw;
			}
			finally {
				ClearParams();
				// LoggerBL.LogMethodEnd (className, methodName);
			}
		}

		internal void ExecuteProcedureNonQueryAsync(string connectionString, string procedureName) {
			const string methodName = "ExecuteProcedureNonQueryAsync";
			this.LogCall(methodName, procedureName);
			try {
				sqlConnection = new MySqlConnection(connectionString);

				MySqlCommand command = sqlConnection.CreateCommand();
				command.CommandType = CommandType.StoredProcedure;
				command.CommandText = procedureName;

				foreach (KeyValuePair<string, object> item in parameterCollection) {
					command.Parameters.AddWithValue(item.Key, item.Value);
				}

				sqlConnection.Open();

				command.BeginExecuteNonQuery(new AsyncCallback(HandleCallback), command);
			}
			catch (Exception ex) {
				// LoggerBL.LogException (className, methodName, new SqlException (GetSqlWithParams (procedureName), ex));
				throw;
			}
			finally {
				ClearParams();
				// LoggerBL.LogMethodEnd (className, methodName);
			}
		}

		internal MySqlDataReader ExecuteProcedureReturnReader(string connectionString, string procedureName) {
			const string methodName = "ExecuteProcedureReturnReader";
			this.LogCall(methodName, procedureName);

			MySqlConnection connection = new MySqlConnection(connectionString);

			try {
				using (MySqlCommand command = connection.CreateCommand()) {
					command.CommandType = CommandType.StoredProcedure;
					command.CommandText = procedureName;

					foreach (KeyValuePair<string, object> item in this.parameterCollection) {
						command.Parameters.AddWithValue(item.Key, item.Value);
					}

					connection.Open();

					return command.ExecuteReader(CommandBehavior.CloseConnection);
				}
			}
			catch (Exception ex) {
				connection.Close();
				// LoggerBL.LogException (className, methodName, new SqlException (GetSqlWithParams (procedureName), ex));
				throw;
			}
			finally {
				ClearParams();
				// LoggerBL.LogMethodEnd (className, methodName);
			}
		}

		internal object ExecuteProcedureReturnScalar(string connectionString, string procedureName) {
			const string methodName = "ExecuteProcedureReturnScalar";
			this.LogCall(methodName, procedureName);
			try {
				using (MySqlConnection connection = new MySqlConnection(connectionString)) {
					using (MySqlCommand command = connection.CreateCommand()) {
						command.CommandType = CommandType.StoredProcedure;
						command.CommandText = procedureName;

						foreach (KeyValuePair<string, object> item in parameterCollection) {
							command.Parameters.AddWithValue(item.Key, item.Value);
						}

						connection.Open();

						return command.ExecuteScalar();
					}
				}
			}
			catch (Exception ex) {
				// LoggerBL.LogException (className, methodName, new SqlException (GetSqlWithParams (procedureName), ex));
				throw;
			}
			finally {
				ClearParams();
				// LoggerBL.LogMethodEnd (className, methodName);
			}
		}

		internal T ExecuteProcedureReturnScalar<T>(string connectionString, string procedureName) {
			object val = this.ExecuteProcedureReturnScalar(connectionString, procedureName);

			if (val == null || val == DBNull.Value) {
				return default(T);
			}
			else {
				return (T) Convert.ChangeType(val, typeof(T));
			}
		}

		internal int ExecuteSqlNonQuery(string sql) {
			return this.ExecuteSqlNonQuery(Utility.ConnectionString, sql);
		}

		internal int ExecuteSqlNonQuery(string connectionString, string sql) {
			const string methodName = "ExecuteSqlNonQuery";
			this.LogCall(methodName, sql);
			int recordsAffected = 0;

			try {
				using (MySqlConnection connection = new MySqlConnection(connectionString)) {
					using (MySqlCommand command = connection.CreateCommand()) {
						command.CommandType = CommandType.Text;
						command.CommandText = sql;

						connection.Open();

						foreach (KeyValuePair<string, object> item in parameterCollection) {
							command.Parameters.AddWithValue(item.Key, item.Value);
						}

						recordsAffected = command.ExecuteNonQuery();
					}
				}
			}
			catch (Exception ex) {
				// LoggerBL.LogException (className, methodName, new SqlException (GetSqlWithParams (sql), ex));
				throw;
			}
			finally {
				ClearParams();
				// LoggerBL.LogMethodEnd (className, methodName);
			}
			return recordsAffected;
		}

		internal DateTime GetDbTime(string connectionString) {
			const string methodName = "GetDbTime";
			// LoggerBL.LogMethodStart (className, methodName);

			const string sql = @"select current_timestamp();";

			DateTime returnValue = DateTime.Parse(this.ExecuteSqlReturnScalar(connectionString, sql).ToString());

			// LoggerBL.LogMethodEnd (className, methodName);
			return returnValue;
		}

		internal int GetIntWithDefault(object value) {
			int returnValue;

			if (Int32.TryParse(value.ToString(), out returnValue)) {
				return returnValue;
			}
			else {
				return 0;
			}
		}

		internal void GetResults(string sql, List<FilterDto> filters, List<KeyValuePair<string, string>> orderByCols, int pageSize, int pageNumber) {
			StringBuilder sb = new StringBuilder();
			int counter = 0;

			sb.AppendLine(sql);

			if (filters.Count > 0) {
				foreach (FilterDto f in filters) {
					if (counter == 0) {
						sb.AppendLine(" where ");
					}
					else {
						sb.AppendLine(" and ");
					}
					sb.AppendLine(f.ColumnName);
					sb.AppendLine(" like ");
					sb.AppendLine(f.ColumnValue);
					counter++;
				}
			}

			if (orderByCols.Count > 0) {
				sb.AppendLine("order by ");
				foreach (KeyValuePair<string, string> k in orderByCols) {
					sb.AppendLine(k.Key);
					sb.AppendLine(" ");
					sb.AppendLine(k.Value);
				}
			}

			if (pageSize != -1) {
				const string LIMIT = "limit {0}, {1}";
				int startCount = pageSize * pageNumber;

				sb.AppendLine(string.Format(LIMIT, startCount, pageSize));
			}

			sb.AppendLine(";");
		}

		private string GetSqlWithParams(string sql) {
			StringBuilder sb = new StringBuilder();

			sb.AppendLine(sql);

			foreach (KeyValuePair<string, object> item in parameterCollection) {
				sb.Append(item.Key).Append(':').AppendLine((item.Value == null) ? "null" : item.Value.ToString());
			}

			return sb.ToString();
		}

		private void HandleCallback(IAsyncResult result) {
			const string methodName = "ExecuteProcedureNonQueryAsync";

			try {
				// Retrieve the original command object, passed
				// to this procedure in the AsyncState property
				// of the IAsyncResult parameter.
				MySqlCommand command = (MySqlCommand) result.AsyncState;
				try {
					command.EndExecuteNonQuery(result);
				}
				catch { }
			}
			catch (Exception ex) {
				// LoggerBL.LogException (className, methodName, ex);
			}
			finally {
				if (sqlConnection != null) {
					sqlConnection.Close();
				}
			}
		}

		private void LogCall(string methodName, string sql) {
			// LoggerBL.LogMethodStart (className, methodName);

			// LoggerBL.LogDebug (className, methodName, GetSqlWithParams (sql));
		}
	}
}