using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PhoneDir.Masters
{
    public partial class Employees : System.Web.UI.Page
    {
        private string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Init(object sender, EventArgs e)
        {
            // Always bind departments dynamically early in the lifecycle
            BindDepartments();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ClearForm();
            }
        }

        private void BindDepartments()
        {
            phDepartments.Controls.Clear();
            DataTable dt = GetDataTable("SELECT DeptID, DeptName FROM DeptMasters ORDER BY SortOrder, DeptName");
            foreach (DataRow dr in dt.Rows)
            {
                CheckBox cb = new CheckBox
                {
                    ID = "dept_" + dr["DeptID"],
                    Text = dr["DeptName"].ToString(),
                    CssClass = "tree-node"
                };
                phDepartments.Controls.Add(cb);
                phDepartments.Controls.Add(new Literal { Text = "<br/>" });
            }
        }

        protected void btnSaveEmployee_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtDesignation.Text.Trim()))
            {
                ShowMessage("Designation is required.", true);
                return;
            }

            int empPK = 0;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();
                try
                {
                    // Insert or update employee
                    if (string.IsNullOrEmpty(hfEmployeePK.Value))
                    {
                        SqlCommand cmd = new SqlCommand(@"
                            INSERT INTO Employees(EmpID, Name, Designation, Extension, Mobile, Location, SubDept)
                            OUTPUT INSERTED.EmployeePK
                            VALUES(@EmpID, @Name, @Desig, @Ext, @Mobile, @Loc, @SubDept)", conn, tran);

                        cmd.Parameters.AddWithValue("@EmpID", string.IsNullOrEmpty(txtEmpID.Text) ? (object)DBNull.Value : txtEmpID.Text);
                        cmd.Parameters.AddWithValue("@Name", string.IsNullOrEmpty(txtName.Text) ? (object)DBNull.Value : txtName.Text);
                        cmd.Parameters.AddWithValue("@Desig", txtDesignation.Text.Trim());
                        cmd.Parameters.AddWithValue("@Ext", string.IsNullOrEmpty(txtExtension.Text) ? (object)DBNull.Value : txtExtension.Text);
                        cmd.Parameters.AddWithValue("@Mobile", string.IsNullOrEmpty(txtMobile.Text) ? (object)DBNull.Value : txtMobile.Text);
                        cmd.Parameters.AddWithValue("@Loc", string.IsNullOrEmpty(txtLocation.Text) ? (object)DBNull.Value : txtLocation.Text);
                        cmd.Parameters.AddWithValue("@SubDept", string.IsNullOrEmpty(txtSubDept.Text) ? (object)DBNull.Value : txtSubDept.Text);

                        empPK = (int)cmd.ExecuteScalar();
                    }
                    else
                    {
                        empPK = Convert.ToInt32(hfEmployeePK.Value);
                        SqlCommand cmd = new SqlCommand(@"
                            UPDATE Employees
                            SET EmpID=@EmpID, Name=@Name, Designation=@Desig, Extension=@Ext,
                                Mobile=@Mobile, Location=@Loc, SubDept=@SubDept
                            WHERE EmployeePK=@EmpPK", conn, tran);

                        cmd.Parameters.AddWithValue("@EmpID", string.IsNullOrEmpty(txtEmpID.Text) ? (object)DBNull.Value : txtEmpID.Text);
                        cmd.Parameters.AddWithValue("@Name", string.IsNullOrEmpty(txtName.Text) ? (object)DBNull.Value : txtName.Text);
                        cmd.Parameters.AddWithValue("@Desig", txtDesignation.Text.Trim());
                        cmd.Parameters.AddWithValue("@Ext", string.IsNullOrEmpty(txtExtension.Text) ? (object)DBNull.Value : txtExtension.Text);
                        cmd.Parameters.AddWithValue("@Mobile", string.IsNullOrEmpty(txtMobile.Text) ? (object)DBNull.Value : txtMobile.Text);
                        cmd.Parameters.AddWithValue("@Loc", string.IsNullOrEmpty(txtLocation.Text) ? (object)DBNull.Value : txtLocation.Text);
                        cmd.Parameters.AddWithValue("@SubDept", string.IsNullOrEmpty(txtSubDept.Text) ? (object)DBNull.Value : txtSubDept.Text);
                        cmd.Parameters.AddWithValue("@EmpPK", empPK);
                        cmd.ExecuteNonQuery();

                        // Delete previous department links
                        SqlCommand cmdDel = new SqlCommand("DELETE FROM EmployeeDeptLink WHERE EmployeePK=@EmpPK", conn, tran);
                        cmdDel.Parameters.AddWithValue("@EmpPK", empPK);
                        cmdDel.ExecuteNonQuery();
                    }

                    // Insert department links
                    foreach (Control ctrl in phDepartments.Controls)
                    {
                        if (ctrl is CheckBox cb && cb.Checked)
                        {
                            string idPart = cb.ID.Replace("dept_", "");
                            if (int.TryParse(idPart, out int deptID))
                            {
                                SqlCommand cmdLink = new SqlCommand(@"
                                    INSERT INTO EmployeeDeptLink(EmployeePK, DeptID, SortOrder)
                                    VALUES(@EmpPK, @DeptID, 0)", conn, tran);

                                cmdLink.Parameters.AddWithValue("@EmpPK", empPK);
                                cmdLink.Parameters.AddWithValue("@DeptID", deptID);
                                cmdLink.ExecuteNonQuery();
                            }
                        }
                    }

                    tran.Commit();
                    ClearForm();
                    ShowMessage("Employee saved successfully!");
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    ShowMessage("Error saving employee: " + ex.Message, true);
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

        private void ClearForm()
        {
            txtEmpID.Text = txtName.Text = txtDesignation.Text = txtExtension.Text = txtMobile.Text = txtLocation.Text = txtSubDept.Text = "";
            hfEmployeePK.Value = "";
            foreach (Control ctrl in phDepartments.Controls)
            {
                if (ctrl is CheckBox cb) cb.Checked = false;
            }
        }
        #endregion
    }
}
