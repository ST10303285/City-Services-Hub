using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MunicipalServicesApp.Forms
{
    partial class RecentRequestsForm
    {
        private IContainer components = null;

        private FlowLayoutPanel panel;
        private Label hdr;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new Container();

            // Form title and basic properties
            this.Text = "Recent Requests";
            this.Size = new Size(840, 620);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = ColorTranslator.FromHtml("#F9F9F9");

            // ---------------------------------------------------------------------------------------- Header ------------------------------------------------------------------
            hdr = new Label
            {
                Text = "Most Recent Service Requests",
                Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 44,
                Padding = new Padding(12, 8, 0, 0)
            };

            // ---------------------------------------------------------------------------------------- Main panel ------------------------------------------------------------
            panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                AutoScroll = true,
                Padding = new Padding(12)
            };

            // Add controls to form
            this.Controls.Add(panel);
            this.Controls.Add(hdr);
        }
    }
}
