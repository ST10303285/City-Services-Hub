using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace MunicipalServicesApp.Forms
{
    partial class ServiceRequestStatusForm
    {
        private IContainer components = null;

        // header
        private Panel headerPanel;
        private Label lblTitle;
        private Label lblSubtitle;

        // main area
        private FlowLayoutPanel cardsPanel;

        // bottom bar + controls
        private FlowLayoutPanel bottomButtonsPanel;
        private Button btnRefresh;
        private Label lblSearch;
        private TextBox txtSearchId;
        private Button btnSearch;
        private Button btnPrioritise;
        private Button btnShowMST;
        private Button btnRecent;
        private Button btnBack;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
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

            // Form base
            this.Text = "Service Request Status";
            this.WindowState = FormWindowState.Maximized;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = ColorTranslator.FromHtml("#F9F9F9");
            this.ClientSize = new Size(1366, 820);

            //----------------------------------------------------------------------------------Header------------------------------------------------------------------------------
            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 110,
                BackColor = ColorTranslator.FromHtml("#6CB2B2")
            };

            lblTitle = new Label
            {
                Text = "Service Request Status",
                Font = new Font("Segoe UI Semibold", 20F, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(24, 18)
            };

            lblSubtitle = new Label
            {
                Text = "View and manage submitted service requests — search, prioritise and analyse routes.",
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.WhiteSmoke,
                AutoSize = true,
                Location = new Point(26, 58)
            };

            headerPanel.Controls.Add(lblTitle);
            headerPanel.Controls.Add(lblSubtitle);

            //----------------------------------------------------------------------------------Bottom bar------------------------------------------------------------------------------
            bottomButtonsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 96,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(10, 14, 10, 14),
                BackColor = ColorTranslator.FromHtml("#F9F9F9"),
                WrapContents = false
            };

            //------------------------------------------------------------------------------styled button factory---------------------------------------------------------------------- 
            Button StyledButton(string text, string colorHex, bool whiteText = false)
            {
                var b = new Button
                {
                    Text = text,
                    Size = new Size(140, 40),
                    BackColor = ColorTranslator.FromHtml(colorHex),
                    FlatStyle = FlatStyle.Flat,
                    ForeColor = whiteText ? Color.White : Color.Black,
                    Font = new Font("Segoe UI", 9F),
                    Margin = new Padding(8, 0, 8, 0)
                };
                b.FlatAppearance.BorderSize = 0;
                b.Region = new Region(RoundedRect(new Rectangle(0, 0, b.Width, b.Height), 8));
                return b;
            }

            btnRefresh = StyledButton("Refresh", "#A8D8EA");
            lblSearch = new Label
            {
                Text = "Search by Request ID:",
                Font = new Font("Segoe UI", 9F),
                AutoSize = true,
                Padding = new Padding(8, 10, 8, 0)
            };
            txtSearchId = new TextBox { Width = 360, Font = new Font("Segoe UI", 9F), Margin = new Padding(4, 10, 8, 0) };
            btnSearch = StyledButton("Search ID", "#A8D8EA");
            btnPrioritise = StyledButton("Show Priority", "#A8D8EA");
            btnShowMST = StyledButton("Analyse routes", "#6CB2B2", true);
            btnRecent = StyledButton("Recent Requests", "#E6E6E6");
            btnBack = StyledButton("Back", "#E6E6E6");

            bottomButtonsPanel.Controls.Add(btnRefresh);
            bottomButtonsPanel.Controls.Add(lblSearch);
            bottomButtonsPanel.Controls.Add(txtSearchId);
            bottomButtonsPanel.Controls.Add(btnSearch);
            bottomButtonsPanel.Controls.Add(btnPrioritise);
            bottomButtonsPanel.Controls.Add(btnShowMST);
            bottomButtonsPanel.Controls.Add(btnRecent);
            bottomButtonsPanel.Controls.Add(btnBack);

            //---------------------------------------------------------------------------------Cards panel------------------------------------------------------------------------
            cardsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(12),
                BackColor = Color.Transparent
            };

            // keep card widths consistent on resize
            cardsPanel.Resize += (s, e) =>
            {
                int targetWidth = Math.Max(600, cardsPanel.ClientSize.Width - 48);
                foreach (Control c in cardsPanel.Controls)
                    if (c is Panel p) p.Width = targetWidth;
            };

            // Add to form 
            this.Controls.Add(cardsPanel);
            this.Controls.Add(bottomButtonsPanel);
            this.Controls.Add(headerPanel);
        }

       
       
    }
}
