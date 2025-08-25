using System;
using System.Collections.Generic;
using System.Web.Services;
using System.Web.Script.Services;

namespace MedicalSystem
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [ScriptService] // allows JSON calls
    public class WebService1 : WebService
    {
        private static Random rnd = new Random();

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public List<Company> GetDirectory()
        {
            return new List<Company>
            {
                new Company { Name="HEISCO", Departments=GenerateDepartments(new string[] { "IT","HR","Finance" }) },
                new Company { Name="GlobalTech", Departments=GenerateDepartments(new string[] { "Operations","Sales","Support" }) },
                new Company { Name="Innovatech", Departments=GenerateDepartments(new string[] { "R&D","Marketing","Customer Care" }) }
            };
        }

        private List<Department> GenerateDepartments(string[] deptNames)
        {
            var depts = new List<Department>();
            foreach (var name in deptNames)
            {
                depts.Add(new Department { Name = name, Employees = GenerateEmployees(name, 10) });
            }
            return depts;
        }

        private List<Employee> GenerateEmployees(string dept, int count)
        {
            var list = new List<Employee>();
            for (int i = 1; i <= count; i++)
            {
                list.Add(new Employee
                {
                    Name = $"{dept} Employee {i}",
                    Designation = $"{dept} Designation {i}",
                    Extension = rnd.Next(100, 999).ToString(),
                    Mobile = $"555-{rnd.Next(1000, 9999)}"
                });
            }
            return list;
        }
    }

    public class Company
    {
        public string Name { get; set; }
        public List<Department> Departments { get; set; }
    }

    public class Department
    {
        public string Name { get; set; }
        public List<Employee> Employees { get; set; }
    }

    public class Employee
    {
        public string Name { get; set; }
        public string Designation { get; set; }
        public string Extension { get; set; }
        public string Mobile { get; set; }
    }
}
