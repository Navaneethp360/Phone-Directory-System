using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace PhoneDir.Masters
{
    public partial class SubDepartments : System.Web.UI.Page
    {
        private string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            LoadAllSubDepartments();
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
                {
                    chkDepartments.Items.Add(new ListItem(dr["DeptName"].ToString(), dr["DeptID"].ToString()));
                }
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
                {
                    ddlDepartments.Items.Add(new ListItem(dr["DeptName"].ToString(), dr["DeptID"].ToString()));
                }
            }
        }

        protected void btnAddSubDept_Click(object sender, EventArgs e)
        {
            string subDeptName = txtSubDeptName.Text.Trim();
            if (string.IsNullOrEmpty(subDeptName)) { ShowMessage("Enter subdepartment name", true); return; }

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
                        "INSERT INTO SubDeptMasters (SubDeptName, SortOrder) OUTPUT INSERTED.SubDeptID VALUES (@Name, ISNULL((SELECT MAX(SortOrder) FROM SubDeptMasters),0)+1)", conn);
                    insertCmd.Parameters.AddWithValue("@Name", subDeptName);
                    subDeptId = (int)insertCmd.ExecuteScalar();
                }
                else
                    subDeptId = (int)obj;

                foreach (var deptId in selectedDepts)
                {
                    SqlCommand mapCmd = new SqlCommand(
                        "IF NOT EXISTS (SELECT 1 FROM SubDeptDepartmentMapping WHERE SubDeptID=@SubDeptID AND DeptID=@DeptID) " +
                        "INSERT INTO SubDeptDepartmentMapping (SubDeptID, DeptID, SortOrder) VALUES (@SubDeptID,@DeptID, ISNULL((SELECT MAX(SortOrder) FROM SubDeptDepartmentMapping WHERE DeptID=@DeptID),0)+1)", conn);
                    mapCmd.Parameters.AddWithValue("@SubDeptID", subDeptId);
                    mapCmd.Parameters.AddWithValue("@DeptID", deptId);
                    mapCmd.ExecuteNonQuery();
                }
            }

            ShowMessage("SubDepartment saved successfully.");
            ClearForm();
            LoadSubDepartmentsForDept();
            LoadAllSubDepartments();
        }

        protected void ddlDepartments_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadSubDepartmentsForDept();
        }

        private void LoadSubDepartmentsForDept()
        {
            tblSubDeptsBody.Controls.Clear();
            int deptId = int.Parse(ddlDepartments.SelectedValue);
            if (deptId == 0) return;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(
                    "SELECT s.SubDeptID, s.SubDeptName FROM SubDeptDepartmentMapping m INNER JOIN SubDeptMasters s ON m.SubDeptID=s.SubDeptID WHERE m.DeptID=@DeptID ORDER BY m.SortOrder", conn);
                cmd.Parameters.AddWithValue("@DeptID", deptId);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    TableRow tr = new TableRow { Attributes = { ["data-id"] = dr["SubDeptID"].ToString() } };
                    tr.Cells.Add(new TableCell { Text = dr["SubDeptName"].ToString() });

                    TableCell actions = new TableCell();
                    Button btnRemove = new Button { Text = "Remove", CssClass = "btn" };
                    btnRemove.CommandArgument = $"{dr["SubDeptID"]},{deptId}";
                    btnRemove.Click += BtnRemove_Click;
                    actions.Controls.Add(btnRemove);

                    tr.Cells.Add(actions);
                    tblSubDeptsBody.Controls.Add(tr);
                }
            }
        }

        private void BtnRemove_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string[] args = btn.CommandArgument.Split(',');
            int subDeptId = int.Parse(args[0]);
            int deptId = int.Parse(args[1]);
            DeleteSubDept(subDeptId, deptId);
        }

        protected void btnSaveOrder_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(hfSubDeptOrder.Value)) return;
            int deptId = int.Parse(ddlDepartments.SelectedValue);
            if (deptId == 0) return;

            string[] ids = hfSubDeptOrder.Value.Split(',');
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                for (int i = 0; i < ids.Length; i++)
                {
                    SqlCommand cmd = new SqlCommand("UPDATE SubDeptDepartmentMapping SET SortOrder=@Sort WHERE SubDeptID=@SubDeptID AND DeptID=@DeptID", conn);
                    cmd.Parameters.AddWithValue("@Sort", i + 1);
                    cmd.Parameters.AddWithValue("@SubDeptID", int.Parse(ids[i]));
                    cmd.Parameters.AddWithValue("@DeptID", deptId);
                    cmd.ExecuteNonQuery();
                }
            }
            ShowMessage("Order saved successfully.");
            LoadSubDepartmentsForDept();
        }

        protected void DeleteSubDept(int subDeptId, int deptId)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(
                    "DELETE FROM SubDeptDepartmentMapping WHERE SubDeptID=@SubDeptID AND DeptID=@DeptID", conn);
                cmd.Parameters.AddWithValue("@SubDeptID", subDeptId);
                cmd.Parameters.AddWithValue("@DeptID", deptId);
                cmd.ExecuteNonQuery();
            }
            ShowMessage("SubDepartment removed from the selected department.");
            LoadSubDepartmentsForDept();
        }

        protected void DeleteSubDeptAll(int subDeptId)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(
                    "DELETE FROM SubDeptDepartmentMapping WHERE SubDeptID=@SubDeptID;" +
                    "DELETE FROM SubDeptMasters WHERE SubDeptID=@SubDeptID;", conn);
                cmd.Parameters.AddWithValue("@SubDeptID", subDeptId);
                cmd.ExecuteNonQuery();
            }
            ShowMessage("SubDepartment deleted from all departments.");
            LoadSubDepartmentsForDept();
            LoadAllSubDepartments();
        }

        protected void btnDeleteAll_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int subDeptId = int.Parse(btn.CommandArgument);
            DeleteSubDeptAll(subDeptId);
        }

        private void LoadAllSubDepartments()
        {
            tblAllSubDeptsBody.Rows.Clear();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT SubDeptID, SubDeptName FROM SubDeptMasters ORDER BY SortOrder", conn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    TableRow tr = new TableRow();

                    TableCell cellName = new TableCell { Text = dr["SubDeptName"].ToString() };
                    tr.Cells.Add(cellName);

                    TableCell cellActions = new TableCell();
                    Button btnDeleteAll = new Button { Text = "Delete All", CssClass = "btn" };
                    btnDeleteAll.CommandArgument = dr["SubDeptID"].ToString();
                    btnDeleteAll.Click += btnDeleteAll_Click;
                    cellActions.Controls.Add(btnDeleteAll);

                    tr.Cells.Add(cellActions);
                    tblAllSubDeptsBody.Rows.Add(tr);
                }
            }
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
