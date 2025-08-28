<%@ Page Language="C#" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="EmployeeOrder.aspx.cs" Inherits="PhoneDir.Masters.EmployeeOrder" MasterPageFile="~/Site.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<style>
.page-content { background: rgba(255,255,255,0.65); backdrop-filter: blur(10px); border: 1px solid rgba(200,200,200,0.4); border-radius:16px; padding:20px; box-shadow:0 8px 20px rgba(0,0,0,0.05); margin-top:20px;}
.form-control { width:100%; padding:8px 10px; border:1px solid #cfdfee; border-radius:6px; font-size:0.9rem; margin-bottom:10px;}
.btn { padding:6px 12px; background: linear-gradient(135deg,#4a90e2 0%,#357ABD 100%); color:white; border:1.5px solid #2c7db1; border-radius:6px; cursor:pointer; font-weight:600; font-size:0.85rem; box-shadow:0 2px 6px rgba(44,125,177,0.35); margin-right:5px; }
.btn:hover { background: linear-gradient(135deg,#357ABD 0%,#255A88 100%); }
.table { width:100%; border-collapse:collapse; background: rgba(255,255,255,0.75); border:1px solid #cfe5f9; border-radius:10px; }
.table th, .table td { padding:10px; border-bottom:1px solid #e2effa; text-align:left; }
.table th { background-color:#f0f8ff; }
.status-message { display:block; padding:10px 16px; margin-bottom:15px; border-radius:6px; font-weight:500; font-size:0.9rem; border:1px solid; background-color:#e8f5e9; color:#2e7d32; border-color:#c8e6c9; box-shadow:0 1px 4px rgba(0,0,0,0.04);}
.status-message.error { background-color:#fcebea; color:#c62828; border-color:#f5c6cb;}
.drag-placeholder { background-color: rgba(44,125,177,0.1); height:40px; margin-bottom:5px; border:1px dashed #2c7db1; border-radius:4px; }
</style>

<div class="page-content">
    <label>Select Department:</label>
    <asp:DropDownList ID="ddlDepartment" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlDepartment_SelectedIndexChanged"></asp:DropDownList>

    <table class="table" id="tblEmployees">
        <thead>
            <tr>
                <th>Name</th>
                <th>EmpID</th>
                <th>Designation</th>
                <th>Location</th>
                <th>SubDept</th>
                <th>Action</th>
            </tr>
        </thead>
        <tbody id="tblEmployeesBody" runat="server"></tbody>
    </table>

    <asp:HiddenField ID="hfEmpOrder" runat="server" />
    <asp:Button ID="btnSaveOrder" runat="server" CssClass="btn" Text="Save Order" OnClick="btnSaveOrder_Click" Style="display:none;" />
    <asp:Label ID="lblMessage" runat="server" CssClass="status-message" Visible="false" />
</div>

<script src="https://cdnjs.cloudflare.com/ajax/libs/Sortable/1.15.0/Sortable.min.js"></script>
<script>
    document.addEventListener('DOMContentLoaded', function () {
        var tbody = document.getElementById('<%= tblEmployeesBody.ClientID %>');
    var hfOrder = document.getElementById('<%= hfEmpOrder.ClientID %>');
    var hiddenBtn = document.getElementById('<%= btnSaveOrder.ClientID %>');

    Sortable.create(tbody, {
        animation: 150,
        ghostClass: 'drag-placeholder',
        handle: 'td',
        onEnd: function () {
            var ids = Array.from(tbody.querySelectorAll('tr')).map(tr => tr.getAttribute('data-empid'));
            hfOrder.value = ids.join(',');
            hiddenBtn.click(); // trigger postback to save order
        }
    });

    tbody.addEventListener('click', function (e) {
        if (e.target && e.target.classList.contains('btn-delete')) {
            if (confirm('Are you sure you want to delete this employee?')) {
                var empId = e.target.getAttribute('data-empid');
                __doPostBack('<%= btnSaveOrder.UniqueID %>', 'delete$' + empId);
            }
        }
    });
});
</script>
</asp:Content>
