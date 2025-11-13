using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace MunicipalServicesApp.Forms
{

    // This form shows a minimum spanning tree (MST) of related service requests.
    public partial class MSTForm : Form
    {
        public class Edge
        {
            public string From { get; set; }
            public string To { get; set; }
            public int Distance { get; set; }
            public string Category { get; set; }
        }

        public MSTForm(IEnumerable<Edge> edges, int totalDistance) // Constructor takes edges and total distance
        {
            // Initialize UI components and populate the list
            InitializeComponent();
            Populate(edges?.ToList() ?? new List<Edge>(), totalDistance);
        }

        //-----------------------------------------------------------------------------------------------------------------//
        private void Populate(List<Edge> edges, int total) // Fill the flow panel with grouped edge cards
        {
            flowPanel.Controls.Clear();

            // If there are no edges, show a small message
            if (edges.Count == 0)
            {
                flowPanel.Controls.Add(new Label
                {
                    Text = "No connections found.",
                    Font = new Font("Segoe UI", 10F),
                    ForeColor = Color.Gray,
                    Padding = new Padding(8)
                });
                return;
            }

            // Normalize category names so similar categories group together
            foreach (var e in edges)
                e.Category = NormalizeCategory(e.Category);

            // Group edges by normalized category and sort groups alphabetically
            var grouped = edges
                .GroupBy(e => e.Category ?? "Miscellaneous")
                .OrderBy(g => g.Key);

            // For each group add a header and a card for every edge in that group
            foreach (var group in grouped)
            {
                var icon = GetCategoryIcon(group.Key);

                var groupHeader = new Label
                {
                    Text = $"{icon} {group.Key}",
                    Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold),
                    ForeColor = ColorTranslator.FromHtml("#333"),
                    AutoSize = false,
                    Height = 30,
                    Padding = new Padding(8, 6, 0, 0),
                    Dock = DockStyle.Top
                };
                flowPanel.Controls.Add(groupHeader);

                foreach (var e in group)
                {
                    var card = CreateEdgeCard(e);
                    flowPanel.Controls.Add(card);
                }
            }

            // Show total network cost at the bottom
            lblTotal.Text = $"Total Network Cost: {total}";
        }

        //-----------------------------------------------------------------------------------------------------------------//
        private string NormalizeCategory(string cat) // Convert messy category strings into friendly names
        {
            if (string.IsNullOrWhiteSpace(cat)) return "Miscellaneous";
            cat = cat.Trim().ToLower();

            if (cat.Contains("water")) return "Water Leak";
            if (cat.Contains("street")) return "Streetlight";
            if (cat.Contains("sanitation")) return "Sanitation";
            if (cat.Contains("dump")) return "Illegal Dumping";
            if (cat.Contains("pothole") || cat.Contains("road")) return "Pothole / Roads";
            if (cat.Contains("waste") || cat.Contains("trash") || cat.Contains("refuse")) return "Waste Management";
            return "Miscellaneous";
        }

        //-----------------------------------------------------------------------------------------------------------------//
        private string GetCategoryIcon(string category) // Return a small emoji icon for the category
        {
            return category switch
            {
                "Water Leak" => "💧",
                "Streetlight" => "💡",
                "Sanitation" => "🧻",
                "Illegal Dumping" => "🗑️",
                "Pothole / Roads" => "🚧",
                "Waste Management" => "♻️",
                _ => "🏷️"
            };
        }

        //-----------------------------------------------------------------------------------------------------------------//
        private Panel CreateEdgeCard(Edge e) // Create a UI card to display one edge
        {
            var card = new Panel
            {
                BackColor = Color.White,
                Size = new Size(flowPanel.ClientSize.Width + 900, 150),
                Margin = new Padding(10),
                Padding = new Padding(12)
            };
            card.Region = new Region(RoundedRect(new Rectangle(0, 0, card.Width, card.Height), 10));

            var lblMain = new Label
            {
                Text = $"{e.From}  ↔  {e.To}", // endpoints of the connection
                Font = new Font("Segoe UI Semibold", 11F),
                AutoSize = false,
                Size = new Size(card.Width - 20, 26),
                Location = new Point(10, 8)
            };

            var lblCost = new Label
            {
                Text = $"Connection Strength: {6 - e.Distance} / 5   (Cost: {e.Distance})", // show strength and cost
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = Color.DimGray,
                AutoSize = false,
                Size = new Size(card.Width - 20, 20),
                Location = new Point(10, 38)
            };

            var lblDesc = new Label
            {
                Text = $"💡 These requests share similarities such as category, date, or priority.",
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(50, 50, 50),
                AutoSize = false,
                Size = new Size(card.Width - 20, 24),
                Location = new Point(10, 64)
            };

            card.Controls.Add(lblMain);
            card.Controls.Add(lblCost);
            card.Controls.Add(lblDesc);

            // Draw a light rounded border on the card
            card.Paint += (s, e2) =>
            {
                e2.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using var pen = new Pen(Color.LightGray);
                e2.Graphics.DrawPath(pen, RoundedRect(new Rectangle(0, 0, card.Width - 1, card.Height - 1), 10));
            };

            return card;
        }

        //-----------------------------------------------------------------------------------------------------------------//
    }
}
