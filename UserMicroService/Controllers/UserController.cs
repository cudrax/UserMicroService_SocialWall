using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using URISUtil.DataAccess;
using UserMicroService.DataAccess;
using UserMicroService.Models;

namespace UserMicroService.Controllers
{
    [RoutePrefix("api/User")]
    public class UserController : ApiController
    {
        [Route(""), HttpGet]
        public IEnumerable GetUsers([FromUri]ActiveStatusEnum active = ActiveStatusEnum.Active)
        {
            return UserDB.GetUsers(active);
        }

        [Route("{id}"), HttpGet]
        public User GetUser(int id)
        {
            return UserDB.GetUser(id);
        }

        [Route(""), HttpPost]
        public User InsertUser([FromBody]User user)
        {
            return UserDB.InsertUser(user);
        }

        [Route("{id}"), HttpPut]
        public User UpdateUser([FromBody]User user, int id)
        {
            return UserDB.UpdateUser(user, id);
        }

        [Route("{id}"), HttpDelete]
        public void DeleteUser(int id)
        {
            UserDB.DeleteUser(id);
        }
    }
}