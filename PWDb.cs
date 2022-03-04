namespace techassessment
{
    public class PWDb
    {
        protected string ConnectionString { get; set; }
        System.Data.SqlClient.SqlConnection conn = default(System.Data.SqlClient.SqlConnection);
        System.Data.SqlClient.SqlCommand cmd = default(System.Data.SqlClient.SqlCommand);
        System.Data.SqlClient.SqlDataAdapter da = default(System.Data.SqlClient.SqlDataAdapter);
        System.Data.SqlClient.SqlTransaction trx = default(System.Data.SqlClient.SqlTransaction);
        public PWDb() { }
        public PWDb(string connectionString)
        {
            conn = new System.Data.SqlClient.SqlConnection(connectionString);
            cmd = new System.Data.SqlClient.SqlCommand();
            da = new System.Data.SqlClient.SqlDataAdapter();
            cmd.Connection = conn;
        }
        public void Disconnect()
        {
            conn.Close();
            conn.Dispose();
        }
        public System.Data.DataSet Execute_FillDataset(string query)
        {
            System.Data.DataSet? ds = new System.Data.DataSet();
            string SQLStr = "";
            try
            {
                if (conn.State == System.Data.ConnectionState.Closed) conn.Open();
                SQLStr = query;
                cmd.CommandText = SQLStr;
                da.SelectCommand = cmd;
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"! Exception while execute dataset fill function. {ex.Message}");
                ds = null;
            }
            finally
            {
                if (conn.State == System.Data.ConnectionState.Open) conn.Close();
            }
            return ds;
        }
        public string Execute_NonQuery(string query, bool IsReturnValue = false)
        {
            string result = "";
            try
            {
                if (conn.State == System.Data.ConnectionState.Closed) conn.Open();
                cmd.CommandText = query;
                if (IsReturnValue == true)
                {
                    result = Convert.ToString(cmd.ExecuteNonQuery());
                }
                else
                {
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"! Exception when executing non query statement. {ex.Message}");
                result = "-1";
            }
            finally
            {
                if (conn.State == System.Data.ConnectionState.Open) conn.Close();
            }
            return result;
        }
        public object Execute_Scalar(string query)
        {
            object result = null;
            try
            {
                if (conn.State == System.Data.ConnectionState.Closed) conn.Open();
                cmd.CommandText = query;
                result = cmd.ExecuteScalar();
                if (result == null)
                {
                    result = "";
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception executing scalar statement. {ex.Message}");
                result = "";
            }
            finally
            {
                if (conn.State == System.Data.ConnectionState.Open) conn.Close();
            }
            return result;
        }
        public string SQLS(string data)
        {
            return string.IsNullOrEmpty(data) ? "" : "N'" + data.Replace("'", "''") + "'";
        }
        public dynamic Dyna(object value)
        {
            try
            {
                if (value != null)
                {
                    string valuetype = value.GetType().Name;
                    switch (valuetype)
                    {
                        case "String":
                            return SQLS(value.ToString().TrimEnd());
                        case "DateTime":
                            return SQLS(Convert.ToDateTime(value).ToString("yyyy-MM-ddTHH:mm:ss"));
                        default:
                            return value;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"! Exception while handling platform well record. {ex.Message}");
                throw;
            }
        }
        private bool IsRecordExist(object id, object tablename)
        {
            bool IsExist = false;
            try
            {
                string SQLQuery = $"SELECT COUNT(id) FROM {tablename} WHERE id={id}";
                int? cnt = Execute_Scalar(SQLQuery) as int?;
                if (cnt != null && cnt > 0)
                    IsExist = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"! Exception while verify record is exist. {ex.Message}");
            }
            return IsExist;
        }
        public void HandlePlatformWellData(List<Platform> pw)
        {
            try
            {
                if (pw != null)
                {
                    if (pw.Count == 0)
                    {
                        Console.WriteLine($"! Platform record empty");
                    }
                    foreach (Platform p in pw)
                    {
                        InsertUpdatePlatformRecord(p);
                        if (p.well != null)
                        {
                            if (p.well.Count == 0)
                            {
                                Console.WriteLine($"! No well record under this platform: {p.uniqueName}");
                            }
                            foreach (Well w in p.well)
                            {
                                InsertUpdateWellRecord(w);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"! Exception while handling platform well record. {ex.Message}");
            }
        }
        private void InsertUpdatePlatformRecord(Platform p)
        {
            try
            {
                string SQLQuery = string.Empty;
                if (IsRecordExist(p.id, "platform"))
                    SQLQuery = $"UPDATE dbo.platform SET uniqueName={Dyna(p.uniqueName)},latitude={Dyna(p.latitude)},longitude={Dyna(p.longitude)}" + (p.createdAt != null ? $",createdAt={Dyna(p.createdAt)}" : "") + (p.updatedAt != null ? $",updatedAt={Dyna(p.updatedAt)}" : "") + $" WHERE id={Dyna(p.id)};";
                else
                    SQLQuery = $"INSERT INTO dbo.platform(id,uniqueName,latitude,longitude{(p.createdAt != null ? ",createdAt" : "")}{(p.updatedAt != null ? ",updatedAt" : "")}) VALUES({ Dyna(p.id)},{Dyna(p.uniqueName)},{Dyna(p.latitude)},{Dyna(p.longitude)}{(p.createdAt != null ? $",{ Dyna(p.createdAt)}" : "")}{(p.updatedAt != null ? $",{ Dyna(p.updatedAt)}" : "")})";
                int affected = Convert.ToInt32(Execute_NonQuery(SQLQuery, true));

            }
            catch (Exception ex)
            {
                Console.WriteLine($"! Exception while inserting platform record. {ex.Message}");
            }
        }
        private void InsertUpdateWellRecord(Well w)
        {
            try
            {
                string SQLQuery = string.Empty;
                if (IsRecordExist(w.id, "well"))
                    SQLQuery = $"UPDATE dbo.well SET platformId = {Dyna(w.platformId)},uniqueName={Dyna(w.uniqueName)},latitude={Dyna(w.latitude)},longitude = {Dyna(w.longitude)}" + (w.createdAt != null ? $",createdAt = {Dyna(w.createdAt)}" : "") + (w.updatedAt != null ? $",updatedAt = {Dyna(w.updatedAt)}" : "") + $"WHERE id = {Dyna(w.id)};";
                else
                    SQLQuery = $"INSERT INTO dbo.well(id,platformId,uniqueName,latitude,longitude{(w.createdAt != null ? ",createdAt" : "")}{(w.updatedAt != null ? ",updatedAt" : "")}) VALUES({ Dyna(w.id)},{Dyna(w.platformId)},{Dyna(w.uniqueName)},{Dyna(w.latitude)},{Dyna(w.longitude)}{(w.createdAt != null ? $",{ Dyna(w.createdAt)}" : "")}{(w.updatedAt != null ? $",{ Dyna(w.updatedAt)}" : "")})";

                int affected = Convert.ToInt32(Execute_NonQuery(SQLQuery, true));

            }
            catch (Exception ex)
            {
                Console.WriteLine($"! Exception while inserting well record. {ex.Message}");
            }
        }
    }
}
