
using BLL;
using Common.Constants;
using Common.Helpers;
using Entity.Entities;
using HotelManagementSystem.Models.Entities;
using UI.Controls;

namespace UI.Forms;

// Giao diện chính của Lễ tân 
public class ReceptionistMainForm : Form
{
    public ReceptionistMainForm()
    {
        Text = "HotelPro - Giao diện Lễ tân";
        Size = new Size(1180, 720);
        StartPosition = FormStartPosition.CenterScreen;
        Controls.Add(new CustomerTab { Dock = DockStyle.Fill });
    }
}

