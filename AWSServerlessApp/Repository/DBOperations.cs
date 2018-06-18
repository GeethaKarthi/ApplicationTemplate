using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AWSServerlessApp.Models;
using AWSServerlessApp.Connection;
using Amazon.Lambda.APIGatewayEvents;
using System.Net;
using Newtonsoft.Json;
using AWSServerlessApp.CustomModels;

namespace AWSServerlessApp.Repository
{
    public class DBOperations : IDBOperation
    {
        private readonly ConnectionStrings connectionStrings;

        public DBOperations(IOptions<ConnectionStrings> options)
        {
            connectionStrings = options.Value;
        }

        public List<AspNetUsers> GetUsers()
        {
            List<AspNetUsers> lstUsers = new List<AspNetUsers>();
            try
            {
                string ConnectionPath = connectionStrings.DbConnection;
                //lstUsers.Add(new AspNetUsers { UserName = "josegeetha" });

              //  string ConnectionPath = "Server = identitycoreawsdb.cl28zty7wqij.us-east-1.rds.amazonaws.com, 1433; Database = NetCoreIdentitySample; Uid = masterUsername; Pwd = masterPassword;";
                // string ConnectionPath = "Data Source = LAPTOP - 4JER7AVV; Initial Catalog = NetCoreIdentitySample; Trusted Connection=true";
                using (var sqlCon = new SqlConnection(ConnectionPath))
                {
                    using (SqlCommand cmd = new SqlCommand("select UserName from AspNetUsers", sqlCon))
                    {
                        cmd.CommandType = CommandType.Text;
                        sqlCon.Open();

                        var list = cmd.ExecuteNonQuery();
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                lstUsers.Add(new AspNetUsers
                                {
                                    //EmployeeID = Convert.ToInt32(dr["EmployeeID"].ToString())
                                    //,
                                    UserName = dr["UserName"].ToString()
                                }

                                    );
                            }
                        }
                    }
                }
            }
            catch(Exception e)
            {
                lstUsers.Add(new AspNetUsers { UserName = e.ToString() });
            }
            return lstUsers;
        }



        //public APIGatewayProxyResponse Get()
        //{
        //    var response = new APIGatewayProxyResponse
        //    {
        //        StatusCode = (int)HttpStatusCode.OK,
        //        Body = JsonConvert.SerializeObject(GetUsers()),
        //        Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
        //    };

        //    return response;

        //}

        ///<Summary>
        /// For validate pin
        ///</Summary>
        public AspNetUserPinModel IsValidUserPin(long Pin)
        {

            //string ConnectionPath = connectionStrings.DbConnection;
            //SqlConnection conn = new SqlConnection(ConnectionPath);
            //conn.Open();

            //AspNetUserPinModel pinmodel = new AspNetUserPinModel();
            //string sql = "isValidUserPin";

            //SqlCommand comm = new SqlCommand(sql, conn);
            //comm.Parameters.AddWithValue("@Pin", Pin.ToString());
            //comm.CommandType = CommandType.StoredProcedure;
            //SqlDataReader dr = comm.ExecuteReader();
            //if (dr.HasRows)
            //{
            //    while (dr.Read())
            //    {
            //        pinmodel.LastName = dr["LastName"].ToString();
            //        pinmodel.FirstName = dr["FirstName"].ToString();
            //        pinmodel.Message = dr["Message"].ToString();
            //    }
            //}

            string ConnectionPath = "Server=identitycoreawsdb.cl28zty7wqij.us-east-1.rds.amazonaws.com;Database=NetCoreIdentitySample;Uid=masterUsername;Pwd=masterPassword";
            SqlConnection conn = new SqlConnection(ConnectionPath);
            conn.Open();

            AspNetUserPinModel pinmodel = new AspNetUserPinModel();
            string sql = "isValidUserPin";

            SqlCommand comm = new SqlCommand(sql, conn);
            comm.Parameters.AddWithValue("@Pin", Pin.ToString());
            comm.CommandType = CommandType.StoredProcedure;
            SqlDataReader dr = comm.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    string s = dr.GetString(0);
                    pinmodel.LastName = dr["LastName"].ToString();
                    pinmodel.FirstName = dr["FirstName"].ToString();
                    pinmodel.Message = dr["Message"].ToString();
                }
            }
            comm.Dispose();
            conn.Close();
            return pinmodel;

        }
    }
}
