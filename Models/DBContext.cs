using System;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Data;
using Notification.Models.Responses;
using Notification.Models.Requests;
using Notification.Models.Tables;

namespace Notification.Models
{
    public class DBContext
    {
        public string ConnectionString { get; set; }
        public DBContext(string connStr)
        {
            this.ConnectionString = connStr;            
        }
        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }

        public GenericGetDataResponse GetCredentialListByProviderName(string ProgramId, string providerName)
        {
            GenericGetDataResponse resp = new GenericGetDataResponse();

            List<Dictionary<string, string>> contents = new List<Dictionary<string, string>>();
            Dictionary<string, string> content = new Dictionary<string, string>();

            string query = "SELECT * FROM CREDENTIAL " +
                " LEFT JOIN PROVIDER ON PROVIDER_ID = CREDENTIAL_PROVIDER_ID" +
                " LEFT JOIN PROVIDER_TYPE  ON PROVIDER_TYPE_ID = CREDENTIAL_PROVIDER_TYPE_ID" +
                " WHERE CREDENTIAL_STATUS='A' ";

            if (!string.IsNullOrEmpty(providerName))
            {
                query = query + " AND PROVIDER_NAME = @PROVIDER_NAME";
            }
            if (!string.IsNullOrEmpty(ProgramId))
            {
                query = query + " AND CREDENTIAL_PROGRAM_ID = @PROGRAM_ID ";
            }
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    DataTable dt = new DataTable();
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    if (!string.IsNullOrEmpty(ProgramId))
                    {
                        cmd.Parameters.Add(new MySqlParameter("@PROGRAM_ID", ProgramId));
                    }
                    if (!string.IsNullOrEmpty(providerName))
                    {
                        cmd.Parameters.Add(new MySqlParameter("@PROVIDER_NAME", providerName));
                    }
                    MySqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        dt.Load(reader);
                        reader.Close();
                        foreach (DataRow dr in dt.Rows)
                        {
                            content = new Dictionary<string, string>();
                            content.Add("CredentialId", dr["CREDENTIAL_ID"].ToString());
                            content.Add("ProviderId", dr["PROVIDER_ID"].ToString());
                            content.Add("ProviderName", dr["PROVIDER_NAME"].ToString());
                            content.Add("ProviderTypeId", dr["PROVIDER_TYPE_ID"].ToString());
                            content.Add("ProviderTypeName", dr["PROVIDER_TYPE_NAME"].ToString());
                            content.Add("ProgramId", dr["CREDENTIAL_PROGRAM_ID"].ToString());

                            JArray json = JsonConvert.DeserializeObject<JArray>(dr["CREDENTIAL_REQUIREMENT"].ToString());
                            foreach (JObject item in json)
                            {
                                foreach (var x in item)
                                {
                                    string name = x.Key;
                                    string value = x.Value.ToString();
                                    content.Add(name, value);
                                }
                            }
                            contents.Add(content);
                        }
                    }
                    conn.Close();
                }
                resp.isSuccess = true;
                resp.Message = "Information has been retrieve.";
                resp.Data = contents;

            }
            catch (Exception ex)
            {
                resp.isSuccess = false;
                resp.Message = ex.Message;
            }
            GenericAPIRequest r = new GenericAPIRequest();
            LogAPICalls("GetCredentials", r, resp);
            return resp;
            //}
        }


        public ProgramAuthUser GetAuthUser(string username, string password)
        {
            List<ProgramAuthUser> _users = new List<ProgramAuthUser>();
            ProgramAuthUser _user = new ProgramAuthUser();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                try
                {
                    MySqlCommand sql = new MySqlCommand("SELECT * FROM AUTH_USER WHERE AUTH_USER_STATUS = 'A' AND AUTH_USER_LOGIN='" +  username + "' AND AUTH_USER_PASSWORD= SHA1(@PASSWORD) ", conn);
                    sql.Parameters.Add(new MySqlParameter("@PASSWORD", password));
                    MySqlDataReader reader = sql.ExecuteReader();
                    if (reader.HasRows)
                    {
                        DataTable dt = new DataTable();
                        dt.Load(reader);
                        foreach (DataRow row in dt.Rows)
                        {
                            _user = new ProgramAuthUser();
                            _user.Id = row["AUTH_USER_ID"].ToString();
                            _user.Name = row["AUTH_USER_NAME"].ToString();
                            _user.Description = row["AUTH_USER_DESCRIPTION"].ToString();
                            _user.Username = row["AUTH_USER_LOGIN"].ToString();
                            _user.Password = row["AUTH_USER_PASSWORD"].ToString();
                            _user.Key = row["AUTH_USER_KEY"].ToString();
                            //_users.Add(_user);
                        }
                    }
                    reader.Close();
                }
                catch
                {

                }
                conn.Close();
            }
            return _user;
        }



        public GenericGetDataResponse GetCredentialList(string ProgramId, string CredentialId = "")
        {
            GenericGetDataResponse resp = new GenericGetDataResponse();

            List<Dictionary<string, string>> contents = new List<Dictionary<string, string>>();
            Dictionary<string, string> content = new Dictionary<string, string>();

            string query = "SELECT * FROM CREDENTIAL " +
                " LEFT JOIN PROVIDER ON PROVIDER_ID = CREDENTIAL_PROVIDER_ID" +
                " LEFT JOIN PROVIDER_TYPE  ON PROVIDER_TYPE_ID = CREDENTIAL_PROVIDER_TYPE_ID" +
                " WHERE CREDENTIAL_STATUS='A' ";

            if(!string.IsNullOrEmpty(CredentialId))
            {
                query = query + " AND CREDENTIAL_ID = @ID";
            }
            if (!string.IsNullOrEmpty(ProgramId))
            {
                query = query + " AND CREDENTIAL_PROGRAM_ID = @PROGRAM_ID ";
            }
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    DataTable dt = new DataTable();
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    if (!string.IsNullOrEmpty(ProgramId))
                    {
                        cmd.Parameters.Add(new MySqlParameter("@PROGRAM_ID", ProgramId));
                    }
                    if (!string.IsNullOrEmpty(CredentialId))
                    {
                        cmd.Parameters.Add(new MySqlParameter("@ID", CredentialId));
                    }
                    MySqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        dt.Load(reader);
                        reader.Close();
                        foreach (DataRow dr in dt.Rows)
                        {
                            content = new Dictionary<string, string>();
                            content.Add("CredentialId", dr["CREDENTIAL_ID"].ToString());
                            content.Add("ProviderId", dr["PROVIDER_ID"].ToString());
                            content.Add("ProviderName", dr["PROVIDER_NAME"].ToString());
                            content.Add("ProviderTypeId", dr["PROVIDER_TYPE_ID"].ToString());
                            content.Add("ProviderTypeName", dr["PROVIDER_TYPE_NAME"].ToString());
                            content.Add("ProgramId", dr["CREDENTIAL_PROGRAM_ID"].ToString());

                            JArray json = JsonConvert.DeserializeObject<JArray>(dr["CREDENTIAL_REQUIREMENT"].ToString());
                            foreach (JObject item in json)
                            {
                                foreach (var x in item)
                                {
                                    string name = x.Key;
                                    string value = x.Value.ToString();
                                    content.Add(name, value);
                                }
                            }
                            contents.Add(content);
                        }
                    }
                    conn.Close();
                }
                resp.isSuccess = true;
                resp.Message = "Information has been retrieve.";
                resp.Data = contents;

            }
            catch (Exception ex)
            {
                resp.isSuccess = false;
                resp.Message = ex.Message;
            }
            GenericAPIRequest r = new GenericAPIRequest();
            LogAPICalls("GetCredentials", r, resp);
            return resp;
            //}
        }

        public bool CheckIfCredentialExist(string ProviderTypeId, string ProgramId, string ProviderId, string CredentialId = "")
        {
            bool isExist = false;
            string query = "SELECT CREDENTIAL_ID FROM CREDENTIAL WHERE CREDENTIAL_PROVIDER_ID = @ProviderId AND CREDENTIAL_PROVIDER_TYPE_ID=@ProviderTypeId AND CREDENTIAL_PROGRAM_ID = @ProgramId AND CREDENTIAL_ID != @CredentialId";

            
            List<MySqlParameter> mySqlParameters = new List<MySqlParameter>();
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.Add(new MySqlParameter("@ProviderId", ProviderId));
                    cmd.Parameters.Add(new MySqlParameter("@ProviderTypeId", ProviderTypeId));
                    cmd.Parameters.Add(new MySqlParameter("@ProgramId", ProgramId));
                    cmd.Parameters.Add(new MySqlParameter("@CredentialId", CredentialId));
                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        isExist = true;
                    }
                    reader.Close();
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return isExist;
        }


        public GenericInsertUpdateResponse CreateCredential(GenericAPIRequest r, bool isNew = true)
        {
            GenericInsertUpdateResponse resp = new GenericInsertUpdateResponse();
            string errMessage = "";
            string successMessage = "";

            if(!CheckIfIdExist("PROVIDER", r.ProviderId))
            {
                errMessage = "Invalid Provider Id.";
                resp.isSuccess = false;
                resp.Message = errMessage;
                return resp;
            }

            if (!CheckIfIdExist("PROVIDER_TYPE", r.ProviderTypeId))
            {
                errMessage = "Invalid Provider Type Id.";
                resp.isSuccess = false;
                resp.Message = errMessage;
                return resp;
            }


            
            if (CheckIfCredentialExist(r.ProviderTypeId, r.ProgramId, r.ProviderId, r.CredentialId))
            {
                errMessage = "Unable to create credential. Credential already exist.";
                resp.isSuccess = false;
                resp.Message = errMessage;
                return resp;
            }
            
            List<MySqlParameter> mySqlParameters = new List<MySqlParameter>();
            string myQuery = "";
            if (isNew)
            {
                myQuery = "INSERT INTO CREDENTIAL (CREDENTIAL_PROVIDER_ID, CREDENTIAL_PROVIDER_TYPE_ID, CREDENTIAL_PROGRAM_ID, CREDENTIAL_REQUIREMENT)" +
                " VALUES(" +
                " @CREDENTIAL_PROVIDER_ID, @CREDENTIAL_PROVIDER_TYPE_ID, @CREDENTIAL_PROGRAM_ID, @CREDENTIAL_REQUIREMENT " +
                " )";
            }
            else
            {
                myQuery = " UPDATE CREDENTIAL SET " +
                    " CREDENTIAL_PROVIDER_ID=@CREDENTIAL_PROVIDER_ID " +
                    " ,CREDENTIAL_PROVIDER_TYPE_ID = @CREDENTIAL_PROVIDER_TYPE_ID " +
                    " ,CREDENTIAL_PROGRAM_ID = @CREDENTIAL_PROGRAM_ID " +
                    " ,CREDENTIAL_REQUIREMENT=@CREDENTIAL_REQUIREMENT " +
                    " WHERE CREDENTIAL_ID = @CREDENTIAL_ID"
                    ;
            }

            mySqlParameters.Add(new MySqlParameter("@CREDENTIAL_PROVIDER_ID", r.ProviderId));
            mySqlParameters.Add(new MySqlParameter("@CREDENTIAL_PROVIDER_TYPE_ID", r.ProviderTypeId));
            mySqlParameters.Add(new MySqlParameter("@CREDENTIAL_PROGRAM_ID", r.ProgramId));
            mySqlParameters.Add(new MySqlParameter("@CREDENTIAL_REQUIREMENT", r.CredsJson));
            if (!isNew)
            {
                mySqlParameters.Add(new MySqlParameter("@CREDENTIAL_ID", r.CredentialId));
            }



            errMessage = "Unable to save credential. Please check your credential information";
            successMessage = "Credential has been saved.";
            string id = "";
            if (ExecuteQuery(myQuery, mySqlParameters, out id))
            {
                resp.isSuccess = true;
                resp.Message = successMessage;
                resp.Id = int.Parse(id);
            }
            else
            {
                errMessage = "Unable to create credential.";
                resp.isSuccess = false;
                resp.Message = errMessage;
            }
            LogAPICalls(r.MethodName, r, resp);
            return resp;
        }

        public GenericInsertUpdateResponse CreateProviderType(GenericAPIRequest r, bool isNew = true)
        {
            GenericInsertUpdateResponse resp = new GenericInsertUpdateResponse();
            string errMessage = "";
            string successMessage = "";
           

            if (CheckIfCodeExist("PROVIDER", r.ProviderTypeCode, r.ProviderTypeId))
            {
                errMessage = "Unable to create provider type. Provider Type Code already exist.";
                resp.isSuccess = false;
                resp.Message = errMessage;
                return resp;
            }

            List<MySqlParameter> mySqlParameters = new List<MySqlParameter>();
            string myQuery = "";
            if (isNew)
            {
                myQuery = "INSERT INTO PROVIDER_TYPE (PROVIDER_TYPE_CODE, PROVIDER_TYPE_NAME, PROVIDER_TYPE_DESCRIPTION)" +
                " VALUES(" +
                " @PROVIDER_TYPE_CODE, @PROVIDER_TYPE_NAME, @PROVIDER_TYPE_DESCRIPTION " +
                " )";
            }
            else
            {
                myQuery = " UPDATE PROVIDER_TYPE SET " +
                    " PROVIDER_TYPE_CODE=@PROVIDER_TYPE_CODE " +
                    " ,PROVIDER_TYPE_NAME = @PROVIDER_TYPE_NAME " +
                    " ,PROVIDER_TYPE_DESCRIPTION = @PROVIDER_TYPE_DESCRIPTION " +
                    " WHERE PROVIDER_TYPE_ID = @PROVIDER_TYPE_ID"
                    ;
            }

            mySqlParameters.Add(new MySqlParameter("@PROVIDER_TYPE_CODE", r.ProviderTypeCode));
            mySqlParameters.Add(new MySqlParameter("@PROVIDER_TYPE_NAME", r.ProviderTypeName));
            mySqlParameters.Add(new MySqlParameter("@PROVIDER_TYPE_DESCRIPTION", r.ProviderTypeDescription));
            if (!isNew)
            {
                mySqlParameters.Add(new MySqlParameter("@PROVIDER_TYPE_ID", r.ProviderTypeId));
            }



            errMessage = "Unable to save provider type. Please check your provider type information";
            successMessage = "Provider Type has been saved.";
            string id = "";
            if (ExecuteQuery(myQuery, mySqlParameters, out id))
            {
                resp.isSuccess = true;
                resp.Message = successMessage;
                resp.Id = int.Parse(id);
            }
            else
            {
                errMessage = "Unable to create provider type.";
                resp.isSuccess = false;
                resp.Message = errMessage;
            }
            LogAPICalls(r.MethodName, r, resp);
            return resp;
        }

        public GenericInsertUpdateResponse CreateProvider(GenericAPIRequest r, bool isNew = true)
        {
            GenericInsertUpdateResponse resp = new GenericInsertUpdateResponse();
            string errMessage = "";
            string successMessage = "";
            //if (!CheckIfIdExist("PROVIDER_TYPE", r.ProviderTypeId))
            //{
            //    errMessage = "Unable to create provider. Invalid provider type.";
            //    resp.isSuccess = false;
            //    resp.Message = errMessage;
            //    return resp;
            //}

         
            if (CheckIfCodeExist("PROVIDER", r.ProviderCode, r.ProviderId))
            {
                errMessage = "Unable to create provider. Provider Code already exist.";
                resp.isSuccess = false;
                resp.Message = errMessage;
                return resp;
            }
            
            List<MySqlParameter> mySqlParameters = new List<MySqlParameter>();
            string myQuery = "";
            if (isNew)
            {
                myQuery = "INSERT INTO PROVIDER (PROVIDER_CODE, PROVIDER_NAME, PROVIDER_DESCRIPTION)" +
                " VALUES(" +
                " @PROVIDER_CODE, @PROVIDER_NAME, @PROVIDER_DESCRIPTION " +
                " )";
            }
            else
            {
                myQuery = " UPDATE PROVIDER SET " +
                    " PROVIDER_CODE=@PROVIDER_CODE " +
                    " ,PROVIDER_NAME = @PROVIDER_NAME " +
                    " ,PROVIDER_DESCRIPTION = @PROVIDER_DESCRIPTION " +
                    " WHERE PROVIDER_ID = @PROVIDER_ID"
                    ;
            }

            
            //mySqlParameters.Add(new MySqlParameter("@PROVIDER_TYPE_ID", r.ProviderTypeId));
            mySqlParameters.Add(new MySqlParameter("@PROVIDER_CODE", r.ProviderCode));
            mySqlParameters.Add(new MySqlParameter("@PROVIDER_NAME", r.ProviderName));
            //mySqlParameters.Add(new MySqlParameter("@PROVIDER_CREDENTIAL", r.CredsJson));
            mySqlParameters.Add(new MySqlParameter("@PROVIDER_DESCRIPTION", r.ProviderDescription));
            if (!isNew)
            {
                mySqlParameters.Add(new MySqlParameter("@PROVIDER_ID", r.ProviderId));
            }



            errMessage = "Unable to save provider. Please check your provider information";
            successMessage = "Provider has been saved.";
            string id = "";
            if(ExecuteQuery(myQuery, mySqlParameters, out id))
            {
                resp.isSuccess = true;
                resp.Message = successMessage;
                resp.Id = int.Parse(id);
            }
            else
            {
                errMessage = "Unable to create provider.";
                resp.isSuccess = false;
                resp.Message = errMessage;
            }
            LogAPICalls(r.MethodName, r, resp);
            return resp;
        }

        public bool ExecuteQuery(string myQuery, List<MySqlParameter> mySqlParameters, out string Id)
        {
            Id = "0";
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                   
                    MySqlCommand cmd = new MySqlCommand(myQuery, conn);
                    foreach (MySqlParameter param in mySqlParameters)
                    {
                        cmd.Parameters.Add(param);
                    }
                    cmd.ExecuteNonQuery();
                    Id = cmd.LastInsertedId.ToString();
                    conn.Close();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }


        public GenericInsertUpdateResponse InsertUpdateData(GenericAPIRequest r, string tableName, bool isCreate = true, bool isDelete = false)
        {
            tableName = tableName.ToUpper();
            DateTime theDate = DateTime.Now;
            string currDate = theDate.ToString("yyyy-MM-dd H:mm:ss");
            List<MySqlParameter> mySqlParameters = new List<MySqlParameter>();
            GenericInsertUpdateResponse resp = new GenericInsertUpdateResponse();
            if (isCreate)
            {
                switch (tableName)
                {
                    case "PROVIDER":
                        return CreateProvider(r);
                    case "UPDATE_PROVIDER":
                        return CreateProvider(r, false);

                    case "PROVIDER_TYPE":
                        return CreateProviderType(r);
                    case "UPDATE_PROVIDER_TYPE":
                        return CreateProviderType(r, false);

                    case "CREDENTIAL":
                        return CreateCredential(r);
                    case "UPDATE_CREDENTIAL":
                        return CreateCredential(r, false);
                }
            }

            LogAPICalls(r.MethodName, r, resp);
            return resp;
        }

        public bool CheckIfCodeExist(string tableName, string Code, string ProviderId = "")
        {
            bool isExist = false;
            string query = "SELECT " + tableName + "_ID" + " FROM  " + tableName  + "  WHERE " + tableName + "_STATUS = 'A' AND " + tableName + "_CODE = @CODE";


            if (!string.IsNullOrEmpty(ProviderId))
            {
                query = query + " AND " + tableName + "_ID <> @ID";
            }

            List<MySqlParameter> mySqlParameters = new List<MySqlParameter>();
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.Add(new MySqlParameter("@CODE", Code));
                    if (!string.IsNullOrEmpty(ProviderId))
                    {
                        cmd.Parameters.Add(new MySqlParameter("@ID", ProviderId));
                    }
                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        isExist = true;
                    }
                    reader.Close();
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return isExist;
        }

        public bool CheckIfIdExist(string tableName, string id)
        {
            bool isExist = false;
            string query = "SELECT " + tableName + "_ID" + " FROM  " + tableName + "  WHERE " + tableName + "_STATUS='A' AND " + tableName + "_ID = @ID";
            List<MySqlParameter> mySqlParameters = new List<MySqlParameter>();

            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.Add(new MySqlParameter("@ID", id));
                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        isExist = true;
                    }
                    reader.Close();
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return isExist;
        }

        public void LogAPICalls(string method_name, GenericAPIRequest param, object resp)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                try
                {
                    if (string.IsNullOrEmpty(param.ApiGuid))
                    {
                        param.ApiGuid = null;
                    }


                    string parameters = JsonConvert.SerializeObject(param, Formatting.Indented);
                    string response = JsonConvert.SerializeObject(resp, Formatting.Indented);

                    //string param = Convert.ToString(parameters); 
                    MySqlCommand sql = new MySqlCommand("INSERT INTO API_LOG(API_METHOD_NAME, API_PARAMETERS, API_RESPONSE, API_GUID) VALUES(@API_METHOD_NAME, @API_PARAMETERS, @API_RESPONSE, @API_GUID)", conn);
                    sql.Parameters.Add(new MySqlParameter("@API_METHOD_NAME", method_name));
                    sql.Parameters.Add(new MySqlParameter("@API_PARAMETERS", parameters));
                    sql.Parameters.Add(new MySqlParameter("@API_RESPONSE", response));
                    sql.Parameters.Add(new MySqlParameter("@API_GUID", param.ApiGuid));
                    sql.ExecuteNonQuery();
                    conn.Close();
                }
                catch (Exception ex)
                {
                    Console.Write(ex.ToString());
                    conn.Close();
                }
            }
        }

        public DataRow GetData(string query)
        {
            DataRow dr = null;
            GenericGetDataResponse resp = new GenericGetDataResponse();
            DataTable dt;
            Dictionary<string, string> raw = new Dictionary<string, string>();
            List<Dictionary<string, string>> contents = new List<Dictionary<string, string>>();
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    dt = new DataTable();
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        dt.Load(reader);
                        //contents = dt.AsEnumerable().Select(row => dt.Columns.Cast<DataColumn>().ToDictionary(column => column.ColumnName, column => row[column].ToString())).ToList();
                        reader.Close();
                        dr = dt.Rows[0];
                    }
                    conn.Close();
                }
            }
            catch (Exception)
            {
                
            }

            return dr;
        }
      
        public void LogMessaging(string receiver, string programId, string type, string content, string logName, string templateId)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                try
                {
                    //string param = Convert.ToString(parameters); 
                    MySqlCommand sql = new MySqlCommand("INSERT INTO LOG(LOG_PROGRAM_ID,LOG_RECEIVER, LOG_NAME, LOG_CONTENT, LOG_TYPE, LOG_CREATE_DATE, LOG_TEMPLATE_ID)" +
                        " VALUES(@LOG_PROGRAM_ID,@LOG_RECEIVER, @LOG_NAME, @LOG_CONTENT, @LOG_TYPE, UTC_TIMESTAMP(),@LOG_TEMPLATE_ID)", conn);
                    sql.Parameters.Add(new MySqlParameter("@LOG_PROGRAM_ID", programId));
                    sql.Parameters.Add(new MySqlParameter("@LOG_RECEIVER", receiver));
                    sql.Parameters.Add(new MySqlParameter("@LOG_NAME", logName));
                    sql.Parameters.Add(new MySqlParameter("@LOG_CONTENT", content));
                    sql.Parameters.Add(new MySqlParameter("@LOG_TYPE", type));
                    sql.Parameters.Add(new MySqlParameter("@LOG_TEMPLATE_ID", templateId));
                    sql.ExecuteNonQuery();
                    conn.Close();
                }
                catch (Exception ex)
                {
                    Console.Write(ex.ToString());
                    conn.Close();
                }
            }
        }

        private string GetSelectStatement(string tableName, string ProgramId, GenericAPIRequest r)
        {
            tableName = tableName.ToUpper();
            string myQuery = "";
            switch (tableName)
            {
                case "PROVIDER":
                    myQuery = "SELECT PROVIDER_ID as ProviderId, PROVIDER_CODE as ProviderCode, PROVIDER_NAME as ProviderName, PROVIDER_DESCRIPTION AS ProviderDescription FROM PROVIDER P  WHERE PROVIDER_STATUS ='A'";

                    break;
                case "PROVIDER_TYPE":
                    myQuery = "SELECT PROVIDER_TYPE_ID as ProviderTypeId, PROVIDER_TYPE_CODE as ProviderTypeCode, PROVIDER_TYPE_DESCRIPTION as ProviderTypeDescription FROM PROVIDER_TYPE WHERE PROVIDER_TYPE_STATUS ='A'";

                    break;

                case "TAG":
                    myQuery = " SELECT TAG_ID as TagId, TAG_CODE as TagCode, TAG_NAME as TagName, TAG_DESCRIPTION as TagDescription, TAG_CREATE_DATE as CreateDate, TAG_UPDATE_DATE as UpdateDate " +
                        //" ,TRAN_TYPE_CODE as TransactionTypeCode" +
                        " FROM FIELD_TAG " +
                        ///*" LEFT JOIN TRANSACTION_TYPE ON TRAN_TYPE_ID = TAG_TRAN_TYPE_ID"*/ +
                        " WHERE TAG_STATUS='A' AND TAG_PROGRAM_ID  = " + ProgramId;
                    break;
                case "TEMPLATE":
                    myQuery = " SELECT TEMPLATE_ID as TemplateId, TEMPLATE_CODE as TemplateCode, TEMPLATE_NAME as TemplateName, TEMPLATE_DESCRIPTION as TemplateDescription, TEMPLATE_CREATE_DATE as CreateDate, TEMPLATE_UPDATE_DATE as UpdateDate " +
                        " ,TEMPLATE_CONTENT AS Content, TEMPLATE_SUBJECT as SUBJECT, TEMPLATE_FOOTER as Footer, TEMPLATE_HEADER AS Header" +
                        " FROM TEMPLATE " +
                        //" LEFT JOIN TRANSACTION_TYPE ON TRAN_TYPE_ID = TEMPLATE_TRAN_TYPE_ID" +
                        " WHERE TEMPLATE_STATUS='A' AND TEMPLATE_PROGRAM_ID  = " + ProgramId;
                    break;
                case "TAG_BY_CODE":
                    myQuery = " SELECT TAG_ID as TagId, TAG_CODE as TagCode, TAG_NAME as TagName, TAG_DESCRIPTION as TagDescription, TAG_CREATE_DATE as CreateDate, TAG_UPDATE_DATE as UpdateDate " +
                        //" ,TRAN_TYPE_CODE as TransactionTypeCode" +
                        " FROM FIELD_TAG " +
                        //" LEFT JOIN TRANSACTION_TYPE ON TRAN_TYPE_ID = TAG_TRAN_TYPE_ID" +
                        " WHERE TAG_STATUS='A' AND TAG_PROGRAM_ID  = " + ProgramId + " AND TAG_TRAN_TYPE_ID= '" + r.TransTypeId + "'";
                    break;

                case "MESSAGING_TYPE":
                    myQuery = " SELECT MESSAGING_TYPE_ID as MessagingTypeId, MESSAGING_TYPE_CODE as MessagingTypeCode, MESSAGING_TYPE_NAME as MessagingTypeName, MESSAGING_TYPE_DESCRIPTION as Description " +
                        " " +
                        " FROM MESSAGING_TYPE INNER JOIN PROGRAM_ACCESS ON  ACCESS_PROGRAM_ID = " + ProgramId + " AND ACCESS_NOTIFICATION_ID = MESSAGING_TYPE_ID " +
                        " WHERE MESSAGING_TYPE_STATUS='A'";
                    break;
                case "TEMPLATE_ACCESS":
                    myQuery = " SELECT ACCESS_ID as AccessId,TEMPLATE_ID AS TemplateId, TEMPLATE_NAME as TemplateName,NOTIFICATION_ID as NotificationId, NOTIFICATION_CODE as NotificationCode, NOTIFICATION_NAME as NotificationName FROM NOTIFICATION" +                        
                        " INNER JOIN TEMPLATE_NOTIFICATION_ACCESS ON ACCESS_NOTIFICATION_ID = NOTIFICATION_ID" +
                        " INNER JOIN TEMPLATE ON TEMPLATE_ID = ACCESS_TEMPLATE_ID" +
                        " WHERE ACCESS_TEMPLATE_ID = " + r.Id;
                    break;
                case "VENDOR_SMS":
                    myQuery = " SELECT * FROM VENDOR WHERE VENDOR_STATUS ='A' AND VENDOR_PROGRAM_ID  = " + ProgramId + " AND VENDOR_NOTIFICATION_CODE ='SMS' ";
                    break;

                case "VENDOR_EMAIL":
                    myQuery = " SELECT * FROM VENDOR WHERE VENDOR_STATUS ='A' AND VENDOR_PROGRAM_ID  = " + ProgramId + " AND VENDOR_NOTIFICATION_CODE ='EMAIL' ";
                    break;

                case "TEMPLATE_CODE":
                    myQuery = " SELECT * FROM TEMPLATE WHERE TEMPLATE" +
                        "_STATUS ='A' AND TEMPLATE_PROGRAM_ID  = " + ProgramId + " AND TEMPLATE_CODE ='"+ r.TemplateCode + "' ";
                    break;

                ////case "TRANSACTION_TYPE":
                ////    myQuery = " SELECT TRAN_TYPE_CODE AS TransactionTypeCode,  TRAN_TYPE_ID AS TransactionTypeId, TRAN_TYPE_NAME as TransactionTypeName, TYPE_DESCRIPTION AS TransactionTypeDescription" +
                ////        " FROM TRANSACTION_TYPE WHERE TRAN_TYPE_STATUS ='A' ";
                //    break;
                case "TEMPLATE_BY_CODE":
                    myQuery = " SELECT TEMPLATE_ID as TemplateId, TEMPLATE_CODE as TemplateCode, TEMPLATE_NAME as TemplateName, TEMPLATE_DESCRIPTION as TemplateDescription, TEMPLATE_CREATE_DATE as CreateDate, TEMPLATE_UPDATE_DATE as UpdateDate " +
                        //" ,TRAN_TYPE_CODE as TransactionTypeCode" +
                        " FROM TEMPLATE " +
                        //" LEFT JOIN TRANSACTION_TYPE ON TRAN_TYPE_ID = TEMPLATE_TRAN_TYPE_ID" +
                        " WHERE TEMPLATE_STATUS='A' AND TEMPLATE_PROGRAM_ID = " + ProgramId + " AND TEMPLATE_TRAN_TYPE_ID= '" + r.TransTypeId + "'";
                    break;

                default:
                    break;
            }

            return myQuery;
        }

        public GenericGetDataResponse GetDataById(GenericAPIRequest r, string tableName, string Id)
        {
            string query = "SELECT * FROM " + tableName + " WHERE " + tableName + "_ID = @ID";

            GenericGetDataResponse resp = new GenericGetDataResponse();
            DataTable dt;
            Dictionary<string, string> raw = new Dictionary<string, string>();
            List<Dictionary<string, string>> contents = new List<Dictionary<string, string>>();
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    dt = new DataTable();
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.Add(new MySqlParameter("@ID", Id));
                    MySqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        dt.Load(reader);
                        contents = dt.AsEnumerable().Select(row => dt.Columns.Cast<DataColumn>().ToDictionary(column => column.ColumnName, column => row[column].ToString())).ToList();
                        reader.Close();
                    }
                    conn.Close();
                }
                resp.isSuccess = true;
                resp.Message = "Information has been retrieve.";
                resp.Data = contents;

            }
            catch (Exception ex)
            {
                resp.isSuccess = false;
                resp.Message = ex.Message;
            }
            LogAPICalls(r.MethodName, r, resp);
            return resp;
        }


        public GenericGetDataResponse DeleteData(string tableName, string id)
        {
            string query = " UPDATE " + tableName + " SET  " + tableName + "_STATUS ='D'" + " WHERE " + tableName + "_ID = @ID";

            GenericGetDataResponse resp = new GenericGetDataResponse();
            DataTable dt;
            Dictionary<string, string> raw = new Dictionary<string, string>();
            List<Dictionary<string, string>> contents = new List<Dictionary<string, string>>();
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    dt = new DataTable();
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.Add(new MySqlParameter("@ID", id));
                    cmd.ExecuteNonQuery();
                    
                    conn.Close();
                }
                resp.isSuccess = true;
                resp.Message = "Data has been deleted.";
                resp.Data = contents;

            }
            catch (Exception ex)
            {
                resp.isSuccess = false;
                resp.Message = ex.Message;
            }
            return resp;
        }

        public GenericGetDataResponse GetDataByCode(GenericAPIRequest r, string tableName, string Code)
        {
            string query = "SELECT * FROM " + tableName + " WHERE " + tableName + "_CODE = @CODE" ;

            GenericGetDataResponse resp = new GenericGetDataResponse();
            DataTable dt;
            Dictionary<string, string> raw = new Dictionary<string, string>();
            List<Dictionary<string, string>> contents = new List<Dictionary<string, string>>();
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    dt = new DataTable();
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.Add(new MySqlParameter("@CODE", Code));
                    MySqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        dt.Load(reader);
                        contents = dt.AsEnumerable().Select(row => dt.Columns.Cast<DataColumn>().ToDictionary(column => column.ColumnName, column => row[column].ToString())).ToList();
                        reader.Close();
                    }
                    conn.Close();
                }
                resp.isSuccess = true;
                resp.Message = "Information has been retrieve.";
                resp.Data = contents;

            }
            catch (Exception ex)
            {
                resp.isSuccess = false;
                resp.Message = ex.Message;
            }
            LogAPICalls(r.MethodName, r, resp);
            return resp;
        }

        
        public GenericGetDataResponse GetData(GenericAPIRequest r,string tableName)
        {
            string query = GetSelectStatement(tableName, r.ProgramId, r);

            GenericGetDataResponse resp = new GenericGetDataResponse();
            DataTable dt;
            Dictionary<string, string> raw = new Dictionary<string, string>();
            List<Dictionary<string, string>> contents = new List<Dictionary<string, string>>();
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    dt = new DataTable();
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        dt.Load(reader);
                        contents = dt.AsEnumerable().Select(row => dt.Columns.Cast<DataColumn>().ToDictionary(column => column.ColumnName,column => row[column].ToString())).ToList();
                        reader.Close();
                    }
                    conn.Close();
                }
                resp.isSuccess = true;
                resp.Message = "Information has been retrieve.";
                resp.Data = contents;

            }
            catch (Exception ex)
            {
                resp.isSuccess = false;
                resp.Message = ex.Message;
            }
            LogAPICalls(r.MethodName, r, resp);
            return resp;
        }

        
        public GetUserListResponse GetUserList(GetUserListRequest r)
        {
            GetUserListResponse resp = new GetUserListResponse();
            
            return resp;
        }


        //FOR VERSION 2
        #region V2
        public GenericGetDataResponse GetDataV2(GenericAPIRequestV2 r, string tableName)
        {
            string query = GetSelectStatementV2(tableName, r);

            GenericGetDataResponse resp = new GenericGetDataResponse();
            DataTable dt;
            Dictionary<string, string> raw = new Dictionary<string, string>();
            List<Dictionary<string, string>> contents = new List<Dictionary<string, string>>();
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    dt = new DataTable();
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        dt.Load(reader);
                        contents = dt.AsEnumerable().Select(row => dt.Columns.Cast<DataColumn>().ToDictionary(column => column.ColumnName, column => row[column].ToString())).ToList();
                        reader.Close();
                    }
                    conn.Close();
                }
                resp.isSuccess = true;
                resp.Message = "Information has been retrieve.";
                resp.Data = contents;

            }
            catch (Exception ex)
            {
                resp.isSuccess = false;
                resp.Message = ex.Message;
            }
            return resp;
        }

        private string GetSelectStatementV2(string tableName, GenericAPIRequestV2 r)
        {
            tableName = tableName.ToUpper();
            string myQuery = "";
            switch (tableName)
            {
                case "PROVIDER":
                    myQuery = "SELECT PROVIDER_ID as providerId, PROVIDER_CODE as providerCode, PROVIDER_NAME as providerName, PROVIDER_DESCRIPTION AS providerDescription FROM PROVIDER P  WHERE PROVIDER_STATUS ='A'";
                    break;
                case "PROVIDER_TYPE":
                    myQuery = "SELECT PROVIDER_TYPE_ID as providerTypeId, PROVIDER_TYPE_CODE as providerTypeCode, PROVIDER_TYPE_DESCRIPTION as providerTypeDescription FROM PROVIDER_TYPE WHERE PROVIDER_TYPE_STATUS ='A'";
                    break;

                default:
                    break;
            }

            return myQuery;
        }

        public GenericInsertUpdateResponse InsertUpdateDataV2(GenericAPIRequestV2 r, string tableName, bool isCreate = true)
        {
            tableName = tableName.ToUpper();
            DateTime theDate = DateTime.Now;
            string currDate = theDate.ToString("yyyy-MM-dd H:mm:ss");
            List<MySqlParameter> mySqlParameters = new List<MySqlParameter>();
            GenericInsertUpdateResponse resp = new GenericInsertUpdateResponse();
            if (isCreate)
            {
                switch (tableName)
                {
                    case "PROVIDER":
                        return CreateProviderV2(r);
                    case "UPDATE_PROVIDER":
                        return CreateProviderV2(r, false);

                    case "PROVIDER_TYPE":
                        return CreateProviderTypeV2(r);
                    case "UPDATE_PROVIDER_TYPE":
                        return CreateProviderTypeV2(r, false);

                    case "CREDENTIAL":
                        return CreateCredentialV2(r);
                    case "UPDATE_CREDENTIAL":
                        return CreateCredentialV2(r, false);

                    default:
                        break;
                }
            }


            

            return resp;
        }

        public GenericInsertUpdateResponse CreateProviderV2(GenericAPIRequestV2 r, bool isNew = true)
        {
            GenericInsertUpdateResponse resp = new GenericInsertUpdateResponse();
            string errMessage = "";
            string successMessage = "";

            if (CheckIfCodeExist("PROVIDER", r.providerCode, r.providerId))
            {
                errMessage = "Unable to create provider. Provider Code already exist.";
                resp.isSuccess = false;
                resp.Message = errMessage;
                return resp;
            }

            List<MySqlParameter> mySqlParameters = new List<MySqlParameter>();
            string myQuery = "";
            if (isNew)
            {
                myQuery = "INSERT INTO PROVIDER (PROVIDER_CODE, PROVIDER_NAME, PROVIDER_DESCRIPTION)" +
                " VALUES(" +
                " @PROVIDER_CODE, @PROVIDER_NAME, @PROVIDER_DESCRIPTION " +
                " )";
            }
            else
            {
                myQuery = " UPDATE PROVIDER SET " +
                    " PROVIDER_CODE=@PROVIDER_CODE " +
                    " ,PROVIDER_NAME = @PROVIDER_NAME " +
                    " ,PROVIDER_DESCRIPTION = @PROVIDER_DESCRIPTION " +
                    " WHERE PROVIDER_ID = @PROVIDER_ID"
                    ;
            }

            mySqlParameters.Add(new MySqlParameter("@PROVIDER_CODE", r.providerCode));
            mySqlParameters.Add(new MySqlParameter("@PROVIDER_NAME", r.providerName));
            mySqlParameters.Add(new MySqlParameter("@PROVIDER_DESCRIPTION", r.providerDescription));
            if (!isNew)
            {
                mySqlParameters.Add(new MySqlParameter("@PROVIDER_ID", r.providerId));
            }

            successMessage = "Provider has been saved.";
            string id = "";
            if (ExecuteQuery(myQuery, mySqlParameters, out id))
            {
                resp.isSuccess = true;
                resp.Message = successMessage;
                resp.Id = int.Parse(id);
            }
            else
            {
                errMessage = "Unable to create provider.";
                resp.isSuccess = false;
                resp.Message = errMessage;
            }
            return resp;
        }

        public GenericInsertUpdateResponse CreateProviderTypeV2(GenericAPIRequestV2 r, bool isNew = true)
        {
            GenericInsertUpdateResponse resp = new GenericInsertUpdateResponse();
            string errMessage = "";
            string successMessage = "";


            if (CheckIfCodeExist("PROVIDER", r.providerTypeCode, r.providerTypeId))
            {
                errMessage = "Unable to create provider type. Provider Type Code already exist.";
                resp.isSuccess = false;
                resp.Message = errMessage;
                return resp;
            }

            List<MySqlParameter> mySqlParameters = new List<MySqlParameter>();
            string myQuery = "";
            if (isNew)
            {
                myQuery = "INSERT INTO PROVIDER_TYPE (PROVIDER_TYPE_CODE, PROVIDER_TYPE_NAME, PROVIDER_TYPE_DESCRIPTION)" +
                " VALUES(" +
                " @PROVIDER_TYPE_CODE, @PROVIDER_TYPE_NAME, @PROVIDER_TYPE_DESCRIPTION " +
                " )";
            }
            else
            {
                myQuery = " UPDATE PROVIDER_TYPE SET " +
                    " PROVIDER_TYPE_CODE=@PROVIDER_TYPE_CODE " +
                    " ,PROVIDER_TYPE_NAME = @PROVIDER_TYPE_NAME " +
                    " ,PROVIDER_TYPE_DESCRIPTION = @PROVIDER_TYPE_DESCRIPTION " +
                    " WHERE PROVIDER_TYPE_ID = @PROVIDER_TYPE_ID"
                    ;
            }

            mySqlParameters.Add(new MySqlParameter("@PROVIDER_TYPE_CODE", r.providerTypeCode));
            mySqlParameters.Add(new MySqlParameter("@PROVIDER_TYPE_NAME", r.providerTypeName));
            mySqlParameters.Add(new MySqlParameter("@PROVIDER_TYPE_DESCRIPTION", r.providerTypeDescription));
            if (!isNew)
            {
                mySqlParameters.Add(new MySqlParameter("@PROVIDER_TYPE_ID", r.providerTypeId));
            }
;
            successMessage = "Provider Type has been saved.";
            string id = "";
            if (ExecuteQuery(myQuery, mySqlParameters, out id))
            {
                resp.isSuccess = true;
                resp.Message = successMessage;
                resp.Id = int.Parse(id);
            }
            else
            {
                errMessage = "Unable to create provider type.";
                resp.isSuccess = false;
                resp.Message = errMessage;
            }

            return resp;
        }

        public GenericInsertUpdateResponse CreateCredentialV2(GenericAPIRequestV2 r, bool isNew = true)
        {
            GenericInsertUpdateResponse resp = new GenericInsertUpdateResponse();
            string errMessage = "";
            string successMessage = "";

            if (!CheckIfIdExist("PROVIDER", r.providerId))
            {
                errMessage = "Invalid Provider Id.";
                resp.isSuccess = false;
                resp.Message = errMessage;
                return resp;
            }

            if (!CheckIfIdExist("PROVIDER_TYPE", r.providerTypeId))
            {
                errMessage = "Invalid Provider Type Id.";
                resp.isSuccess = false;
                resp.Message = errMessage;
                return resp;
            }

            if (CheckIfCredentialExist(r.providerTypeId, r.programDescription, r.providerId, r.credentialId))
            {
                errMessage = "Unable to create credential. Credential already exist.";
                resp.isSuccess = false;
                resp.Message = errMessage;
                return resp;
            }

            List<MySqlParameter> mySqlParameters = new List<MySqlParameter>();
            string myQuery = "";
            if (isNew)
            {
                myQuery = "INSERT INTO CREDENTIAL (CREDENTIAL_PROVIDER_ID, CREDENTIAL_PROVIDER_TYPE_ID, CREDENTIAL_PROGRAM_ID, CREDENTIAL_REQUIREMENT)" +
                " VALUES(" +
                " @CREDENTIAL_PROVIDER_ID, @CREDENTIAL_PROVIDER_TYPE_ID, @CREDENTIAL_PROGRAM_ID, @CREDENTIAL_REQUIREMENT " +
                " )";
            }
            else
            {
                myQuery = " UPDATE CREDENTIAL SET " +
                    " CREDENTIAL_PROVIDER_ID=@CREDENTIAL_PROVIDER_ID " +
                    " ,CREDENTIAL_PROVIDER_TYPE_ID = @CREDENTIAL_PROVIDER_TYPE_ID " +
                    " ,CREDENTIAL_PROGRAM_ID = @CREDENTIAL_PROGRAM_ID " +
                    " ,CREDENTIAL_REQUIREMENT=@CREDENTIAL_REQUIREMENT " +
                    " WHERE CREDENTIAL_ID = @CREDENTIAL_ID"
                    ;
            }

            mySqlParameters.Add(new MySqlParameter("@CREDENTIAL_PROVIDER_ID", r.providerId));
            mySqlParameters.Add(new MySqlParameter("@CREDENTIAL_PROVIDER_TYPE_ID", r.providerTypeId));
            mySqlParameters.Add(new MySqlParameter("@CREDENTIAL_PROGRAM_ID", r.programId));
            mySqlParameters.Add(new MySqlParameter("@CREDENTIAL_REQUIREMENT", r.credsJson));
            if (!isNew)
            {
                mySqlParameters.Add(new MySqlParameter("@CREDENTIAL_ID", r.credentialId));
            }

            successMessage = "Credential has been saved.";
            string id = "";
            if (ExecuteQuery(myQuery, mySqlParameters, out id))
            {
                resp.isSuccess = true;
                resp.Message = successMessage;
                resp.Id = int.Parse(id);
            }
            else
            {
                errMessage = "Unable to create credential.";
                resp.isSuccess = false;
                resp.Message = errMessage;
            }

            return resp;
        }

        public GenericGetDataResponse GetDataByIdV2(string tableName, string Id)
        {
            string query = "SELECT * FROM " + tableName + " WHERE " + tableName + "_ID = @ID";

            GenericGetDataResponse resp = new GenericGetDataResponse();
            DataTable dt;
            Dictionary<string, string> raw = new Dictionary<string, string>();
            List<Dictionary<string, string>> contents = new List<Dictionary<string, string>>();
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    dt = new DataTable();
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.Add(new MySqlParameter("@ID", Id));
                    MySqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        dt.Load(reader);
                        contents = dt.AsEnumerable().Select(row => dt.Columns.Cast<DataColumn>().ToDictionary(column => column.ColumnName, column => row[column].ToString())).ToList();
                        reader.Close();
                    }
                    conn.Close();
                }
                resp.isSuccess = true;
                resp.Message = "Information has been retrieve.";
                resp.Data = contents;

            }
            catch (Exception ex)
            {
                resp.isSuccess = false;
                resp.Message = ex.Message;
            }

            return resp;
        }

        public GenericGetDataResponse GetCredentialListV2(string ProgramId, string CredentialId = "")
        {
            GenericGetDataResponse resp = new GenericGetDataResponse();

            List<Dictionary<string, string>> contents = new List<Dictionary<string, string>>();
            Dictionary<string, string> content = new Dictionary<string, string>();

            string query = "SELECT * FROM CREDENTIAL " +
                " LEFT JOIN PROVIDER ON PROVIDER_ID = CREDENTIAL_PROVIDER_ID" +
                " LEFT JOIN PROVIDER_TYPE  ON PROVIDER_TYPE_ID = CREDENTIAL_PROVIDER_TYPE_ID" +
                " WHERE CREDENTIAL_STATUS='A' ";

            if (!string.IsNullOrEmpty(CredentialId))
            {
                query = query + " AND CREDENTIAL_ID = @ID";
            }
            if (!string.IsNullOrEmpty(ProgramId))
            {
                query = query + " AND CREDENTIAL_PROGRAM_ID = @PROGRAM_ID ";
            }
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    DataTable dt = new DataTable();
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    if (!string.IsNullOrEmpty(ProgramId))
                    {
                        cmd.Parameters.Add(new MySqlParameter("@PROGRAM_ID", ProgramId));
                    }
                    if (!string.IsNullOrEmpty(CredentialId))
                    {
                        cmd.Parameters.Add(new MySqlParameter("@ID", CredentialId));
                    }
                    MySqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        dt.Load(reader);
                        reader.Close();
                        foreach (DataRow dr in dt.Rows)
                        {
                            content = new Dictionary<string, string>();
                            content.Add("credentialId", dr["CREDENTIAL_ID"].ToString());
                            content.Add("providerId", dr["PROVIDER_ID"].ToString());
                            content.Add("providerName", dr["PROVIDER_NAME"].ToString());
                            content.Add("providerTypeId", dr["PROVIDER_TYPE_ID"].ToString());
                            content.Add("providerTypeName", dr["PROVIDER_TYPE_NAME"].ToString());
                            content.Add("programId", dr["CREDENTIAL_PROGRAM_ID"].ToString());

                            JArray json = JsonConvert.DeserializeObject<JArray>(dr["CREDENTIAL_REQUIREMENT"].ToString());
                            foreach (JObject item in json)
                            {
                                foreach (var x in item)
                                {
                                    string name = x.Key;
                                    string value = x.Value.ToString();
                                    content.Add(name, value);
                                }
                            }
                            contents.Add(content);
                        }
                    }
                    conn.Close();
                }
                resp.isSuccess = true;
                resp.Message = "Information has been retrieve.";
                resp.Data = contents;

            }
            catch (Exception ex)
            {
                resp.isSuccess = false;
                resp.Message = ex.Message;
            }
            GenericAPIRequest r = new GenericAPIRequest();
            LogAPICalls("GetCredentials", r, resp);
            return resp;
            //}
        }

        public GenericGetDataResponse GetCredentialListByProviderNameV2(string ProgramId, string providerName)
        {
            GenericGetDataResponse resp = new GenericGetDataResponse();

            List<Dictionary<string, string>> contents = new List<Dictionary<string, string>>();
            Dictionary<string, string> content = new Dictionary<string, string>();

            string query = "SELECT * FROM CREDENTIAL " +
                " LEFT JOIN PROVIDER ON PROVIDER_ID = CREDENTIAL_PROVIDER_ID" +
                " LEFT JOIN PROVIDER_TYPE  ON PROVIDER_TYPE_ID = CREDENTIAL_PROVIDER_TYPE_ID" +
                " WHERE CREDENTIAL_STATUS='A' ";

            if (!string.IsNullOrEmpty(providerName))
            {
                query = query + " AND PROVIDER_NAME = @PROVIDER_NAME";
            }
            if (!string.IsNullOrEmpty(ProgramId))
            {
                query = query + " AND CREDENTIAL_PROGRAM_ID = @PROGRAM_ID ";
            }
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    DataTable dt = new DataTable();
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    if (!string.IsNullOrEmpty(ProgramId))
                    {
                        cmd.Parameters.Add(new MySqlParameter("@PROGRAM_ID", ProgramId));
                    }
                    if (!string.IsNullOrEmpty(providerName))
                    {
                        cmd.Parameters.Add(new MySqlParameter("@PROVIDER_NAME", providerName));
                    }
                    MySqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        dt.Load(reader);
                        reader.Close();
                        foreach (DataRow dr in dt.Rows)
                        {
                            content = new Dictionary<string, string>();
                            content.Add("credentialId", dr["CREDENTIAL_ID"].ToString());
                            content.Add("providerId", dr["PROVIDER_ID"].ToString());
                            content.Add("providerName", dr["PROVIDER_NAME"].ToString());
                            content.Add("providerTypeId", dr["PROVIDER_TYPE_ID"].ToString());
                            content.Add("providerTypeName", dr["PROVIDER_TYPE_NAME"].ToString());
                            content.Add("programId", dr["CREDENTIAL_PROGRAM_ID"].ToString());

                            JArray json = JsonConvert.DeserializeObject<JArray>(dr["CREDENTIAL_REQUIREMENT"].ToString());
                            foreach (JObject item in json)
                            {
                                foreach (var x in item)
                                {
                                    string name = x.Key;
                                    string value = x.Value.ToString();
                                    content.Add(name, value);
                                }
                            }
                            contents.Add(content);
                        }
                    }
                    conn.Close();
                }
                resp.isSuccess = true;
                resp.Message = "Information has been retrieve.";
                resp.Data = contents;

            }
            catch (Exception ex)
            {
                resp.isSuccess = false;
                resp.Message = ex.Message;
            }
            return resp;
        }
        #endregion
    }
}
