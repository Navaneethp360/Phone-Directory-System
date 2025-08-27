using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace PhoneDir.Masters
{
    public partial class SubDepartments : System.Web.UI.Page
    {
        private string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Init(object sender, EventArgs e)
        {
            if (ddlDepartments.SelectedIndex > 0)
                LoadSubDeptsForDepartment();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            LoadAllSubDepts();
            if (!IsPostBack)
            {
                BindDepartmentsCheckList();
                BindDepartmentDropdown();
            }
        }

        private void BindDepartmentsCheckList()
        {
            chkDepartments.Items.Clear();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT DeptID, DeptName FROM DeptMasters ORDER BY SortOrder", conn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                    chkDepartments.Items.Add(new ListItem(dr["DeptName"].ToString(), dr["DeptID"].ToString()));
            }
        }

        private void BindDepartmentDropdown()
        {
            ddlDepartments.Items.Clear();
            ddlDepartments.Items.Add(new ListItem("--Select Department--", "0"));
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT DeptID, DeptName FROM DeptMasters ORDER BY SortOrder", conn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                    ddlDepartments.Items.Add(new ListItem(dr["DeptName"].ToString(), dr["DeptID"].ToString()));
            }
        }

        protected void btnAddSubDept_Click(object sender, EventArgs e)
        {
            string subDeptName = txtSubDeptName.Text.Trim();
            if (string.IsNullOrEmpty(subDeptName)) { ShowMessage("Enter sub-department name", true); return; }

            List<int> selectedDepts = new List<int>();
            foreach (ListItem li in chkDepartments.Items)
                if (li.Selected) selectedDepts.Add(int.Parse(li.Value));

            if (selectedDepts.Count == 0) { ShowMessage("Select at least one department", true); return; }

            int subDeptId = 0;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand checkCmd = new SqlCommand("SELECT SubDeptID FROM SubDeptMasters WHERE SubDeptName=@Name", conn);
                checkCmd.Parameters.AddWithValue("@Name", subDeptName);
                var obj = checkCmd.ExecuteScalar();
                if (obj == null)
                {
                    SqlCommand insertCmd = new SqlCommand(
                        "INSERT INTO SubDeptMasters (SubDeptName, SortOrder) OUTPUT INSERTED.SubDeptID " +
                        "VALUES (@Name, ISNULL((SELECT MAX(SortOrder) FROM SubDeptMasters),0)+1)", conn);
                    insertCmd.Parameters.AddWithValue("@Name", subDeptName);
                    subDeptId = (int)insertCmd.ExecuteScalar();
                }
                else
                    subDeptId = (int)obj;

                foreach (var deptId in selectedDepts)
                {
                    SqlCommand mapCmd = new SqlCommand(
                        "IF NOT EXISTS (SELECT 1 FROM SubDeptDepartmentMapping WHERE SubDeptID=@SubDeptID AND DeptID=@DeptID) " +
                        "INSERT INTO SubDeptDepartmentMapping (SubDeptID, DeptID, SortOrder) " +
                        "VALUES (@SubDeptID, @DeptID, ISNULL((SELECT MAX(SortOrder) FROM SubDeptDepartmentMapping WHERE DeptID=@DeptID),0)+1)", conn);
                    mapCmd.Parameters.AddWithValue("@SubDeptID", subDeptId);
                    mapCmd.Parameters.AddWithValue("@DeptID", deptId);
                    mapCmd.ExecuteNonQuery();
                }
            }

            ShowMessage("Sub-department saved successfully.");
            ClearForm();
            LoadSubDeptsForDepartment();
            LoadAllSubDepts();
        }

        protected void ddlDepartments_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadSubDeptsForDepartment();
        }

        private void LoadSubDeptsForDepartment()
        {
            gvSubDepts.DataSource = null;
            gvSubDepts.DataBind();

            if (!int.TryParse(ddlDepartments.SelectedValue, out int deptId) || deptId == 0)
                return;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(
                    "SELECT s.SubDeptID, s.SubDeptName FROM SubDeptDepartmentMapping m " +
                    "INNER JOIN SubDeptMasters s ON m.SubDeptID=s.SubDeptID " +
                    "WHERE m.DeptID=@DeptID ORDER BY m.SortOrder", conn);
                cmd.Parameters.AddWithValue("@DeptID", deptId);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gvSubDepts.DataSource = dt;
                gvSubDepts.DataBind();
            }
        }

        protected void gvSubDepts_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes["data-id"] = gvSubDepts.DataKeys[e.Row.RowIndex].Value.ToString();
            }
        }

        protected void gvSubDepts_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "DeleteSubDept") return;
            if (!int.TryParse(e.CommandArgument.ToString(), out int subDeptId)) return;
            if (!int.TryParse(ddlDepartments.SelectedValue, out int deptId) || deptId == 0)
            {
                ShowMessage("Select a department first", true);
                return;
            }

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(
                    "DELETE FROM SubDeptDepartmentMapping WHERE SubDeptID=@SubDeptID AND DeptID=@DeptID", conn);
                cmd.Parameters.AddWithValue("@SubDeptID", subDeptId);
                cmd.Parameters.AddWithValue("@DeptID", deptId);
                int rows = cmd.ExecuteNonQuery();
                if (rows == 0)
                {
                    ShowMessage("Nothing was deleted. Mapping may not exist.", true);
                    return;
                }
            }

            ShowMessage("Sub-department removed from the selected department.");
            LoadSubDeptsForDepartment();
        }

        protected void btnSaveOrder_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(hfSubDeptOrder.Value)) return;
            if (!int.TryParse(ddlDepartments.SelectedValue, out int deptId) || deptId == 0) return;

            string[] ids = hfSubDeptOrder.Value.Split(',');
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                for (int i = 0; i < ids.Length; i++)
                {
                    SqlCommand cmd = new SqlCommand(
                        "UPDATE SubDeptDepartmentMapping SET SortOrder=@Sort WHERE SubDeptID=@SubDeptID AND DeptID=@DeptID", conn);
                    cmd.Parameters.AddWithValue("@Sort", i + 1);
                    cmd.Parameters.AddWithValue("@SubDeptID", int.Parse(ids[i]));
                    cmd.Parameters.AddWithValue("@DeptID", deptId);
                    cmd.ExecuteNonQuery();
                }
            }
            ShowMessage("Order saved successfully.");
            LoadSubDeptsForDepartment();
        }

        private void LoadAllSubDepts()
        {
            tblAllSubDeptsBody.Rows.Clear();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT SubDeptID, SubDeptName FROM SubDeptMasters ORDER BY SubDeptName", conn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    TableRow tr = new TableRow();
                    tr.Cells.Add(new TableCell { Text = dr["SubDeptName"].ToString() });

                    TableCell actions = new TableCell();
                    Button btnDeleteAll = new Button { Text = "Delete All", CssClass = "btn" };
                    btnDeleteAll.CommandArgument = dr["SubDeptID"].ToString();
                    btnDeleteAll.Click += BtnDeleteAll_Click;
                    actions.Controls.Add(btnDeleteAll);

                    tr.Cells.Add(actions);
                    tblAllSubDeptsBody.Rows.Add(tr);
                }
            }
        }

        private void BtnDeleteAll_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            if (!int.TryParse(btn.CommandArgument, out int subDeptId)) return;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmdMap = new SqlCommand("DELETE FROM SubDeptDepartmentMapping WHERE SubDeptID=@SubDeptID", conn);
                cmdMap.Parameters.AddWithValue("@SubDeptID", subDeptId);
                cmdMap.ExecuteNonQuery();

                SqlCommand cmdDept = new SqlCommand("DELETE FROM SubDeptMasters WHERE SubDeptID=@SubDeptID", conn);
                cmdDept.Parameters.AddWithValue("@SubDeptID", subDeptId);
                cmdDept.ExecuteNonQuery();
            }

            ShowMessage("Sub-department deleted completely.");
            LoadSubDeptsForDepartment();
            LoadAllSubDepts();
        }

        private void ShowMessage(string msg, bool isError = false)
        {
            lblMessage.Text = msg;
            lblMessage.CssClass = isError ? "status-message error" : "status-message";
            lblMessage.Visible = true;
        }

        private void ClearForm()
        {
            txtSubDeptName.Text = "";
            foreach (ListItem li in chkDepartments.Items) li.Selected = false;
        }
    }
}
