using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using globelinkapi.Entities;
using globelinkapi.Helpers;
using globelinkapi.Models;
using System.Data;
using System.Collections;
using System.Data.SqlClient;

using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;

namespace globelinkapi.services
{
    public interface IUserService
    {
        AuthenticateResponse Authenticate(AuthenticateRequest model);
        bool insUser(UsersRequest model);
        IEnumerable<User> GetAll();
        User GetById(int id);
        bool updUser(UsersRequest model);
    }

    public class UserService : IUserService
    {
        // users hardcoded for simplicity, store in a db with hashed passwords in production applications
        private List<User> _users = new List<User>();
       
        private User myUser = new User();
        private readonly AppSettings _appSettings;

        public UserService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
            getUsers();
        }
        private void getUsers()
        {
            DbAccess db = new DbAccess();
            DataTable dt = db.GetData("select * from ma_users");
            foreach (DataRow r in dt.Rows)
            {
                User u = new User();
                u.Id = int.Parse(r["id"].ToString());
                u.Username = r["username"].ToString();
                u.Password = r["password"].ToString();
                u.FirstName = r["first_name"].ToString();
                u.LastName = r["last_name"].ToString();
                u.Password = r["password"].ToString();
                u.Status = r["status"].ToString();
                u.CreatedBy = r["created_by"].ToString();
                u.CreatedDate = r["created_date"].ToString();
                u.UpdatedBy = r["updated_by"].ToString();
                u.UpdatedDate = r["updated_date"].ToString();
                _users.Add(u);
            }
        }
        public AuthenticateResponse Authenticate(AuthenticateRequest model)
        {
           
            string hashedPassword = getHashedPassword(model.Password);
            myUser = _users.SingleOrDefault(x => x.Username == model.Username && x.Password == hashedPassword);

            // return null if user not found
            if (myUser == null) return null;

            // authentication successful so generate jwt token
            var token = generateJwtToken(myUser);

            return new AuthenticateResponse(myUser, token);
        }
        private string getHashedPassword(String password)
        {
            byte[] salt = new byte[128 / 8];
               string hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
               password: password,
               salt: salt,
               prf: KeyDerivationPrf.HMACSHA1,  
               iterationCount: 10000,
               numBytesRequested: 256 / 8));
            return hashedPassword;
        }
        public bool insUser(UsersRequest model){
            bool result = false;
            try{
               
               
                DbAccess db = new DbAccess();
                StringBuilder sb = new StringBuilder();
                sb.Append("insert into ma_users(username,first_name,last_name,password,status,created_by,created_date,updated_by,updated_date) ");
                sb.Append(" values(@username,@first_name,@last_name,@password,@status,@created_by,SYSDATETIME(),@updated_by,SYSDATETIME())");
                var paramenters = new ArrayList();
                SqlParameter para = new SqlParameter();
                para.Value = model.userName;
                para.ParameterName = "username";
                paramenters.Add(para);

                para = new SqlParameter();
                para.Value = model.firstName;
                para.ParameterName = "first_name";
                paramenters.Add(para);

                para = new SqlParameter();
                para.Value = model.lastName;
                para.ParameterName = "last_name";
                paramenters.Add(para);

                para = new SqlParameter();
                para.Value = getHashedPassword(model.password);
                para.ParameterName = "password";
                paramenters.Add(para);

                para = new SqlParameter();
                para.Value = model.status;
                para.ParameterName = "status";
                paramenters.Add(para);

                para = new SqlParameter();
                para.Value = model.createdBy;
                para.ParameterName = "created_by";
                paramenters.Add(para);

                para = new SqlParameter();
                para.Value = model.updatedBy;
                para.ParameterName = "updated_by";
                paramenters.Add(para);

                result = db.ExecuteNonQuery(sb.ToString(),paramenters);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return result;
        }
        public bool updUser(UsersRequest model)
        {
            bool result = false;
            try
            {


                DbAccess db = new DbAccess();
                StringBuilder sb = new StringBuilder();
                sb.Append("update ma_users set  " +
                    " first_name = @first_name " +
                    ",last_name = @last_name " +
                    ",status = @status " +
                    ",updated_by = @updated_by" +
                    ",updated_date = SYSDATETIME() " +
                    " where id = @id ");


                var paramenters = new ArrayList();
                SqlParameter para = new SqlParameter();
                para.Value = model.firstName;
                para.ParameterName = "first_name";
                paramenters.Add(para);

                para = new SqlParameter();
                para.Value = model.lastName;
                para.ParameterName = "last_name";
                paramenters.Add(para);

                para = new SqlParameter();
                para.Value = model.status;
                para.ParameterName = "status";
                paramenters.Add(para);

                para = new SqlParameter();
                para.Value = model.updatedBy;
                para.ParameterName = "updated_by";
                paramenters.Add(para);

                para = new SqlParameter();
                para.Value = model.id;
                para.ParameterName = "id";
                paramenters.Add(para);

                result = db.ExecuteNonQuery(sb.ToString(), paramenters);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return result;
        }
        public IEnumerable<User> GetAll()
        {
            return _users;
        }

        public User GetById(int id)
        {
            return _users.FirstOrDefault(x => x.Id == id);
        }

        // helper methods

        private string generateJwtToken(User user)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }

}
