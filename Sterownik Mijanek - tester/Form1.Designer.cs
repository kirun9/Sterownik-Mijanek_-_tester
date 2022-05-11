namespace Sterownik_Mijanek___tester;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
            this.components = new System.ComponentModel.Container();
            this.pulpit1 = new Sterownik_Mijanek___tester.Pulpit(this.components);
            this.SuspendLayout();
            // 
            // pulpit1
            // 
            this.pulpit1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pulpit1.Location = new System.Drawing.Point(0, 0);
            this.pulpit1.Name = "pulpit1";
            this.pulpit1.Size = new System.Drawing.Size(1200, 450);
            this.pulpit1.TabIndex = 0;
            this.pulpit1.Text = "pulpit1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1200, 450);
            this.Controls.Add(this.pulpit1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosed += new FormClosedEventHandler(Form1_FormClosed);
            this.ResumeLayout(false);

    }

    #endregion

    private Pulpit pulpit1;
}
