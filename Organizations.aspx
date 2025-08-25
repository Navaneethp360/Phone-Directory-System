<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Organizations.aspx.cs" Inherits="PhoneDir.Masters.Organizations" MasterPageFile="~/Site.master" %>

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
    -webkit-backdrop-filter: var(--blur-amt);
    border: 1px solid var(--glass-border);
    border-radius: 16px;
    padding: 20px;
    box-shadow: var(--glass-shadow);
    margin-top: 20px;
    color: var(--text-main);
}

.page-header {
    margin-bottom: 15px;
    padding: 10px 15px;
    border-bottom: 1px solid #e9f2fa;
    background-color: #e9f2fa;
    border-radius: 8px;
}

.page-header h2 {
    color: var(--primary);
    font-size: 1.5rem;
    margin-bottom: 2px;
    display: flex;
    align-items: center;
}

.form-group { margin-bottom: 12px; }
.form-control { width: 100%; padding: 8px 10px; border: 1px solid #cfdfee; border-radius: 6px; font-size: 0.9rem; }
.form-control:focus { border-color: var(--primary); outline:none; }

.btn {
    padding: 8px 16px;
    background: linear-gradient(135deg,#4a90e2 0%,#357ABD 100%);
    color:white; border:1.5px solid var(--primary); border-radius:6px;
    cursor:pointer; font-weight:600; font-size:0.9rem; box-shadow:0 2px 6px rgba(44,125,177,0.35);
}

.btn:hover { background: linear-gradient(135deg,#357ABD 0%,#255A88 100%); }

.table { width:100%; border-collapse:collapse; margin-top:15px; background: rgba(255,255,255,0.75); border:1px solid #cfe5f9; border-radius:10px; overflow:hidden; box-shadow:var(--glass-shadow); }
.table th, .table td { padding:10px 12px; border-bottom:1px solid #e2effa; text-align:left; }
.table th { background:#f0f8ff; font-weight:600; color:var(--primary); }
.table tr:hover { background: rgba(240,248,255,0.3); cursor: grab; }

.status-message { display:block; padding:10px 16px; margin-bottom:15px; border-radius:6px; font-weight:500; font-size:0.9rem; border:1px solid; background-color:#e8f5e9; color:#2e7d32; border-color:#c8e6c9; box-shadow:0 1px 4px rgba(0,0,0,0.04); }
.status-message.error { background-color:#fcebea; color:#c62828; border-color:#f5c6cb; }

.drag-placeholder { background-color: rgba(44,125,177,0.1); height: 40px; margin-bottom:5px; border:1px dashed var(--primary); border-radius:4px; }
</style>

<div class="page-content">
    <div class="page-header">
        <h2>🏢 Manage Organizations</h2>
        <p class="subtitle">Add, delete, and reorder organizations</p>
    </div>

    <asp:Label ID="lblMessage" runat="server" CssClass="status-message" Visible="false"></asp:Label>

    <div class="form-group">
        <label for="txtOrgName">Add New Organization:</label>
        <asp:TextBox ID="txtOrgName" runat="server" CssClass="form-control" />
    </div>
    <asp:Button ID="btnAddOrg" runat="server" Text="Add Organization" CssClass="btn" OnClick="btnAddOrg_Click" />

    <table id="orgTable" class="table">
        <thead>
            <tr>
                <th>Organization Name</th>
                <th>Action</th>
            </tr>
        </thead>
        <tbody id="orgTableBody" runat="server">
        </tbody>
    </table>

    <asp:Button ID="btnSaveOrder" runat="server" Text="Save Order" CssClass="btn" OnClick="btnSaveOrder_Click" Style="margin-top:15px;" />
</div>

<script src="https://cdnjs.cloudflare.com/ajax/libs/Sortable/1.15.0/Sortable.min.js"></script>
<script>
    document.addEventListener('DOMContentLoaded', function () {
        var tbody = document.getElementById('<%= orgTableBody.ClientID %>');
    var sortable = Sortable.create(tbody, {
        animation: 150,
        ghostClass: 'drag-placeholder'
    });

    var saveBtn = document.getElementById('<%= btnSaveOrder.ClientID %>');
    saveBtn.addEventListener('click', function () {
        var rows = tbody.querySelectorAll('tr');
        var orderInput = document.getElementById('hiddenOrder');
        if (!orderInput) {
            orderInput = document.createElement('input');
            orderInput.type = 'hidden';
            orderInput.name = 'hiddenOrder';
            orderInput.id = 'hiddenOrder';
            document.forms[0].appendChild(orderInput);
        }
        var ids = [];
        rows.forEach(function (row) {
            ids.push(row.getAttribute('data-id'));
        });
        orderInput.value = ids.join(',');
    });
});
</script>
</asp:Content>
