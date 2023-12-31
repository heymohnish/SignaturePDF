﻿using SignaturePDF.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using System.Web.UI;
using System.Runtime.InteropServices.ComTypes;

namespace SignaturePDF.Repository
{
    public class LoginRepository
    {
        private string connectionString = "Data Source=SYSLP616;Initial Catalog=TeamTask;Integrated Security=True;";
        public Registration GetUser(Models.Login login)
        {
            Registration registration = new Registration();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("sp_GeUser", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@userName", login.UserName);
                    command.Parameters.AddWithValue("@password", login.Password);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            registration.Id = (int)reader["id"];
                            registration.UserName = reader["userName"].ToString();
                            registration.PassWord = reader["password"].ToString();
                            registration.Name = reader["name"].ToString();
                 
                        }
                    }
                }
            }

            return registration;
        }
        public List<Document> GetAllDocumentsByUser(int userId)
        {
            List<Document> documents = new List<Document>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("sp_GetAllDocByUser", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@userId_Fk", userId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Document document = new Document
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                Name = reader["documentName"].ToString(),
                                UserId = Convert.ToInt32(reader["userId_Fk"]),
                                Status = (Status)Convert.ToInt32(reader["status"]),
                                Documents = (byte[])reader["document"]
                            };

                            documents.Add(document);
                        }
                    }
                }
            }
            return documents;
        }
        public int uploadDocuments(Document document)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("sp_InsertDocument", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@documentName", document.Name);
                    command.Parameters.AddWithValue("@document", document.Documents);
                    command.Parameters.AddWithValue("@status", document.Status);
                    command.Parameters.AddWithValue("@userId_Fk",document.UserId );
                    SqlParameter generatedIdParameter = new SqlParameter("@generatedId", SqlDbType.Int);
                    generatedIdParameter.Direction = ParameterDirection.Output;
                    command.Parameters.Add(generatedIdParameter);
                    command.ExecuteNonQuery();

                    // Get the generated ID from the output parameter
                    int generatedId = Convert.ToInt32(generatedIdParameter.Value);
                    return generatedId;

                }
            }
        }

        public void AppenDocSignDetails(DocSign signs)
        {
            string fieldsPagesString = string.Join(",", signs.FieldsPages);
            string Top = string.Join(",", signs.Top);
            string Right = string.Join(",", signs.Right);
            string Bottom = string.Join(",", signs.Bottom);
            string Left = string.Join(",", signs.Left);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("InsertDocSign", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@DocId", signs.DocId);
                    command.Parameters.AddWithValue("@UserId", signs.UserId);
                    command.Parameters.AddWithValue("@TotalFields", signs.TotalFields);
                    command.Parameters.AddWithValue("@FieldsPages", fieldsPagesString);
                    command.Parameters.AddWithValue("@Toppx", Top);
                    command.Parameters.AddWithValue("@Rightpx", Right);
                    command.Parameters.AddWithValue("@Bottompx", Bottom);
                    command.Parameters.AddWithValue("@Leftpx", Left);

                    command.ExecuteNonQuery();
                }
            }

            Console.WriteLine("Data inserted successfully!");
        }
        public void UpdateDocSignDetails(DocSign signs)
        {
            string fieldsPagesString = string.Join(",", signs.FieldsPages);
            string Top = string.Join(",", signs.Top);
            string Right = string.Join(",", signs.Right);
            string Bottom = string.Join(",", signs.Bottom);
            string Left = string.Join(",", signs.Left);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("UpdateDoc", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@DocId", signs.DocId);
                    command.Parameters.AddWithValue("@TotalFields", signs.TotalFields);
                    command.Parameters.AddWithValue("@FieldsPages", fieldsPagesString);
                    command.Parameters.AddWithValue("@Toppx", Top);
                    command.Parameters.AddWithValue("@Rightpx", Right);
                    command.Parameters.AddWithValue("@Bottompx", Bottom);
                    command.Parameters.AddWithValue("@Leftpx", Left);

                    command.ExecuteNonQuery();
                }
            }

            Console.WriteLine("Data inserted successfully!");
        }

        public DocSign GetSignValue(int id)
        {
              using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("GetDocSignById", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add the parameter for the stored procedure
                        command.Parameters.AddWithValue("@DocSignId", id);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new DocSign
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    DocId = reader.GetInt32(reader.GetOrdinal("DocId")),
                                    UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                                    TotalFields = reader.GetInt32(reader.GetOrdinal("TotalFields")),
                                    FieldsPages = DeserializeJsonList<int>(reader.GetString(reader.GetOrdinal("FieldsPages"))),
                                    Top = DeserializeJsonList<int>(reader.GetString(reader.GetOrdinal("Toppx"))),
                                    Right = DeserializeJsonList<int>(reader.GetString(reader.GetOrdinal("Rightpx"))),
                                    Bottom = DeserializeJsonList<int>(reader.GetString(reader.GetOrdinal("Bottompx"))),
                                    Left = DeserializeJsonList<int>(reader.GetString(reader.GetOrdinal("Leftpx")))
                                };
                            }
                        }
                    }

                  return null;
                }
            
        }

        private List<int> DeserializeJsonList<T>(string json)
        {
                return json.Split(',').Select(int.Parse).ToList();
          
        }

        public void saveDatew(SignPosition inputField)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("InsertDataDoc", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Add parameters
                    command.Parameters.AddWithValue("@PageId", inputField.id);
                    command.Parameters.AddWithValue("@insetTop", inputField.insetTop);
                    command.Parameters.AddWithValue("@insetRight", inputField.insetRight);
                    command.Parameters.AddWithValue("@insetBottom", inputField.insetBottom);
                    command.Parameters.AddWithValue("@insetLeft", inputField.insetLeft);
                    command.Parameters.AddWithValue("@DocId", inputField.DocId);
                    command.Parameters.AddWithValue("@Bytes", inputField.Bytes);

                    // Execute the stored procedure
                    command.ExecuteNonQuery();

                    Console.WriteLine("Data inserted successfully.");
                }
            }
        }

        public List<SignPosition> GetAllDataByDocId(int docId)
        {
            List<SignPosition> data = new List<SignPosition>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("GetAllDataByDocId", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Add parameters
                    command.Parameters.Add("@DocId", SqlDbType.Int).Value = docId;

                    try
                    {
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                SignPosition item = new SignPosition
                                {
                                    SignId = Convert.ToInt32(reader["ID"]),
                                    id = Convert.ToInt32(reader["PageId"]),
                                    insetTop = Convert.ToInt32(reader["insetTop"]),
                                    insetRight = Convert.ToInt32(reader["insetRight"]),
                                    insetBottom = Convert.ToInt32(reader["insetBottom"]),
                                    insetLeft = Convert.ToInt32(reader["insetLeft"]),
                                    DocId = Convert.ToInt32(reader["DocId"]),
                                    Bytes = reader["Bytes"] == DBNull.Value ? null : (byte[])reader["Bytes"]
                                };

                                data.Add(item);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Handle exceptions as needed
                        Console.WriteLine(ex.Message);
                    }
                }
            }

            return data;
        }
    }
}