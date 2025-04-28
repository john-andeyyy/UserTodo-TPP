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
    
    [ApiVersion("2.0")]
    [Route("v{v:apiVersion}/")]
    [Authorize]
    [ApiController]
    public class GenericControllerV2 : Controller
    {
        private DBContext db;
        private readonly IWebHostEnvironment hostingEnvironment;
        private IHttpContextAccessor _IPAccess;
        public IBasicAuthService _authService;

        public GenericControllerV2(DBContext context, IWebHostEnvironment environment, IHttpContextAccessor accessor, IBasicAuthService authService)
        {
            _IPAccess = accessor;
            db = context;
            hostingEnvironment = environment;
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticateModelV2 model)
        {
            BasicAuthServices._listusers = db.GetAuthUser(model.username, model.password);
            var user = await _authService.Authenticate(model.username, model.password, "");

            if (user == null)
                return BadRequest(new genericResponseV2 { message = "Username or password is incorrect" });

            return Ok(new genericResponseWithDataV2 { message = "Success.", data = user });
        }


        [HttpGet("providerType")]
        public IActionResult ProviderTypes()
        {
            GenericGetDataResponse resp = new GenericGetDataResponse();
            GenericAPIRequestV2 r = new GenericAPIRequestV2();

            r.methodName = HttpContext.Request.Path;
            r.ipAddress = _IPAccess.HttpContext.Connection.RemoteIpAddress.ToString();
            r.programId = _authService.GetUser().Id;
            resp = db.GetDataV2(r, "PROVIDER_TYPE");
            if (resp.isSuccess)
            {
                return Ok(new genericResponseWithDataV2 { message = resp.Message, data = resp.Data });
            }
            else
            {
                return BadRequest(new genericResponseV2 { message = resp.Message });
            }
        }

        [HttpPost("providerType")]
        public IActionResult ProviderTypeRegister([FromBody] GenericAPIRequestV2 r)
        {
            GenericInsertUpdateResponse resp = new GenericInsertUpdateResponse();
            r.methodName = HttpContext.Request.Path;
            r.ipAddress = _IPAccess.HttpContext.Connection.RemoteIpAddress.ToString();
            try
            {

                if (string.IsNullOrEmpty(r.providerTypeCode))
                {
                    return BadRequest(new genericResponseV2 { message = "Please specify a provider type code" });
                }

                if (string.IsNullOrEmpty(r.providerTypeName))
                {
                    return BadRequest(new genericResponseV2 { message = "Please specify a provider type name" });
                }

                if (string.IsNullOrEmpty(r.providerTypeDescription))
                {
                    r.providerDescription = "";
                }
                r.providerTypeId = "";

                resp = db.InsertUpdateDataV2(r, "PROVIDER_TYPE", true);

                if(resp.isSuccess)
                {
                    var res = new
                    {
                        id = resp.Id
                    };
                    return Ok(new genericResponseWithDataV2 { message = resp.Message, data = res });
                }
                else
                {
                    return BadRequest(new genericResponseV2 { message = resp.Message });
                }

            }
            catch (Exception)
            {
                return BadRequest(new genericResponseV2 { message = "Unable to process request." });
            }
        }

        [HttpGet("providerType/{id}")]
        public IActionResult ProviderTypeRetrieve(string id)
        {
            GenericGetDataResponse resp = new GenericGetDataResponse();
            GenericAPIRequestV2 r = new GenericAPIRequestV2();
            r.providerTypeId = id;
            r.methodName = HttpContext.Request.Path;
            r.ipAddress = _IPAccess.HttpContext.Connection.RemoteIpAddress.ToString();
            try
            {

                if (string.IsNullOrEmpty(r.providerTypeId))
                {
                    return BadRequest(new genericResponseV2 { message = "Please specify a provider id" });
                }

                resp = db.GetDataByIdV2("PROVIDER_TYPE", r.providerTypeId);
                if (resp.isSuccess)
                {
                    List<Dictionary<string, string>> contents = new List<Dictionary<string, string>>();
                    Dictionary<string, string> content = new Dictionary<string, string>
                    {
                        { "providerTypeId", resp.Data[0]["PROVIDER_TYPE_ID"].ToString() },
                        { "providerTypeCode", resp.Data[0]["PROVIDER_TYPE_CODE"].ToString() },
                        { "providerTypeName", resp.Data[0]["PROVIDER_TYPE_NAME"].ToString() }
                    };

                    contents.Add(content);
                    resp.Data = contents;
                    return Ok(new genericResponseWithDataV2 { message = resp.Message, data = resp.Data });
                }
                else
                {
                    return BadRequest(new genericResponseV2 { message = resp.Message });
                }
            }
            catch (Exception)
            {
                return BadRequest(new genericResponseV2 { message = "Unable to process request." });
            }
        }


        [HttpPut("providerType/{id}")]
        public IActionResult ProviderTypeUpdate(string id, [FromBody] GenericAPIRequestV2 r)
        {
            GenericInsertUpdateResponse resp = new GenericInsertUpdateResponse();
            r.providerTypeId = id;
            r.methodName = HttpContext.Request.Path;
            r.ipAddress = _IPAccess.HttpContext.Connection.RemoteIpAddress.ToString();
            try
            {
                if (string.IsNullOrEmpty(r.providerTypeId))
                {
                    return BadRequest("Please specify a provider type id");
                }

                if (string.IsNullOrEmpty(r.providerTypeCode))
                {
                    return BadRequest("Please specify a provider type code");
                }

                if (string.IsNullOrEmpty(r.providerTypeName))
                {
                    return BadRequest("Please specify a provider type name");
                }

                if (string.IsNullOrEmpty(r.providerTypeDescription))
                {
                    r.providerTypeDescription = "";
                }

                resp = db.InsertUpdateDataV2(r, "UPDATE_PROVIDER_TYPE", true);
                if (resp.isSuccess)
                {
                    var res = new
                    {
                        id = resp.Id
                    };
                    return Ok(new genericResponseWithDataV2 { message = resp.Message, data = res });
                }
                else
                {
                    return BadRequest(new genericResponseV2 { message = resp.Message });
                }
            }
            catch (Exception)
            {
                return BadRequest(new genericResponseV2 { message = "Unable to process request." });
            }
        }


        [HttpDelete("providerType/{id}")]
        public IActionResult ProviderTypeDelete(string id)
        {
            GenericGetDataResponse resp = new GenericGetDataResponse();
            GenericAPIRequestV2 r = new GenericAPIRequestV2();
            r.providerTypeId = id;
            r.methodName = HttpContext.Request.Path;
            r.ipAddress = _IPAccess.HttpContext.Connection.RemoteIpAddress.ToString();
            try
            {

                if (string.IsNullOrEmpty(r.providerTypeId))
                {
                    return BadRequest(new genericResponseV2 { message = "Please specify a provider id" });
                }
                resp = db.DeleteData("PROVIDER_TYPE", r.providerTypeId);
                if (resp.isSuccess)
                {
                    return Ok(new genericResponseV2 { message = resp.Message});
                }
                else
                {
                    return BadRequest(new genericResponseV2 { message = resp.Message });
                }
            }
            catch (Exception)
            {
                return BadRequest(new genericResponseV2 { message = "Unable to process request." });
            }
        }



        //End of Provider Type Route

        //Provider
        [HttpGet("provider")]
        public IActionResult ProviderList()
        {
            GenericGetDataResponse resp = new GenericGetDataResponse();
            GenericAPIRequestV2 r = new GenericAPIRequestV2();

            r.methodName = HttpContext.Request.Path;
            r.ipAddress = _IPAccess.HttpContext.Connection.RemoteIpAddress.ToString();
            r.programId = _authService.GetUser().Id;
            resp = db.GetDataV2(r, "PROVIDER");
            if (resp.isSuccess)
            {
                return Ok(new genericResponseWithDataV2 { message = resp.Message, data = resp.Data});
            }
            else
            {
                return BadRequest(new genericResponseV2 { message = resp.Message });
            }
        }

        [HttpPost("provider")]
        public IActionResult ProviderRegister([FromBody] GenericAPIRequestV2 r)
        {
            GenericInsertUpdateResponse resp = new GenericInsertUpdateResponse();
            r.methodName = HttpContext.Request.Path;
            r.ipAddress = _IPAccess.HttpContext.Connection.RemoteIpAddress.ToString();
            try
            {

                if (string.IsNullOrEmpty(r.providerCode))
                {
                    return BadRequest(new genericResponseV2 { message = "Please specify a provider code" });
                }

                if (string.IsNullOrEmpty(r.providerName))
                {
                    return BadRequest(new genericResponseV2 { message = "Please specify a provider name" });
                }

                if (string.IsNullOrEmpty(r.programDescription))
                {
                    r.providerDescription = "";
                }

                string jsonCreds = JsonConvert.SerializeObject(r.credentials, Formatting.Indented);
                r.credsJson = jsonCreds;
                resp = db.InsertUpdateDataV2(r, "PROVIDER", true);

                if (resp.isSuccess)
                {
                    var res = new
                    {
                        id = resp.Id
                    };
                    return Ok(new genericResponseWithDataV2 { message = resp.Message, data = res });
                }
                else
                {
                    return BadRequest(new genericResponseV2 { message = resp.Message });
                }
            }
            catch (Exception)
            {
                return BadRequest(new genericResponseV2 { message = "Unable to process request." });
            }
        }


        [HttpGet("provider/{id}")]
        public IActionResult ProviderRetrieve(string id)
        {
            GenericGetDataResponse resp = new GenericGetDataResponse();
            GenericAPIRequestV2 r = new GenericAPIRequestV2();
            r.providerId = id;
            r.methodName = HttpContext.Request.Path;
            r.ipAddress = _IPAccess.HttpContext.Connection.RemoteIpAddress.ToString();
            try
            {

                if (string.IsNullOrEmpty(r.providerId))
                {
                    return BadRequest(new genericResponseV2 { message = "Please specify a provider id" });
                }

                resp = db.GetDataByIdV2("PROVIDER", r.providerId);
                if (resp.isSuccess && resp.Data.Count > 0)
                {
                    List<Dictionary<string, string>> contents = new List<Dictionary<string, string>>();
                    Dictionary<string, string> content = new Dictionary<string, string>();

                    content.Add("providerId", resp.Data[0]["PROVIDER_ID"].ToString());
                    content.Add("providerCode", resp.Data[0]["PROVIDER_CODE"].ToString());
                    content.Add("providerName", resp.Data[0]["PROVIDER_NAME"].ToString());

                    contents.Add(content);
                    resp.Data = contents;
                    return Ok(new genericResponseWithDataV2 { message = resp.Message, data = resp.Data });
                }
                else
                {
                    resp.Message = "No record has been retrieved.";
                    return BadRequest(new genericResponseV2 { message = resp.Message});
                }
            }
            catch (Exception)
            {
                return BadRequest(new genericResponseV2 { message = "Unable to process request." });
            }
        }


        [HttpPut("provider/{id}")]
        public IActionResult ProviderUpdate(string id, [FromBody] GenericAPIRequestV2 r)
        {
            GenericInsertUpdateResponse resp = new GenericInsertUpdateResponse();
            r.providerId = id;
            r.methodName = HttpContext.Request.Path;
            r.ipAddress = _IPAccess.HttpContext.Connection.RemoteIpAddress.ToString();
            try
            {
                if (string.IsNullOrEmpty(r.providerId))
                {
                    return BadRequest(new genericResponseV2 { message = "Please specify a provider id" });
                }

                if (string.IsNullOrEmpty(r.providerCode))
                {
                    return BadRequest(new genericResponseV2 { message = "Please specify a provider code" });
                }

                if (string.IsNullOrEmpty(r.providerName))
                {
                    return BadRequest(new genericResponseV2 { message = "Please specify a provider name" });
                }

                if (string.IsNullOrEmpty(r.providerDescription))
                {
                    r.providerDescription = "";
                }

                resp = db.InsertUpdateDataV2(r, "UPDATE_PROVIDER", true);

                if (resp.isSuccess)
                {
                    var res = new
                    {
                        id = resp.Id
                    };
                    return Ok(new genericResponseWithDataV2 { message = resp.Message, data = res });
                }
                else
                {
                    return BadRequest(new genericResponseV2 { message = resp.Message });
                }
            }
            catch (Exception)
            {
                return BadRequest(new genericResponseV2 { message = "Unable to process request." });
            }
        }

        [HttpDelete("provider/{id}")]
        public IActionResult ProviderDelete(string id)
        {
            GenericGetDataResponse resp = new GenericGetDataResponse();
            GenericAPIRequestV2 r = new GenericAPIRequestV2();
            r.providerId = id;
            r.methodName = HttpContext.Request.Path;
            r.ipAddress = _IPAccess.HttpContext.Connection.RemoteIpAddress.ToString();
            try
            {

                if (string.IsNullOrEmpty(r.providerId))
                {
                    return BadRequest(new genericResponseV2 { message = "Please specify a provider id" });
                }
                resp = db.DeleteData("PROVIDER", r.providerId);
                if (resp.isSuccess)
                {
                    return Ok(new genericResponseV2 { message = resp.Message });
                }
                else
                {
                    return BadRequest(new genericResponseV2 { message = resp.Message });
                }
            }
            catch (Exception)
            {
                return BadRequest(new genericResponseV2 { message = "Unable to process request." });
            }
        }

        //End of Provider


        //Credentials

        [HttpGet("credential/{program_id}")]
        public IActionResult CredentialList(string program_id)
        {
            GenericGetDataResponse resp = new GenericGetDataResponse();
            GenericAPIRequestV2 r = new GenericAPIRequestV2();

            r.methodName = HttpContext.Request.Path;
            r.ipAddress = _IPAccess.HttpContext.Connection.RemoteIpAddress.ToString();
            r.programId = _authService.GetUser().Id;
            resp = db.GetCredentialListV2(program_id);
            if (resp.isSuccess)
            {
                return Ok(new genericResponseWithDataV2 { message = resp.Message, data = resp.Data });
            }
            else
            {
                return BadRequest(new genericResponseV2 { message = resp.Message });
            }
        }

        [HttpPost("credential")]
        public IActionResult CredentialRegister([FromBody] GenericAPIRequestV2 r)
        {
            GenericInsertUpdateResponse resp = new GenericInsertUpdateResponse();
            r.methodName = HttpContext.Request.Path;
            r.ipAddress = _IPAccess.HttpContext.Connection.RemoteIpAddress.ToString();
            try
            {

                if (string.IsNullOrEmpty(r.providerId))
                {
                    return BadRequest(new genericResponseV2 { message = "Please specify a provider id" });
                }

                if (string.IsNullOrEmpty(r.providerTypeId))
                {
                    return BadRequest(new genericResponseV2 { message = "Please specify a provider type id" });
                }

                if (string.IsNullOrEmpty(r.programId))
                {
                    return BadRequest(new genericResponseV2 { message = "Please specify a program id" });
                }

                string jsonCreds = JsonConvert.SerializeObject(r.credentials, Formatting.Indented);
                r.credsJson = jsonCreds;
                resp = db.InsertUpdateDataV2(r, "CREDENTIAL", true);

                if (resp.isSuccess)
                {
                    var res = new
                    {
                        id = resp.Id
                    };
                    return Ok(new genericResponseWithDataV2 { message = resp.Message, data = res});
                }
                else
                {
                    return BadRequest(new genericResponseV2 { message = resp.Message });
                }
            }
            catch (Exception)
            {
                return BadRequest(new genericResponseV2 { message = "Unable to process request." });
            }
        }

        [HttpGet("credential/{program_id}/{id}")]
        public IActionResult CredentialRetrieve(string program_id, string id)
        {
            GenericGetDataResponse resp = new GenericGetDataResponse();
            GenericAPIRequestV2 r = new GenericAPIRequestV2();
            r.providerCode = id;
            r.methodName = HttpContext.Request.Path;
            r.ipAddress = _IPAccess.HttpContext.Connection.RemoteIpAddress.ToString();
            try
            {

                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest(new genericResponseV2 { message = "Please specify a credential id" });
                }

                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest(new genericResponseV2 { message = "Please specify a program id" });
                }

                resp = db.GetCredentialListV2(program_id, id);
                if (resp.isSuccess)
                {
                    return Ok(new genericResponseWithDataV2 { message = resp.Message, data = resp.Data });
                }
                else
                {
                    return BadRequest(new genericResponseV2 { message = resp.Message });
                }
            }
            catch (Exception)
            {
                return BadRequest(new genericResponseV2 { message = "Unable to process request." });
            }
        }


        [HttpPut("credential/{id}")]
        public IActionResult CredentialUpdate(string id, [FromBody] GenericAPIRequestV2 r)
        {
            GenericInsertUpdateResponse resp = new GenericInsertUpdateResponse();
            r.methodName = HttpContext.Request.Path;
            r.ipAddress = _IPAccess.HttpContext.Connection.RemoteIpAddress.ToString();
            try
            {
                r.credentialId = id;
                if (string.IsNullOrEmpty(r.providerId))
                {
                    return BadRequest(new genericResponseV2 { message = "Please specify a provider id" });
                }

                if (string.IsNullOrEmpty(r.providerTypeId))
                {
                    return BadRequest(new genericResponseV2 { message = "Please specify a provider type id" });
                }

                if (string.IsNullOrEmpty(r.programId))
                {
                    return BadRequest(new genericResponseV2 { message = "Please specify a program id" });
                }

                string jsonCreds = JsonConvert.SerializeObject(r.credentials, Formatting.Indented);
                r.credsJson = jsonCreds;
                resp = db.InsertUpdateDataV2(r, "UPDATE_CREDENTIAL", true);
                if (resp.isSuccess)
                {
                    var res = new
                    {
                        id = resp.Id
                    };
                    return Ok(new genericResponseWithDataV2 { message = resp.Message, data = res });
                }
                else
                {
                    return BadRequest(new genericResponseV2 { message = resp.Message });
                }
            }
            catch (Exception)
            {
                return BadRequest(new genericResponseV2 { message = "Unable to process request." });
            }
        }

        [HttpDelete("credential/{id}")]
        public IActionResult CredentialDelete(string id)
        {
            GenericGetDataResponse resp = new GenericGetDataResponse();
            GenericAPIRequestV2 r = new GenericAPIRequestV2();
            r.credentialId = id;
            r.methodName = HttpContext.Request.Path;
            r.ipAddress = _IPAccess.HttpContext.Connection.RemoteIpAddress.ToString();
            try
            {

                if (string.IsNullOrEmpty(r.credentialId))
                {
                    return BadRequest(new genericResponseV2 { message = "Please specify a credential id" });
                }
                resp = db.DeleteData("CREDENTIAL", r.credentialId);
                if (resp.isSuccess)
                {
                    return Ok(new genericResponseV2 { message = resp.Message });
                }
                else
                {
                    return BadRequest(new genericResponseV2 { message = resp.Message });
                }
            }
            catch (Exception)
            {
                return BadRequest(new genericResponseV2 { message = "Unable to process request." });
            }
        }

        //End of Credentials

        [HttpGet("creds/{program_id}/{name}")]
        public IActionResult Creds(string program_id, string name)
        {
            GenericGetDataResponse resp = new GenericGetDataResponse();
            GenericAPIRequestV2 r = new GenericAPIRequestV2();
            r.providerCode = name;
            r.methodName = HttpContext.Request.Path;
            r.ipAddress = _IPAccess.HttpContext.Connection.RemoteIpAddress.ToString();
            try
            {

                if (string.IsNullOrEmpty(name))
                {
                    return BadRequest(new genericResponseV2 { message = "Please specify a provider name" });
                }

                if (string.IsNullOrEmpty(name))
                {
                    return BadRequest(new genericResponseV2 { message = "Please specify a program id" });
                }

                resp = db.GetCredentialListByProviderNameV2(program_id, name);
                if (resp.isSuccess)
                {
                    return Ok(new genericResponseWithDataV2 { message = resp.Message, data = resp.Data});
                }
                else
                {
                    return BadRequest(new genericResponseV2 { message = resp.Message });
                }
            }
            catch (Exception)
            {
                return BadRequest(new genericResponseV2 { message = "Unable to process request." });
            }
        }

    }
}
