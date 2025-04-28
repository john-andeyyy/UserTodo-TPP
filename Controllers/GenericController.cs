using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Notification.Models;
using Notification.Models.BasicAuth;
using Notification.Models.Requests;
using Notification.Models.Responses;
using Notification.Models.Tables;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Notification.Models.BasicAuth.BasicAuthServices;

namespace TPP.Controllers
{
    [Authorize]
    [ApiController]
    public class GenericController : Controller
    {
        private DBContext db;
        private readonly IWebHostEnvironment hostingEnvironment;
        private IHttpContextAccessor _IPAccess;
        public IBasicAuthService _authService;

        public GenericController(DBContext context, IWebHostEnvironment environment, IHttpContextAccessor accessor, IBasicAuthService authService)
        {
            _IPAccess = accessor;
            db = context;
            hostingEnvironment = environment;
            _authService = authService;
        }



        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticateModel model)
        {
            BasicAuthServices._listusers = db.GetAuthUser(model.Username, model.Password);
            var user = await _authService.Authenticate(model.Username, model.Password, "");

            if (user == null)
                Console.WriteLine("Username or password is incorrect");
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(user);
        }

        
        [HttpGet("ProviderType")]
        public IActionResult ProviderTypes()
        {
            GenericGetDataResponse resp = new GenericGetDataResponse();
            string objName = "DBContext";

            var type = Type.GetType(objName);
            
     
            GenericAPIRequest r = new GenericAPIRequest();

            r.MethodName = HttpContext.Request.Path;
            r.IpAddress = _IPAccess.HttpContext.Connection.RemoteIpAddress.ToString();
            r.ProgramId = _authService.GetUser().Id;
            resp = db.GetData(r, "PROVIDER_TYPE");
            if (resp.isSuccess)
            {
                return Ok(resp);
            }
            else
            {
                return BadRequest(resp.Message);
            }
        }

        [HttpPost("ProviderType")]
        public IActionResult ProviderTypeRegister([FromBody] GenericAPIRequest r)
        {
            GenericInsertUpdateResponse resp = new GenericInsertUpdateResponse();
            r.MethodName = HttpContext.Request.Path;
            r.IpAddress = _IPAccess.HttpContext.Connection.RemoteIpAddress.ToString();
            try
            {

                if (string.IsNullOrEmpty(r.ProviderTypeCode))
                {
                    return BadRequest("Please specify a provider type code");
                }

                if (string.IsNullOrEmpty(r.ProviderTypeName))
                {
                    return BadRequest("Please specify a provider type name");
                }

                if (string.IsNullOrEmpty(r.ProviderDescription))
                {
                    r.ProviderDescription = "";
                }
                r.ProviderTypeId = "";

                resp = db.InsertUpdateData(r, "PROVIDER_TYPE", true);
                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.isSuccess = false;
                resp.Message = ex.ToString();
                return Ok(resp);
            }
        }

        [HttpGet("ProviderType/{id}")]
        public IActionResult ProviderTypeRetrieve(string id)
        {
            GenericGetDataResponse resp = new GenericGetDataResponse();
            GenericAPIRequest r = new GenericAPIRequest();
            r.ProviderTypeId = id;
            r.MethodName = HttpContext.Request.Path;
            r.IpAddress = _IPAccess.HttpContext.Connection.RemoteIpAddress.ToString();
            try
            {

                if (string.IsNullOrEmpty(r.ProviderTypeId))
                {
                    return BadRequest("Please specify a provider id");
                }

                resp = db.GetDataById(r, "PROVIDER_TYPE", r.ProviderTypeId);
                if (resp.isSuccess)
                {
                    List<Dictionary<string, string>> contents = new List<Dictionary<string, string>>();
                    Dictionary<string, string> content = new Dictionary<string, string>();

                    content.Add("ProviderTypeId", resp.Data[0]["PROVIDER_TYPE_ID"].ToString());
                    content.Add("ProviderTypeCode", resp.Data[0]["PROVIDER_TYPE_CODE"].ToString());
                    content.Add("ProviderTypeName", resp.Data[0]["PROVIDER_TYPE_NAME"].ToString());

                    contents.Add(content);
                    resp.Data = contents;
                    return Ok(resp);
                }
                else
                {
                    return BadRequest(resp.Message);
                }
            }
            catch (Exception ex)
            {
                resp.isSuccess = false;
                resp.Message = ex.ToString();
                return Ok(resp);
            }
        }


        [HttpPut("ProviderType/{id}")]
        public IActionResult ProviderTypeUpdate(string id, [FromBody] GenericAPIRequest r)
        {
            GenericInsertUpdateResponse resp = new GenericInsertUpdateResponse();
            r.ProviderTypeId = id;
            r.MethodName = HttpContext.Request.Path;
            r.IpAddress = _IPAccess.HttpContext.Connection.RemoteIpAddress.ToString();
            try
            {
                if (string.IsNullOrEmpty(r.ProviderTypeId))
                {
                    return BadRequest("Please specify a provider type id");
                }

                if (string.IsNullOrEmpty(r.ProviderTypeCode))
                {
                    return BadRequest("Please specify a provider type code");
                }

                if (string.IsNullOrEmpty(r.ProviderTypeName))
                {
                    return BadRequest("Please specify a provider type name");
                }

                if (string.IsNullOrEmpty(r.ProviderTypeDescription))
                {
                    r.ProviderTypeDescription = "";
                }
                resp = db.InsertUpdateData(r, "UPDATE_PROVIDER_TYPE", true);
                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.isSuccess = false;
                resp.Message = ex.ToString();
                return Ok(resp);
            }
        }


        [HttpDelete("ProviderType/{id}")]
        public IActionResult ProviderTypeDelete(string id)
        {
            GenericGetDataResponse resp = new GenericGetDataResponse();
            GenericAPIRequest r = new GenericAPIRequest();
            r.ProviderTypeId = id;
            r.MethodName = HttpContext.Request.Path;
            r.IpAddress = _IPAccess.HttpContext.Connection.RemoteIpAddress.ToString();
            try
            {

                if (string.IsNullOrEmpty(r.ProviderTypeId))
                {
                    return BadRequest("Please specify a provider id");
                }
                resp = db.DeleteData("PROVIDER_TYPE", r.ProviderTypeId);
                if (resp.isSuccess)
                {
                    return Ok(resp);
                }
                else
                {
                    return BadRequest(resp.Message);
                }
            }
            catch (Exception ex)
            {
                resp.isSuccess = false;
                resp.Message = ex.ToString();
                return Ok(resp);
            }
        }



        //End of Provider Type Route

        //Provider
        [HttpGet("Provider")]
        public IActionResult ProviderList()
        {
            GenericGetDataResponse resp = new GenericGetDataResponse();
            string objName = "DBContext";

            var type = Type.GetType(objName);


            GenericAPIRequest r = new GenericAPIRequest();

            r.MethodName = HttpContext.Request.Path;
            r.IpAddress = _IPAccess.HttpContext.Connection.RemoteIpAddress.ToString();
            r.ProgramId = _authService.GetUser().Id;
            resp = db.GetData(r, "PROVIDER");
            if (resp.isSuccess)
            {
                return Ok(resp);
            }
            else
            {
                return BadRequest(resp.Message);
            }
        }

        [HttpPost("Provider")]
        public IActionResult ProviderRegister([FromBody] GenericAPIRequest r)
        {
            GenericInsertUpdateResponse resp = new GenericInsertUpdateResponse();
            r.MethodName = HttpContext.Request.Path;
            r.IpAddress = _IPAccess.HttpContext.Connection.RemoteIpAddress.ToString();
            try
            {

                if (string.IsNullOrEmpty(r.ProviderCode))
                {
                    return BadRequest("Please specify a provider code");
                }

                if (string.IsNullOrEmpty(r.ProviderName))
                {
                    return BadRequest("Please specify a provider name");
                }

                if (string.IsNullOrEmpty(r.ProviderDescription))
                {
                    r.ProviderDescription = "";
                }

                string jsonCreds = JsonConvert.SerializeObject(r.Credentials, Formatting.Indented);
                r.CredsJson = jsonCreds;
                resp = db.InsertUpdateData(r, "PROVIDER", true);
                return Ok(resp);
                //if (resp.isSuccess)
                //{
                //    return Ok(resp);
                //}
                //else
                //{
                //    return BadRequest(resp.Message);
                //}
            }
            catch (Exception ex)
            {
                resp.isSuccess = false;
                resp.Message = ex.ToString();
                return Ok(resp);
            }
        }


        [HttpGet("Provider/{id}")]
        public IActionResult ProviderRetrieve(string id)
        {
            GenericGetDataResponse resp = new GenericGetDataResponse();
            GenericAPIRequest r = new GenericAPIRequest();
            r.ProviderId = id;
            r.MethodName = HttpContext.Request.Path;
            r.IpAddress = _IPAccess.HttpContext.Connection.RemoteIpAddress.ToString();
            try
            {

                if (string.IsNullOrEmpty(r.ProviderId))
                {
                    return BadRequest("Please specify a provider id");
                }

                //string jsonCreds = JsonConvert.SerializeObject(r.Credentials, Formatting.Indented);

                resp = db.GetDataById(r, "PROVIDER", r.ProviderId);
                if (resp.isSuccess && resp.Data.Count>0)
                {
                    //JArray json = JsonConvert.DeserializeObject<JArray>(resp.Data[0]["PROVIDER_CREDENTIAL"].ToString());
                    List<Dictionary<string, string>> contents = new List<Dictionary<string, string>>();
                    Dictionary<string, string> content = new Dictionary<string, string>();

                    content.Add("ProviderId", resp.Data[0]["PROVIDER_ID"].ToString());
                    content.Add("ProviderCode", resp.Data[0]["PROVIDER_CODE"].ToString());
                    content.Add("ProviderName", resp.Data[0]["PROVIDER_NAME"].ToString());

                    //foreach (JObject item in json)
                    //{
                    //    foreach(var x in item)
                    //    {
                    //        string name = x.Key;
                    //        string value = x.Value.ToString();
                    //        content.Add(name, value);
                    //    }
                    //}
                    
                    contents.Add(content);
                    resp.Data = contents;
                    return Ok(resp);
                }
                else
                {
                    resp.Message = "No record has been retrieved.";
                    return Ok(resp);
                }
            }
            catch (Exception ex)
            {
                resp.isSuccess = false;
                resp.Message = ex.ToString();
                return Ok(resp);
            }
        }


        [HttpPut("Provider/{id}")]
        public IActionResult ProviderUpdate(string id,[FromBody] GenericAPIRequest r)
        {
            GenericInsertUpdateResponse resp = new GenericInsertUpdateResponse();
            r.ProviderId = id;
            r.MethodName = HttpContext.Request.Path;
            r.IpAddress = _IPAccess.HttpContext.Connection.RemoteIpAddress.ToString();
            try
            {
                if (string.IsNullOrEmpty(r.ProviderId))
                {
                    return BadRequest("Please specify a provider id");
                }

                if (string.IsNullOrEmpty(r.ProviderCode))
                {
                    return BadRequest("Please specify a provider code");
                }

                if (string.IsNullOrEmpty(r.ProviderName))
                {
                    return BadRequest("Please specify a provider name");
                }

                if (string.IsNullOrEmpty(r.ProviderDescription))
                {
                    r.ProviderDescription = "";
                }

                //string jsonCreds = JsonConvert.SerializeObject(r.Credentials, Formatting.Indented);
                //r.CredsJson = jsonCreds;
                resp = db.InsertUpdateData(r, "UPDATE_PROVIDER", true);
                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.isSuccess = false;
                resp.Message = ex.ToString();
                return Ok(resp);
            }
        }

        [HttpDelete("Provider/{id}")]
        public IActionResult ProviderDelete(string id)
        {
            GenericGetDataResponse resp = new GenericGetDataResponse();
            GenericAPIRequest r = new GenericAPIRequest();
            r.ProviderId = id;
            r.MethodName = HttpContext.Request.Path;
            r.IpAddress = _IPAccess.HttpContext.Connection.RemoteIpAddress.ToString();
            try
            {

                if (string.IsNullOrEmpty(r.ProviderId))
                {
                    return BadRequest("Please specify a provider id");
                }
                resp = db.DeleteData("PROVIDER", r.ProviderId);
                if (resp.isSuccess)
                {
                    return Ok(resp);
                }
                else
                {
                    return BadRequest(resp.Message);
                }
            }
            catch (Exception ex)
            {
                resp.isSuccess = false;
                resp.Message = ex.ToString();
                return Ok(resp);
            }
        }

        //End of Provider


        //Credentials

        [HttpGet("Credential/{program_id}")]
        public IActionResult CredentialList(string program_id)
        {
            GenericGetDataResponse resp = new GenericGetDataResponse();
            string objName = "DBContext";

            var type = Type.GetType(objName);


            GenericAPIRequest r = new GenericAPIRequest();

            r.MethodName = HttpContext.Request.Path;
            r.IpAddress = _IPAccess.HttpContext.Connection.RemoteIpAddress.ToString();
            r.ProgramId = _authService.GetUser().Id;
            resp = db.GetCredentialList(program_id);
            if (resp.isSuccess)
            {
                return Ok(resp);
            }
            else
            {
                return BadRequest(resp.Message);
            }
        }

        [HttpPost("Credential")]
        public IActionResult CredentialRegister([FromBody] GenericAPIRequest r)
        {
            GenericInsertUpdateResponse resp = new GenericInsertUpdateResponse();
            r.MethodName = HttpContext.Request.Path;
            r.IpAddress = _IPAccess.HttpContext.Connection.RemoteIpAddress.ToString();
            try
            {

                if (string.IsNullOrEmpty(r.ProviderId))
                {
                    return BadRequest("Please specify a provider id");
                }

                if (string.IsNullOrEmpty(r.ProviderTypeId))
                {
                    return BadRequest("Please specify a provider type id");
                }

                if (string.IsNullOrEmpty(r.ProgramId))
                {
                    return BadRequest("Please specify a program id");
                }

                string jsonCreds = JsonConvert.SerializeObject(r.Credentials, Formatting.Indented);
                r.CredsJson = jsonCreds;
                resp = db.InsertUpdateData(r, "CREDENTIAL", true);
                return Ok(resp);
                //if (resp.isSuccess)
                //{
                //    return Ok(resp);
                //}
                //else
                //{
                //    return BadRequest(resp.Message);
                //}
            }
            catch (Exception ex)
            {
                resp.isSuccess = false;
                resp.Message = ex.ToString();
                return Ok(resp);
            }
        }

        [HttpGet("Credential/{program_id}/{id}")]
        public IActionResult CredentialRetrieve(string program_id,string id)
        {
            GenericGetDataResponse resp = new GenericGetDataResponse();
            GenericAPIRequest r = new GenericAPIRequest();
            r.ProviderCode = id;
            r.MethodName = HttpContext.Request.Path;
            r.IpAddress = _IPAccess.HttpContext.Connection.RemoteIpAddress.ToString();
            try
            {

                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest("Please specify a credential id");
                }

                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest("Please specify a program id");
                }

                resp = db.GetCredentialList(program_id, id);
                if (resp.isSuccess)
                {
                    return Ok(resp);
                }
                else
                {
                    return BadRequest(resp.Message);
                }
            }
            catch (Exception ex)
            {
                resp.isSuccess = false;
                resp.Message = ex.ToString();
                return Ok(resp);
            }
        }


        [HttpPut("Credential/{id}")]
        public IActionResult CredentialUpdate(string id, [FromBody] GenericAPIRequest r)
        {
            GenericInsertUpdateResponse resp = new GenericInsertUpdateResponse();
            r.MethodName = HttpContext.Request.Path;
            r.IpAddress = _IPAccess.HttpContext.Connection.RemoteIpAddress.ToString();
            try
            {
                r.CredentialId = id;
                if (string.IsNullOrEmpty(r.ProviderId))
                {
                    return BadRequest("Please specify a provider id");
                }

                if (string.IsNullOrEmpty(r.ProviderTypeId))
                {
                    return BadRequest("Please specify a provider type id");
                }

                if (string.IsNullOrEmpty(r.ProgramId))
                {
                    return BadRequest("Please specify a program id");
                }

                string jsonCreds = JsonConvert.SerializeObject(r.Credentials, Formatting.Indented);
                r.CredsJson = jsonCreds;
                resp = db.InsertUpdateData(r, "UPDATE_CREDENTIAL", true);
                return Ok(resp);
            }
            catch (Exception ex)
            {
                resp.isSuccess = false;
                resp.Message = ex.ToString();
                return Ok(resp);
            }
        }

        [HttpDelete("Credential/{id}")]
        public IActionResult CredentialDelete(string id)
        {
            GenericGetDataResponse resp = new GenericGetDataResponse();
            GenericAPIRequest r = new GenericAPIRequest();
            r.CredentialId = id;
            r.MethodName = HttpContext.Request.Path;
            r.IpAddress = _IPAccess.HttpContext.Connection.RemoteIpAddress.ToString();
            try
            {

                if (string.IsNullOrEmpty(r.CredentialId))
                {
                    return BadRequest("Please specify a credential id");
                }
                resp = db.DeleteData("CREDENTIAL", r.CredentialId);
                if (resp.isSuccess)
                {
                    return Ok(resp);
                }
                else
                {
                    return BadRequest(resp.Message);
                }
            }
            catch (Exception ex)
            {
                resp.isSuccess = false;
                resp.Message = ex.ToString();
                return Ok(resp);
            }
        }

        //End of Credentials

        [HttpGet("Creds/{program_id}/{name}")]
        public IActionResult Creds(string program_id, string name)
        {
            GenericGetDataResponse resp = new GenericGetDataResponse();
            GenericAPIRequest r = new GenericAPIRequest();
            r.ProviderCode = name;
            r.MethodName = HttpContext.Request.Path;
            r.IpAddress = _IPAccess.HttpContext.Connection.RemoteIpAddress.ToString();
            try
            {

                if (string.IsNullOrEmpty(name))
                {
                    return BadRequest("Please specify a provider name");
                }

                if (string.IsNullOrEmpty(name))
                {
                    return BadRequest("Please specify a program id");
                }

                resp = db.GetCredentialListByProviderName(program_id, name);
                if (resp.isSuccess)
                {
                    return Ok(resp);
                }
                else
                {
                    return BadRequest(resp.Message);
                }
            }
            catch (Exception ex)
            {
                resp.isSuccess = false;
                resp.Message = ex.ToString();
                return Ok(resp);
            }
        }

    }
}
