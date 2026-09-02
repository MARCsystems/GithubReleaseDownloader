# MARCsystems_GuiElements

A simple Region-Based Chart/Graph Visualizer built on **GraphicsPath** and **Region** concepts for .NET applications.

---

## 📦 Installation

You can install the package from NuGet:

```bash
dotnet add package MARCsystems_GuiElements --version 1.0.1
```

Or via Visual Studio Package Manager:

```bash
Install-Package MARCsystems_GuiElements -Version 1.0.1
```

## 📊 How to use

Assign the Pie to either `Form`, `UserControl`, `PictureBox`, or `Panel`, depending on your goal.
The Pie will cut your element to the Pie Shape;

![Pie Sample](../Images/PieSample.PNG)

Initialization
```C#
internal partial class BaseForm : Form
{
    internal BaseForm()
    {
        InitializeComponent();
        InitializePie();
    }

    private Pie pieChart;
    private int outer = 0, inner = 0;  // Update these with values from any source
    private double current = 0, max = 0, multiplier = 0; // Update these with values from any source
    private float sweepStart = 0f;  // Update these with values from any source

    private void InitializePie()
    {
        pieChart?.Dispose();
        pieChart = Pie.Create(200, 200, 0); // Change the values to link with any source, based on usage
    }
}
```

Live Update
```C#
// Assume that pnl_Pie is a Panel Element

private void UpdatePieLive()
{
    UpdateValues();
    Task.Run(() =>
    {
        pieChart.OuterDiameter = outer;
        pieChart.InnerDiameter = inner;
        pieChart.PrecalculatePie(current, max, multiplier, sweepStart);

        try
        {
            Invoke((MethodInvoker)delegate ()
            {
                pnl_Pie.Region?.Dispose();
                pnl_Pie.Region = pieChart.GetPrecalculatedPie();
            });
        }
        catch (Exception err)
        {
            Console.WriteLine(err.Message);
        }
    });
}
```

Update (Trigger-based Sample)
```C#
// Assume that pnl_Pie is a Panel Element

private void UpdatePieOnce()
{
    UpdateValues();
    pieChart.OuterDiameter = outer;
    pieChart.InnerDiameter = inner;
    pnl_Pie.Region?.Dispose();
    pnl_Pie.Region = pieChart.SetValues(current, max, multiplier, sweepStart);
}
```
NOTE: The color and design of the Pie depends on the control's BackColor/Image assigned on it.