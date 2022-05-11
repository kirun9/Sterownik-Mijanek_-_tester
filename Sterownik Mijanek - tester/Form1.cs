namespace Sterownik_Mijanek___tester;

public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();
    }

    private void Form1_FormClosed(object sender, FormClosedEventArgs e)
    {
        Environment.Exit(Environment.ExitCode);
    }
}
