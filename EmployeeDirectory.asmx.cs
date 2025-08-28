using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.Script.Services;
using System.Web.Services;
using System.Collections.Generic;

namespace PhoneDir
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [ScriptService] // allows JSON responses
    public class EmployeeDirectory : System.Web.Services.WebService
    {
        private string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public class Employee
        {
            public int EmployeePK;
            public string EmpID;
            public string Name;
            public string Designation;
            public string Extension;
            public string Mobile;
            public string Location;
            public string SubDept;
        }

        public class Department
        {
            public int DeptID;
            public string DeptName;
            public List<Employee> Employees = new List<Employee>();
        }

        public class Company
        {
            public int OrgID;
            public string OrgName;
            public List<Department> Departments = new List<Department>();
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public List<Company> GetDirectory()
        {
            List<Company> companies = new List<Company>();

            // Get active companies
            DataTable dtCompanies = GetDataTable("SELECT OrgID, OrgName FROM OrgMasters WHERE IsActive=1 ORDER BY SortOrder, OrgName");

            // Get department mappings
            DataTable dtDeptMap = GetDataTable("SELECT DeptID, OrgID, SortOrder FROM DeptCompanyMapping ORDER BY SortOrder");

            // Get all departments
            DataTable dtDepts = GetDataTable("SELECT DeptID, DeptName FROM DeptMasters ORDER BY SortOrder");

            // Get employees linked to departments
            DataTable dtLinks = GetDataTable(@"
                SELECT l.LinkID, l.EmployeePK, l.DeptID, l.SortOrder AS EmpSortOrder, 
                       e.Name, e.EmpID, e.Designation, e.Extension, e.Mobile, e.Location, e.SubDept
                FROM EmployeeDeptLink l
                INNER JOIN Employees e ON l.EmployeePK = e.EmployeePK
                ORDER BY l.SortOrder, e.Name
            ");

            foreach (DataRow comp in dtCompanies.Rows)
            {
                Company c = new Company { OrgID = (int)comp["OrgID"], OrgName = comp["OrgName"].ToString() };

                // Get all dept mappings for this company
                DataRow[] mappedDepts = dtDeptMap.Select("OrgID=" + c.OrgID);

                foreach (DataRow map in mappedDepts)
                {
                    int deptID = (int)map["DeptID"];
                    DataRow deptRow = dtDepts.Select("DeptID=" + deptID)[0];

                    Department d = new Department
                    {
                        DeptID = deptID,
                        DeptName = deptRow["DeptName"].ToString()
                    };

                    // Get employees for this dept (can be empty)
                    DataRow[] empRows = dtLinks.Select("DeptID=" + deptID, "EmpSortOrder ASC");
                    foreach (DataRow empRow in empRows)
                    {
                        Employee e = new Employee
                        {
                            EmployeePK = (int)empRow["EmployeePK"],
                            EmpID = empRow["EmpID"].ToString(),
                            Name = empRow["Name"].ToString(),
                            Designation = empRow["Designation"].ToString(),
                            Extension = empRow["Extension"]?.ToString(),
                            Mobile = empRow["Mobile"]?.ToString(),
                            Location = empRow["Location"]?.ToString(),
                            SubDept = empRow["SubDept"]?.ToString()
                        };
                        d.Employees.Add(e);
                    }

                    c.Departments.Add(d);
                }

                companies.Add(c);
            }

            return companies;
        }

        private DataTable GetDataTable(string sql)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                da.Fill(dt);
            return dt;
        }
    }
}
