using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using MunicipalServicesApp.Model;

namespace MunicipalServicesApp.Forms
{

    // This form shows a list of recently reported service requests.
    public partial class RecentRequestsForm : Form
    {
        private List<ServiceRequest> recentList = new List<ServiceRequest>(); // in-memory list of recent requests

        public RecentRequestsForm(IEnumerable<ServiceRequest> recent, int max = 10) // Constructor takes list of requests
        {
            // Initialize UI and keep only up to 'max' recent items
            InitializeComponent();
            recentList = (recent ?? Enumerable.Empty<ServiceRequest>()).Take(max).ToList();
            Populate(recentList);
        }

        //-----------------------------------------------------------------------------------------------------------------//
        private void Populate(List<ServiceRequest> list) // Fill the panel with cards for each recent request
        {
            panel.Controls.Clear();

            foreach (var r in list)
            {
                var card = new Panel
                {
                    Size = new Size(this.ClientSize.Width - 60, 100),
                    BackColor = Color.White,
                    Margin = new Padding(8),
                    Padding = new Padding(10)
                };
                card.Region = new Region(RoundedRect(new Rectangle(0, 0, card.Width, card.Height), 8));

                // draw light rounded border for card
                card.Paint += (s, e) =>
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    using var pen = new Pen(Color.LightGray);
                    e.Graphics.DrawPath(pen, RoundedRect(new Rectangle(0, 0, card.Width - 1, card.Height - 1), 8));
                };

                // Title line shows date, shortened category and location
                var title = new Label
                {
                    Text = $"{r.DateReported:yyyy-MM-dd HH:mm} | {Shorten(r.Category, 30)} | {Shorten(r.Location, 30)}",
                    Font = new Font("Segoe UI Semibold", 10F),
                    Location = new Point(8, 8),
                    AutoSize = false,
                    Size = new Size(card.Width - 140, 22)
                };

                // Short description preview
                var desc = new Label
                {
                    Text = Shorten(r.Description ?? "", 180),
                    Font = new Font("Segoe UI", 9F),
                    ForeColor = Color.DimGray,
                    Location = new Point(8, 32),
                    Size = new Size(card.Width - 140, 48)
                };

                // Details button opens a simple MessageBox with full details
                var btn = new Button
                {
                    Text = "Details",
                    Size = new Size(100, 36),
                    Location = new Point(card.Width - 112, card.Height - 48),
                    BackColor = ColorTranslator.FromHtml("#6CB2B2"),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                btn.FlatAppearance.BorderSize = 0;
                btn.Region = new Region(RoundedRect(new Rectangle(0, 0, btn.Width, btn.Height), 8));
                btn.Click += (s, e) =>
                {
                    MessageBox.Show($"ID: {r.Id}\n\n{r.Description}", "Request Details", MessageBoxButtons.OK, MessageBoxIcon.Information);
                };

                card.Controls.Add(title);
                card.Controls.Add(desc);
                card.Controls.Add(btn);

                // Make card responsive when resized
                card.Resize += (s, e) =>
                {
                    title.Size = new Size(card.Width - 140, 22);
                    desc.Size = new Size(card.Width - 140, 48);
                    btn.Location = new Point(card.Width - 112, card.Height - 48);
                };

                panel.Controls.Add(card);
            }

            // when there are no recent requests, show a small message
            if (!list.Any())
            {
                var empty = new Label
                {
                    Text = "No recent requests found.",
                    Font = new Font("Segoe UI", 10F),
                    ForeColor = Color.DimGray,
                    AutoSize = true,
                    Padding = new Padding(12)
                };
                panel.Controls.Add(empty);
            }
        }

        //-----------------------------------------------------------------------------------------------------------------//
        private string Shorten(string s, int max) => string.IsNullOrEmpty(s) ? "" : (s.Length <= max ? s : s.Substring(0, max - 3) + "..."); // Shorten long text safely

        //-----------------------------------------------------------------------------------------------------------------//
        private GraphicsPath RoundedRect(Rectangle bounds, int radius) // Helper: rounded rectangle path for nicer visuals
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
        //-----------------------------------------------------------------------------------------------------------------//
    }
}
