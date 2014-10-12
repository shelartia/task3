using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WithMongo.Models
{
    interface IUserInfoRepository
    {
        IEnumerable<UserInfo> GetAllUserInfoes();
        UserInfo GetUserInfoById(string id);
        UserInfo Add(UserInfo userinfo);
        bool Update(string objectId,UserInfo userinfo);
        bool Delete(string objectId);
    }
}
