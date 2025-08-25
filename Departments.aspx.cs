using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace PhoneDir.Masters
{
    public partial class Departments : System.Web.UI.Page
    {
        private string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindCompaniesCheckList();
                BindCompanyDropdown();
            }
        }

        private void BindCompaniesCheckList()
        {
            chkCompanies.Items.Clear();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT OrgID, OrgName FROM OrgMasters ORDER BY SortOrder", conn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    chkCompanies.Items.Add(new ListItem(dr["OrgName"].ToString(), dr["OrgID"].ToString()));
                }
            }
        }

        private void BindCompanyDropdown()
        {
            ddlCompanies.Items.Clear();
            ddlCompanies.Items.Add(new ListItem("--Select Company--", "0"));
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT OrgID, OrgName FROM OrgMasters ORDER BY SortOrder", conn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    ddlCompanies.Items.Add(new ListItem(dr["OrgName"].ToString(), dr["OrgID"].ToString()));
                }
            }
        }

        protected void btnAddDept_Click(object sender, EventArgs e)
        {
            string deptName = txtDeptName.Text.Trim();
            if (string.IsNullOrEmpty(deptName)) { ShowMessage("Enter department name", true); return; }

            List<int> selectedCompanies = new List<int>();
            foreach (ListItem li in chkCompanies.Items)
                if (li.Selected) selectedCompanies.Add(int.Parse(li.Value));

            if (selectedCompanies.Count == 0) { ShowMessage("Select at least one company", true); return; }

            int deptId = 0;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand checkCmd = new SqlCommand("SELECT DeptID FROM DeptMasters WHERE DeptName=@Name", conn);
                checkCmd.Parameters.AddWithValue("@Name", deptName);
                var obj = checkCmd.ExecuteScalar();
                if (obj == null)
                {
                    SqlCommand insertCmd = new SqlCommand(
                        "INSERT INTO DeptMasters (DeptName, SortOrder) OUTPUT INSERTED.DeptID VALUES (@Name, ISNULL((SELECT MAX(SortOrder) FROM DeptMasters),0)+1)", conn);
                    insertCmd.Parameters.AddWithValue("@Name", deptName);
                    deptId = (int)insertCmd.ExecuteScalar();
                }
                else
                    deptId = (int)obj;

                foreach (var orgId in selectedCompanies)
                {
                    SqlCommand mapCmd = new SqlCommand(
                        "IF NOT EXISTS (SELECT 1 FROM DeptCompanyMapping WHERE DeptID=@DeptID AND OrgID=@OrgID) " +
                        "INSERT INTO DeptCompanyMapping (DeptID, OrgID, SortOrder) VALUES (@DeptID,@OrgID, ISNULL((SELECT MAX(SortOrder) FROM DeptCompanyMapping WHERE OrgID=@OrgID),0)+1)", conn);
                    mapCmd.Parameters.AddWithValue("@DeptID", deptId);
                    mapCmd.Parameters.AddWithValue("@OrgID", orgId);
                    mapCmd.ExecuteNonQuery();
                }
            }

            ShowMessage("Department saved successfully.");
            ClearForm();
            LoadDepartmentsForCompany();
        }

        protected void ddlCompanies_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadDepartmentsForCompany();
        }

        private void LoadDepartmentsForCompany()
        {
            tblDeptsBody.Controls.Clear();
            int orgId = int.Parse(ddlCompanies.SelectedValue);
            if (orgId == 0) return;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(
                    "SELECT d.DeptID, d.DeptName FROM DeptCompanyMapping m INNER JOIN DeptMasters d ON m.DeptID=d.DeptID WHERE m.OrgID=@OrgID ORDER BY m.SortOrder", conn);
                cmd.Parameters.AddWithValue("@OrgID", orgId);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    TableRow tr = new TableRow { Attributes = { ["data-id"] = dr["DeptID"].ToString() } };
                    tr.Cells.Add(new TableCell { Text = dr["DeptName"].ToString() });

                    TableCell actions = new TableCell();
                    Button btn = new Button { Text = "Delete", CssClass = "btn" };
                    btn.Attributes["onclick"] = $"deleteDept({dr["DeptID"]}); return false;";
                    actions.Controls.Add(btn);
                    tr.Cells.Add(actions);

                    tblDeptsBody.Controls.Add(tr);
                }
            }
        }

        protected void btnSaveOrder_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(hfDeptOrder.Value)) return;
            int orgId = int.Parse(ddlCompanies.SelectedValue);
            if (orgId == 0) return;

            string[] ids = hfDeptOrder.Value.Split(',');
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                for (int i = 0; i < ids.Length; i++)
                {
                    SqlCommand cmd = new SqlCommand("UPDATE DeptCompanyMapping SET SortOrder=@Sort WHERE DeptID=@DeptID AND OrgID=@OrgID", conn);
                    cmd.Parameters.AddWithValue("@Sort", i + 1);
                    cmd.Parameters.AddWithValue("@DeptID", int.Parse(ids[i]));
                    cmd.Parameters.AddWithValue("@OrgID", orgId);
                    cmd.ExecuteNonQuery();
                }
            }
            ShowMessage("Order saved successfully.");
            LoadDepartmentsForCompany();
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            var arg = hdnDeleteArgs?.Value;
            if (string.IsNullOrEmpty(arg)) { ShowMessage("No delete parameters passed", true); return; }

            var parts = arg.Split('|');
            if (parts.Length < 2) { ShowMessage("Invalid delete parameters", true); return; }

            int deptId = int.Parse(parts[0]);
            int orgId = int.Parse(parts[1]);

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(
                    "DELETE FROM DeptCompanyMapping WHERE DeptID=@DeptID AND OrgID=@OrgID", conn);
                cmd.Parameters.AddWithValue("@DeptID", deptId);
                cmd.Parameters.AddWithValue("@OrgID", orgId);
                cmd.ExecuteNonQuery();
            }

            ShowMessage("Department removed from the selected company.");
            LoadDepartmentsForCompany();
        }

        private void ShowMessage(string msg, bool isError = false)
        {
            lblMessage.Text = msg;
            lblMessage.CssClass = isError ? "status-message error" : "status-message";
            lblMessage.Visible = true;
        }

        private void ClearForm()
        {
            txtDeptName.Text = "";
            foreach (ListItem li in chkCompanies.Items) li.Selected = false;
        }
    }
}
