using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Data;
using WebAPIPractice.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace WebAPIPractice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        public EmployeeController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        [HttpGet]
        public JsonResult Get()
        {                   // Raw Queries for simplicity: better to use stored procedures w/params
            string query = @"       
                            select EmployeeId, EmployeeName, Department,
                            convert(varchar(10), DateOfJoining,120) as DateOfJoining,PhotoFileName
                            from dbo.Employee";
            DataTable table = new DataTable();
            // Stores Database connection string
            string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);

                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult(table);
        }

        [HttpPost]
        public JsonResult Post(Employee emp)
        {
            string query = @"       
                            insert into dbo.Employee 
                            (EmployeeName, Department,DateOfJoining,PhotoFileName)
                            values
                            (
                            '" + emp.EmployeeName + @"',
                            '" + emp.Department + @"',
                            '" + emp.DateOfJoining + @"',
                            '" + emp.PhotoFileName + @"'
                            )
                            ";  
            DataTable table = new DataTable();
            // Stores Database connection string
            string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);

                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult("Added Successfully");
        }

        [HttpPut]
        public JsonResult Put(Employee emp)
        {
            {
                string query = @"       
                            update dbo.Employee set 
                            EmployeeName = '" + emp.EmployeeName + @"',
                            Department = '" + emp.Department + @"',
                            DateOfJoining = '" + emp.DateOfJoining + @"',
                            PhotoFileName = '" + emp.PhotoFileName + @"'
                            
                            where EmployeeId = " + emp.EmployeeId + @"
                            ";
                DataTable table = new DataTable();
                // Stores Database connection string
                string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
                SqlDataReader myReader;
                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    myCon.Open();
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myReader = myCommand.ExecuteReader();
                        table.Load(myReader);

                        myReader.Close();
                        myCon.Close();
                    }
                }

                return new JsonResult("Updated Successfully");
            }
        }

        [HttpDelete("{id}")]
        public JsonResult Delete(int id)
        {
            {
                string query = @"       
                            delete from dbo.Employee 
                            where EmployeeId = " + id + @"
                            ";
                DataTable table = new DataTable();
                // Stores Database connection string
                string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
                SqlDataReader myReader;
                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    myCon.Open();
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myReader = myCommand.ExecuteReader();
                        table.Load(myReader);

                        myReader.Close();
                        myCon.Close();
                    }
                }

                return new JsonResult("Deleted Successfully");
            }
        }
        
        [Route("SaveFile")]
        [HttpPost]
        public JsonResult SaveFile()
        {
            try
            {
                var httpRequest = Request.Form;
                var postedFile = httpRequest.Files[0];
                string fileName = postedFile.FileName;
                var physicalPath = _env.ContentRootPath + "/Photos/" + fileName;


                using (var stream = new FileStream(physicalPath, FileMode.Create))
                {
                    postedFile.CopyTo(stream);  // save Photo
                }

                return new JsonResult(fileName);
            }

            catch (Exception)
            {

                return new JsonResult("anonymous.png");
            }
        }

        // CONINUE HERE, 36:18

    }
}
