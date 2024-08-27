using System.Windows.Controls;

namespace MechEngineerSaver;

public class ContextHolder<Context> : Grid where Context : class
{
    public Context? DC => DataContext as Context;
}
