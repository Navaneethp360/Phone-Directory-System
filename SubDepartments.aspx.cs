using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace PhoneDir.Masters
{
    public partial class SubDepartments : System.Web.UI.Page
    {
        private string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            BindDepartments();
            BindGrid();
        }

        private void BindDepartments()
        {
            departmentCheckboxes.Controls.Clear();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT DeptID, DeptName FROM DeptMasters ORDER BY SortOrder", conn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    CheckBox chk = new CheckBox { ID = "chk_" + dr["DeptID"], Text = dr["DeptName"].ToString(), InputAttributes = { ["value"] = dr["DeptID"].ToString() } };
                    departmentCheckboxes.Controls.Add(chk);
                }
            }
        }

        private void BindGrid()
        {
            subDeptTableBody.Controls.Clear();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string sql = @"
            SELECT d.DeptID, d.DeptName,
       s.SubDeptID, s.SubDeptName,
       m.SortOrder
FROM SubDeptDepartmentMapping m
INNER JOIN SubDeptMasters s ON m.SubDeptID = s.SubDeptID
INNER JOIN DeptMasters d ON m.DeptID = d.DeptID
ORDER BY d.SortOrder, m.SortOrder";

                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                foreach (DataRow dr in dt.Rows)
                {
                    TableRow tr = new TableRow();
                    tr.Attributes["data-id"] = dr["SubDeptID"].ToString();

                    TableCell tcName = new TableCell { Text = dr["SubDeptName"].ToString() };
                    tr.Cells.Add(tcName);

                    TableCell tcDepartments = new TableCell { Text = dr["Departments"].ToString() };
                    tr.Cells.Add(tcDepartments);

                    TableCell tcActions = new TableCell();

                    // Edit button (client-side, no server click)
                    Button btnEdit = new Button { Text = "Edit", CssClass = "btn", ID = "btnEdit_" + dr["SubDeptID"] };
                    var ids = GetDepartmentIDs(Convert.ToInt32(dr["SubDeptID"]));
                    btnEdit.Attributes["onclick"] = $"editSubDept({dr["SubDeptID"]}, '{dr["SubDeptName"]}', [{string.Join(",", ids)}]); return false;";
                    tcActions.Controls.Add(btnEdit);

                    tcActions.Controls.Add(new LiteralControl("&nbsp;"));

                    // Delete button (server-side)
                    Button btnDelete = new Button
                    {
                        Text = "Delete",
                        CssClass = "btn",
                        CommandArgument = dr["SubDeptID"].ToString(),
                        ID = "btnDelete_" + dr["SubDeptID"]
                    };
                    btnDelete.Click += BtnDelete_Click;
                    tcActions.Controls.Add(btnDelete);

                    tr.Cells.Add(tcActions);
                    subDeptTableBody.Controls.Add(tr);
                }
            }
        }

        private List<int> GetDepartmentIDs(int subDeptId)
        {
            List<int> list = new List<int>();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT DeptID FROM SubDeptDepartmentMapping WHERE SubDeptID=@SubDeptID", conn);
                cmd.Parameters.AddWithValue("@SubDeptID", subDeptId);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read()) list.Add(Convert.ToInt32(dr["DeptID"]));
            }
            return list;
        }

        protected void btnSaveSubDept_Click(object sender, EventArgs e)
        {
            string name = txtSubDeptName.Text.Trim();
            if (string.IsNullOrEmpty(name)) { ShowMessage("SubDepartment name cannot be empty.", true); return; }

            List<int> selectedDepartments = new List<int>();
            foreach (Control c in departmentCheckboxes.Controls)
                if (c is CheckBox chk && chk.Checked)
                    selectedDepartments.Add(int.Parse(chk.InputAttributes["value"]));

            if (!selectedDepartments.Any()) { ShowMessage("Select at least one department.", true); return; }

            int subDeptId = 0;
            if (!string.IsNullOrEmpty(hfSubDeptID.Value)) subDeptId = int.Parse(hfSubDeptID.Value);

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                if (subDeptId == 0)
                {
                    SqlCommand cmd = new SqlCommand("INSERT INTO SubDeptMasters (SubDeptName, SortOrder) OUTPUT INSERTED.SubDeptID VALUES (@Name, ISNULL((SELECT MAX(SortOrder) FROM SubDeptMasters),0)+1)", conn);
                    cmd.Parameters.AddWithValue("@Name", name);
                    subDeptId = (int)cmd.ExecuteScalar();
                }
                else
                {
                    SqlCommand cmd = new SqlCommand("UPDATE SubDeptMasters SET SubDeptName=@Name WHERE SubDeptID=@ID", conn);
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@ID", subDeptId);
                    cmd.ExecuteNonQuery();

                    SqlCommand del = new SqlCommand("DELETE FROM SubDeptDepartmentMapping WHERE SubDeptID=@ID", conn);
                    del.Parameters.AddWithValue("@ID", subDeptId);
                    del.ExecuteNonQuery();
                }

                foreach (var deptID in selectedDepartments)
                {
                    SqlCommand map = new SqlCommand("INSERT INTO SubDeptDepartmentMapping (SubDeptID, DeptID) VALUES (@SubDeptID,@DeptID)", conn);
                    map.Parameters.AddWithValue("@SubDeptID", subDeptId);
                    map.Parameters.AddWithValue("@DeptID", deptID);
                    map.ExecuteNonQuery();
                }
            }

            ClearForm();
            ShowMessage("SubDepartment saved successfully.");
            BindGrid();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            int subDeptId = Convert.ToInt32(btn.CommandArgument);

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand delMap = new SqlCommand("DELETE FROM SubDeptDepartmentMapping WHERE SubDeptID=@ID", conn);
                delMap.Parameters.AddWithValue("@ID", subDeptId);
                delMap.ExecuteNonQuery();

                SqlCommand delSubDept = new SqlCommand("DELETE FROM SubDeptMasters WHERE SubDeptID=@ID", conn);
                delSubDept.Parameters.AddWithValue("@ID", subDeptId);
                delSubDept.ExecuteNonQuery();
            }

            ShowMessage("SubDepartment deleted successfully.");
            BindGrid();
        }

        protected void btnSaveOrder_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(hfSubDeptOrder.Value)) return;
            string[] ids = hfSubDeptOrder.Value.Split(',');

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                for (int i = 0; i < ids.Length; i++)
                {
                    SqlCommand cmd = new SqlCommand("UPDATE SubDeptMasters SET SortOrder=@Sort WHERE SubDeptID=@ID", conn);
                    cmd.Parameters.AddWithValue("@Sort", i + 1);
                    cmd.Parameters.AddWithValue("@ID", int.Parse(ids[i]));
                    cmd.ExecuteNonQuery();
                }
            }
            ShowMessage("SubDepartment order updated successfully.");
            BindGrid();
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
            hfSubDeptID.Value = "";
            foreach (Control c in departmentCheckboxes.Controls) if (c is CheckBox chk) chk.Checked = false;
        }
    }
}
