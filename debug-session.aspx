<%@ Page Language="C#" AutoEventWireup="true" CodeFile="debug-session.aspx.cs" Inherits="VTT.debug_session" %>

    <!DOCTYPE html>
    <html>

    <head>
        <meta charset="utf-8" />
        <title>Debug Session</title>
        <style>
            body {
                font-family: monospace;
                padding: 20px;
                background: #f8fafc;
            }

            table {
                border-collapse: collapse;
                width: 100%;
                max-width: 700px;
                background: #fff;
            }

            td,
            th {
                border: 1px solid #cbd5e1;
                padding: 8px 12px;
                text-align: left;
            }

            th {
                background: #2563eb;
                color: #fff;
            }

            .null-val {
                color: #dc2626;
                font-style: italic;
            }
        </style>
    </head>

    <body>
        <h2>Kiểm tra Session hiện tại</h2>
        <table>
            <thead>
                <tr>
                    <th>Session Key</th>
                    <th>Giá trị</th>
                </tr>
            </thead>
            <tbody>
                <asp:Literal ID="litSessionRows" runat="server"></asp:Literal>
            </tbody>
        </table>
    </body>

    </html>