using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MunicipalServicesApp.Forms
{
    partial class RequestPriorityForm
    {
        private IContainer components = null;

        private FlowLayoutPanel cardsPanel;
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

            // Form
            this.Text = "Priority Order";
            this.Size = new Size(900, 700);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = ColorTranslator.FromHtml("#F9F9F9");

            // ---------------------------------------------------------------------------------------- Header ------------------------------------------------------------------
            hdr = new Label
            {
                Text = "Priority Order — Requests sorted by priority then date",
                Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#333"),
                AutoSize = false,
                Height = 40,
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(16, 8, 0, 0)
            };

            // ---------------------------------------------------------------------------------------- Cards Panel ------------------------------------------------------------
            cardsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(16),
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                BackColor = Color.Transparent
            };

            // Add controls to form
            this.Controls.Add(cardsPanel);
            this.Controls.Add(hdr);
        }
    }
}
