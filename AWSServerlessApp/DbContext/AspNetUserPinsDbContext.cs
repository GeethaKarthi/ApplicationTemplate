using AWSServerlessApp.Controllers;
using AWSServerlessApp.CustomModels;
using AWSServerlessApp.LogProvider;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace AWSServerlessApp.DbContext
{
   public class AspNetUserPinsDbContext
    {
        private IConfiguration _iconfiguration;
        private readonly ILogger<AccountsController> _logger;
        private  Random random = new Random();

        public AspNetUserPinsDbContext( IConfiguration iconfiguration,ILogger<AccountsController> logger)
        {
            _iconfiguration = iconfiguration;
            _logger = logger;
        }
        public AspNetUserPinModel isValidatePin(string Pin)
        {
            _logger.LogInformation((int)LoggingEvents.VALIDATE_ITEM, "Validating PIN");
            string ConnectionPath1 = _iconfiguration.GetSection("ConnectionStrings").GetSection("DbConnection").Value;
            //  string ConnectionPath = "Server=identitycoreawsdb.cl28zty7wqij.us-east-1.rds.amazonaws.com;Database=NetCoreIdentitySample;Uid=masterUsername;Pwd=masterPassword";
            SqlConnection conn = new SqlConnection(ConnectionPath1);
            conn.Open();

            AspNetUserPinModel pinmodel = new AspNetUserPinModel();
            string sql = "isValidUserPin";

            SqlCommand comm = new SqlCommand(sql, conn);
            comm.Parameters.AddWithValue("@Pin", Pin);
            comm.CommandType = CommandType.StoredProcedure;
            SqlDataReader dr = comm.ExecuteReader();
            _logger.LogInformation((int)LoggingEvents.VALIDATE_ITEM, "Returned valid pin from database {t}",dr.HasRows);
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    pinmodel.Message = dr.GetString(0);
                    if (pinmodel.Message == "Pin Valid")
                    {

                        pinmodel.LastName = dr["LastName"].ToString();
                        pinmodel.FirstName = dr["FirstName"].ToString();
                    }
                }
            }
            comm.Dispose();
            conn.Close();
            return pinmodel;
        }

        public List<AspNetUserPinModel> GetAspNetUserPins()
        {
            //  string ConnectionPath = _iconfiguration.GetSection("ConnectionStrings").GetSection("DbConnection").Value;
            var test = _iconfiguration;
            string ConnectionPath = _iconfiguration.GetSection("ConnectionStrings").GetSection("DbConnection").Value;
            SqlConnection conn = new SqlConnection(ConnectionPath);
            conn.Open();

            List<AspNetUserPinModel> list = new List<AspNetUserPinModel>();
           
            string sql = "getAspNetUserPins";

            SqlCommand comm = new SqlCommand(sql, conn);
            comm.CommandType = CommandType.StoredProcedure;
            SqlDataReader dr = comm.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    AspNetUserPinModel pinmodel = new AspNetUserPinModel();
                    pinmodel.Id = dr["Id"].ToString();
                    pinmodel.LastName = dr["LastName"].ToString();
                    pinmodel.FirstName = dr["FirstName"].ToString();
                    pinmodel.Pin = dr["Pin"].ToString();
                   // pinmodel.CreatedDate =  Convert.ToDateTime(dr["CreatedDate"].ToString());
                    pinmodel.EmailAddress= dr["EmailAddress"].ToString();
                    list.Add(pinmodel);
                }
                
            }
            comm.Dispose();
            conn.Close();
            return list;
        }
        public AspNetUserPinModel AddAspNetUserPins(AspNetUserPinModel pinmodel)
        {
            string ConnectionPath = _iconfiguration.GetSection("ConnectionStrings").GetSection("DbConnection").Value;
            //  string ConnectionPath = "Server=identitycoreawsdb.cl28zty7wqij.us-east-1.rds.amazonaws.com;Database=NetCoreIdentitySample;Uid=masterUsername;Pwd=masterPassword";
            SqlConnection conn = new SqlConnection(ConnectionPath);
            conn.Open();

            string sql = "addAspNetUserPins";
            pinmodel.Pin = RandomString(9);
            SqlCommand comm = new SqlCommand(sql, conn);
            comm.Parameters.AddWithValue("@Id", Guid.NewGuid());
            comm.Parameters.AddWithValue("@Pin", pinmodel.Pin);
            comm.Parameters.AddWithValue("@FirstName", pinmodel.FirstName);
            comm.Parameters.AddWithValue("@LastName", pinmodel.LastName);
            comm.Parameters.AddWithValue("@EmailAddress", pinmodel.EmailAddress);
            comm.CommandType = CommandType.StoredProcedure;
             comm.ExecuteNonQuery();
            comm.Dispose();
            conn.Close();
            return pinmodel;
        }

        public AspNetUserPinModel UpdateAspNetUserPins(AspNetUserPinModel pinmodel)
        {
            string ConnectionPath = _iconfiguration.GetSection("ConnectionStrings").GetSection("DbConnection").Value;
            //  string ConnectionPath = "Server=identitycoreawsdb.cl28zty7wqij.us-east-1.rds.amazonaws.com;Database=NetCoreIdentitySample;Uid=masterUsername;Pwd=masterPassword";
            SqlConnection conn = new SqlConnection(ConnectionPath);
            conn.Open();
         
      
        string sql = "updateAspNetUserPins";

            SqlCommand comm = new SqlCommand(sql, conn);
            comm.Parameters.AddWithValue("@Id", pinmodel.Id);
            comm.Parameters.AddWithValue("@FirstName", pinmodel.FirstName);
            comm.Parameters.AddWithValue("@LastName", pinmodel.LastName);
            comm.Parameters.AddWithValue("@EmailAddress", pinmodel.EmailAddress);
            comm.CommandType = CommandType.StoredProcedure;
            comm.ExecuteNonQuery();
            comm.Dispose();
            conn.Close();
            return pinmodel;
        }

        public bool isEmailExists(string EmailAddress)
        {
            string ConnectionPath = _iconfiguration.GetSection("ConnectionStrings").GetSection("DbConnection").Value;
            //  string ConnectionPath = "Server=identitycoreawsdb.cl28zty7wqij.us-east-1.rds.amazonaws.com;Database=NetCoreIdentitySample;Uid=masterUsername;Pwd=masterPassword";
            SqlConnection conn = new SqlConnection(ConnectionPath);
            conn.Open();

            string sql = "isEmailExists";

            SqlCommand comm = new SqlCommand(sql, conn);
            comm.CommandType = CommandType.StoredProcedure;
            comm.Parameters.AddWithValue("@EmailAddress", EmailAddress);
            bool result=  Convert.ToBoolean(comm.ExecuteScalar().ToString());
            return result;
        }
        public string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public List<AspNetUserPinModel> GetFilteredAspNetUserPins(string FirstName, string LastName, string EmailAddress)
        {
            var test = _iconfiguration;
            string ConnectionPath = _iconfiguration.GetSection("ConnectionStrings").GetSection("DbConnection").Value;
            //  string ConnectionPath = "Server=identitycoreawsdb.cl28zty7wqij.us-east-1.rds.amazonaws.com;Database=NetCoreIdentitySample;Uid=masterUsername;Pwd=masterPassword";
            SqlConnection conn = new SqlConnection(ConnectionPath);
            conn.Open();

            List<AspNetUserPinModel> list = new List<AspNetUserPinModel>();

            string sql = "getFilteredAspNetUserPins";

            SqlCommand comm = new SqlCommand(sql, conn);
            comm.CommandType = CommandType.StoredProcedure;
            comm.Parameters.AddWithValue("@FirstName",FirstName);
            comm.Parameters.AddWithValue("@LastName", LastName);
            comm.Parameters.AddWithValue("@EmailAddress",  EmailAddress);
            SqlDataReader dr = comm.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    AspNetUserPinModel pinmodel = new AspNetUserPinModel();
                    pinmodel.Id = dr["Id"].ToString();
                    pinmodel.LastName = dr["LastName"].ToString();
                    pinmodel.FirstName = dr["FirstName"].ToString();
                    pinmodel.Pin = dr["Pin"].ToString();
                    // pinmodel.CreatedDate =  Convert.ToDateTime(dr["CreatedDate"].ToString());
                    pinmodel.EmailAddress = dr["EmailAddress"].ToString();
                    list.Add(pinmodel);
                }
             
            }
            comm.Dispose();
            conn.Close();
            return list;
        }
    }



}
