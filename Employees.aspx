<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Employees.aspx.cs" Inherits="PhoneDir.Masters.Employees" MasterPageFile="~/Site.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<style>
.page-content { background: rgba(255,255,255,0.65); backdrop-filter: blur(10px); border: 1px solid rgba(200,200,200,0.4); border-radius:16px; padding:20px; box-shadow:0 8px 20px rgba(0,0,0,0.05); margin-top:20px;}
.form-group { margin-bottom:12px; }
.form-control { width:100%; padding:8px 10px; border:1px solid #cfdfee; border-radius:6px; font-size:0.9rem; }
.form-control:focus { border-color:#2c7db1; outline:none; }
.btn { padding:8px 16px; background: linear-gradient(135deg,#4a90e2 0%,#357ABD 100%); color:white; border:1.5px solid #2c7db1; border-radius:6px; cursor:pointer; font-weight:600; font-size:0.9rem; box-shadow:0 2px 6px rgba(44,125,177,0.35); margin-right:5px; }
.btn:hover { background: linear-gradient(135deg,#357ABD 0%,#255A88 100%); }
.status-message { display:block; padding:10px 16px; margin-bottom:15px; border-radius:6px; font-weight:500; font-size:0.9rem; border:1px solid; background-color:#e8f5e9; color:#2e7d32; border-color:#c8e6c9; box-shadow:0 1px 4px rgba(0,0,0,0.04);}
.status-message.error { background-color:#fcebea; color:#c62828; border-color:#f5c6cb;}
.tree-node { margin-left:20px; display:block; }
</style>

<div class="page-content">
    <div class="form-group"><label>EmpID:</label><asp:TextBox ID="txtEmpID" runat="server" CssClass="form-control" /></div>
    <div class="form-group"><label>Name:</label><asp:TextBox ID="txtName" runat="server" CssClass="form-control" /></div>
    <div class="form-group"><label>Designation: <span style="color:red">*</span></label><asp:TextBox ID="txtDesignation" runat="server" CssClass="form-control" /></div>
    <div class="form-group"><label>Extension:</label><asp:TextBox ID="txtExtension" runat="server" CssClass="form-control" /></div>
    <div class="form-group"><label>Mobile:</label><asp:TextBox ID="txtMobile" runat="server" CssClass="form-control" /></div>
    <div class="form-group"><label>Location:</label><asp:TextBox ID="txtLocation" runat="server" CssClass="form-control" /></div>
    <div class="form-group"><label>SubDepartment:</label><asp:TextBox ID="txtSubDept" runat="server" CssClass="form-control" /></div>

    <div class="form-group"><label>Assign to Departments:</label>
        <asp:PlaceHolder ID="phDepartments" runat="server"></asp:PlaceHolder>
    </div>

    <asp:Button ID="btnSaveEmployee" runat="server" CssClass="btn" Text="Save Employee" OnClick="btnSaveEmployee_Click" />
    <asp:HiddenField ID="hfEmployeePK" runat="server" />
    <asp:Label ID="lblMessage" runat="server" CssClass="status-message" Visible="false" />
</div>
</asp:Content>
