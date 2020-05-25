using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Gtk;

public partial class MainWindow : Gtk.Window
{
    public MainWindow() : base(Gtk.WindowType.Toplevel)
    {
        #pragma warning disable RECS0021
        Build();
        #pragma warning restore RECS0021
        ChangeContainerFont(this.vcontainer, "Lucida 11");
    }

    public static void ChangeContainerFont(Gtk.Container container, string fontDesc)
    {
        ChangeWidgetFont(container, fontDesc);
        foreach (Gtk.Widget subw in container.Children)
        {
            ChangeWidgetFont(subw, fontDesc);
            if (subw is Gtk.MenuItem menuItem)
            {
                var subMenu = menuItem.Submenu;
                ChangeContainerFont(menuItem, fontDesc);
                if (subMenu is Gtk.Container subContainer)
                {
                    ChangeContainerFont(subContainer, fontDesc);
                }
                else
                {
                    if (subMenu != null)
                    {
                        ChangeWidgetFont(subMenu, fontDesc);
                    }
                }
            }
            else
            if (subw is Gtk.Container subContainer)
            {
                ChangeContainerFont(subContainer, fontDesc);
            }
        }
    }

    public static void ChangeWidgetFont(Gtk.Widget w, string fontDesc)
    {
        w.ModifyFont(Pango.FontDescription.FromString(fontDesc));
    }

    public void RemoveEntryLastChar(string s)
    {
        if (s.Length > 0)
            entry.Text = entry.Text.Remove(entry.Text.Length - 1);
    }

    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
        a.RetVal = true;
    }

    protected void OnEntryChanged(object sender, EventArgs e)
    {
        if (lblErr.Text.Length > 0) lblErr.Text = "";
        if (entry.Text.Length > 0)
        {
            var rgx = new Regex(@"[0-9]$");
            string chr = entry.Text[entry.Text.Length - 1].ToString();
            string signs = "()+-*/.";
            bool charAllowed = signs.Contains(chr);
            if (!rgx.IsMatch(entry.Text) &&
                !charAllowed
                )
                entry.Text = entry.Text.Remove(entry.Text.Length - 1);
        }
    }

    protected void OnBtnCharClicked(object sender, EventArgs e)
    {
        entry.Text += ( (Button) sender).Label;
    }
    protected void OnBtnCClicked(object sender, EventArgs e) => entry.Text = "";

    protected void OnBtnBackspaceClicked(object sender, EventArgs e) => RemoveEntryLastChar(entry.Text);

    protected void OnBtnSqrtClicked(object sender, EventArgs e)
    {
        try
        {
            double n = int.Parse(entry.Text);
            if (n > 0)
            {
                n = Math.Sqrt(n);
                entry.Text = n.ToString();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            lblErr.Text = "Expresión inválida.";
        }
    }

    protected void OnBtnEqualClicked(object sender, EventArgs e)
    {
        try
        {
            var ans = Eval(entry.Text);
            entry.Text = ans.ToString();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            lblErr.Text = "Expresión inválida";
        }
    }

    public static double Eval(string expr)
    {

        Stack<string> stack = new Stack<string>();

        string value = "";
        for (int i = 0; i < expr.Length; i++)
        {
            string s = expr.Substring(i, 1);
            char chr = s.ToCharArray()[0];

            if (!char.IsDigit(chr) && chr != '.' && value != "")
            {
                stack.Push(value);
                value = "";
            }
            if (s.Equals("("))
            {
                string innerExp = "";
                i++;
                int bracketCount = 0;
                for (; i < expr.Length; i++)
                {
                    s = expr.Substring(i, 1);

                    if (s.Equals("("))
                        bracketCount++;
                    if (s.Equals(")"))
                        if (bracketCount == 0)
                            break;
                        else
                            bracketCount--;
                    innerExp += s;
                }
                stack.Push(Eval(innerExp).ToString());
            }
            else if (s.Equals("+")) stack.Push(s);
            else if (s.Equals("-")) stack.Push(s);
            else if (s.Equals("*")) stack.Push(s);
            else if (s.Equals("/")) stack.Push(s);
            else if (s.Equals("sqrt")) stack.Push(s);
            else if (s.Equals(")")){}
            else if (char.IsDigit(chr) || chr == '.')
            {
                value += s;
                if (value.Split('.').Length > 2)
                    throw new Exception("Invalid decimal.");
                if (i == (expr.Length - 1))
                    stack.Push(value);
            }
            else
                throw new Exception("Invalid character.");
        }
        double result = 0;
        while (stack.Count >= 3)
        {
            double right = Convert.ToDouble(stack.Pop());
            string op = stack.Pop();
            double left = Convert.ToDouble(stack.Pop());
            if (op == "+") result = left + right;
            else if (op == "-") result = left - right;
            else if (op == "*") result = left * right;
            else if (op == "/") result = left / right;
            stack.Push(result.ToString());
        }
        return Convert.ToDouble(stack.Pop());
    }
}
