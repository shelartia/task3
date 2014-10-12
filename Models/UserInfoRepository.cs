using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WithMongo.Properties;

namespace WithMongo.Models
{
    public class UserInfoRepository:IUserInfoRepository
    {
        public MongoDatabase MongoDatabase;
        public MongoCollection UserInfoesCollection;
        public bool ServerIsDown = false;
 
        // Constructor
        public UserInfoRepository()
        {
            // Get the Mongo Client
            var mongoClient = new MongoClient(Settings.Default.UserInfoesConnetcionString);
 
            // Get the Mongo Server from the Cliet Instance
            var server = mongoClient.GetServer();
 
            // Assign the database to mongoDatabase
            MongoDatabase = server.GetDatabase(Settings.Default.DB);
 
            // get the Employees collection (table) and assign to UserInfoesCollection
            // "UserInfo"- db name is same as collection (table) name.
            UserInfoesCollection = MongoDatabase.GetCollection("UserInfo");
                
 
            //test if server is up and running
            try
            {
                MongoDatabase.Server.Ping(); 
            // Ping() method throws exception if not able to connect
 
            }
            catch (Exception ex)
            {
                ServerIsDown = true;
            }
        }
 
        #region Test Data
 
        private UserInfo[] _testUserInfoData = new UserInfo[]
        {
            new UserInfo()
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = "Sunny",
                LastName = "Kumar",
                Address = "New Delhi",
                
            },
            new UserInfo()
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = "Manish",
                LastName = "Kumar",
                Address = "Mumbai",
                
            },
            new UserInfo()
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = "Fanish",
                LastName = "Gumar",
                Address = "Dubai",
                
            },
            new UserInfo()
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = "Vanish",
                LastName = "Omar",
                Address = "Kobai",
                
            }
            
        };
 
        #endregion

        private List<UserInfo> _userinfoesList = new List<UserInfo>();

        public IEnumerable<UserInfo> GetAllUserInfoes()
        {
            if (ServerIsDown) return null;
 
            if (Convert.ToInt32(UserInfoesCollection.Count()) > 0)
            {
                _userinfoesList.Clear();
                var userinfoes = UserInfoesCollection.FindAs(typeof(UserInfo), Query.NE("FirstName", "null"));
                if (userinfoes.Count() > 0)
                {
                    foreach (UserInfo userinfo in userinfoes)
                    {
                        _userinfoesList.Add(userinfo);
                    }
                }
            }
            else
            {
                #region add test data if DB is empty
 
                UserInfoesCollection.RemoveAll();
                foreach (var userinfo in _testUserInfoData)
                {
                    _userinfoesList.Add(userinfo);

                    Add(userinfo); // add data to mongo db also
                }
 
                #endregion
            }
 
            var result = _userinfoesList.AsQueryable();
            return result;
        }


        public UserInfo GetUserInfoById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("id", "User Id is empty!");
            }
            var userinfo = (UserInfo)UserInfoesCollection.FindOneAs(typeof(UserInfo), Query.EQ("_id", id));
            return userinfo;
        }


        public UserInfo Add(UserInfo userinfo)
        {
            if (string.IsNullOrEmpty(userinfo.Id))
            {
                userinfo.Id = Guid.NewGuid().ToString();
            }
            UserInfoesCollection.Save(userinfo);
            return userinfo;
        }

        public bool Update(string objectId, UserInfo userinfo)
        {
            UpdateBuilder updateBuilder = MongoDB.Driver.Builders.Update
                .Set("FirstName", userinfo.FirstName)
                .Set("LastName", userinfo.LastName)
                .Set("Address", userinfo.Address)
                .Set("Photo", userinfo.Photo);
            UserInfoesCollection.Update(Query.EQ("_id", objectId), updateBuilder);
 
            return true;
        }
 
        public bool Delete(string objectId)
        {
            UserInfoesCollection.Remove(Query.EQ("_id", objectId));
            return true;
        }
    }
    
}