<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LicenseActivation.aspx.cs" Inherits="YourNamespace.LicenseActivation" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Medical System - License Activation</title>
    <style>
        :root {
            --frost-bg: rgba(0, 94, 184, 0.15);
            --frost-border: rgba(0, 94, 184, 0.25);
            --frost-shadow: rgba(0, 94, 184, 0.4);
            --primary-color: #005eb8;
            --success-color: #28a745;
            --danger-color: #e53e3e;
        }

        html, body {
            height: 100%;
            margin: 0;
            font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif;
            background: linear-gradient(135deg, #f0f4f8, #dceeff);
            display: flex;
            align-items: center;
            justify-content: center;
        }

        .glass-card {
            background: var(--frost-bg);
            backdrop-filter: blur(20px);
            -webkit-backdrop-filter: blur(20px);
            border: 1px solid var(--frost-border);
            border-radius: 20px;
            padding: 40px;
            width: 100%;
            max-width: 460px;
            box-shadow: 0 12px 32px var(--frost-shadow);
            color: #000;
            text-align: center;
        }

        .glass-card h2 {
            font-size: 2rem;
            font-weight: 700;
            margin-bottom: 20px;
            color: var(--primary-color);
        }

        .glass-card h4 {
            font-size: 1rem;
            font-weight: 400;
            color: #333;
            margin-bottom: 30px;
        }

        .form-group {
            text-align: left;
            margin-bottom: 24px;
        }

        label {
            display: block;
            font-weight: 600;
            margin-bottom: 6px;
        }

        .form-control {
            width: 93%;
            padding: 12px;
            border: 1px solid #ccc;
            border-radius: 10px;
            font-size: 1rem;
            background-color: rgba(255,255,255,0.6);
        }

        .form-control:focus {
            outline: none;
            border-color: var(--primary-color);
            background-color: #fff;
        }

        .btn-activate {
            width: 100%;
            padding: 12px 0;
            font-size: 1rem;
            font-weight: 600;
            border: none;
            border-radius: 10px;
            background-color: var(--primary-color);
            color: white;
            cursor: pointer;
            box-shadow: 0 4px 12px rgba(0, 94, 184, 0.4);
            transition: all 0.3s ease;
        }

        .btn-activate:hover {
            background-color: #004a94;
        }

        .text-danger {
            color: var(--danger-color);
            font-weight: 600;
            margin-top: 10px;
            min-height: 22px;
        }

        .text-success {
            color: var(--success-color);
            font-weight: 600;
            margin-top: 10px;
        }

        .footer {
            margin-top: 30px;
            font-size: 0.9rem;
            color: #666;
        }

        .footer a {
            color: var(--primary-color);
            font-weight: 600;
            text-decoration: none;
        }

        .footer a:hover {
            text-decoration: underline;
        }

        @media (max-width: 576px) {
            .glass-card {
                padding: 28px;
                max-width: 90vw;
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:Panel ID="pnlContainer" runat="server" CssClass="glass-card">
            <h2>Medical System</h2>
            <h4>License Activation</h4>

            <asp:Label ID="lblMessage" runat="server" CssClass="text-danger" EnableViewState="false" />

            <asp:Panel ID="pnlActivate" runat="server">
                <div class="form-group">
                    <label for="txtLicenseKey">Enter License Key:</label>
                    <asp:TextBox ID="txtLicenseKey" runat="server" CssClass="form-control" MaxLength="100" TextMode="Password" />
                </div>
                <asp:Button ID="btnActivate" runat="server" CssClass="btn-activate" Text="Activate License" OnClick="btnActivate_Click" />
            </asp:Panel>

            <asp:Button ID="btnGoToLogin" runat="server" CssClass="btn-activate" Text="Go to Login" OnClick="btnGoToLogin_Click" Visible="false" />

           <div class="footer">
            <a href="https://github.com/Navaneethp360" target="_blank" rel="noopener noreferrer">https://github.com/Navaneethp360</a>
        </div>
        </asp:Panel>
    </form>
</body>
</html>