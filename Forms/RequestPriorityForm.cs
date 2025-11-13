using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using MunicipalServicesApp.Model;

namespace MunicipalServicesApp.Forms
{
    // This form shows requests ordered by priority (highest priority first).
    public partial class RequestPriorityForm : Form
    {

        // Constructor: create UI and fill it with provided requests.
        public RequestPriorityForm(IEnumerable<ServiceRequest> orderedRequests)
        {
            InitializeComponent();
            // convert to list and populate the cards
            Populate(orderedRequests?.ToList() ?? new List<ServiceRequest>());
        }

        //-----------------------------------------------------------------------------------------------------------------//
        // Put cards into the panel. If no items, show a message.
        private void Populate(List<ServiceRequest> list)
        {
            // clear old cards
            cardsPanel.Controls.Clear();
            int idx = 1;
            foreach (var r in list) // add one card per request
            {
                var card = CreateCard(r, idx++);
                cardsPanel.Controls.Add(card);
            }

            if (!list.Any())
            {
                var empty = new Label
                {
                    Text = "No requests available.",
                    Font = new Font("Segoe UI", 10F),
                    ForeColor = Color.DimGray,
                    AutoSize = true,
                    Padding = new Padding(12)
                };
                cardsPanel.Controls.Add(empty);
            }
        }

        //-----------------------------------------------------------------------------------------------------------------//
        // Make one card UI for a request. This is a panel with text and a button.
        private Panel CreateCard(ServiceRequest r, int index)
        {
            var w = Math.Max(760, this.ClientSize.Width - 80);
            var card = new Panel
            {
                Size = new Size(1500, 200),
                BackColor = Color.White,
                Margin = new Padding(8),
                Padding = new Padding(12)
            };
            card.Region = new Region(RoundedRect(new Rectangle(0, 0, card.Width, card.Height), 10));
            card.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using var pen = new Pen(Color.LightGray);
                e.Graphics.DrawPath(pen, RoundedRect(new Rectangle(0, 0, card.Width - 1, card.Height - 1), 10));
            };

            // Title shows index, category and location
            var title = new Label
            {
                Text = $"{index}. {Shorten(r.Category, 30)} — {Shorten(r.Location, 30)}",
                Font = new Font("Segoe UI Semibold", 11F),
                Location = new Point(12, 8),
                AutoSize = false,
                Size = new Size(card.Width - 200, 26)
            };

            var idLbl = new Label
            {
                Text = $"ID: {r.Id}",
                Font = new Font("Segoe UI", 8F),
                ForeColor = Color.DimGray,
                Location = new Point(12, 36),
                AutoSize = true
            };

            // meta line with priority, status and date
            var meta = new Label
            {
                Text = $"Priority: {r.Priority}  |  Status: {r.Status}  |  Date: {r.DateReported:yyyy/MM/dd HH:mm}",
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(80, 80, 80),
                Location = new Point(12, 56),
                AutoSize = false,
                Size = new Size(card.Width - 160, 22)
            };

            var desc = new Label
            {
                Text = Shorten(r.Description ?? "", 160),
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.Black,
                Location = new Point(12, 80),
                Size = new Size(card.Width - 260, 28)
            };

            // button to open full details in a message box
            var btn = new Button
            {
                Text = "Open Details",
                Size = new Size(120, 36),
                Location = new Point(card.Width - 140, card.Height - 48),
                BackColor = ColorTranslator.FromHtml("#6CB2B2"),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Region = new Region(RoundedRect(new Rectangle(0, 0, btn.Width, btn.Height), 8));
            // When button clicked, show a simple details dialog.
            btn.Click += (s, e) =>
            {
                var details = $"ID: {r.Id}\nCategory: {r.Category}\nLocation: {r.Location}\nPriority: {r.Priority}\nStatus: {r.Status}\nDate: {r.DateReported:u}\n\nDescription:\n{r.Description}";
                MessageBox.Show(details, "Request Details", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            card.Controls.Add(title);
            card.Controls.Add(idLbl);
            card.Controls.Add(meta);
            card.Controls.Add(desc);
            card.Controls.Add(btn);

            card.Resize += (s, e) =>
            {
                title.Size = new Size(card.Width - 200, 26);
                meta.Size = new Size(card.Width - 160, 22);
                desc.Size = new Size(card.Width - 260, 28);
                btn.Location = new Point(card.Width - 140, card.Height - 48);
            };

            return card;
        }

        //-----------------------------------------------------------------------------------------------------------------//
        private string Shorten(string s, int max)
        {
            if (string.IsNullOrEmpty(s)) return "";
            return s.Length <= max ? s : s.Substring(0, max - 3) + "...";
        }

        //-----------------------------------------------------------------------------------------------------------------//
        // utility used by CreateCard for rounded corners
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
        //-----------------------------------------------------------------------------------------------------------------//
    }
}
