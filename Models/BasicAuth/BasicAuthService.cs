using Notification.Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Notification.Models.BasicAuth
{
    public class BasicAuthServices
    {
        public static ProgramAuthUser _listusers;
        public static string conn;
        public interface IBasicAuthService
        {
            Task<ProgramAuthUser> Authenticate(string username, string password, string programKey);
            Task<ProgramAuthUser> GetAll();
            ProgramAuthUser GetUser();
        }

        public class BasicAuthService : IBasicAuthService
        {

            private ProgramAuthUser _users = _listusers;

            public async Task<ProgramAuthUser> Authenticate(string username, string password, string programKey)
            {
                DBContext db = new DBContext(conn);
                //_users = db.GetProgramUser(username, password);
                var _user = await Task.Run(() => db.GetAuthUser(username, password));
                _users = _user;
                if (_users == null)
                    return null;

                return _users.WithoutPassword();
            }
            public ProgramAuthUser GetUser()
            {
                return _users;
            }

            public async Task<ProgramAuthUser> GetAll()
            {
                return await Task.Run(() => _users.WithoutPassword());
            }
        }
    }
}
