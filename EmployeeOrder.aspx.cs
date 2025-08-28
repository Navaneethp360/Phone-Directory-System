using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PhoneDir.Masters
{
    public partial class EmployeeOrder : System.Web.UI.Page
    {
        private string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                BindDepartments();

            if (ddlDepartment.SelectedValue != "0")
                BindEmployees(Convert.ToInt32(ddlDepartment.SelectedValue));

            // Handle delete postback
            string eventArg = Request["__EVENTARGUMENT"];
            if (!string.IsNullOrEmpty(eventArg) && eventArg.StartsWith("delete$"))
            {
                int empPK = Convert.ToInt32(eventArg.Split('$')[1]);
                DeleteEmployee(empPK);
                BindEmployees(Convert.ToInt32(ddlDepartment.SelectedValue));
                ShowMessage("Employee deleted successfully!");
            }
        }

        private void BindDepartments()
        {
            ddlDepartment.Items.Clear();
            ddlDepartment.Items.Add(new ListItem("--Select Department--", "0"));
            DataTable dt = GetDataTable("SELECT DeptID, DeptName FROM DeptMasters ORDER BY SortOrder, DeptName");
            foreach (DataRow dr in dt.Rows)
                ddlDepartment.Items.Add(new ListItem(dr["DeptName"].ToString(), dr["DeptID"].ToString()));
        }

        protected void ddlDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            int deptID = Convert.ToInt32(ddlDepartment.SelectedValue);
            BindEmployees(deptID);
        }

        private void BindEmployees(int deptID)
        {
            tblEmployeesBody.Controls.Clear();
            if (deptID == 0) return;

            string sql = @"SELECT l.LinkID, e.EmployeePK, e.EmpID, e.Name, e.Designation, e.Location, e.SubDept
                           FROM EmployeeDeptLink l
                           INNER JOIN Employees e ON l.EmployeePK = e.EmployeePK
                           WHERE l.DeptID=@DeptID
                           ORDER BY l.SortOrder, e.Name";

            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@DeptID", deptID);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    da.Fill(dt);
            }

            foreach (DataRow dr in dt.Rows)
            {
                TableRow tr = new TableRow();
                tr.Attributes["data-empid"] = dr["EmployeePK"].ToString();
                tr.Cells.Add(new TableCell { Text = dr["Name"].ToString() });
                tr.Cells.Add(new TableCell { Text = dr["EmpID"].ToString() });
                tr.Cells.Add(new TableCell { Text = dr["Designation"].ToString() });
                tr.Cells.Add(new TableCell { Text = dr["Location"].ToString() });
                tr.Cells.Add(new TableCell { Text = dr["SubDept"].ToString() });

                TableCell actionCell = new TableCell();
                actionCell.Text = $"<button type='button' class='btn btn-delete' data-empid='{dr["EmployeePK"]}'>Delete</button>";
                tr.Cells.Add(actionCell);

                tblEmployeesBody.Controls.Add(tr);
            }
        }

        protected void btnSaveOrder_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(hfEmpOrder.Value)) return;
            string[] ids = hfEmpOrder.Value.Split(',');

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                for (int i = 0; i < ids.Length; i++)
                {
                    SqlCommand cmd = new SqlCommand("UPDATE EmployeeDeptLink SET SortOrder=@Sort WHERE EmployeePK=@EmpPK AND DeptID=@DeptID", conn);
                    cmd.Parameters.AddWithValue("@Sort", i + 1);
                    cmd.Parameters.AddWithValue("@EmpPK", Convert.ToInt32(ids[i]));
                    cmd.Parameters.AddWithValue("@DeptID", Convert.ToInt32(ddlDepartment.SelectedValue));
                    cmd.ExecuteNonQuery();
                }
            }
            BindEmployees(Convert.ToInt32(ddlDepartment.SelectedValue));
            ShowMessage("Order saved successfully!");
        }

        private void DeleteEmployee(int empPK)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();
                try
                {
                    SqlCommand cmdDelLinks = new SqlCommand("DELETE FROM EmployeeDeptLink WHERE EmployeePK=@EmpPK", conn, tran);
                    cmdDelLinks.Parameters.AddWithValue("@EmpPK", empPK);
                    cmdDelLinks.ExecuteNonQuery();

                    SqlCommand cmdDelEmp = new SqlCommand("DELETE FROM Employees WHERE EmployeePK=@EmpPK", conn, tran);
                    cmdDelEmp.Parameters.AddWithValue("@EmpPK", empPK);
                    cmdDelEmp.ExecuteNonQuery();

                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                    ShowMessage("Error deleting employee.", true);
                }
            }
        }

        #region Helpers
        private DataTable GetDataTable(string sql)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                da.Fill(dt);
            return dt;
        }

        private void ShowMessage(string msg, bool isError = false)
        {
            lblMessage.Text = msg;
            lblMessage.CssClass = isError ? "status-message error" : "status-message";
            lblMessage.Visible = true;
        }
        #endregion
    }
}
