using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MunicipalServicesApp.Model;
using MunicipalServicesApp.Data;
using MunicipalServicesApp.Forms;


namespace MunicipalServicesApp
{
    public partial class MainMenuForm : Form
    {

        public MainMenuForm() // Constructor
        {
            InitialiseComponent();
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.AutoSize = false;
            this.WindowState = FormWindowState.Maximized; ;
        }
        //--------------------------------------------------------------------------------------------------------------------------//
        private void BtnReport_Click(object sender, EventArgs e) // Event handler for report button
        {
            var f = new ReportIssuesForm(); // Create new ReportIssuesForm
            f.FormClosed += (s, args) => Show();
            f.Show();
            Hide();
        }

        //--------------------------------------------------------------------------------------------------------------------------//
        private void BtnEvents_Click(object sender, EventArgs e) // Event handler for events button
        {
            var f = new LocalEventsForm(Program.EventsManager); // Create new LocalEventsForm
            f.FormClosed += (s, args) => Show();
            f.Show();
            Hide();
        }

        //--------------------------------------------------------------------------------------------------------------------------//
        private void BtnStatus_Click(object sender, EventArgs e) // Event handler for status button
        {
            var f = new ServiceRequestStatusForm(); // Create new ServiceRequestStatusForm
            f.FormClosed += (s, args) => Show();
            f.Show();
            Hide();
        }

        private void InitializeComponent()
        {

        }
    }
}
