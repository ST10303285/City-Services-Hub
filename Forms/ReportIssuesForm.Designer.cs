using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace MunicipalServicesApp
{
    partial class ReportIssuesForm
    {
        private IContainer components = null;

        private Panel headerPanel;
        private Label lblHeader;
        private Label lblSubtitle;

        // main layout panels & controls
        private Panel mainPanel;            // fills the form, contains the rounded content panel
        private Panel contentCard;         // white rounded card that holds inputs
        private PictureBox picIllustration; // illustration to show it's a report page

        private Label lblLocation;
        private TextBox txtLocation;
        private Label lblCategory;
        private ComboBox cmbCategory;
        private Label lblDescription;
        private RichTextBox rtbDescription;
        private Button btnAttach;
        private Label lblAttachment;
        private ProgressBar pbEngagement;
        private Label lblEncouragement;
        private Button btnSubmit;
        private Button btnBack;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitialiseComponent()
        {
            components = new Container();

            // ---------------------------------------------------------------------------------------- Form Setup -----------------------------------------------------------
            this.Text = "Report an Issue";
            this.WindowState = FormWindowState.Maximized;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = ColorTranslator.FromHtml("#F3F6F6"); // subtle page background

            // ---------------------------------------------------------------------------------------- Header Panel ----------------------------------------------------------
            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 120,
                BackColor = ColorTranslator.FromHtml("#6CB2B2")
            };

            lblHeader = new Label
            {
                Text = "Report an Issue",
                Font = new Font("Segoe UI Semibold", 26F, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(28, 18)
            };

            lblSubtitle = new Label
            {
                Text = "Help us improve your community by reporting problems in your area.",
                Font = new Font("Segoe UI", 11F),
                ForeColor = Color.WhiteSmoke,
                AutoSize = true,
                Location = new Point(28, 80)
            };

            headerPanel.Controls.Add(lblHeader);
            headerPanel.Controls.Add(lblSubtitle);
            this.Controls.Add(headerPanel);

            // ---------------------------------------------------------------------------------------- Main Panel -----------------------------------------------------------
            mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(28),
                BackColor = ColorTranslator.FromHtml("#F3F6F6")
            };

            
            contentCard = new Panel
            {
                BackColor = ColorTranslator.FromHtml("#e1f5f2"),
                Size = new Size(this.ClientSize.Width - 120, this.ClientSize.Height - headerPanel.Height - 120),
                Location = new Point(28, headerPanel.Bottom + 24),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            // draw rounded background + subtle shadow in Paint
            contentCard.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                var rect = new Rectangle(0, 0, contentCard.Width - 1, contentCard.Height - 1);
                using var path = RoundedRect(rect, 16);
                using var shadowBrush = new SolidBrush(Color.FromArgb(18, 0, 0, 0));
                var shadowRect = rect;
                shadowRect.Offset(2, 2);
                using var shadowPath = RoundedRect(shadowRect, 16);
                e.Graphics.FillPath(shadowBrush, shadowPath);

                // fill white card and draw border
                using var fillBrush = new SolidBrush(ColorTranslator.FromHtml("#e1f5f2"));
                e.Graphics.FillPath(fillBrush, path);
                using var pen = new Pen(ColorTranslator.FromHtml("#E6E6E6"));
                e.Graphics.DrawPath(pen, path);
            };

            // ---------------------------------------------------------------------------------------- Illustration -------------------------------------------------------
            picIllustration = new PictureBox
            {
                Size = new Size(400, 400),
                Location = new Point(contentCard.Width - 380, 40),
                SizeMode = PictureBoxSizeMode.Zoom,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                BackColor = Color.Transparent
            };
            
            try
            {
                picIllustration.Image = Properties.Resources.report__1_;
            }
            catch
            {
                // resource not found — silently ignore
            }

            // ---------------------------------------------------------------------------------------- Form Controls Layout ------------------------------------------------
            
            int leftX = 40;
            int rightX = 500; // leave room for illustration on right
            int currentY = 45;
            int labelGap = 45;
            int inputHeight = 32;
            int inputWidth = contentCard.Width - rightX - 80;

            // Location label + textbox
            lblLocation = new Label
            {
                Text = "Location (suburb / street):",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(leftX, currentY),
                AutoSize = true
            };
            contentCard.Controls.Add(lblLocation);

            txtLocation = new TextBox
            {
                Location = new Point(leftX + 220, currentY - 3),
                Size = new Size(380, inputHeight),
                Font = new Font("Segoe UI", 10F),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            txtLocation.TextChanged += AnyInputChanged;
            contentCard.Controls.Add(txtLocation);

            currentY += labelGap + 8;

            // Category label + combobox
            lblCategory = new Label
            {
                Text = "Category:",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(leftX, currentY),
                AutoSize = true
            };
            contentCard.Controls.Add(lblCategory);

            cmbCategory = new ComboBox
            {
                Location = new Point(leftX + 220, currentY - 3),
                Size = new Size(260, inputHeight),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10F),
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };
            cmbCategory.Items.AddRange(new object[] { "Sanitation", "Roads", "Utilities", "StreetLight", "Public Safety", "Other" });
            cmbCategory.SelectedIndexChanged += AnyInputChanged;
            contentCard.Controls.Add(cmbCategory);

            currentY += labelGap + 4;

            // Description label + RichTextBox (make larger)
            lblDescription = new Label
            {
                Text = "Description:",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(leftX, currentY),
                AutoSize = true
            };
            contentCard.Controls.Add(lblDescription);

            rtbDescription = new RichTextBox
            {
                Location = new Point(leftX + 220, currentY - 6),
                Size = new Size(520, 240),
                Font = new Font("Segoe UI", 10F),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            rtbDescription.TextChanged += AnyInputChanged;
            contentCard.Controls.Add(rtbDescription);

            currentY += 260;

            // Attach button + attachment label
            btnAttach = new Button
            {
                Text = "Attach Image / Document",
                Location = new Point(leftX + 220, currentY),
                Size = new Size(220, 36),
                BackColor = ColorTranslator.FromHtml("#6CB2B2"),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F),
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };
            btnAttach.FlatAppearance.BorderSize = 0;
            btnAttach.Region = new Region(RoundedRect(new Rectangle(0, 0, btnAttach.Width, btnAttach.Height), 10));
            btnAttach.Click += BtnAttach_Click;
            contentCard.Controls.Add(btnAttach);

            lblAttachment = new Label
            {
                Text = "No file attached",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(leftX + 460, currentY + 8),
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };
            contentCard.Controls.Add(lblAttachment);

            currentY += 56;

            // Engagement progress bar and label
            pbEngagement = new ProgressBar
            {
                Location = new Point(leftX + 220, currentY),
                Size = new Size(520, 18),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            contentCard.Controls.Add(pbEngagement);

            currentY += 30;

            lblEncouragement = new Label
            {
                Text = "Complete fields to improve report quality.",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(leftX + 220, currentY),
                Size = new Size(520, 26),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            contentCard.Controls.Add(lblEncouragement);

            currentY += 46;

            // Submit / Back buttons (rounded)
            btnSubmit = new Button
            {
                Text = "Submit Report",
                Size = new Size(180, 44),
                Location = new Point(leftX + 220, currentY),
                BackColor = ColorTranslator.FromHtml("#6CB2B2"),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F)
            };
            btnSubmit.FlatAppearance.BorderSize = 0;
            btnSubmit.Region = new Region(RoundedRect(new Rectangle(0, 0, btnSubmit.Width, btnSubmit.Height), 10));
            btnSubmit.Click += BtnSubmit_Click;
            contentCard.Controls.Add(btnSubmit);

            btnBack = new Button
            {
                Text = "Back to Menu",
                Size = new Size(160, 44),
                Location = new Point(leftX + 420, currentY),
                BackColor = ColorTranslator.FromHtml("#A8D8EA"),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F)
            };
            btnBack.FlatAppearance.BorderSize = 0;
            btnBack.Region = new Region(RoundedRect(new Rectangle(0, 0, btnBack.Width, btnBack.Height), 10));
            btnBack.Click += BtnBack_Click;
            contentCard.Controls.Add(btnBack);

           
            contentCard.Controls.Add(picIllustration);

            // Ensure controls resize nicely when the contentCard resizes (responsive)
            contentCard.Resize += (s, e) =>
            {
                
                picIllustration.Location = new Point(contentCard.Width - picIllustration.Width - 250, 200);

                // adjust sizes of input controls that are anchored to right
                int rightSpace = contentCard.Width - picIllustration.Width - 120;
                txtLocation.Width = Math.Min(520, Math.Max(320, rightSpace - 260));
                rtbDescription.Width = Math.Min(720, rightSpace - 260);
                pbEngagement.Width = Math.Min(720, rightSpace - 260);
                lblEncouragement.Width = pbEngagement.Width;
            };

            // add card to main panel, then main to form
            mainPanel.Controls.Add(contentCard);
            this.Controls.Add(mainPanel);

            // trigger an initial layout pass so sizes are correct
            this.Load += (s, e) =>
            {
                contentCard.Invalidate();
                contentCard.PerformLayout();
                // initial resize actions
                contentCard.Width = this.ClientSize.Width - 56;
                contentCard.Height = this.ClientSize.Height - headerPanel.Height - 80;
                contentCard.Left = 28;
                contentCard.Top = headerPanel.Bottom + 24;
                contentCard.Refresh();
            };
        }

        // Utility for rounded corners
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
