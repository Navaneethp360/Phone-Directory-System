<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SubDepartments.aspx.cs" 
    Inherits="PhoneDir.Masters.SubDepartments" MasterPageFile="~/Site.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<style>
.page-content { background: rgba(255,255,255,0.65); backdrop-filter: blur(10px); border:1px solid rgba(200,200,200,0.4); border-radius:16px; padding:20px; box-shadow:0 8px 20px rgba(0,0,0,0.05); }
.page-header { margin-bottom:15px; padding:10px 15px; border-bottom:1px solid #e9f2fa; background-color:#e9f2fa; border-radius:8px; }
.page-header h2 { color:#2c7db1; font-size:1.5rem; display:flex; align-items:center; }
.form-group { margin-bottom:12px; }
.form-control, select { width:100%; padding:8px 10px; border:1px solid #cfdfee; border-radius:6px; font-size:0.9rem; }
.form-control:focus, select:focus { border-color:#2c7db1; outline:none; }
.btn { padding:8px 16px; background:linear-gradient(135deg,#4a90e2,#357ABD); color:white; border:1.5px solid #2c7db1; border-radius:6px; cursor:pointer; font-weight:600; font-size:0.9rem; margin-right:5px; }
.btn:hover { background:linear-gradient(135deg,#357ABD,#255A88); }
.table { width:100%; border-collapse:collapse; margin-top:15px; border-radius:10px; overflow:hidden; }
.table th, .table td { padding:10px 12px; border-bottom:1px solid #e2effa; text-align:left; }
.table th { background:#f0f8ff; font-weight:600; color:#2c7db1; }
.table tr:hover { background: rgba(240,248,255,0.3); cursor: grab; }
.status-message { display:block; padding:10px 16px; margin-bottom:15px; border-radius:6px; font-weight:500; font-size:0.9rem; border:1px solid; background-color:#e8f5e9; color:#2e7d32; border-color:#c8e6c9; box-shadow:0 1px 4px rgba(0,0,0,0.04); }
.status-message.error { background-color:#fcebea; color:#c62828; border-color:#f5c6cb; }
.drag-placeholder { background-color: rgba(44,125,177,0.1); height:50px; margin-bottom:5px; border:1px dashed #2c7db1; border-radius:4px; }
</style>
<div class="page-content">

    <div class="page-header">
        <h2>📂 Manage Sub-Departments</h2>
        <p class="subtitle">Add, reorder, or remove sub-departments per department</p>
    </div>

    <asp:Label ID="lblMessage" runat="server" CssClass="status-message" Visible="false" />

    <!-- SubDept name input -->
    <div class="form-group">
        <label>Sub-Department Name:</label>
        <asp:TextBox ID="txtSubDeptName" runat="server" CssClass="form-control" />
    </div>

    <!-- Assign to departments -->
    <div class="form-group">
        <label>Assign to Departments:</label>
        <asp:CheckBoxList ID="chkDepartments" runat="server" RepeatDirection="Horizontal" CssClass="form-control" />
    </div>
    <asp:Button ID="btnAddSubDept" runat="server" Text="Add / Update Sub-Department" CssClass="btn" 
        OnClick="btnAddSubDept_Click" />

    <hr />

    <!-- Dropdown select Dept -->
    <div class="form-group">
        <label>Select Department:</label>
        <asp:DropDownList ID="ddlDepartments" runat="server" CssClass="form-control"
            AutoPostBack="true" OnSelectedIndexChanged="ddlDepartments_SelectedIndexChanged" />
    </div>

    <asp:HiddenField ID="hfSubDeptOrder" runat="server" />

    <!-- SubDept grid per Dept -->
    <asp:GridView ID="gvSubDepts" runat="server" AutoGenerateColumns="False" CssClass="table"
        DataKeyNames="SubDeptID" OnRowCommand="gvSubDepts_RowCommand" OnRowDataBound="gvSubDepts_RowDataBound">
        <Columns>
            <asp:BoundField DataField="SubDeptName" HeaderText="Sub-Department Name" />
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:Button ID="btnDelete" runat="server" Text="Delete" CssClass="btn"
                        CommandName="DeleteSubDept" CommandArgument='<%# Eval("SubDeptID") %>' />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>

    <asp:Button ID="btnSaveOrder" runat="server" Text="Save Order" CssClass="btn" OnClick="btnSaveOrder_Click" />

    <hr />
    <div class="page-header">
        <h2>🗂️ All Sub-Departments</h2>
        <p class="subtitle">Delete any sub-department completely from all departments</p>
    </div>

    <asp:Table ID="tblAllSubDeptsBody" runat="server" CssClass="table"></asp:Table>

</div>

<script src="https://cdnjs.cloudflare.com/ajax/libs/Sortable/1.15.0/Sortable.min.js"></script>
<script>
    document.addEventListener('DOMContentLoaded', function () {
        var tbody = document.querySelector("#<%= gvSubDepts.ClientID %> tbody");
        if (tbody) {
            Sortable.create(tbody, {
                animation: 150,
                ghostClass: 'drag-placeholder',
                handle: 'td',
                draggable: 'tr',
                onEnd: function () {
                    var ids = Array.from(tbody.querySelectorAll('tr[data-id]'))
                        .map(r => r.getAttribute('data-id'));
                    document.getElementById('<%= hfSubDeptOrder.ClientID %>').value = ids.join(',');
                }
            });
        }
    });
</script>
</asp:Content>
