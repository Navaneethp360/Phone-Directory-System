<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SubDepartments.aspx.cs" Inherits="PhoneDir.Masters.SubDepartments" MasterPageFile="~/Site.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<style>
:root {
    --glass-bg: rgba(255,255,255,0.65);
    --glass-border: rgba(200,200,200,0.4);
    --glass-shadow: 0 8px 20px rgba(0,0,0,0.05);
    --blur-amt: blur(10px);
    --primary: #2c7db1;
    --text-main: #1c1c1c;
}

.page-content {
    background: var(--glass-bg);
    backdrop-filter: var(--blur-amt);
    border: 1px solid var(--glass-border);
    border-radius: 16px;
    padding: 20px;
    box-shadow: var(--glass-shadow);
    margin-top: 20px;
    color: var(--text-main);
}

.page-header { margin-bottom: 15px; padding: 10px 15px; border-bottom: 1px solid #e9f2fa; background-color: #e9f2fa; border-radius: 8px; }
.page-header h2 { color: var(--primary); font-size: 1.5rem; margin-bottom: 2px; display: flex; align-items: center; }

.form-group { margin-bottom: 12px; }
.form-control { width: 100%; padding: 8px 10px; border: 1px solid #cfdfee; border-radius: 6px; font-size: 0.9rem; }
.form-control:focus { border-color: var(--primary); outline:none; }

.btn { padding: 8px 16px; background: linear-gradient(135deg,#4a90e2 0%,#357ABD 100%); color:white; border:1.5px solid var(--primary); border-radius:6px; cursor:pointer; font-weight:600; font-size:0.9rem; box-shadow:0 2px 6px rgba(44,125,177,0.35); margin-right:5px; }
.btn:hover { background: linear-gradient(135deg,#357ABD 0%,#255A88 100%); }

.table { width:100%; border-collapse:collapse; margin-top:15px; background: rgba(255,255,255,0.75); border:1px solid #cfe5f9; border-radius:10px; overflow:hidden; box-shadow:var(--glass-shadow); }
.table th, .table td { padding:10px 12px; border-bottom:1px solid #e2effa; text-align:left; vertical-align: middle; }
.table th { background:#f0f8ff; font-weight:600; color:var(--primary); }
.table tr:hover { background: rgba(240,248,255,0.3); cursor: grab; }

.status-message { display:block; padding:10px 16px; margin-bottom:15px; border-radius:6px; font-weight:500; font-size:0.9rem; border:1px solid; background-color:#e8f5e9; color:#2e7d32; border-color:#c8e6c9; box-shadow:0 1px 4px rgba(0,0,0,0.04); }
.status-message.error { background-color:#fcebea; color:#c62828; border-color:#f5c6cb; }

.drag-placeholder { background-color: rgba(44,125,177,0.1); height: 50px; margin-bottom:5px; border:1px dashed var(--primary); border-radius:4px; }
.checkbox-container { display:flex; flex-wrap:wrap; gap:8px; margin-top:5px; }
.checkbox-container label { display:flex; align-items:center; gap:4px; }
</style>

<div class="page-content">
    <div class="page-header">
        <h2>🏢 Manage SubDepartments</h2>
        <p class="subtitle">Add, edit, delete subdepartments and assign to departments</p>
    </div>

    <asp:Label ID="lblMessage" runat="server" CssClass="status-message" Visible="false"></asp:Label>
    <asp:HiddenField ID="hfSubDeptID" runat="server" />
    <asp:HiddenField ID="hfSubDeptOrder" runat="server" />

    <div class="form-group">
        <label for="txtSubDeptName">SubDepartment Name:</label>
        <asp:TextBox ID="txtSubDeptName" runat="server" CssClass="form-control" />
    </div>

    <div class="form-group">
        <label>Assign to Departments:</label>
        <div id="departmentCheckboxes" class="checkbox-container" runat="server"></div>
    </div>

    <asp:Button ID="btnSaveSubDept" runat="server" Text="Save SubDepartment" CssClass="btn" OnClick="btnSaveSubDept_Click" />
    <asp:Button ID="btnSaveOrder" runat="server" Text="Save Order" CssClass="btn" OnClick="btnSaveOrder_Click" />

    <table id="subDeptTable" class="table">
        <thead>
            <tr>
                <th>SubDepartment Name</th>
                <th>Departments</th>
                <th>Actions</th>
            </tr>
        </thead>
        <tbody id="subDeptTableBody" runat="server">
        </tbody>
    </table>
</div>

<script src="https://cdnjs.cloudflare.com/ajax/libs/Sortable/1.15.0/Sortable.min.js"></script>
<script>
    document.addEventListener('DOMContentLoaded', function () {
        var tbody = document.getElementById('<%= subDeptTableBody.ClientID %>');
    var hfOrder = document.getElementById('<%= hfSubDeptOrder.ClientID %>');

    Sortable.create(tbody, {
        animation: 150,
        ghostClass: 'drag-placeholder',
        onEnd: function () {
            var ids = [];
            tbody.querySelectorAll('tr').forEach(tr => ids.push(tr.getAttribute('data-id')));
            hfOrder.value = ids.join(',');
        }
    });

    function setCheckboxes(selectedIds) {
        var container = document.getElementById('<%= departmentCheckboxes.ClientID %>');
        var inputs = container.querySelectorAll('input[type="checkbox"]');
        inputs.forEach(chk => chk.checked = selectedIds.includes(chk.value));
    }

    window.editSubDept = function(id, name, deptIds) {
        document.getElementById('<%= hfSubDeptID.ClientID %>').value = id;
        document.getElementById('<%= txtSubDeptName.ClientID %>').value = name;
        setCheckboxes(deptIds.map(x => x.toString()));
    }
});
</script>
</asp:Content>
