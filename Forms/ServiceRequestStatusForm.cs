using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MunicipalServicesApp.Data;
using MunicipalServicesApp.Model;

namespace MunicipalServicesApp.Forms
{
    // Logic / event handlers for the ServiceRequestStatusForm
    public partial class ServiceRequestStatusForm : Form
    {
        // repo holds saved requests on disk
        private readonly ServiceRequestRepo repo;
        // data structures used in the form
        private AvlTree avl;
        private RequestPriorityHeap heap;
        private ServiceRouteNetwork graph;

        // overlay controls (created lazily)
        private Panel overlayPanel;
        private Button btnCloseOverlay;

        public ServiceRequestStatusForm()
        {
            InitializeComponent();

            // load repository (file-backed)
            repo = new ServiceRequestRepo();
            SampleRequestSeedercs.SeedIfEmpty(repo);

            // build AVL and heap
            avl = new AvlTree();
            heap = new RequestPriorityHeap();
            foreach (var r in repo.All)
            {
                avl.Insert(r);
                heap.Insert(r);
            }

            // build demo graph
            graph = new ServiceRouteNetwork();
            graph.BuildFromRequests(repo.All, threshold: 10.0);
            var rand = new Random();
            var locations = repo.All.Select(r => r.Location).Distinct().Take(6).ToList();
            for (int i = 0; i < locations.Count - 1; i++)
                for (int j = i + 1; j < locations.Count; j++)
                    graph.AddEdge(locations[i], locations[j], rand.Next(1, 15));

            // wire up basic buttons
            btnRefresh.Click += (s, e) => RefreshCards();
            btnSearch.Click += (s, e) => SearchById();
            btnPrioritise.Click += (s, e) => ShowPriorityOrder();
            btnShowMST.Click += (s, e) => ShowGraphMST();
            btnBack.Click += (s, e) => Close();
            btnRecent.Click += (s, e) => ShowRecentRequests();

            // Populate after the form is displayed so layout sizes are available
            this.Shown += (s, e) => RefreshCards();
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        // Refresh card UI
        private void RefreshCards()
        {
            try
            {
                // schedule after layout pass
                this.BeginInvoke((Action)(() =>
                {
                    PopulateCards();
                    if (cardsPanel.Controls.Count > 0)
                    {
                        cardsPanel.ScrollControlIntoView(cardsPanel.Controls[0]);
                    }
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load requests: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void PopulateCards()
        {
            cardsPanel.SuspendLayout();
            cardsPanel.Controls.Clear();

            var list = repo?.All?.OrderByDescending(x => x.DateReported).ToList()
                       ?? new System.Collections.Generic.List<ServiceRequest>();

            if (list.Count == 0)
            {
                var empty = new Label
                {
                    Text = "No requests available. Try Refresh or add a new request from the main menu.",
                    Font = new Font("Segoe UI", 12F),
                    ForeColor = Color.DimGray,
                    AutoSize = false,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Size = new Size(Math.Max(400, cardsPanel.ClientSize.Width - 48), 120),
                    Margin = new Padding(20)
                };
                cardsPanel.Controls.Add(empty);
                cardsPanel.ResumeLayout();
                return;
            }

            foreach (var r in list)
            {
                var card = CreateRequestCard(r);
                card.Width = Math.Max(600, cardsPanel.ClientSize.Width - 48);
                cardsPanel.Controls.Add(card);
            }

            cardsPanel.ResumeLayout();
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        // Create a single request card 
        private Panel CreateRequestCard(ServiceRequest r)
        {
            // Pick color by priority (1–5)
            Color bg;
            Color border;

            switch (r.Priority)
            {
                case 1:
                    bg = ColorTranslator.FromHtml("#FFCCCC");   // pastel red
                    border = ColorTranslator.FromHtml("#E6A6A6");
                    break;

                case 2:
                    bg = ColorTranslator.FromHtml("#FFE6CC");   // pastel peach
                    border = ColorTranslator.FromHtml("#E6C7A6");
                    break;

                case 3:
                    bg = ColorTranslator.FromHtml("#C1CDF5");   // pastel lavender-blue (your choice)
                    border = ColorTranslator.FromHtml("#9AAFE6");
                    break;

                case 4:
                    bg = ColorTranslator.FromHtml("#D6F5D6");   // pastel mint green
                    border = ColorTranslator.FromHtml("#B3E6B3");
                    break;

                case 5:
                    bg = ColorTranslator.FromHtml("#A7CEEB");   // light blue
                    border = ColorTranslator.FromHtml("#224761");
                    break;

                default:
                    bg = Color.White;
                    border = Color.LightGray;
                    break;
            }

            var card = new Panel
            {
                Height = 140,
                BackColor = bg,
                Margin = new Padding(8),
                Padding = new Padding(12),
                Cursor = Cursors.Hand
            };

            // rounded region; will be repainted on size
            card.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using (var path = RoundedRect(new Rectangle(0, 0, card.Width - 1, card.Height - 1), 10))
                using (var pen = new Pen(border))
                using (var fill = new SolidBrush(bg))
                {
                    e.Graphics.FillPath(fill, path);
                    e.Graphics.DrawPath(pen, path);
                }
            };

            // Labels
            var lblTitle = new Label
            {
                Text = $"{Shorten(r.Category, 30)} — {Shorten(r.Location, 60)}",
                Font = new Font("Segoe UI Semibold", 11F),
                Location = new Point(12, 10),
                AutoSize = true
            };

            var lblMeta = new Label
            {
                Text = $"Priority: {r.Priority}  |  Status: {r.Status}  |  {r.DateReported:yyyy-MM-dd HH:mm}",
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(80, 80, 80),
                Location = new Point(12, 36),
                AutoSize = true
            };

            var lblDesc = new Label
            {
                Text = Shorten(r.Description ?? "", 180),
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.DimGray,
                Location = new Point(12, 60),
                AutoSize = true
            };

            var lblId = new Label
            {
                Text = $"ID: {r.Id}",
                Font = new Font("Segoe UI", 8F),
                ForeColor = Color.FromArgb(130, 130, 130),
                Location = new Point(12, 100),
                AutoSize = true
            };

            // Open button
            var btnOpen = new Button
            {
                Text = "Open",
                Size = new Size(110, 36),
                BackColor = ColorTranslator.FromHtml("#6CB2B2"),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnOpen.FlatAppearance.BorderSize = 0;
            btnOpen.Region = new Region(RoundedRect(new Rectangle(0, 0, btnOpen.Width, btnOpen.Height), 8));
            btnOpen.Click += (s, e) =>
            {
                var details = $"ID: {r.Id}\nCategory: {r.Category}\nLocation: {r.Location}\nPriority: {r.Priority}\nStatus: {r.Status}\nDate: {r.DateReported}\n\nDescription:\n{r.Description}";
                MessageBox.Show(details, "Request Details", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            card.Controls.Add(lblTitle);
            card.Controls.Add(lblMeta);
            card.Controls.Add(lblDesc);
            card.Controls.Add(lblId);
            card.Controls.Add(btnOpen);

            card.Resize += (s, e) =>
            {
                btnOpen.Location = new Point(card.ClientSize.Width - btnOpen.Width - 18, (card.ClientSize.Height - btnOpen.Height) / 2);

                int rightLimit = btnOpen.Location.X - 24;
                lblTitle.MaximumSize = new Size(Math.Max(100, rightLimit - lblTitle.Location.X), 0);
                lblMeta.MaximumSize = new Size(Math.Max(100, rightLimit - lblMeta.Location.X), 0);
                lblDesc.MaximumSize = new Size(Math.Max(100, rightLimit - lblDesc.Location.X), 0);
            };

            // initial layout trigger
            card.Width = Math.Max(600, this.ClientSize.Width - 80);
            card.PerformLayout();

            return card;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        // Basic search by ID
        private void SearchById()
        {
            var id = txtSearchId.Text?.Trim();
            if (string.IsNullOrWhiteSpace(id))
            {
                MessageBox.Show("Enter a Request ID to search (copy from the list).", "Input required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var req = avl?.SearchById(id) ?? repo.GetById(id);

            if (req == null)
            {
                MessageBox.Show("No request found with that ID.", "Not found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var details = $"ID: {req.Id}\nCategory: {req.Category}\nLocation: {req.Location}\nPriority: {req.Priority}\nStatus: {req.Status}\nDate: {req.DateReported}\n\nDescription:\n{req.Description}";
            MessageBox.Show(details, "Request Details", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        // Show priority form in overlay
        private void ShowPriorityOrder()
        {
            if (repo.All == null || repo.All.Count == 0)
            {
                MessageBox.Show("No requests available.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var ordered = repo.All.OrderBy(r => r.Priority).ThenByDescending(r => r.DateReported).ToList();
            var priorityForm = new RequestPriorityForm(ordered);
            ShowOverlayForm(priorityForm);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        // Show MST form in overlay
        private void ShowGraphMST()
        {
            if (graph == null)
            {
                MessageBox.Show("Graph not initialised.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var (edgesTuples, totalCost) = graph.GetMinimumSpanningTree();

            if (edgesTuples == null || edgesTuples.Count == 0)
            {
                MessageBox.Show("No MST could be generated (not enough requests).", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var edges = edgesTuples.Select(t =>
            {
                var fromReq = repo.GetById(t.from);
                var toReq = repo.GetById(t.to);

                string fromLabel = fromReq != null ? $"{fromReq.Category} — {fromReq.Location}" : t.from;
                string toLabel = toReq != null ? $"{toReq.Category} — {toReq.Location}" : t.to;

                return new MSTForm.Edge
                {
                    From = fromLabel,
                    To = toLabel,
                    Distance = t.weight,
                    Category = t.category
                };
            }).ToList();

            var mstForm = new MSTForm(edges, totalCost);
            ShowOverlayForm(mstForm);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        // Show recent requests in overlay
        private void ShowRecentRequests()
        {
            if (avl == null)
            {
                MessageBox.Show("No AVL tree available.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var recent10 = avl.GetMostRecent(10);
            if (recent10 == null || !recent10.Any())
            {
                MessageBox.Show("No requests found in AVL tree.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var recentForm = new RecentRequestsForm(recent10, 10);
            ShowOverlayForm(recentForm);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        // Overlay helpers
        private void EnsureOverlay()
        {
            if (overlayPanel != null) return;

            overlayPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(245, 245, 245),
                Visible = false
            };

            btnCloseOverlay = new Button
            {
                Text = "Close",
                Size = new Size(80, 30),
                BackColor = ColorTranslator.FromHtml("#E6E6E6"),
                FlatStyle = FlatStyle.Flat,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnCloseOverlay.FlatAppearance.BorderSize = 0;
            btnCloseOverlay.Region = new Region(RoundedRect(new Rectangle(0, 0, btnCloseOverlay.Width, btnCloseOverlay.Height), 6));
            btnCloseOverlay.Click += (s, e) => CloseOverlay();

            overlayPanel.Resize += (s, e) =>
            {
                btnCloseOverlay.Location = new Point(Math.Max(8, overlayPanel.ClientSize.Width - btnCloseOverlay.Width - 12), 8);
            };

            overlayPanel.Controls.Add(btnCloseOverlay);
            this.Controls.Add(overlayPanel);
            overlayPanel.BringToFront();
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void ShowOverlayForm(Form f)
        {
            EnsureOverlay();
            // remove any children except close button
            for (int i = overlayPanel.Controls.Count - 1; i >= 0; i--)
            {
                var c = overlayPanel.Controls[i];
                if (c == btnCloseOverlay) continue;
                overlayPanel.Controls.Remove(c);
                if (c is Form old) { try { old.Dispose(); } catch { } }
            }

            f.TopLevel = false;
            f.FormBorderStyle = FormBorderStyle.None;
            f.Dock = DockStyle.Fill;
            overlayPanel.Controls.Add(f);
            overlayPanel.Controls.SetChildIndex(btnCloseOverlay, 0);
            overlayPanel.Visible = true;
            cardsPanel.Visible = false;
            f.Show();
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void CloseOverlay()
        {
            if (overlayPanel == null) return;
            for (int i = overlayPanel.Controls.Count - 1; i >= 0; i--)
            {
                var c = overlayPanel.Controls[i];
                if (c == btnCloseOverlay) continue;
                overlayPanel.Controls.Remove(c);
                if (c is Form f) { try { f.Close(); f.Dispose(); } catch { } }
            }
            overlayPanel.Visible = false;
            cardsPanel.Visible = true;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        // helpers
        private string Shorten(string s, int max)
        {
            if (string.IsNullOrEmpty(s)) return "";
            return s.Length <= max ? s : s.Substring(0, max - 3) + "...";
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        // Rounded rectangle
        private System.Drawing.Drawing2D.GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            var path = new System.Drawing.Drawing2D.GraphicsPath();
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
