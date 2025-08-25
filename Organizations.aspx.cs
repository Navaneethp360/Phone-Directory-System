using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace PhoneDir.Masters
{
    public partial class Organizations : System.Web.UI.Page
    {
        private string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
                BindGrid();
        }

        private void BindGrid()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = "SELECT OrgID, OrgName FROM OrgMasters ORDER BY SortOrder";
                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                orgTableBody.Controls.Clear();
                foreach (DataRow dr in dt.Rows)
                {
                    HtmlTableRow tr = new HtmlTableRow();
                    tr.Attributes["data-id"] = dr["OrgID"].ToString();

                    HtmlTableCell tdName = new HtmlTableCell();
                    tdName.InnerText = dr["OrgName"].ToString();
                    tr.Cells.Add(tdName);

                    HtmlTableCell tdAction = new HtmlTableCell();
                    Button btnDelete = new Button();
                    btnDelete.Text = "Delete";
                    btnDelete.CssClass = "btn";
                    btnDelete.CommandArgument = dr["OrgID"].ToString();
                    btnDelete.ID = "btnDelete_" + dr["OrgID"].ToString(); // unique ID
                    btnDelete.Click += BtnDelete_Click;
                    tdAction.Controls.Add(btnDelete);
                    tr.Cells.Add(tdAction);

                    orgTableBody.Controls.Add(tr);
                }
            }
        }

        protected void btnAddOrg_Click(object sender, EventArgs e)
        {
            string orgName = txtOrgName.Text.Trim();
            if (string.IsNullOrEmpty(orgName))
            {
                lblMessage.Text = "Organization name cannot be empty.";
                lblMessage.CssClass = "status-message error";
                lblMessage.Visible = true;
                return;
            }

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string sql = "INSERT INTO OrgMasters (OrgName, SortOrder) VALUES (@OrgName, ISNULL((SELECT MAX(SortOrder) FROM OrgMasters),0)+1)";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@OrgName", orgName);
                cmd.ExecuteNonQuery();
            }

            txtOrgName.Text = "";
            lblMessage.Text = "Organization added successfully.";
            lblMessage.CssClass = "status-message";
            lblMessage.Visible = true;
            BindGrid();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            int orgId = Convert.ToInt32(btn.CommandArgument);

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string sql = "DELETE FROM OrgMasters WHERE OrgID = @OrgID";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@OrgID", orgId);
                cmd.ExecuteNonQuery();
            }

            lblMessage.Text = "Organization deleted successfully.";
            lblMessage.CssClass = "status-message";
            lblMessage.Visible = true;
            BindGrid();
        }

        protected void btnSaveOrder_Click(object sender, EventArgs e)
        {
            string order = Request.Form["hiddenOrder"];
            if (string.IsNullOrEmpty(order))
            {
                lblMessage.Text = "No order changes detected.";
                lblMessage.CssClass = "status-message error";
                lblMessage.Visible = true;
                return;
            }

            string[] ids = order.Split(',');
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                for (int i = 0; i < ids.Length; i++)
                {
                    string sql = "UPDATE OrgMasters SET SortOrder=@SortOrder WHERE OrgID=@OrgID";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@SortOrder", i + 1);
                    cmd.Parameters.AddWithValue("@OrgID", Convert.ToInt32(ids[i]));
                    cmd.ExecuteNonQuery();
                }
            }

            lblMessage.Text = "Order updated successfully.";
            lblMessage.CssClass = "status-message";
            lblMessage.Visible = true;
            BindGrid();
        }
    }
}
