namespace TerminalRenderer.UI;

internal static class Ui
{
    public static Window Window(string title, params Control[] children)
    {
        var w = new Window(title);
        foreach (var c in children) w.Children.Add(c);
        return w;
    }

    public static StackPanel Stack(params Control[] children)
    {
        var s = new StackPanel();
        foreach (var c in children) s.Children.Add(c);
        return s;
    }

    public static Label Label(string text) => new(text);

    public static Button Button(string text, Action? onClick = null)
        => new(text) { OnClick = onClick };
}
