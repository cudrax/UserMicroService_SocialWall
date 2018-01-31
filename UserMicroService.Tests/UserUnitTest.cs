using System;
using NUnit.Framework;
using UserMicroService.DataAccess;
using System.Collections.Generic;
using UserMicroService.Models;
using URISUtil.DataAccess;

namespace UserMicroService.Tests
{
    public class UserUnitTest
    {

        ActiveStatusEnum active = ActiveStatusEnum.Active;

        [Test]
        public void GetUsersSuccess()
        {
            List<User> users = UserDB.GetUsers(active);
            Assert.AreEqual(users.Count, 3);
            //ocekivani broj korisnika (active)
        }

        [Test]
        public void GetUserSuccess()
        {
            int id = UserDB.GetUsers(active)[0].Id;
            User user = UserDB.GetUser(id);
            Assert.IsNotNull(user);
        }

        [Test]

        public void GetUserFailed()
        {
            int id = 100;
            User user = UserDB.GetUser(id);
            Assert.IsNull(user);
        }

        [Test]
        public void InsertUserSuccess()
        {
            User user = new User
            {
                FirstName = "Nikola",
                LastName = "Nikolic",
                Email = "nnikola@gmail.com",
                Birthday= new DateTime(1900, 5, 1, 8, 30, 52),
                Phone = "06112345678",
                City = "Novi Sad",
                Country = "Serbia",
                Username = "Nidzo",
                Password = "nidzakornjaca",
                Active = true
            };
            int oldNumberOfUsers = UserDB.GetUsers(active).Count;
            UserDB.InsertUser(user);
            Assert.AreEqual(UserDB.GetUsers(active).Count, oldNumberOfUsers + 1);
        }

        [Test]
        public void InsertUserFailed()
        {
            Assert.AreEqual(1, 1);
        }

        [Test]
        public void UpdateUserSuccess()
        {
            int id = UserDB.GetUsers(active)[0].Id;
            User user = new User
            {
                FirstName = "Nikola_U",
                LastName = "Nikolic_U",
                Email = "nnikola@gmail.com_U",
                Birthday = new DateTime(1900, 5, 1, 8, 30, 52),
                Phone = "06112345678_U",
                City = "Novi Sad_U",
                Country = "Serbia_U",
                Username = "Nidzo_U",
                Password = "nidzakornjaca_U",
                Active = true
            };

            User updatedUser = UserDB.GetUser(id);
            UserDB.UpdateUser(user, id);

            Assert.AreEqual(user.FirstName, updatedUser.FirstName);
            Assert.AreEqual(user.LastName, updatedUser.LastName);
            Assert.AreEqual(user.Email, updatedUser.Email);
            Assert.AreEqual(user.Birthday, updatedUser.Birthday);
            Assert.AreEqual(user.Phone, updatedUser.Phone);
            Assert.AreEqual(user.City, updatedUser.City);
            Assert.AreEqual(user.Country, updatedUser.Country);
            Assert.AreEqual(user.Username, updatedUser.Username);
            Assert.AreEqual(user.Password, updatedUser.Password);
            Assert.AreEqual(user.Active, updatedUser.Active);
        }

        [Test]
        public void UpdateUserFailed()
        {
            int id = 100;
            User user = new User
            {
                FirstName = "Niko",
                LastName = "Nikic",
                Email = "nniko@gmail.com",
                Birthday = new DateTime(1900, 5, 1, 8, 30, 52),
                Phone = "06112678",
                City = "NovS",
                Country = "SerbiS",
                Username = "Nikodz",
                Password = "nikodzkornjaca",
                Active = true
            };

            User updatedUser = UserDB.UpdateUser(user, id);
            Assert.IsNull(updatedUser);

        }

        [Test]
        public void DeleteUserSuccess()
        {
            int id = UserDB.GetUsers(active)[0].Id;
            UserDB.DeleteUser(id);
            Assert.AreEqual(UserDB.GetUser(id).Active, false);
        }

        [Test]
        public void DeleteUserFailed()
        {
            int numberOfOldUsers = UserDB.GetUsers(active).Count;
            UserDB.DeleteUser(100);
            Assert.AreEqual(numberOfOldUsers, UserDB.GetUsers(active).Count);
        }

    }
}
