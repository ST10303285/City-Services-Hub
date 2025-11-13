using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace MunicipalServicesApp.Forms
{
    partial class LocalEventsForm
    {
        private IContainer components = null;

        private Panel headerPanel;
        private Label lblTitle;
        private Label lblSubtitle;
        private TextBox txtSearch;
        private ComboBox cmbCategory;
        private DateTimePicker dtFrom;
        private DateTimePicker dtTo;
        private ComboBox cmbSort;
        private Button btnSearch;
        private FlowLayoutPanel eventsPanel;
        private Panel rightPanel;
        private Label lblRecommendations;
        private FlowLayoutPanel lstRecommendations;
        private Button btnBack;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitialiseComponent()
        {
            this.Text = "Local Events & Announcements";
            this.WindowState = FormWindowState.Maximized;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = ColorTranslator.FromHtml("#e1f5f2");

            // ----------------------------------------------------------------- Header Panel --------------------------------------------------------
            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 110,
                BackColor = ColorTranslator.FromHtml("#6CB2B2")
            };

            lblTitle = new Label
            {
                Text = "Local Events & Announcements",
                Font = new Font("Segoe UI Semibold", 20F, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(24, 18)
            };

            lblSubtitle = new Label
            {
                Text = "Search, filter and view upcoming events in your area.",
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.WhiteSmoke,
                AutoSize = true,
                Location = new Point(26, 58)
            };

            headerPanel.Controls.Add(lblTitle);
            headerPanel.Controls.Add(lblSubtitle);

            // ------------------------------------------------------------------Filters -------------------------------------------------------
            txtSearch = new TextBox
            {
                PlaceholderText = "Search by name or description...",
                Font = new Font("Segoe UI", 10F),
                Size = new Size(500, 36),
                Location = new Point(50, 130),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            cmbCategory = new ComboBox
            {
                Font = new Font("Segoe UI", 10F),
                Size = new Size(160, 36),
                Location = new Point(600, 130),
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.White
            };

            dtFrom = new DateTimePicker
            {
                Font = new Font("Segoe UI", 10F),
                Format = DateTimePickerFormat.Short,
                Size = new Size(140, 36),
                Location = new Point(800, 130),
                ShowCheckBox = true
            };

            dtTo = new DateTimePicker
            {
                Font = new Font("Segoe UI", 10F),
                Format = DateTimePickerFormat.Short,
                Size = new Size(140, 36),
                Location = new Point(1000, 130),
                ShowCheckBox = true
            };

            cmbSort = new ComboBox
            {
                Font = new Font("Segoe UI", 10F),
                Size = new Size(180, 36),
                Location = new Point(1200, 130),
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.White
            };
            cmbSort.Items.AddRange(new string[] { "Sort: Date Asc", "Sort: Date Desc", "Sort: Category", "Sort: Name" });
            cmbSort.SelectedIndex = 0;
            cmbSort.SelectedIndexChanged += (s, e) => ApplySort();

            btnSearch = new Button
            {
                Text = "Search / Filter",
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                Size = new Size(150, 36),
                Location = new Point(1400, 130),
                BackColor = ColorTranslator.FromHtml("#A8D8EA"),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.Black
            };
            btnSearch.FlatAppearance.BorderSize = 0;
            btnSearch.Region = new Region(RoundedRect(new Rectangle(0, 0, btnSearch.Width, btnSearch.Height), 8));
            btnSearch.Click += (s, e) => DoSearch();

            // ------------------------------------------------------------- Events Panel ---------------------------------------------------------
            eventsPanel = new FlowLayoutPanel
            {
                Location = new Point(24, 180),
                Size = new Size(1200, 800),
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false
            };

            // ----------------------------------------------------------- Recommendations Panel ----------------------------------------------------
            rightPanel = new Panel
            {
                Location = new Point(1250, 180),
                Size = new Size(300, 700),
                BackColor = ColorTranslator.FromHtml("#0e6263"), 
                Padding = new Padding(8)
            };

            lblRecommendations = new Label
            {
                Text = "Recommended for you",
                Font = new Font("Segoe UI Semibold", 11F),
                ForeColor = ColorTranslator.FromHtml("#fcfcfc"),
                AutoSize = true,
                Location = new Point(12, 12)
            };

            lstRecommendations = new FlowLayoutPanel
            {
                Location = new Point(12, 40),
                Size = new Size(276, 480),
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false
            };

            btnBack = new Button
            {
                Text = "Back to Menu",
                Size = new Size(276, 40),
                Location = new Point(12, 530),
                BackColor = ColorTranslator.FromHtml("#A8D8EA"),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.Black,
                Font = new Font("Segoe UI", 10F)
            };
            btnBack.FlatAppearance.BorderSize = 0;
            btnBack.Region = new Region(RoundedRect(new Rectangle(0, 0, btnBack.Width, btnBack.Height), 8));
            btnBack.Click += (s, e) => this.Close();

            rightPanel.Controls.Add(lblRecommendations);
            rightPanel.Controls.Add(lstRecommendations);
            rightPanel.Controls.Add(btnBack);

            // --------------------------------------------------------------Add controls to Form ---------------------------------------------------
            this.Controls.Add(headerPanel);
            this.Controls.Add(txtSearch);
            this.Controls.Add(cmbCategory);
            this.Controls.Add(dtFrom);
            this.Controls.Add(dtTo);
            this.Controls.Add(cmbSort);
            this.Controls.Add(btnSearch);
            this.Controls.Add(eventsPanel);
            this.Controls.Add(rightPanel);
        }

        private GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            var path = new GraphicsPath();
            int d = radius * 2;
            path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
            path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
