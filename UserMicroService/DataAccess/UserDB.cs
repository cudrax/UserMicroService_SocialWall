using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using URISUtil.DataAccess;
using URISUtil.Logging;
using URISUtil.Response;
using UserMicroService.Models;

namespace UserMicroService.DataAccess
{
    public class UserDB
    {

        private static User ReadRow(SqlDataReader reader)
        {
            User retVal = new User
            {
                Id = (int)reader["Id"],
                FirstName = reader["FirstName"] as string,
                LastName = reader["LastName"] as string,
                Email = reader["Email"] as string,
                Birthday = (DateTime)reader["Birthday"],
                Phone = reader["Phone"] as string,
                City = reader["City"] as string,
                Country = reader["Country"] as string,

                Username = reader["Username"] as string,
                Password = reader["Password"] as string,

                Active = (bool)reader["Active"]
            };

            return retVal;
        }

        private static int ReadId(SqlDataReader reader)
        {
            return (int)reader["Id"];
        }

        private static string AllColumnSelect
        {
            get
            {
                return @"
                    [User].[Id],
	                [User].[FirstName],
	                [User].[LastName],
                    [User].[Email],
                    [User].[Birthday],
                    [User].[Phone],
                    [User].[City],
                    [User].[Country],
                    [User].[Username],
                    [User].[Password],
	                [User].[Active]
                ";
            }
        }

        private static void FillData(SqlCommand command, User user)
        {
            //command.AddParameter("@Id", SqlDbType.Int, user.Id);
            command.AddParameter("@FirstName", SqlDbType.NVarChar, user.FirstName);
            command.AddParameter("@LastName", SqlDbType.NVarChar, user.LastName);
            command.AddParameter("@Email", SqlDbType.NVarChar, user.Email);
            command.AddParameter("@Birthday", SqlDbType.DateTime, user.Birthday);
            command.AddParameter("@Phone", SqlDbType.NVarChar, user.Phone);
            command.AddParameter("@City", SqlDbType.NVarChar, user.City);
            command.AddParameter("@Country", SqlDbType.NVarChar, user.Country);
            command.AddParameter("@Username", SqlDbType.NVarChar, user.Username);
            command.AddParameter("@Password", SqlDbType.NVarChar, user.Password);
            command.AddParameter("@Active", SqlDbType.Bit, user.Active);
        }

        private static object CreateLikeQueryString(string str)
        {
            return str == null ? (object)DBNull.Value : "%" + str + "%";
        }

        public static List<User> GetUsers(ActiveStatusEnum active)
        {
            try
            {
                List<User> retVal = new List<User>();

                using (SqlConnection connection = new SqlConnection(DBFunctions.ConnectionString))
                {
                    SqlCommand command = connection.CreateCommand();
                    command.CommandText = String.Format(@"
                        SELECT
                            {0}
                        FROM
                            [dbo].[User]
                        WHERE
                            (@Active IS NULL OR [dbo].[User].Active = @Active)
                    ", AllColumnSelect);
                    command.Parameters.Add("@Active", SqlDbType.Bit);
                    
                    switch (active)
                    {
                        case ActiveStatusEnum.Active:
                            command.Parameters["@Active"].Value = true;
                            break;
                        case ActiveStatusEnum.Inactive:
                            command.Parameters["@Active"].Value = false;
                            break;
                        case ActiveStatusEnum.All:
                            command.Parameters["@Active"].Value = DBNull.Value;
                            break;
                    }

                    System.Diagnostics.Debug.WriteLine(command.CommandText);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            retVal.Add(ReadRow(reader));
                        }
                    }
                }
                return retVal;
            }
            catch (Exception ex)
            {
                Logger.WriteLog(ex);
                throw ErrorResponse.ErrorMessage(HttpStatusCode.BadRequest, ex);
            }
        }

        public static User GetUser(int id)
        {
            try
            {
                User retVal = new User();

                using (SqlConnection connection = new SqlConnection(DBFunctions.ConnectionString))
                {
                    SqlCommand command = connection.CreateCommand();
                    command.CommandText = String.Format(@"
                        SELECT
                            {0}
                        FROM
                            [dbo].[User]
                        WHERE
                            [Id] = @Id
                    ", AllColumnSelect);

                    command.AddParameter("@Id", SqlDbType.Int, id);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            retVal = ReadRow(reader);
                        }
                        else
                        {
                            return null;
                        }
                    }
                }

                return retVal;
            }
            catch (Exception ex)
            {
                Logger.WriteLog(ex);
                throw ErrorResponse.ErrorMessage(HttpStatusCode.BadRequest, ex);
            }
        }

        public static User InsertUser(User user)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(DBFunctions.ConnectionString))
                {
                    SqlCommand command = connection.CreateCommand();
                    command.CommandText = @"                     
                        INSERT INTO [dbo].[User]
                        (
                            [FirstName],
                            [LastName],
                            [Email],
                            [Birthday],
                            [Phone],
                            [City],
                            [Country],
                            [Username],
                            [Password],
                            [Active]                
                        )
                        VALUES
                        (
                            @FirstName,
                            @LastName,
                            @Email,
                            @Birthday,
                            @Phone,
                            @City,
                            @Country,
                            @Username,
                            @Password,
                            @Active 
                        )
                        
                    ";
                    FillData(command, user);
                    connection.Open();
                    command.ExecuteNonQuery();
                    //return GetUser(user.Id);
                    return user;

                    int id = 0;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            id = ReadId(reader);
                        }
                    }

                    return GetUser(id);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog(ex);
                throw ErrorResponse.ErrorMessage(HttpStatusCode.BadRequest, ex);
            }
        }

        public static User UpdateUser(User user, int id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(DBFunctions.ConnectionString))
                {
                    SqlCommand command = connection.CreateCommand();
                    command.CommandText = String.Format(@"
                        UPDATE
                            [dbo].[User]
                        SET
                            [FirstName] = @FirstName,
                            [LastName] = @LastName,
                            [Email] = @Email,
                            [Birthday] = @Birthday,
                            [Phone] = @Phone,
                            [City] = @City,
                            [Country] = @Country,
                            [Username] = @Username,
                            [Password] = @Password,
                            [Active] = @Active 
                        WHERE
                            [Id] = @Id
                    ");
                    FillData(command, user);
                    command.AddParameter("@Id", SqlDbType.Int, id);
                    connection.Open();
                    command.ExecuteNonQuery();
                    
                    return GetUser(id);
                }
            }
            catch (Exception ex)
            {

                Logger.WriteLog(ex);

                return null;
                throw ErrorResponse.ErrorMessage(HttpStatusCode.BadRequest, ex);
            }
        }

        public static void DeleteUser(int id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(DBFunctions.ConnectionString))
                {
                    SqlCommand command = connection.CreateCommand();
                    command.CommandText = String.Format(@"
                        UPDATE
                            [dbo].[User]
                        SET
                            [Active] = 'False'
                        WHERE
                            [Id] = @Id     
                    ");

                    command.AddParameter("@Id", SqlDbType.Int, id);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog(ex);
                throw ErrorResponse.ErrorMessage(HttpStatusCode.BadRequest, ex);
            }
        }

    }
}